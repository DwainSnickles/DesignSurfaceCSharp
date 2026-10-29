using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.MSDN.Samples
{

    public class EdgeDetectConvolution
    {
        public const short EDGE_DETECT_KIRSH = 1;
        public const short EDGE_DETECT_PREWITT = 2;
        public const short EDGE_DETECT_SOBEL = 3;

        public static Bitmap ProcessImage(Bitmap b, short nType, byte nThreshold)
        {

            var filt = new convolutionFilters();
            var m = new convolutionFilters.ConvMatrix();
            Bitmap bTemp = (Bitmap)b.Clone();

            // First we run the vertical version of whichever filter it is
            // and store in one bitmap:

            switch (nType)
            {
                case EDGE_DETECT_SOBEL:
                    {
                        m.SetAll(0);
                        m[0] = 1;        // 1  0  -1
                        m[2] = -1;       // 2  0  -2
                        m[3] = 2;        // 1  0  -1
                        m[5] = -2;
                        m[6] = 1;
                        m[8] = -1;
                        m.Offset = 0;
                        break;
                    }
                case EDGE_DETECT_PREWITT:
                    {
                        m.SetAll(0);
                        m[0] = -1;       // -1  0  1
                        m[2] = 1;        // -1  0  1
                        m[3] = -1;       // -1  0  1
                        m[5] = 1;
                        m[6] = -1;
                        m[8] = 1;
                        m.Offset = 0;
                        break;
                    }
                case EDGE_DETECT_KIRSH:
                    {
                        m.SetAll(-3);
                        m[4] = 0;        // 5  -3  -3
                        m[0] = 5;        // 5   0  -3
                        m[3] = 5;        // 5  -3  -3
                        m[6] = 5;
                        m.Offset = 0;
                        break;
                    }
            }

            // store in b..
            bool result = false;
            result = filt.conv3x3(b, m);
            // wait for a result...
            // otherwise a big image can crash it (
            do
                Application.DoEvents();
            while (!result);


            // Next run the horizontal version of the filter on the other bitmap
            // which still has the virgin source image.

            switch (nType)
            {
                case EDGE_DETECT_SOBEL:
                    {
                        m.SetAll(0);
                        m[0] = 1;         // 1  2  1
                        m[1] = 2;         // 0  0  0
                        m[2] = 1;         // -1 -2 -1
                        m[6] = -1;
                        m[7] = -2;
                        m[8] = -1;
                        m.Offset = 0;
                        break;
                    }
                case EDGE_DETECT_PREWITT:
                    {
                        m.SetAll(0);
                        m[0] = 1;          // 1  1  1
                        m[1] = 1;          // 0  0  0
                        m[2] = 1;          // -1 -1 -1
                        m[6] = -1;
                        m[7] = -1;
                        m[8] = -1;
                        m.Offset = 0;
                        break;
                    }
                case EDGE_DETECT_KIRSH:
                    {
                        m.SetAll(-3);
                        m[4] = 0;         // -3 -3 -3
                        m[6] = 5;         // -3  0 -3
                        m[7] = 5;         // 5  5  5
                        m[8] = 5;
                        m.Offset = 0;
                        break;
                    }
            }
            result = false;
            result = filt.conv3x3(bTemp, m);
            do
                Application.DoEvents();
            while (!result);

            // now we merge the two bitmaps with pixel = sqrt(pixel1 * pixel1 + pixel2 * pixel2)

            // Lock each image:
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var bmData2 = bTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // store the pointers to the start of the pixel data for each bitmap
            var scan0 = bmData.Scan0;
            var scan02 = bmData2.Scan0;

            // arrays to hold the bitmaps color information:
            var pixels1 = new byte[(4 * b.Width * b.Height)];
            var pixels2 = new byte[(4 * bTemp.Width * bTemp.Height)];

            // fill the arrays
            Marshal.Copy(scan0, pixels1, 0, pixels1.Length);
            Marshal.Copy(scan02, pixels2, 0, pixels2.Length);

            // don't need bTemp as the array has the info
            // do need b as we are going to write to it and return it
            bTemp.UnlockBits(bmData2);
            bTemp.Dispose();

            int nPixel;

            for (int i = 0, loopTo = pixels1.Length - 1; i <= loopTo; i++)
            {
                // calculate the new value
                nPixel = Convert.ToInt32(Math.Sqrt(Math.Pow(pixels1[i], 2d) + Math.Pow(pixels2[i], 2d)));
                // make sure it is in range
                if (nPixel < nThreshold)
                    nPixel = nThreshold;
                if (nPixel > 255)
                    nPixel = 255;
                // write it back to the array
                pixels1[i] = Convert.ToByte(nPixel);
            }

            // write the array back to the bitmap memory data
            Marshal.Copy(pixels1, 0, scan0, pixels1.Length);
            b.UnlockBits(bmData);
            return b;

        }

    }
}