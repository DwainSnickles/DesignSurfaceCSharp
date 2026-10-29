using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class Contrast
    {


        // Change the Contrast

        // see "inverse" for comments on the way we are reading and writing the data
        // and an expanation of why we do what we do for the indexed colors.
        // see "greyscale" for comments on splitting the color into A,R,G,B values

        public static Bitmap Process(Bitmap b, int level)
        {

            // specify the same pixelformat as the bitmap
            var bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            var scan0 = bmd.Scan0;

            // This time we will copy each component into a byte array:
            var pixels = new byte[4 * (b.Width * b.Height - 1) + 1];
            Copy(scan0, pixels, 0, pixels.Length);


            // For the contrast we have used a trackbar that generates a number from -100 to 100
            // We want a number from 0 to 4
            // so add 100 to get numbers from 0 to 200
            // divide by 100 to get numbers from 0 to 2
            // square to get numbers from 0 to 4
            double contrast;
            contrast = Math.Pow((level + 100) / 100d, 2d);

            // The contrast algorithm uses a color value from 0 to 1
            // Store it in this:
            double pixel;

            // loop through all pixels

            for (int i = 0, loopTo = pixels.Length - 1; i <= loopTo; i++)
            {


                // turn it into a double between 0 and 1
                pixel = pixels[i] / 255.0d;
                // pass it through the contrast algorithm
                pixel -= 0.5d;
                pixel *= contrast;
                pixel += 0.5d;
                pixel *= 255d;

                // clip anything that is out of range
                if (pixel < 0d)
                    pixel = 0d;
                else
                {
                    if (pixel > 255d)
                        pixel = 255d;
                }

                // write it back
                pixels[i] = Convert.ToByte(pixel);

            }

            Copy(pixels, 0, scan0, pixels.Length);
            // unlock the bits.
            b.UnlockBits(bmd);

            return b;
        }




    }
}