using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Microsoft.MSDN.Samples
{

    public class EdgeDetectionVertical
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

            // yy loop:
            // first row is row 0 
            // can't do top 3 pixels so start at row 4 (y=3)
            // can't do last 3 pixels so stop at height - 3
            // first row is 0, so remove another 1 from height:
            for (int y = 3, loopTo = b.Height - 4; y <= loopTo; y++)
            {

                // xx loop:
                // looping through pixels in a row

                for (int x = 0, loopTo1 = b.Width - 3; x <= loopTo1; x++)
                {
                    for (int colorBytes = 0; colorBytes <= 2; colorBytes++) // loop through blue, green, red, skip alpha
                    {
                        // 
                        // 
                        // this time the 7x3 grid looks like this: 
                        // 
                        // .--------------.
                        // | -1 |  0 |  1 |
                        // |--------------|
                        // | -1 |  0 |  1 |
                        // |--------------|
                        // | -1 |  0 |  1 |
                        // |--------------|
                        // | -1 |  0 |  1 |
                        // |--------------|
                        // | -1 |  0 |  1 |
                        // |--------------|
                        // | -1 |  0 |  1 |
                        // |--------------|
                        // | -1 |  0 |  1 |
                        // '--------------'
                        // 
                        // We start by setting x,y to (0,3) = blue in the first pixel, fourth row
                        // 
                        // 
                        for (int yy = -3; yy <= 3; yy++) // rows in the 3x7 grid relative to our position
                        {
                            // in the fourth row 

                            for (int xx = -1; xx <= 1; xx += 2)
                            {
                                // xx will loop -1, +1 but add 1 to it later
                                // middle cells in row are 0 so we can skip them

                                // Read the color information:

                                // Current position from scan0 (start of the bitmap memory):
                                int position;
                                // add the number of rows down * width of row
                                position = (y + yy) * stride;
                                // add distance along the current row
                                // xx needs to refer to 0 or 2 so add 1

                                position += (x + xx + 1) * 4;
                                position += colorBytes;
                                int col1 = pixelsSrc[position];
                                // now if we are in the left 3x7 column then xx  = -1 
                                // and we want to subtract the color value from the total
                                // if we are in the right column then xx = 1
                                // and we want to add the color value to the total
                                // So:
                                total = total + xx * col1;
                            }
                        }

                        if (total > 255)
                            total = 255;
                        if (total < 0)
                            total = 0;
                        pixels[y * stride + (x + 1) * 4 + colorBytes] = (byte)total;

                        // reset the total for the next byte of color info:
                        // (interesting effect if you comment this out)
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