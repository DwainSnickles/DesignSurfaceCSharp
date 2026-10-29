using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Microsoft.MSDN.Samples
{

    public class EdgeDetectionTwoPixelTest
    {

        // see
        // http://www.gamedev.net/reference/articles/article2007.asp
        // This is algorithm1 - edge detection.
        // Here we get each pixels RGB
        // and the RGB for its R hand neighbour
        // and the RGB for its downstairs neighbour
        // We then calculate the COLOR DIFFERENCE
        // and if it is above a certain threshold we have an edge
        // if it is an edge we make it white
        // otherwise black.


        public static Bitmap ProcessImage(Bitmap b, int nThreshold)
        {

            // Draw the new bitmap in this:
            var bTarget = new Bitmap(b.Width, b.Height);

            // set the background to black
            var g = Graphics.FromImage(bTarget);
            g.Clear(Color.Black);
            g.Dispose();

            // lock bits
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bmTarget = bTarget.LockBits(new Rectangle(0, 0, bTarget.Width, bTarget.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // width of bitmap row in memory
            int stride = bmData.Stride;

            // pointers to the start of the bitmap data
            var scan0Src = bmData.Scan0;
            var scan0Target = bmTarget.Scan0;

            // arrays to hold the color values
            var pixelsSrc = new byte[(stride * b.Height)];
            var pixelsTarget = new byte[(stride * bTarget.Height)];

            // fill the arrays
            Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length);
            Marshal.Copy(scan0Target, pixelsTarget, 0, pixelsTarget.Length);

            // don't need the srcBitmap as we have the data in the array now
            b.UnlockBits(bmData);
            b.Dispose();

            // store position of current pixel
            int position;

            // store the current pixels ARGB:
            int R0, G0, B0;
            // R hand neighbour:
            int RRight, GRight, BRight;
            // Pixel Below:
            int RBelow, GBelow, BBelow;

            // store the color difference between current pixel and R pixel
            int colorDifferenceR;
            // and the pixel below
            int colorDifferenceB;

            for (int y = 0, loopTo = bTarget.Height - 2; y <= loopTo; y++)
            {
                // Get the first pixels RGB in the outer loop
                // This will only happen at the start of a row, we get the other pixels R0, G0, B0 later

                B0 = pixelsSrc[y * stride];
                G0 = pixelsSrc[y * stride + 1];
                R0 = pixelsSrc[y * stride + 2];
                for (int x = 0, loopTo1 = bTarget.Width - 2; x <= loopTo1; x++)
                {

                    // position of current pixel's first byte:
                    position = y * stride + x * 4;

                    // colors for the Right pixel
                    BRight = pixelsSrc[position + 4];
                    GRight = pixelsSrc[position + 5];
                    RRight = pixelsSrc[position + 6];

                    // colors for the pixel beneath
                    BBelow = pixelsSrc[position + stride];
                    GBelow = pixelsSrc[position + stride + 1];
                    RBelow = pixelsSrc[position + stride + 2];

                    colorDifferenceR = (int)Math.Round(Math.Sqrt(Math.Pow(R0 - RRight, 2d) + Math.Pow(G0 - GRight, 2d) + Math.Pow(B0 - BRight, 2d)));

                    colorDifferenceB = (int)Math.Round(Math.Sqrt(Math.Pow(R0 - RBelow, 2d) + Math.Pow(G0 - GBelow, 2d) + Math.Pow(B0 - BBelow, 2d)));

                    if (colorDifferenceR > nThreshold || colorDifferenceB > nThreshold)
                    {
                        // make the pixel white
                        pixelsTarget[position] = 255;
                        pixelsTarget[position + 1] = 255;
                        pixelsTarget[position + 2] = 255;
                    }

                    // On the next pixel, its R0 is the current RR
                    // so Set it now and we wont have to Get it later
                    R0 = RRight;
                    G0 = GRight;
                    B0 = BRight;
                }
            }

            Marshal.Copy(pixelsTarget, 0, scan0Target, pixelsTarget.Length);
            bTarget.UnlockBits(bmTarget);
            return bTarget;

        }
    }
}