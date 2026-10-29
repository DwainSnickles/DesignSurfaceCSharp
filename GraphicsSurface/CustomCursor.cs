using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.Msdn.Samples.DesignSurface
{


    // here's how to use a bitmap as a custom cursor
    // you can make it neater. this is just a quick example.
    // Dim img As New Bitmap("coda.png")
    // the image can have transparent areas.
    // img.MakeTransparent(Color.White)
    // the parameters for CreateCursor are the image to use
    // + the x + y coordinates of the hotspot
    // Me.Cursor = create.CreateCursor(img, 47, 41)

    public class CustomCursor
    {
        #region    CreateIconIndirect

        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll", EntryPoint = "CreateIconIndirect")]
        private static extern IntPtr CreateIconIndirect(IntPtr iconInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
    /// CreateCursor
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns>custom Cursor</returns>
    /// <remarks>creates a custom cursor from a bitmap</remarks>
        public static Cursor CreateCursor(Bitmap bmp, int xHotspot, int yHotspot)
        {
            // Setup the Cursors IconInfo
            var tmp = new IconInfo();
            tmp.xHotspot = xHotspot;
            tmp.yHotspot = yHotspot;
            tmp.fIcon = false;
            tmp.hbmMask = bmp.GetHbitmap();
            tmp.hbmColor = bmp.GetHbitmap();

            // Create the Pointer for the Cursor Icon
            var pnt = Marshal.AllocHGlobal(Marshal.SizeOf(tmp));
            Marshal.StructureToPtr(tmp, pnt, true);
            var curPtr = CreateIconIndirect(pnt);

            // Clean Up
            DestroyIcon(pnt);
            DeleteObject(tmp.hbmMask);
            DeleteObject(tmp.hbmColor);

            return new Cursor(curPtr);
        }

        #endregion

    }
}