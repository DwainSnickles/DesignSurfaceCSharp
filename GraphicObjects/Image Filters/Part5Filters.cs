using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.MSDN.Samples
{

    public class Part5Filters
    {

        public static bool OffsetFilterAbs(ref Bitmap b, Point[,] offset)
        {

            // absolute...

            // takes an array of points called offset()
            // and a bitmap in.
            // The offset array has the same dimensions as the bitmap
            // say that the first point stored in the offset array 
            // is (10,10). 
            // i.e. offset(0,0) stores point(10,10)
            // we look at point (10,10) in the source bitmap, get the color
            // and then set point(0,0) in the target bitmap.

            Bitmap bSrc = (Bitmap)b.Clone();

            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var Scan0 = bmData.Scan0;
            var SrcScan0 = bmDataSrc.Scan0;

            var pixels = new int[(b.Width * b.Height)];
            var srcPixels = new int[(bSrc.Width * bSrc.Height)];

            Copy(Scan0, pixels, 0, pixels.Length);
            Copy(SrcScan0, srcPixels, 0, srcPixels.Length);

            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            int offsetx, offsety;

            for (int y = 0, loopTo = b.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = b.Width - 1; x <= loopTo1; x++)
                {

                    offsetx = offset[x, y].X;
                    offsety = offset[x, y].Y;

                    // index could go out of range

                    if (offsetx > 0 && offsetx < b.Width - 1 && offsety > 0 && offsety < b.Height - 1)
                    {
                        pixels[y * b.Width + x] = srcPixels[offsetx + offsety * b.Width];
                    }
                }
            }

            Copy(pixels, 0, Scan0, pixels.Length);
            b.UnlockBits(bmData);

            return true;

        }

        public static bool OffsetFilter(ref Bitmap b, Point[,] offset)
        {

            // similar to the function above, but the values in offset are
            // relative to the index of offset. blurgh, by example:
            // if the values in offset(10,10) are (2, -8) then you
            // get the colors from (10 + 2, 10 - 8) and write them
            // into (10,10)

            Bitmap bSrc = (Bitmap)b.Clone();

            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var Scan0 = bmData.Scan0;
            var SrcScan0 = bmDataSrc.Scan0;

            var pixels = new int[(b.Width * b.Height)];
            var srcPixels = new int[(bSrc.Width * bSrc.Height)];

            Copy(Scan0, pixels, 0, pixels.Length);
            Copy(SrcScan0, srcPixels, 0, srcPixels.Length);

            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            int offsetx, offsety;

            for (int y = 0, loopTo = b.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = b.Width - 1; x <= loopTo1; x++)
                {
                    // random jitter can make the value go out of range
                    // skip the test if your implementation keeps values in range...
                    offsetx = x + offset[x, y].X;
                    offsety = y + offset[x, y].Y;
                    if (offsetx > 0 && offsetx < b.Width - 1 && offsety > 0 && offsety < b.Height - 1)
                    {
                        pixels[y * b.Width + x] = srcPixels[offsetx + b.Width * offsety];
                    }
                }
            }

            Copy(pixels, 0, Scan0, pixels.Length);
            b.UnlockBits(bmData);

            return true;
        }

        public struct FloatPoint
        {
            public double X;
            public double Y;
        }

        public static bool OffsetFilterAntiAlias(ref Bitmap b, FloatPoint[,] fp)
        {

            // this one is absolute, and uses the bilinear filter. 

            Bitmap bSrc = (Bitmap)b.Clone();

            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bmDataSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = bmData.Stride;
            var Scan0 = bmData.Scan0;
            var SrcScan0 = bmDataSrc.Scan0;

            var pixels = new byte[(stride * b.Height)];
            var srcPixels = new byte[(stride * bSrc.Height)];

            Copy(Scan0, pixels, 0, pixels.Length);
            Copy(SrcScan0, srcPixels, 0, srcPixels.Length);

            bSrc.UnlockBits(bmDataSrc);
            bSrc.Dispose();

            int xoffset, yoffset;

            double fraction_x, fraction_y, one_minus_x, one_minus_y;

            int ceil_x, ceil_y, floor_x, floor_y;
            byte p1, p2;

            byte color1, color2;

            for (int x = 0, loopTo = b.Width - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = b.Height - 1; y <= loopTo1; y++)
                {

                    xoffset = (int)Math.Round(fp[x, y].X);
                    yoffset = (int)Math.Round(fp[x, y].Y);

                    // Setup

                    floor_x = (int)Math.Round(Math.Floor((decimal)xoffset));
                    floor_y = (int)Math.Round(Math.Floor((decimal)yoffset));
                    ceil_x = floor_x + 1;
                    ceil_y = floor_y + 1;
                    fraction_x = xoffset - floor_x;
                    fraction_y = yoffset - floor_y;
                    one_minus_x = 1.0d - fraction_x;
                    one_minus_y = 1.0d - fraction_y;

                    if (floor_y >= 0 && ceil_y < b.Height && floor_x >= 0 && ceil_x < b.Width)
                    {


                        // blue
                        color1 = srcPixels[floor_y * stride + floor_x * 4];
                        color2 = srcPixels[floor_y * stride + ceil_x * 4];
                        p1 = Convert.ToByte(one_minus_x * color1 + fraction_x * color2);

                        color1 = srcPixels[ceil_y * stride + floor_x * 4];
                        color2 = srcPixels[ceil_y * stride + ceil_x * 4];
                        p2 = Convert.ToByte(one_minus_x * color1 + fraction_x * color2);

                        pixels[y * stride + x * 4] = Convert.ToByte(one_minus_y * p1 + fraction_y * p2);

                        // green
                        color1 = srcPixels[floor_y * stride + floor_x * 4 + 1];
                        color2 = srcPixels[floor_y * stride + ceil_x * 4 + 1];
                        p1 = Convert.ToByte(one_minus_x * color1 + fraction_x * color2);

                        color1 = srcPixels[ceil_y * stride + floor_x * 4 + 1];
                        color2 = srcPixels[ceil_y * stride + ceil_x * 4 + 1];
                        p2 = Convert.ToByte(one_minus_x * color1 + fraction_x * color2);

                        pixels[y * stride + x * 4 + 1] = Convert.ToByte(one_minus_y * p1 + fraction_y * p2);

                        // Red
                        color1 = srcPixels[floor_y * stride + floor_x * 4 + 2];
                        color2 = srcPixels[floor_y * stride + ceil_x * 4 + 2];
                        p1 = Convert.ToByte(one_minus_x * color1 + fraction_x * color2);

                        color1 = srcPixels[ceil_y * stride + floor_x * 4 + 2];
                        color2 = srcPixels[ceil_y * stride + ceil_x * 4 + 2];
                        p2 = Convert.ToByte(one_minus_x * color1 + fraction_x * color2);

                        pixels[y * stride + x * 4 + 2] = Convert.ToByte(one_minus_y * p1 + fraction_y * p2);
                    }
                }
            }
            Copy(pixels, 0, Scan0, pixels.Length);
            b.UnlockBits(bmData);
            return true;
        }

        public static Bitmap Flip(Bitmap b, bool bHorz, bool bVert)
        {

            // fill up a point array with the co-ords of the flipped points

            var ptFlip = new Point[b.Width + 1, b.Height + 1];
            int nWidth = b.Width;
            int nHeight = b.Height;
            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {
                    ptFlip[x, y].X = Conversions.ToInteger(Interaction.IIf(bHorz, nWidth - (x + 1), x));
                    ptFlip[x, y].Y = Conversions.ToInteger(Interaction.IIf(bVert, nHeight - (y + 1), y));
                }
            }

            bool result = false;
            result = OffsetFilterAbs(ref b, ptFlip); // run bitmap b through the offset filter abs
                                                     // so that all of b's pixels get moved to the values we just stored in ptFlip.
            do
                // waiting for calculation to finish before returning.
                Application.DoEvents();
            while (!result);
            return b;

        }

        public static Bitmap RandomJitter(ref Bitmap b, short nDegree)
        {

            // fill the point array up with random distaces 
            // between (-nDegree\2) and (+nDegree\2)

            var ptRandomJitter = new Point[b.Width + 1, b.Height + 1];

            int nWidth = b.Width;
            int nHeight = b.Height;

            int newX, newY;

            short nHalf = (short)Math.Round(Math.Floor(nDegree / 2d));
            var rand = new Random();
            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {
                    newX = rand.Next(nDegree) - nHalf;
                    if (x + newX > 0 && x + newX < nWidth)
                    {
                        ptRandomJitter[x, y].X = newX;
                    }
                    else
                    {
                        ptRandomJitter[x, y].X = 0;
                    }
                    newY = rand.Next(nDegree) - nHalf;

                    if (y + newY > 0 && y + newY < nWidth)
                    {
                        ptRandomJitter[x, y].Y = newY;
                    }
                    else
                    {
                        ptRandomJitter[x, y].Y = 0;
                    }
                }

            }

            bool result = false;
            // we use the offset filter, so the point in b gets moved by the random 
            // distances we have put into ptRandomJitter
            result = OffsetFilter(ref b, ptRandomJitter);
            do
                Application.DoEvents();
            while (!result);

            return b;
        }

        public static Bitmap Swirl(Bitmap b, double fDegree, bool bSmoothing)
        {

            // complicated 
            // I've just translated it...

            int nWidth = b.Width;
            int nHeight = b.Height;

            var fp = new FloatPoint[nWidth + 1, nHeight + 1];
            var pt = new Point[nWidth + 1, nHeight + 1];

            var midl = new Point();
            midl.X = (int)Math.Round(nWidth / 2d);
            midl.Y = (int)Math.Round(nHeight / 2d);

            double theta, radius, newx, newy;
            int trueX, trueY;

            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {
                    trueX = x - midl.X;
                    trueY = y - midl.Y;
                    theta = Math.Atan2(trueY, trueX);
                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);
                    newx = midl.X + radius * Math.Cos(theta + fDegree * radius);
                    if (newx > 0d && newx < nWidth - 1)
                    {
                        fp[x, y].X = newx;
                        pt[x, y].X = Convert.ToInt32(newx);
                    }
                    else
                    {
                        fp[x, y].X = x;
                        pt[x, y].X = x;
                    }
                    newy = midl.Y + radius * Math.Sin(theta + fDegree * radius);
                    if (newy > 0d && newy < nHeight - 1)
                    {
                        fp[x, y].Y = newy;
                        pt[x, y].Y = Convert.ToInt32(newy);
                    }
                    else
                    {
                        fp[x, y].Y = y;
                        pt[x, y].Y = y;
                    }
                }
            }

            bool result = false;

            if (bSmoothing)
            {
                result = OffsetFilterAntiAlias(ref b, fp);
            }
            else
            {
                result = OffsetFilterAbs(ref b, pt);
            }

            do
                Application.DoEvents();
            while (!result);

            return b;

        }

        public static Bitmap Sphere(Bitmap b, bool bSmoothing)
        {

            // complicated 
            // I've just translated it...

            int nWidth = b.Width;
            int nHeight = b.Height;

            var fp = new FloatPoint[nWidth + 1, nHeight + 1];
            var pt = new Point[nWidth + 1, nHeight + 1];

            var midl = new Point();
            midl.X = (int)Math.Round(nWidth / 2d);
            midl.Y = (int)Math.Round(nHeight / 2d);

            double theta, radius, newx, newy, newRadius;
            int trueX, trueY;

            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {
                    trueX = x - midl.X;
                    trueY = y - midl.Y;
                    theta = Math.Atan2(trueY, trueX);
                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);
                    newRadius = radius * radius / Math.Max(midl.X, midl.Y);
                    newx = midl.X + newRadius * Math.Cos(theta);
                    if (newx > 0d && newx < nWidth)
                    {
                        fp[x, y].X = newx;
                        pt[x, y].X = Convert.ToInt32(newx);
                    }
                    else
                    {
                        fp[x, y].X = 0.0d;
                        fp[x, y].Y = 0.0d;
                        pt[0, 0].X = 0;
                        pt[0, 0].Y = 0;
                    }
                    newy = midl.Y + newRadius * Math.Sin(theta);
                    if (newy > 0d && newy < nHeight && newx > 0d && newx < nWidth)
                    {
                        fp[x, y].Y = newy;
                        pt[x, y].Y = Convert.ToInt32(newy);
                    }
                    else
                    {
                        fp[x, y].X = 0.0d;
                        fp[x, y].Y = 0.0d;
                        pt[x, y].X = 0;
                        pt[x, y].Y = 0;
                    }
                }
            }

            bool result = false;

            if (bSmoothing)
            {
                result = OffsetFilterAbs(ref b, pt);
            }
            else
            {
                result = OffsetFilterAntiAlias(ref b, fp);
            }

            do
                Application.DoEvents();
            while (!result);

            return b;
        }


        public static Bitmap TimeWarp(Bitmap b, byte factor, bool bSmoothing)
        {

            // complicated 
            // I've just translated it...

            int nWidth = b.Width;
            int nHeight = b.Height;

            var fp = new FloatPoint[nWidth + 1, nHeight + 1];
            var pt = new Point[nWidth + 1, nHeight + 1];

            var midl = new Point();
            midl.X = (int)Math.Round(nWidth / 2d);
            midl.Y = (int)Math.Round(nHeight / 2d);

            double theta, radius, newx, newy, newRadius;
            int trueX, trueY;

            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {
                    trueX = x - midl.X;
                    trueY = y - midl.Y;
                    theta = Math.Atan2(trueY, trueX);
                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);
                    newRadius = Math.Sqrt(radius) * factor;
                    newx = midl.X + newRadius * Math.Cos(theta);
                    if (newx > 0d && newx < nWidth)
                    {
                        fp[x, y].X = newx;
                        pt[x, y].X = Convert.ToInt32(newx);
                    }
                    else
                    {
                        fp[x, y].X = 0.0d;
                        pt[0, 0].X = 0;
                    }
                    newy = midl.Y + newRadius * Math.Sin(theta);
                    if (newy > 0d && newy < nHeight)
                    {
                        fp[x, y].Y = newy;
                        pt[x, y].Y = Convert.ToInt32(newy);
                    }
                    else
                    {
                        fp[x, y].Y = 0.0d;
                        pt[x, y].Y = 0;
                    }

                }
            }

            bool result = false;

            if (bSmoothing)
            {
                result = OffsetFilterAbs(ref b, pt);
            }
            else
            {
                result = OffsetFilterAntiAlias(ref b, fp);
            }

            do
                Application.DoEvents();
            while (!result);

            return b;

        }

        public static Bitmap Moire(Bitmap b, double fDegree)
        {

            // complicated 
            // I've just translated it...

            int nWidth = b.Width;
            int nHeight = b.Height;

            var pt = new Point[nWidth + 1, nHeight + 1];

            var midl = new Point();
            midl.X = (int)Math.Round(nWidth / 2d);
            midl.Y = (int)Math.Round(nHeight / 2d);

            double theta, radius, newx, newy;
            int trueX, trueY;

            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {
                    trueX = x - midl.X;
                    trueY = y - midl.Y;
                    theta = Math.Atan2(trueX, trueY);
                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);
                    newx = Convert.ToInt32(radius * Math.Sin(theta + fDegree * radius));

                    if (newx > 0d && newx < nWidth)
                    {
                        pt[x, y].X = Convert.ToInt32(newx);
                    }
                    else
                    {
                        pt[0, 0].X = 0;
                    }
                    newy = Convert.ToInt32(radius * Math.Sin(theta + fDegree * radius));
                    if (newy > 0d && newy < nHeight)
                    {
                        pt[x, y].Y = Convert.ToInt32(newy);
                    }
                    else
                    {
                        pt[x, y].Y = 0;
                    }

                }
            }

            bool result = false;
            result = OffsetFilterAbs(ref b, pt);
            do
                Application.DoEvents();
            while (!result);
            return b;

        }

        public static Bitmap Water(Bitmap b, short nWave, bool bSmoothing)
        {

            // bit easier
            // but, I've just translated it...


            int nWidth = b.Width;
            int nHeight = b.Height;
            var fp = new FloatPoint[nWidth + 1, nHeight + 1];
            var pt = new Point[nWidth + 1, nHeight + 1];

            var midl = new Point();
            midl.X = (int)Math.Round(nWidth / 2d);
            midl.Y = (int)Math.Round(nHeight / 2d);

            double newx, newy, xo, yo;

            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {

                    xo = nWave * Math.Sin(2.0d * Math.PI * (y / 128.0d));
                    yo = nWave * Math.Cos(2.0d * Math.PI * (x / 128.0d));
                    newx = x + xo;
                    newy = y + yo;

                    if (newx > 0d && newx < nWidth)
                    {
                        fp[x, y].X = newx;
                        pt[x, y].X = Convert.ToInt32(newx);
                    }
                    else
                    {
                        fp[x, y].X = 0.0d;
                        pt[x, y].X = 0;
                    }
                    if (newy > 0d && newy < nHeight)
                    {
                        fp[x, y].Y = newy;
                        pt[x, y].Y = Convert.ToInt32(newy);
                    }
                    else
                    {
                        fp[x, y].Y = 0.0d;
                        pt[x, y].Y = 0;
                    }
                }
            }

            bool result = false;

            if (bSmoothing)
            {
                result = OffsetFilterAbs(ref b, pt);
            }
            else
            {
                result = OffsetFilterAntiAlias(ref b, fp);
            }

            do
                Application.DoEvents();
            while (!result);

            return b;

        }

        public static Bitmap Pixelate(Bitmap b, short pixel, bool bGrid)
        {

            // grid can look pretty weird as the color is variable on the grid lines
            // I thought it was borked for a while but it was my source image.
            // I'll leave it as it is, but you could use gdi to draw a grid instead.

            // This one stores the offset to the pixel in the top left corner of the
            // current small square. The pixelate2 filter  calculates the
            // average color of pixels in the small square.

            int nWidth = b.Width;
            int nHeight = b.Height;

            var pt = new Point[nWidth + 1, nHeight + 1];

            int newx, newy;

            for (int x = 0, loopTo = nWidth - 1; x <= loopTo; x++)
            {
                for (int y = 0, loopTo1 = nHeight - 1; y <= loopTo1; y++)
                {

                    newx = pixel - x % pixel;

                    if (bGrid && newx == pixel)
                    {
                        pt[x, y].X = -x;
                    }
                    else if (x + newx > 0 && x + newx < nWidth)
                    {
                        pt[x, y].X = newx;
                    }
                    else
                    {
                        pt[x, y].X = 0;
                    }

                    newy = pixel - y % pixel;

                    if (bGrid && newy == pixel)
                    {
                        pt[x, y].Y = -y;
                    }
                    else if (y + newy > 0 && y + newy < nHeight)
                    {
                        pt[x, y].Y = newy;
                    }
                    else
                    {
                        pt[x, y].Y = 0;
                    }
                }
            }

            bool result = false;
            result = OffsetFilter(ref b, pt);
            do
                Application.DoEvents();
            while (!result);
            return b;

        }




    }
}