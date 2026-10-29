using System;
// ******************************************************************
// ***************Copyright Bobo Enterprises 2005********************
// ***************Written by MrBobo**********************************
// This code was submitted to Planet Source Code (www.planetsourcecode.com)
// If you downloaded it elsewhere, they stole it and I'll eat them alive


using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.MSDN.Samples
{

    static class ModGen
    {
        // API used to create an offscreen bitmap
        [DllImport("gdi32", EntryPoint = "CreateDCA")]
        private static extern int CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, string lpInitData);
        [DllImport("GDI32")]
        private static extern int CreateCompatibleDC(int hDC);
        [DllImport("GDI32")]
        private static extern int CreateCompatibleBitmap(int hDC, int nWidth, int nHeight);
        [DllImport("GDI32")]
        private static extern int SelectObject(int hDC, int hObject);
        [DllImport("GDI32")]
        private static extern int DeleteObject(int hObj);
        [DllImport("GDI32")]
        private static extern int BitBlt(int hDestDC, int X, int Y, int nWidth, int nHeight, int hSrcDC, int SrcX, int SrcY, int Rop);
        [DllImport("GDI32")]
        private static extern int DeleteDC(int hDC);
        private const int SRCCOPY = 0xCC0020;
        // Draw a pallette - like the commondialog color dialog
        public static Bitmap BuildPalette()
        {
            int zx;
            int zy;
            var bmp = new Bitmap(240, 240);
            for (zx = 0; zx <= 239; zx++)
            {
                for (zy = 239; zy >= 0; zy -= 1)
                    bmp.SetPixel(zx, zy, ColorFromHSB(zx, 239 - zy, 120f));
            }
            return bmp;
        }
        public static Bitmap BuildScreen()
        {
            int hDesktopDC;
            int hOffscreenDC;
            int hBitmap;
            int hOldBmp;
            Bitmap bmp = null;
            string arglpDriverName = "DISPLAY";
            string arglpDeviceName = "";
            string arglpOutput = "";
            string arglpInitData = "";
            // removed 'ref' keywords
            hDesktopDC = ModGen.CreateDC(arglpDriverName, arglpDeviceName, arglpOutput, arglpInitData);
             hOffscreenDC = CreateCompatibleDC(hDesktopDC);
            if (Conversions.ToBoolean(hDesktopDC))
            {
                hOffscreenDC = CreateCompatibleDC(hDesktopDC);
                if (Conversions.ToBoolean(hOffscreenDC))
                {
                    hBitmap = CreateCompatibleBitmap(hDesktopDC, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    if (Conversions.ToBoolean(hBitmap))
                    {
                        hOldBmp = SelectObject(hOffscreenDC, hBitmap);
                        BitBlt(hOffscreenDC, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, hDesktopDC, 0, 0, SRCCOPY);
                        bmp = Image.FromHbitmap(new IntPtr(hBitmap));
                        DeleteObject(SelectObject(hOffscreenDC, hOldBmp));
                    }
                    DeleteDC(hOffscreenDC);
                }
                DeleteDC(hDesktopDC);
            }
            return bmp;
        }

        public static void VerticalLabel(PictureBox TargetBox, string lblText)
        {
            // Printing text vertically
            var TextBrush = new SolidBrush(SystemColors.MenuText);
            TargetBox.Image = new Bitmap(TargetBox.Width, TargetBox.Height);
            var g = Graphics.FromImage(TargetBox.Image);
            var sFormat = new StringFormat();
            sFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            g.DrawString(lblText, TargetBox.Font, TextBrush, 0f, 0f, sFormat);
            TargetBox.Invalidate();
            TextBrush.Dispose();
            g.Dispose();
            sFormat.Dispose();
        }
        public static Color ColorFromHSB(float Hue, float Saturation, float Brightness, bool UseNETHSB = false)
        {
            // This is a slightly modified version of code submitted by Morten Kvistgaard
            // I found this routine more efficient than the one I was using previously
            if (!UseNETHSB)
            {
                Hue = (float)(Hue * (360d / 240d));
                Saturation /= 240f;
                Brightness /= 240f;
            }
            if (Brightness == 0f)
            {
                return Color.Black;
            }
            else if (Saturation == 0f)
            {
                return Color.FromArgb((int)Math.Round(255f * Brightness), (int)Math.Round(255f * Brightness), (int)Math.Round(255f * Brightness));
            }
            else
            {
                int i;
                double temp1;
                double temp2;
                Hue /= 360f;
                double[] t3 = new double[] { (double)Hue + 1.0d / 3.0d, Hue, (double)Hue - 1.0d / 3.0d };
                double[] clr = new double[] { 0d, 0d, 0d };
                if ((double)Brightness <= 0.5d)
                {
                    temp2 = (double)Brightness * (1.0d + (double)Saturation);
                }
                else
                {
                    temp2 = (double)(Brightness + Saturation - Brightness * Saturation);
                }
                temp1 = 2.0d * (double)Brightness - temp2;
                for (i = 0; i <= 2; i++)
                {
                    if (t3[i] < 0d)
                        t3[i] += 1.0d;
                    if (t3[i] > 1d)
                        t3[i] -= 1.0d;
                    if (6.0d * t3[i] < 1.0d)
                    {
                        clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0d;
                    }
                    else if (2.0d * t3[i] < 1.0d)
                    {
                        clr[i] = temp2;
                    }
                    else if (3.0d * t3[i] < 2.0d)
                    {
                        clr[i] = temp1 + (temp2 - temp1) * (2.0d / 3.0d - t3[i]) * 6.0d;
                    }
                    else
                    {
                        clr[i] = temp1;
                    }
                }
                return Color.FromArgb((int)Math.Round(255d * clr[0]), (int)Math.Round(255d * clr[1]), (int)Math.Round(255d * clr[2]));
            }
        }

    }
}