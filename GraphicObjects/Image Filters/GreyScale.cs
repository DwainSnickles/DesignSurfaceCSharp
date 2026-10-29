using System;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.Marshal;

namespace Microsoft.MSDN.Samples
{

    public class greyScale
    {

        // Remove the color from an image
        // Greys occur when R=G=B
        // You can use the average of R, G and B for the grey value
        // but you get better results with
        // grey value = (0.299 * red) + (0.587 * green) + (0.114 * blue)
        // which is just a weighted average. 

        // Greyscale2 uses ColorMatrix

        // see "inverse" for comments on the way we are reading and writing the data

        public static Bitmap Process(Bitmap b)
        {

            // To get a color on the greyscale you need to have R = G = B
            // you could avrage the R,G and B component of each pixels color
            // and set them to that. But the boss (c.g) says we should set 
            // the R, G and B to:
            // (0.299 * red) + (0.587 * green) + (0.114 * blue)

            var bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            var scan0 = bmd.Scan0;

            int alpha, red, green, blue;


            var pixels = new int[(b.Width * b.Height)];
            Copy(scan0, pixels, 0, pixels.Length);

            // hold the grey value
            int grey;

            // loop through all pixels

            for (int i = 0, loopTo = pixels.Length - 1; i <= loopTo; i++)
            {

                // get the individual color components from the int 32
                // say the color is A=255, R=100, G=200, B=144
                // in hex that is: &HFF64C890
                // in binary it is: 
                // 11111111 01100100 11001000 10010000
                // as an example, to get the Red component we need to shift it right 2 bytes (16 bits):
                // when you use the right shift operator, bits "drop off" the right end of the number:
                // it becomes just: 11111111 01100100 or &HFF64
                // we now need to prune off the high byte, for which we use bitwise AND:
                // 11111111 01100100   &HFF64
                // 00000000 11111111   &H00FF
                // -------------------------- AND
                // 00000000 01100100   &H0064
                // 
                // which is our value for red.
                // 
                // You could do it this way:

                // dim c as color = color.fromargb(&HFF64C890)
                // red = c.R
                // alpha = c.A
                // green = c.G
                // blue = c.B

                // but it is slow.

                alpha = pixels[i] >> 24 & 0xFF;
                red = pixels[i] >> 16 & 0xFF;
                green = pixels[i] >> 8 & 0xFF;
                blue = pixels[i] & 0xFF;

                // This is an optimised greyscale formula, it is supposed to look better for 
                // photos and such, there is not much difference with just averaging the colors.

                // grey is an integer instead of byte as we are going to do << on the byte values.
                grey = Convert.ToInt32(0.299d * red + 0.587d * green + 0.114d * blue);
                // alternative, averaging : 
                // Dim grey As Integer = convert.toint32(R) + convert.toint32(G) + convert.toint32(B ) 
                // grey = grey / 3 ' integer division

                // The recombination is the breakup in reverse.
                // we binary shift A,R and G bits left 
                // and then bitwise OR the results

                pixels[i] = 255 << 24 | grey << 16 | grey << 8 | grey;


            }

            Copy(pixels, 0, scan0, pixels.Length);
            b.UnlockBits(bmd);

            return b;

        }


    }
}