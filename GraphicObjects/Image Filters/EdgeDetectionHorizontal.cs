using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Microsoft.MSDN.Samples
{

    public class EdgeDetectionHorizontal
    {

        public static Bitmap ProcessImage(Bitmap b)
        {

            // make a copy of the source
            Bitmap bSrc = (Bitmap)b.Clone();

            // Now its lockbits time again.
            // Lock each image:
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // stride is the width of a row in memory
            int stride = bmData.Stride;

            // store the pointers to the start of the pixel data for each bitmap
            var scan0Src = bmDataSrc.Scan0;
            var scan0 = bmData.Scan0;

            // arrays to hold the color values
            var pixelsSrc = new byte[(stride * bSrc.Height)];
            var pixels = new byte[(stride * b.Height)];

            // fill the arrays
            Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length);
            Marshal.Copy(scan0, pixels, 0, pixels.Length);

            // don't need the srcBitmap as we have the data in the array now
            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            // Sum the color values as we move through a grid
            var total = default(int);

            // The pixel we set is the one in the middle
            // So when we put the grid in the top left corner of the bitmap
            // we are setting pixel(3,1)
            // This means that there will be 3 pixels untouched on the left and right edges
            // and 1 untouched at the top and bottom edges.

            // y loop:
            // can't do the first row (row 0) so start at 1
            // b.height is one to big when we start at 0, so remove 1 (b.height -1)
            // and another -1 as we cant do the last row:
            for (int y = 1, loopTo = b.Height - 2; y <= loopTo; y++)
            {

                // x loop:
                // we need to stop before the end of the row, 
                // to account for the width of the grid.
                // looping through each pixel.

                for (int x = 0, loopTo1 = b.Width - 7; x <= loopTo1; x++)
                {

                    // c.g.'s version starts at byte (9,2) 
                    // 
                    // For the fisrt run through the loop he adds / subtracts the  
                    // color values of pixels like so:
                    // 
                    // (0,2) + (3,2) + (6,2) + (9,2) + (12,2) + (15,2) + (18,2)  
                    // - (0,0) - (3,0) - (6,0) - (9,0) - (12,0) - (15,0) - (18,0)  
                    // 
                    // These are multiples of three as we want to get every third byte
                    // so that it is the same color component. BGRBGRBGRBGRBGRBGRBGR 
                    // ^  ^  ^  ^  ^  ^  ^   
                    // which in effect is using this grid: 
                    // 
                    // .----------------------------------.
                    // | -1 | -1 | -1 | -1 | -1 | -1 | -1 |
                    // |----------------------------------|
                    // |  0 |  0 |  0 |  0 |  0 |  0 |  0 |
                    // |----------------------------------|
                    // |  1 |  1 |  1 |  1 |  1 |  1 |  1 |
                    // '----------------------------------'
                    // 
                    // and moving it through each byte.
                    // 
                    // WE start by setting x,y to (0,1) = blue in the first pixel, second row
                    // 
                    // 
                    // loop though the B G and R
                    for (int colorByte = 0; colorByte <= 2; colorByte++)
                    {
                        // 
                        // yy loop:
                        // rows in the 7x3 grid (missing out the middle row)
                        // relative to the middle row:
                        for (int yy = -1; yy <= 1; yy += 2)
                        {
                            // xx loop:
                            // across the cells in a 7x3 row
                            // start at 0 so end at 6

                            for (int xx = 0; xx <= 6; xx++)
                            {

                                // Read the color information:

                                // Current position from scan0 (start of the bitmap memory):
                                int position;
                                position = (y + yy) * stride; // add the number of rows down * width of row
                                position += (x + xx) * 4;  // add the cell position along the current row
                                position += colorByte; // and the current byte
                                int col1 = pixelsSrc[position];
                                // now if we are in the top 7x3 row then yy  = -1 
                                // and we want to subtract the color value from the total
                                // if we are in the bottom row then yy = 1
                                // and we want to add the color value to the total
                                // So
                                total = total + yy * col1;
                            }
                        }

                        if (total > 255)
                            total = 255;
                        if (total < 0)
                            total = 0;
                        // Now total has the value for the grid starting at (x,y) 
                        // The actual pixel we want to set is the one in the middle of
                        // We are starting on line 1 anyway, so the y value is fine
                        // but we need to bump x along by 3 pixels 

                        pixels[y * stride + (x + 3) * 4 + colorByte] = (byte)total;

                        // reset the total for the next byte of color info:
                        // (btw: interesting effect if you comment this out)
                        total = 0;
                    }
                }
            }

            Marshal.Copy(pixels, 0, scan0, pixels.Length);

            b.UnlockBits(bmData);

            return b;

        }

    }
}