using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Microsoft.MSDN.Samples
{

    public class convolutionFilters
    {

        // the matrix class
        public class ConvMatrix
        {

            public int Factor = 1;
            public int Offset = 0;
            // instead of all those integers, we'll use an array:
            public int[] grid = new int[9];
            // then we can get it in a for... next loop

            public ConvMatrix()
            {
                // fill up with defaults
                for (int i = 0; i <= 8; i++)
                {
                    grid[i] = 0;
                    grid[4] = 1;
                }
            }

            public void SetAll(int value)
            {
                // allows us to set all the  items in grid to the same value
                for (int i = 0; i <= 8; i++)
                    grid[i] = value;
            }

            public int this[int index]
            {
                // default property means we dont have to use the keyword "item"
                // lets us set the values at grid(value)
                get
                {
                    return grid[index];
                }
                set
                {
                    grid[index] = value;
                }
            }

        }

        private ConvMatrix _grid; // for the property:

        // Allow us to get a grid
        public ConvMatrix GetGrid
        {
            get
            {
                return _grid;
            }
        }

        public convolutionFilters()
        {
            _grid = new ConvMatrix();
        }

        public bool conv3x3(Bitmap b, ConvMatrix m)
        {

            // this will always be fairly slow as for each pixel we are reading 9 other pixels to get
            // the final color value. The perpixel filters just read from one pixel.

            // avoid / by 0
            if (m.Factor == 0)
                return true;

            // We are moving through the bitmap, and changing pixels based on their surrounding
            // pixels. This means that we need a copy of the source bitmap, so that we don't
            // end up using pixels that we have changed (and so filter based on changed pixels)
            // So here is a copy of the input bitmap:
            Bitmap bSrc = (Bitmap)b.Clone();

            // So we want Two lockbits now. We lock the source bits
            var bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            // And we lock the target bits
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            // We need the pointers to the start of both images data:
            var Scan0 = bmData.Scan0;
            var SrcScan0 = bmSrc.Scan0;

            // We are storing all the pixels as int32s in arrays:
            var pixels = new int[(b.Width * b.Height)];
            var pixelsSrc = new int[(b.Width * b.Height)];

            // Marshal copy grabs the pixels
            Marshal.Copy(Scan0, pixels, 0, pixels.Length);
            Marshal.Copy(SrcScan0, pixelsSrc, 0, pixelsSrc.Length);

            // keep a running total (more later)...
            var total = new float[4]; // store sum of the pixels read from the source

            int index;

            float gridValueDivmFactor;
            float ninthOfOffset = (float)(m.Offset / 9d);

            // We are manipulating a pixel based on all its surrounding pixels
            // the pixels on the edges don't have all the surrounding pixels
            // so we skip them by staring these loops at 1 and ending at blah -2
            for (int y = 1, loopTo = b.Height - 2; y <= loopTo; y++)
            {
                for (int x = 1, loopTo1 = b.Width - 2; x <= loopTo1; x++)
                {

                    // We have defined a 3x3 grid and we apply it to the pixels 
                    // around the current one: (E is current pixel)
                    // 
                    // pixels         grid
                    // A B C          1 2 3
                    // D E F   with   4 5 6
                    // G H I          7 8 9
                    // 
                    // (this is not matrix multiplication)
                    // 
                    // We get: (A*1) + (B*2) + (C*3) + (D*4) + (E*5) + (F*6) + (G*7) + (H*8) + (I*9)
                    // we then divide this by m.factor and add m.offset...
                    // 
                    // e.g. with the starting grid:
                    // 000
                    // 010
                    // 000 we get the result E = the value of the central pixel so no change to the image
                    // 
                    // Our For ... Next loop puts the current pixel at (x,y)
                    // This means we need to get the pixels in the following positions:
                    // (x-1,y-1) ( x ,y-1) (x+1,y-1)
                    // (x-1, y ) ( x , y ) (x+1, y )
                    // (x-1,y+1) ( x ,y+1) (x+1,y+1)
                    // 



                    // Another loop
                    // This one loops through a 3x3 grid:
                    for (int yy = -1; yy <= +1; yy++)
                    {
                        for (int xx = -1; xx <= +1; xx++)
                        {

                            // the central pixel(x,y) is in the array at this index:
                            // (y * b.Width) pixels in the rows above + x pixels in the current row
                            // (y * b.width) + x

                            // the yyxx for next loop gives us:    (-1,-1)  (0,-1) (1,-1)   
                            // (-1,0)   (0,0)  (1,0)  
                            // (-1,1)   (0,1)  (1,1)
                            // 
                            // so... the locaion of the pixels in the loop is (x+xx,y+yy)
                            // and the position in the array is:
                            // 
                            index = (y + yy) * b.Width + x + xx;

                            // we want to total the argb of the current pixel * the grid value for its position
                            // we'll do the factor here too, and add on 1/9 of offset
                            // trying to keep the number of calculations in the loops to a minimum

                            gridValueDivmFactor = (float)(m.grid[(yy + 1) * 3 + xx + 1] / (double)m.Factor);

                            // ignore alpha, it messes up with some filters. will just set it to 255

                            // total(0) += (((pixelsSrc(index) >> 24) And &HFF) * gridValueDivmFactor) + ninthOfOffset
                            total[1] += (pixelsSrc[index] >> 16 & 0xFF) * gridValueDivmFactor + ninthOfOffset;
                            total[2] += (pixelsSrc[index] >> 8 & 0xFF) * gridValueDivmFactor + ninthOfOffset;
                            total[3] += (pixelsSrc[index] & 0xFF) * gridValueDivmFactor + ninthOfOffset;

                        }
                    }

                    for (int i = 0; i <= 3; i++)
                    {
                        if (total[i] > 255f)
                            total[i] = 255f;
                        if (total[i] < 0f)
                            total[i] = 0f;
                    }


                    // finally we set the pixel in the destination bitmap

                    pixels[y * b.Width + x] = 255 << 24 | Convert.ToInt32(total[1]) << 16 | Convert.ToInt32(total[2]) << 8 | Convert.ToInt32(total[3]);



                    // reset total for next pixel
                    Array.Clear(total, 0, 4);

                }
            }
            Marshal.Copy(pixels, 0, Scan0, pixels.Length);
            // unlock both bitmaps
            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);


            return true;

        }



    }
}