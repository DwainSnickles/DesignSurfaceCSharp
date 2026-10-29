using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class Brightness
    {

        // Change the brightness
        // If you change each R, G, B by the same amount you alter the 
        // brightness.

        // see "inverse" for comments on the way we are reading and writing the data
        // and an expanation of why we do what we do for the indexed colors.
        // see "greyscale" for comments on splitting the color into A,R,G,B values

        public static Bitmap Process(Bitmap b, int level)
        {

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

                // calculate the new values by adding the level the user has selected
                blue = blue + level;
                green = green + level;
                red = red + level;

                // clip anything that is out of range 0 to 255
                if (blue < 0)
                    blue = 0;
                else
                {
                    if (blue > 255)
                        blue = 255;
                }
                if (green < 0)
                    green = 0;
                else
                {
                    if (green > 255)
                        green = 255;
                }
                if (red < 0)
                    red = 0;
                else
                {
                    if (red > 255)
                        red = 255;
                }

                pixels[i] = 255 << 24 | red << 16 | green << 8 | blue;



            }

            Copy(pixels, 0, scan0, pixels.Length);
            b.UnlockBits(bmd);
            return b;
        }





    }
}