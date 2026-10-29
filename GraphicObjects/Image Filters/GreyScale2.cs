using System.Drawing;
using System.Drawing.Imaging;

namespace Microsoft.MSDN.Samples
{

    public class greyScale2
    {

        // Remove the color from the image
        // Do it with a colormatrix

        public static Bitmap ProcessImage(ref Bitmap _sourceBitmap)
        {

            Bitmap _resultBitmap;
            var ia = new ImageAttributes();
            float[][] colorMatrixElements = new[] { new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f }, new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f }, new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } };





            var colorMatrix = new ColorMatrix(colorMatrixElements);
            ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Bitmap);
            _resultBitmap = new Bitmap(_sourceBitmap.Width, _sourceBitmap.Height);
            var g = Graphics.FromImage(_resultBitmap);

            ia.SetColorMatrix(colorMatrix);
            g.DrawImage(_sourceBitmap, new Rectangle(0, 0, _sourceBitmap.Width, _sourceBitmap.Height), 0, 0, _sourceBitmap.Width, _sourceBitmap.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            return _resultBitmap;

        }

    }
}