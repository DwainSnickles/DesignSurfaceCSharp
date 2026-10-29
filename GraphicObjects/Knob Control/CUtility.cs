using System;
using System.Drawing;

namespace Microsoft.MSDN.Samples
{

    public class CUtility
    {

        public Color getDarkColor(Color c, byte d)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (c.R > d)
                r = (byte)(c.R - d);
            if (c.G > d)
                g = (byte)(c.G - d);
            if (c.B > d)
                b = (byte)(c.B - d);

            var c1 = Color.FromArgb(r, g, b);
            return c1;
        }

        public Color getLightColor(Color c, byte d)
        {
            byte r = 255;
            byte g = 255;
            byte b = 255;

            if (c.R + d <= 255)
                r = (byte)(c.R + d);
            if (c.G + d <= 255)
                g = (byte)(c.G + d);
            if (c.B + d <= 255)
                b = (byte)(c.B + d);

            var c2 = Color.FromArgb(r, g, b);
            return c2;
        }

        // Method which checks is particular point is in rectangle
        // <param name="p">Point to be Chaecked</param>
        // <param name="r">Rectangle</param>
        // <returns>true is Point is in rectangle, else false</returns>
        public bool isPointinRectangle(Point p, Rectangle r)
        {
            bool flag = false;
            if (p.X > r.X & p.X < r.X + r.Width & p.Y > r.Y & p.Y < r.Y + r.Height)
            {
                flag = true;
            }
            return flag;
        }

        public void DrawInsetCircle(ref Graphics g, ref Rectangle r, Pen p)
        {
            int i;
            var p1 = new Pen(getDarkColor(p.Color, 50));
            var p2 = new Pen(getLightColor(p.Color, 50));

            var loopTo = (int)Math.Round(p.Width);
            for (i = 0; i <= loopTo; i++)
            {
                var r1 = new Rectangle(r.X + i, r.Y + i, r.Width - i * 2, r.Height - i * 2);
                g.DrawArc(p2, r1, -45, 180f);
                g.DrawArc(p1, r1, 135f, 180f);
            }
        }

    }
}