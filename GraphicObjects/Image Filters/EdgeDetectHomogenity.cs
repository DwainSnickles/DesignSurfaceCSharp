using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.MSDN.Samples
{

    public class EdgeDetectHomogenity
    {

        public static Bitmap ProcessImage(Bitmap b, int nThreshold)
        {

            // Keep a copy of the original to refer to
            Bitmap bSrc = (Bitmap)b.Clone();

            // lock bits
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // width of bitmap row in memory
            int stride = bmData.Stride;

            // pointers to the start of the bitmap data
            var scan0Src = bmDataSrc.Scan0;
            var scan0 = bmData.Scan0;

            // Arrays to hold the bytes of color info
            var pixelsSrc = new byte[(stride * bSrc.Height)];
            var pixels = new byte[(stride * b.Height)];

            // copy data:
            Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length);
            Copy(scan0, pixels, 0, pixels.Length);

            // don't need the srcBitmap as we have the data in the array now
            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            // store position of current color component of the current pixel in the array
            int index;

            // store the max difference
            byte maxDifference;

            // and the current difference
            byte currentDifference;

            // and the central pixels color value for this byte
            int col0;

            for (int y = 1, loopTo = b.Height - 2; y <= loopTo; y++)
            {
                for (int x = 1, loopTo1 = b.Width - 2; x <= loopTo1; x++)
                {
                    // loop through the color bytes for a given pixel skipping alpha (BGRA when doing bytes)
                    for (int i = 0; i <= 2; i++)
                    {
                        // position of central pixel's byte in the array:
                        index = y * stride + x * 4 + i;
                        // central pixels color
                        col0 = pixelsSrc[index];

                        // Calculate the differences between col0 and the
                        // corresponding byte in the pixel's 8 neighbours
                        // We want the largest difference so keep a running
                        // highscore, and test each as against it as we go

                        // pixel above and left is one stride up, 4 bytes back
                        // start by setting the highscore
                        maxDifference = (byte)Math.Abs(col0 - pixelsSrc[index - stride - 4]);

                        // pixel above is one stride up----------------vvvvvvvvvvvvvv
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index - stride]);

                        // set the highest difference from the two so far.
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // third one is the pixel above and right
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index - stride + 4]);
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // bug in c.g's version, he misses the MR and ML pixels and does
                        // TM and TB twice instead.

                        // fourth one is ML (middle row, left col)
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index - 4]);
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // MR
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index + 4]);
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // BL
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index + stride - 4]);
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // BM
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index + stride]);
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // BR
                        currentDifference = (byte)Math.Abs(col0 - pixelsSrc[index + stride + 4]);
                        maxDifference = Conversions.ToByte(Interaction.IIf(maxDifference > currentDifference, maxDifference, currentDifference));

                        // phew.

                        // If the maxDifference is less than the threshold then zero it
                        if (maxDifference < 0)
                            maxDifference = 0;

                        // Write the pixel to the targer bitmap
                        pixels[index] = maxDifference;
                    }

                }
            }

            // copy array back into bitmap
            Copy(pixels, 0, scan0, pixels.Length);
            b.UnlockBits(bmData);
            return b;

        }



    }
}