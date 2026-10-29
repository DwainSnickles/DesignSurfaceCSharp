using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{


    public class ColorFilter
    {

        // For color we add an R/G/B level to the R/G/B for each pixel.

        // see "inverse" for comments on the way we are reading and writing the data
        // and an expanation of why we do what we do for the indexed colors.

        public static Bitmap Process(Bitmap b, int levelR, int levelG, int levelB)
        {

            // specify the same pixelformat as the bitmap
            var bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            var scan0 = bmd.Scan0;
            var pixels = new int[(b.Width * b.Height)];
            Copy(scan0, pixels, 0, pixels.Length);
            int alpha, red, green, blue;

            // loop through all pixels

            for (int i = 0, loopTo = pixels.Length - 1; i <= loopTo; i++)
            {


                alpha = pixels[i] >> 24 & 0xFF;
                red = pixels[i] >> 16 & 0xFF;
                green = pixels[i] >> 8 & 0xFF;
                blue = pixels[i] & 0xFF;

                // Here we just add the level selected for each color.
                // And check they don't go off the top
                red = red + levelR;
                green = green + levelG;
                blue = blue + levelB;

                red = Math.Max(0, red);   // get 0 or red whichever is bigger
                red = Math.Min(255, red); // get 255 or red whichever is smaller
                                          // so we wont have anything <0 or >255

                green = Math.Max(0, green);
                green = Math.Min(255, green);

                blue = Math.Max(0, blue);
                blue = Math.Min(255, blue);

                pixels[i] = 255 << 24 | red << 16 | green << 8 | blue;



            }

            Copy(pixels, 0, scan0, pixels.Length);
            // unlock the bits.
            b.UnlockBits(bmd);
            return b;
        }



    }
}