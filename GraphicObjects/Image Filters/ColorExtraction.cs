using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Microsoft.MSDN.Samples
{

    public class ColorExtraction
    {


        public static Bitmap ProcessImage(ref Bitmap b, Color searchColor, int tolerance)
        {

            // lock bits
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // width of bitmap row in memory
            int stride = bmData.Stride;

            // pointers to the start of the bitmap data
            var scan0 = bmData.Scan0;

            // array to hold the colors
            var pixels = new byte[(stride * b.Height)];
            Marshal.Copy(scan0, pixels, 0, pixels.Length);

            // store position of current pixel
            int position;

            // store the current pixels RGB:
            int R0, G0, B0;
            // store the searchColors RGB
            int R1, G1, B1;
            R1 = searchColor.R;
            G1 = searchColor.G;
            B1 = searchColor.B;

            // store the color difference between current pixel and  searchPixel
            int colorDifference;

            for (int y = 0, loopTo = b.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = b.Width - 1; x <= loopTo1; x++)
                {
                    // position of current pixel's first byte:
                    position = y * stride + x * 4;

                    B0 = pixels[position];
                    G0 = pixels[position + 1];
                    R0 = pixels[position + 2];
                    // calculate the color difference

                    colorDifference = (int)Math.Round(Math.Sqrt(Math.Pow(R0 - R1, 2d) + Math.Pow(G0 - G1, 2d) + Math.Pow(B0 - B1, 2d)));

                    // If the color is close enough, set it to the search color ( could just leave it if you wanted)
                    // if not set it to black

                    if (colorDifference <= tolerance)
                    {
                        // set it to the search color
                        pixels[position] = Convert.ToByte(B1);
                        pixels[position + 1] = Convert.ToByte(G1);
                        pixels[position + 2] = Convert.ToByte(R1);
                    }
                    else
                    {
                        // set it to black
                        pixels[position] = Convert.ToByte(0);
                        pixels[position + 1] = Convert.ToByte(0);
                        pixels[position + 2] = Convert.ToByte(0);
                    }
                }
            }
            Marshal.Copy(pixels, 0, scan0, pixels.Length);

            b.UnlockBits(bmData);

            return (Bitmap)b.Clone();

        }
    }
}