using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class ResizeLockBits
    {


        public static Bitmap ProcessImage(Bitmap b, int nWidth, int nHeight, bool bBilinear)
        {



            Bitmap bTemp = (Bitmap)b.Clone();
            b = new Bitmap(nWidth, nHeight, PixelFormat.Format32bppArgb);
            double nXFactor = bTemp.Width / (double)nWidth;
            double nYFactor = bTemp.Height / (double)nHeight;

            var bTempData = bTemp.LockBits(new Rectangle(0, 0, bTemp.Width, bTemp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            var scan0Temp = bTempData.Scan0;
            int strideTemp = bTempData.Stride; // these will vary as the sizes do

            var tempPixels = new byte[(strideTemp * bTemp.Height)];
            Copy(scan0Temp, tempPixels, 0, tempPixels.Length);



            var bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            var scan0b = bData.Scan0;
            int strideb = bData.Stride;

            var bPixels = new byte[(strideb * b.Height)];
            Copy(scan0b, bPixels, 0, bPixels.Length);

            int position;

            if (!bBilinear)
            {

                byte alpha, red, green, blue;
                int xTemp, yTemp;

                for (int x = 0, loopTo = b.Width - 1; x <= loopTo; x++)
                {
                    for (int y = 0, loopTo1 = b.Height - 1; y <= loopTo1; y++)
                    {

                        // set the position in bTemp where we are grabbing the pixel
                        xTemp = Convert.ToInt32(Math.Floor(x * nXFactor));
                        yTemp = Convert.ToInt32(Math.Floor(y * nYFactor));
                        // position in btemp then is
                        position = yTemp * strideTemp + xTemp * 4;
                        // get colors from btemp
                        blue = tempPixels[position];
                        green = tempPixels[position + 1];
                        red = tempPixels[position + 2];
                        alpha = tempPixels[position + 3];
                        // reset position to refer to the target bitmap location
                        position = y * strideb + x * 4;
                        // write to b
                        bPixels[position] = blue;
                        bPixels[position + 1] = green;
                        bPixels[position + 2] = red;
                        bPixels[position + 3] = alpha;
                    }
                }
                bTemp.UnlockBits(bTempData);
                bTemp.Dispose();
                Copy(bPixels, 0, scan0b, bPixels.Length);
                b.UnlockBits(bData);

                return b;
            }
            else
            {
                // The Better Way:
                double fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                // store ARGB info in these:
                var c1 = new int[5];
                var c2 = new int[5];
                var c3 = new int[5];
                var c4 = new int[5];
                byte red, green, blue, alpha;
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

                        // ugly - we need the argb's for four pixels...
                        position = floor_y * strideTemp + floor_x * 4;
                        c1[0] = tempPixels[position];
                        c1[1] = tempPixels[position + 1];
                        c1[2] = tempPixels[position + 2];
                        c1[3] = tempPixels[position + 3];
                        position = floor_y * strideTemp + ceil_x * 4;
                        c2[0] = tempPixels[position];
                        c2[1] = tempPixels[position + 1];
                        c2[2] = tempPixels[position + 2];
                        c2[3] = tempPixels[position + 3];
                        position = ceil_y * strideTemp + floor_x * 4;
                        c3[0] = tempPixels[position];
                        c3[1] = tempPixels[position + 1];
                        c3[2] = tempPixels[position + 2];
                        c3[3] = tempPixels[position + 3];
                        position = ceil_y * strideTemp + ceil_x * 4;
                        c4[0] = tempPixels[position];
                        c4[1] = tempPixels[position + 1];
                        c4[2] = tempPixels[position + 2];
                        c4[3] = tempPixels[position + 3];

                        // Blue
                        b1 = Convert.ToByte(one_minus_x * c1[0] + fraction_x * c2[0]);
                        b2 = Convert.ToByte(one_minus_x * c3[0] + fraction_x * c4[0]);
                        blue = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);
                        // Green
                        b1 = Convert.ToByte(one_minus_x * c1[1] + fraction_x * c2[1]);
                        b2 = Convert.ToByte(one_minus_x * c3[1] + fraction_x * c4[1]);
                        green = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);
                        // Red
                        b1 = Convert.ToByte(one_minus_x * c1[2] + fraction_x * c2[2]);
                        b2 = Convert.ToByte(one_minus_x * c3[2] + fraction_x * c4[2]);
                        red = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);
                        // Alpha ? unsure what to do.

                        b1 = Convert.ToByte(one_minus_x * c1[3] + fraction_x * c2[3]);
                        b2 = Convert.ToByte(one_minus_x * c3[3] + fraction_x * c4[3]);
                        alpha = Convert.ToByte(one_minus_y * b1 + fraction_y * b2);

                        position = y * strideb + x * 4;
                        bPixels[position] = blue;
                        bPixels[position + 1] = green;
                        bPixels[position + 2] = red;
                        bPixels[position + 3] = alpha;
                    }
                }
                bTemp.UnlockBits(bTempData);
                bTemp.Dispose();
                Copy(bPixels, 0, scan0b, bPixels.Length);
                b.UnlockBits(bData);
                return b;
            }

        }


    }
}