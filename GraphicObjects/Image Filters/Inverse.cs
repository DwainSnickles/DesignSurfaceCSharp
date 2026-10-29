using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class Inverse
    {

        // Invert all color in the image

        public static Bitmap Process(Bitmap b)
        {


            // 32 bit per pixel
            // 1 byte per color component per pixel (A, R, G, B).
            // we don't want to change the alpha as normal images would become invisible

            // The lockbits method locks the bitmap in memory and it won't be garbage collected until unlocked
            // We get a pointer called scan0 that is the address of the start of the data. 
            // The first 4 bytes are the color of the top left pixel in the bitmap 
            // I have been fussy about using 32bpp ARGB to ensure the image is laid out in a predictable way. 
            // It is also the easiest to use...

            var bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            var scan0 = bmd.Scan0;
            int stride = bmd.Stride;

            // Here's the speedier method.
            // define an array to store each pixels color as an int32 
            var pixels = new int[(b.Width * b.Height)];

            // Old method used readbyte for each color component of each pixel in a loop.
            // Instead we copy all the data at once into the array and work with that
            // this is much faster. (almost as fast as using c#, lockbits, unsafe and pointers

            // this is system.runtime.interopservices.marshall.copy
            Copy(scan0, pixels, 0, pixels.Length);

            // loop through all pixels and invert

            for (int i = 0, loopTo = pixels.Length - 1; i <= loopTo; i++)

                // calculate the inverse:

                pixels[i] = (~pixels[i] & 0xFFFFFF | (((pixels[i] & (unchecked((int)0xFF000000))))));

            // Copy the data back from the array to the locked memory
            Copy(pixels, 0, scan0, pixels.Length);

            // finally we unlock the bits. 
            b.UnlockBits(bmd);
            return b;

        }



    }
}