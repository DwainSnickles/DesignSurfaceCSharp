using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.MSDN.Samples
{

    public class EdgeEnhance
    {
        // the bulk is the same as edgeDetectDifference

        public static Bitmap ProcessImage(Bitmap b, int nThreshold)
        {

            // Keep a copy of the original to refer to
            Bitmap bSrc = (Bitmap)b.Clone();

            // lock bits
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // width of bitmap row in memory
            int stride = bmData.Stride;

            // pointers to the start of the bitmap data
            var scan0 = bmData.Scan0;
            var scan0Src = bmDataSrc.Scan0;

            // arrays to hold the color values
            var pixelsSrc = new byte[(stride * bSrc.Height)];
            var pixels = new byte[(stride * b.Height)];

            // fill the arrays
            Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length);
            Marshal.Copy(scan0, pixels, 0, pixels.Length);

            // don't need the srcBitmap as we have the data in the array now
            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            int index; // position in the array of pixel at (x,y) in the following loop


            // store the max difference
            byte maxDifference;

            // and the current difference
            byte currentDifference;


            // and the test pixels colors
            int testColor1;
            int testcolor2;

            for (int y = 1, loopTo = b.Height - 2; y <= loopTo; y++)
            {
                for (int x = 1, loopTo1 = b.Width - 2; x <= loopTo1; x++)
                {
                    // Do loop through the color bytes for a given pixel
                    for (int i = 0; i <= 2; i++)
                    {
                        // position of central pixel's byte:
                        index = y * stride + x * 4 + i;

                        // 0 and 8
                        // 0
                        testColor1 = pixelsSrc[index - stride - 4]; // 1 row up, 1 pixel left
                                                                    // 8
                        testcolor2 = pixelsSrc[index + stride + 4]; // 1 row down, 1 pixel right

                        // start by setting maxDifference. anything bigger will overwrite later
                        maxDifference = (byte)Math.Abs(testColor1 - testcolor2);

                        // 1 and 7
                        // 1
                        testColor1 = pixelsSrc[index - stride];  // 1 row up
                                                                 // 7
                        testcolor2 = pixelsSrc[index + stride];  // 1 row down
                        currentDifference = (byte)Math.Abs(testColor1 - testcolor2);
                        // set max to current if current is bigger
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // 2 and 6
                        // 2
                        testColor1 = pixelsSrc[index - stride + 4];  // 1 row up, 1 pixel right
                                                                     // 6
                        testcolor2 = pixelsSrc[index + stride - 4]; // 1 row down, 1 pixel left
                        currentDifference = (byte)Math.Abs(testColor1 - testcolor2);
                        // set max to current if current is bigger
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // 3 and 5
                        // 3
                        testColor1 = pixelsSrc[index - 4];   // 1 pixel left
                                                             // 5 
                        testcolor2 = pixelsSrc[index + 4]; // 1 pixel right
                        currentDifference = (byte)Math.Abs(testColor1 - testcolor2);
                        // set max to current if current is bigger
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));
                        // Here is the difference. 
                        // If maxDifference is bigger than the threshold, and bigger
                        // than the original value of the central pixel
                        // then set the pixel to maxDifference.

                        if (maxDifference > nThreshold && maxDifference > pixelsSrc[index])
                        {
                            pixels[index] = maxDifference;
                        }

                    }
                }
            }

            Marshal.Copy(pixels, 0, scan0, pixels.Length);
            b.UnlockBits(bmData);

            return b;

        }
    }
}