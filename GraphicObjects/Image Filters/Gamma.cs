using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class Gamma
    {

        // Change each pixel to a new color taken from a gamma table

        // see "inverse" for comments on the way we are reading and writing the data
        // and an expanation of why we do what we do for the indexed colors.

        public static Bitmap Process(Bitmap b, double levelR, double levelG, double levelB)
        {

            // First we build up the gamma map 
            // http://www.codeproject.com/cs/media/csharpgraphicfilterspixelformat.Format32bppArgb11.asp
            var redGamma = new byte[256];
            var greenGamma = new byte[256];
            var blueGamma = new byte[256];
            for (int i = 0; i <= 255; i++)
            {

                redGamma[i] = Convert.ToByte(Math.Min(255, Convert.ToInt32(255.0d * Math.Pow(i / 255.0d, 1.0d / levelR) + 0.5d)));

                greenGamma[i] = Convert.ToByte(Math.Min(255, Convert.ToInt32(255.0d * Math.Pow(i / 255.0d, 1.0d / levelG) + 0.5d)));

                blueGamma[i] = Convert.ToByte(Math.Min(255, Convert.ToInt32(255.0d * Math.Pow(i / 255.0d, 1.0d / levelB) + 0.5d)));

            }

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

                // Write the gamma version of the byte
                blue = blueGamma[blue];
                green = greenGamma[green];
                red = redGamma[red];

                pixels[i] = 255 << 24 | red << 16 | green << 8 | blue;


            }

            Copy(pixels, 0, scan0, pixels.Length);
            // unlock the bits.
            b.UnlockBits(bmd);
            return b;
        }



    }
}