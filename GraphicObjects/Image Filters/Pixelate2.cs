using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class Pixelate2
    {
        public static Bitmap ProcessImage(Bitmap bm, int squaresize)
        {

            int x;
            int y;
            byte red, green, blue, alpha;

            var bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var pixels = new byte[(bmd.Stride * bm.Height)];
            Copy(bmd.Scan0, pixels, 0, pixels.Length);

            var loopTo = bmd.Height - 1;
            for (y = 0; squaresize >= 0 ? y <= loopTo : y >= loopTo; y += squaresize)
            {
                var loopTo1 = bmd.Width - 1;
                for (x = 0; squaresize >= 0 ? x <= loopTo1 : x >= loopTo1; x += squaresize)
                {
                    var reds = new ArrayList();
                    var greens = new ArrayList();
                    var blues = new ArrayList();
                    var alphas = new ArrayList();
                    // from the starting pixel x,y
                    // GET color values for all the pixels in the
                    // square of squaresize
                    for (int yy = 0, loopTo2 = squaresize - 1; yy <= loopTo2; yy++)
                    {
                        for (int xx = 0, loopTo3 = squaresize - 1; xx <= loopTo3; xx++)
                        {
                            int thisPixelX = x + xx;
                            int thisPixelY = y + yy;
                            // with some squaresize values, x+xx can go outside the width of the image:
                            if (thisPixelX < bm.Width && thisPixelY < bm.Height)
                            {
                                // get blue in pixel x + xx, y + yy
                                blue = pixels[bmd.Stride * (y + yy) + 4 * (x + xx)];
                                blues.Add(blue);
                                // get green in pixel x + xx, y + yy
                                green = pixels[bmd.Stride * (y + yy) + 4 * (x + xx) + 1];
                                greens.Add(green);
                                // get red in pixel x + xx, y + yy
                                red = pixels[bmd.Stride * (y + yy) + 4 * (x + xx) + 2];
                                reds.Add(red);
                                // get alpha in pixel (x+xx, y+yy)
                                alpha = pixels[bmd.Stride * (y + yy) + 4 * (x + xx) + 3];
                                alphas.Add(alpha);
                            }
                        }
                    }
                    // average colors:
                    red = averageColor(ref reds);
                    green = averageColor(ref greens);
                    blue = averageColor(ref blues);
                    alpha = averageColor(ref alphas);
                    // from the starting pixel x,y
                    // SET color values for all the pixels in the
                    // square of squaresize to the average color...
                    for (int xx = 0, loopTo4 = squaresize - 1; xx <= loopTo4; xx++)
                    {
                        for (int yy = 0, loopTo5 = squaresize - 1; yy <= loopTo5; yy++)
                        {
                            int thisPixelX = x + xx;
                            int thisPixelY = y + yy;
                            if (thisPixelX < bm.Width && thisPixelY < bm.Height)
                            {
                                // set blue in pixel x + xx, y + yy
                                pixels[bmd.Stride * (y + yy) + 4 * (x + xx)] = blue;
                                // set green in pixel x + xx, y + yy
                                pixels[bmd.Stride * (y + yy) + 4 * (x + xx) + 1] = green;
                                // set red in pixel x + xx, y + yy
                                pixels[bmd.Stride * (y + yy) + 4 * (x + xx) + 2] = red;
                                // set alpha
                                pixels[bmd.Stride * (y + yy) + 4 * (x + xx) + 3] = alpha;
                            }
                        }
                    }
                }
            }
            Copy(pixels, 0, bmd.Scan0, pixels.Length);
            bm.UnlockBits(bmd);
            return bm;

        }

        public static byte averageColor(ref ArrayList colorList)
        {
            // loop through arraylist items
            // add up all the bytes
            // return the average
            var total = default(int);
            for (int i = 0, loopTo = colorList.Count - 1; i <= loopTo; i++)
                total += Convert.ToInt32(colorList[i]);
            return Convert.ToByte(total / (double)colorList.Count);
        }

    }
}