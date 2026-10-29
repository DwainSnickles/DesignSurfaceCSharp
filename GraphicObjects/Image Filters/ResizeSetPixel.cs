using System;
using System.Drawing;

namespace Microsoft.MSDN.Samples
{

    public class ResizeSetPixel
    {


        public static Bitmap ProcessImage(Bitmap b, int nWidth, int nHeight, bool bBilinear)
        {

            // this is just a straight copy of c.g's code...

            Bitmap bTemp = (Bitmap)b.Clone();
            b = new Bitmap(nWidth, nHeight, bTemp.PixelFormat);
            double nXFactor = bTemp.Width / (double)nWidth;
            double nYFactor = bTemp.Height / (double)nHeight;

            if (!bBilinear)
            {
                // The not very good way:
                // We are looping through the new image size
                // and grabbing the pixel color from a real pixel
                // in the source bitmap
                // The color comes from the pixel found by scaling again
                // and rounding the result.
                for (int x = 0, loopTo = b.Width - 1; x <= loopTo; x++)
                {
                    for (int y = 0, loopTo1 = b.Height - 1; y <= loopTo1; y++)
                        b.SetPixel(x, y, bTemp.GetPixel(Convert.ToInt32(Math.Floor(x * nXFactor)), Convert.ToInt32(Math.Floor(y * nYFactor))));
                }

                return b;
            }
            else
            {
                // The Better Way:
                double fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                var c1 = new Color();
                var c2 = new Color();
                var c3 = new Color();
                var c4 = new Color();
                byte red, green, blue;
                byte b1, b2;
                for (int x = 0, loopTo2 = b.Width - 1; x <= loopTo2; x++)
                {
                    for (int y = 0, loopTo3 = b.Height - 1; y <= loopTo3; y++)
                    {
                        // Setup
                        floor_x = Convert.ToInt32(Math.Floor(x * nXFactor));
                        floor_y = Convert.ToInt32(Math.Floor(y * nYFactor));
                        ceil_x = floor_x + 1;
                        if (ceil_x >= bTemp.Width)
                            ceil_x = floor_x;
                        ceil_y = floor_y + 1;
                        if (ceil_y >= bTemp.Height)
                            ceil_y = floor_y;
                        fraction_x = x * nXFactor - floor_x;
                        fraction_y = y * nYFactor - floor_y;
                        one_minus_x = 1.0d - fraction_x;
                        one_minus_y = 1.0d - fraction_y;
                        c1 = bTemp.GetPixel(floor_x, floor_y);
                        c2 = bTemp.GetPixel(ceil_x, floor_y);
                        c3 = bTemp.GetPixel(floor_x, ceil_y);
                        c4 = bTemp.GetPixel(ceil_x, ceil_y);

                        // Blue
                        b1 = Convert.ToByte(one_minus_x * c1.B + fraction_x * c2.B);
                        b2 = Convert.ToByte(one_minus_x * c3.B + fraction_x * c4.B);
                        blue = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);
                        // Green
                        b1 = Convert.ToByte(one_minus_x * c1.G + fraction_x * c2.G);
                        b2 = Convert.ToByte(one_minus_x * c3.G + fraction_x * c4.G);
                        green = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);
                        // Red
                        b1 = Convert.ToByte(one_minus_x * c1.R + fraction_x * c2.R);
                        b2 = Convert.ToByte(one_minus_x * c3.R + fraction_x * c4.R);
                        red = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);
                        b.SetPixel(x, y, Color.FromArgb(255, red, green, blue));
                    }
                }
                return b;
            }

        }

    }
}