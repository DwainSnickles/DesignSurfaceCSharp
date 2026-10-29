using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.MSDN.Samples
{

    public class EdgeDetectDifference
    {

        public static Bitmap ProcessImage(Bitmap b, int nThreshold)
        {

            // Keep a copy of the original to refer to
            Bitmap bSrc = (Bitmap)b.Clone();

            // lock bits
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // pointers to the start of the bitmap data
            var scan0 = bmData.Scan0;
            var scan0Src = bmDataSrc.Scan0;

            // store the width of 1 row of pixels in memory
            int stride = bmData.Stride;

            // arrays to hold the color values
            var pixelsSrc = new byte[(stride * bSrc.Height)];
            var pixels = new byte[(stride * b.Height)];

            // fill the arrays
            Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length);
            Marshal.Copy(scan0, pixels, 0, pixels.Length);

            // don't need the srcBitmap as we have the data in the array now
            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            // store the max difference
            int maxDifference;

            // and the current difference
            int currentDifference;

            // and the test pixels colors
            int testColor1;
            int testcolor2;

            int index; // position in the array of pixel at (x,y) in the following loop

            // we cant do the edges as pixels are compared with their surrounding pixels, so start at (1,1)
            // instead of (0,0) and run up to width -2, height -2 (as it is 0-based)

            for (int y = 1, loopTo = b.Height - 2; y <= loopTo; y++)
            {
                for (int x = 1, loopTo1 = b.Width - 2; x <= loopTo1; x++)
                {

                    for (int i = 0; i <= 2; i++) // skips alpha which is the last byte (BGRA when using bytes)
                    {
                        // index of pixel at (x,y) 
                        // stride is the number of bytes taken up by 1 row of pixels 
                        // (4 * bitmap.width for this pixelformat), (other pixelformats get rounded up to nearest 4!)

                        index = y * stride + x * 4 + i;


                        // We move through each pixel
                        // and look at the surrounding pixel pairs:
                        // 
                        // 0 1 2      \|/
                        // 3 4 5      -o-
                        // 6 7 8      /|\
                        // 
                        // So current pixel is 4.
                        // we get the color difference between 0 and 8, 2 and 6, 3 and 5, 1 and 7
                        // we keep track of the biggest color difference.
                        // If all the pixels are very much the same then the difference is small
                        // and the color we set 4 to at the end will tend towards black
                        // If there is a big difference then 4 will be set high
                        // If this occurs for all 3 bytes of color then it will tend towards white
                        // and show as an edge.
                        // 
                        // 0 and 8
                        // 0
                        testColor1 = pixelsSrc[index - stride - 4]; // 1 row up, 1 pixel left
                                                                    // 8
                        testcolor2 = pixelsSrc[index + stride + 4]; // 1 row down, 1 pixel right

                        // start by setting maxDifference. anything bigger will overwrite later
                        maxDifference = Math.Abs(testColor1 - testcolor2);

                        // 1 and 7
                        // 1
                        testColor1 = pixelsSrc[index - stride];  // 1 row up
                                                                 // 7
                        testcolor2 = pixelsSrc[index + stride];  // 1 row down
                        currentDifference = Math.Abs(testColor1 - testcolor2);
                        // set max to current if current is bigger
                        maxDifference = Conversions.ToInteger(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // 2 and 6
                        // 2
                        testColor1 = pixelsSrc[index - stride + 4];  // 1 row up, 1 pixel right
                                                                     // 6
                        testcolor2 = pixelsSrc[index + stride - 4]; // 1 row down, 1 pixel left
                        currentDifference = Math.Abs(testColor1 - testcolor2);
                        // set max to current if current is bigger
                        maxDifference = Conversions.ToInteger(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // 3 and 5
                        // 3
                        testColor1 = pixelsSrc[index - 4];   // 1 pixel left
                                                             // 5 
                        testcolor2 = pixelsSrc[index + 4]; // 1 pixel right
                        currentDifference = Math.Abs(testColor1 - testcolor2);
                        // set max to current if current is bigger
                        maxDifference = Conversions.ToInteger(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // If the maxDifference is less than the threshold then zero it

                        if (maxDifference < 0)
                            maxDifference = 0;

                        // Write the color component to the target bitmaps array
                        pixels[index] = (byte)maxDifference;
                    }
                }
            }

            Marshal.Copy(pixels, 0, scan0, pixels.Length);
            b.UnlockBits(bmData);

            return b;

        }
    }
}