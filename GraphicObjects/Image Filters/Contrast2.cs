using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Microsoft.MSDN.Samples
{

    public class Contrast2
    {


        // Change the Contrast
        // Using another algorithm 
        // seen on http://www.gamedev.net/reference/articles/article2007.asp

        public static Bitmap ProcessImage(ref Bitmap source, int level)
        {

            var bmd = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            // scan0 is a pointer to the first pixel in the bitmapData
            var scan0 = bmd.Scan0;

            var pixels = new byte[(bmd.Stride * source.Height)];
            Marshal.Copy(scan0, pixels, 0, pixels.Length);

            // the contrast level coming in is from 0 - 90 degrees
            // we want it in radians

            double contrast = level * (Math.PI / 180d);

            // Position of the current byte
            int position;

            double input;

            // loop through all bytes:
            for (int y = 0, loopTo = bmd.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = bmd.Width - 1; x <= loopTo1; x++)
                {
                    for (int thisByte = 0; thisByte <= 2; thisByte++) // skip alpha
                    {
                        position = y * bmd.Stride + x * 4 + thisByte;

                        // change it to an double so we can do some maths
                        input = pixels[position];

                        // this is from the webpage mentioned at the top
                        // it produces a slightly different image to contrast1
                        double temp = 128.0d * Math.Tan(contrast);
                        if (input < temp + 128.0d && input > 128.0d - temp)
                        {
                            input = (input - 128d) / Math.Tan(contrast) + 128d;
                        }
                        else if (input >= temp + 128.0d)
                        {
                            input = 255d;
                        }
                        else
                        {
                            input = 0d;
                        }
                        // write the new value
                        pixels[position] = Convert.ToByte(input);

                    }
                }
            }
            Marshal.Copy(pixels, 0, scan0, pixels.Length);
            // unlock the bits
            source.UnlockBits(bmd);
            return source;
        }

    }
}