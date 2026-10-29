using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Microsoft.MSDN.Samples
{
    public class ColorMatrixes
    {

        // A compilation of uses for the ColorMatrix class
        // in Visual Basic .Net 2003
        // by John Harald Apeland
        // email jh@johnharald.net
        // May 2005

        // Note that the subject is still being researched, and the class may therefore be updated in future.

        // The .Net framework ColorMatrix class provide easy linear color filters. 
        // There exist a scant methodology for some common uses of this to modify color, 
        // best known are scaling and translation with their common contrast and brightness adjustments. 
        // Applying the ColorMatrix as an imageattribute is really fast for image editing. 

        // Best use in image editor is to provide user input in 100-precision, 
        // then take input and divide by 100 before using the functions below. 
        // For rotation input is 0-360 in degrees. 
        // A couple functions is just a plain convertion with no settings, image in - image out. 
        // Limits and defaults are commented in class and also displayed by the example app. 
        // Error handling seems unnecessary since this is an 'internal' class, 
        // and settings limits/defaults are always handled by the programmer. 
        // 
        // For instance a -100 to 100 value brightness scroll, defaults 100, uservalue 110: 
        // picbox.image = classinstanceCM.brightness(freshimage,uservalue/100). 
        // This will apply a syncronized scale of value 1.1 returning a brighter image. 
        // See example app how this is done in all cases of using this class. 

        #region  general purpose 

        public Image apply_cm(Image img, ColorMatrix cm)
        {
            // return new image with given ImageAttributes set
            var bm = new Bitmap(img.Width, img.Height);
            var g = Graphics.FromImage(bm);
            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            return bm;
        }

        // returns degrees in radians
        private double de_ra(double de)
        {
            return de * (Math.PI / 180d);
        }
        private double ra_de(double ra)
        {
            return ra * (180d / Math.PI);
        }

        #endregion

        #region  translate 
        // settings -1 to 1, default 0

        public Image translate(Image img, float a, float r, float g, float b)
        {

            var cm = new ColorMatrix(new float[][] { new float[] { 1f, 0f, 0f, 0f, 0f }, new float[] { 0f, 1f, 0f, 0f, 0f }, new float[] { 0f, 0f, 1f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { r, g, b, a, 1f } });





            return apply_cm(img, cm);
        }

        public Image brightness(Image img, float bri)
        {
            // brightness is a syncronized translation
            return translate(img, 1f, bri, bri, bri);
        }

        #endregion

        #region  scale 
        // settings 0 to 2, default 1

        public Image scale(Image img, float a, float r, float g, float b)
        {

            // There is one small problem with this theory and that is the ColorMatrix has a nasty habit of causing 
            // arithmetic overflows in the images colour values such that simply scaling will cause an undesirable 
            // colourization of the image. Any red, green or blue colour elements having a zero value can overflow 
            // to some large value which causes the pixel to take on a totally different colour. To fix this, and it 
            // is a blatant fudge, we simply need to shift the colours minutely in one direction or another and the 
            // problem goes away. The fudge in the code below offsets the image colours by a measly 0.001, just a tenth 
            // of one percent. This means that our final matrix looks like this... (Bob Powell)

            var cm = new ColorMatrix(new float[][] { new float[] { r, 0f, 0f, 0f, 0f }, new float[] { 0f, g, 0f, 0f, 0f }, new float[] { 0f, 0f, b, 0f, 0f }, new float[] { 0f, 0f, 0f, a, 0f }, new float[] { 0.001f, 0.001f, 0.001f, 0f, 1f } });





            return apply_cm(img, cm);
        }


        public Image contrast(Image img, float con)
        {
            // contrast is a syncronized scale
            return scale(img, 1f, con, con, con);
        }

        public Image invert(Image img)
        {
            // is a scale, but needs the non-translation (1,1,1) for some special zero colors)
            // a "transpose" ?
            var cm = new ColorMatrix(new float[][] { new float[] { -1, 0f, 0f, 0f, 0f }, new float[] { 0f, -1, 0f, 0f, 0f }, new float[] { 0f, 0f, -1, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 1f, 1f, 1f, 0f, 1f } });





            return apply_cm(img, cm);
        }

        #endregion

        #region  saturation 
        // sat(0) img blackwhite (greyscale)
        // sat(1) img unchanged
        // sat(>0-1<) interpolating
        // sat(<0-1>) extrapolating 

        public Image blackwhite(Image img)
        {
            // using saturation formula
            return saturation(img, 0f);
        }

        public Image complimentary(Image img)
        {
            // using saturation formula
            return saturation(img, -1);
        }

        public Image saturation(Image img, float s)
        {
            // http://www.sgi.com/misc/grafica/matrix/
            // // The luminance weight factors for the RGB color space.
            // // These values are actually preferable to the better known factors of
            // // Y = 0.30R + 0.59G + 0.11B, the formula which is used in color television technique.
            const float lumR = 0.3086f;
            const float lumG = 0.6094f;
            const float lumB = 0.082f;
            float a = (float)((1.0d - (double)s) * (double)lumR + (double)s);
            float b = (float)((1.0d - (double)s) * (double)lumR);
            float c = (float)((1.0d - (double)s) * (double)lumR);
            float d = (float)((1.0d - (double)s) * (double)lumG);
            float e = (float)((1.0d - (double)s) * (double)lumG + (double)s);
            float f = (float)((1.0d - (double)s) * (double)lumG);
            float g = (float)((1.0d - (double)s) * (double)lumB);
            float h = (float)((1.0d - (double)s) * (double)lumB);
            float i = (float)((1.0d - (double)s) * (double)lumB + (double)s);

            var cm = new ColorMatrix(new float[][] { new float[] { a, b, c, 0f, 0f }, new float[] { d, e, f, 0f, 0f }, new float[] { g, h, i, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });





            return apply_cm(img, cm);
        }

        #endregion

        #region  rotate 
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdicpp/GDIPlus/usingGDIPlus/recoloring/rotatingcolors.asp

        // r = 0-360 degrees, also known as X rotation
        public Image rotateR(Image img, float r)
        {
            float sin = (float)Math.Sin(de_ra((double)r));
            float cos = (float)Math.Cos(de_ra((double)r));
            var cm = new ColorMatrix(new float[][] { new float[] { 1f, 0f, 0f, 0f, 0f }, new float[] { 0f, cos, sin, 0f, 0f }, new float[] { 0f, -sin, cos, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });





            return apply_cm(img, cm);
        }

        // r = 0-360 degrees, also known as Y rotation
        public Image rotateG(Image img, float r)
        {
            float sin = (float)Math.Sin(de_ra((double)r));
            float cos = (float)Math.Cos(de_ra((double)r));
            var cm = new ColorMatrix(new float[][] { new float[] { cos, 0f, -sin, 0f, 0f }, new float[] { 0f, 1f, 0f, 0f, 0f }, new float[] { sin, 0f, cos, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });





            return apply_cm(img, cm);
        }

        // r = 0 to 360 degrees, also known as Z rotation
        public Image rotateB(Image img, float r)
        {
            float sin = (float)Math.Sin(de_ra((double)r));
            float cos = (float)Math.Cos(de_ra((double)r));
            var cm = new ColorMatrix(new float[][] { new float[] { cos, sin, 0f, 0f, 0f }, new float[] { -sin, cos, 0f, 0f, 0f }, new float[] { 0f, 0f, 1f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });





            return apply_cm(img, cm);
        }

        #endregion

    }
}