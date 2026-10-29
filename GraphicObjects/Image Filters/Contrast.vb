Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Drawing

Public Class Contrast


    ' Change the Contrast

    'see "inverse" for comments on the way we are reading and writing the data
    'and an expanation of why we do what we do for the indexed colors.
    'see "greyscale" for comments on splitting the color into A,R,G,B values

    Public Shared Function Process(ByVal b As Bitmap, ByVal level As Integer) As Bitmap

        'specify the same pixelformat as the bitmap
        Dim bmd As BitmapData = _
        b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim scan0 As IntPtr = bmd.Scan0

        ' This time we will copy each component into a byte array:
        Dim pixels(4 * (b.Width * b.Height - 1)) As Byte
        Copy(scan0, pixels, 0, pixels.Length)


        ' For the contrast we have used a trackbar that generates a number from -100 to 100
        ' We want a number from 0 to 4
        ' so add 100 to get numbers from 0 to 200
        ' divide by 100 to get numbers from 0 to 2
        ' square to get numbers from 0 to 4
        Dim contrast As Double
        contrast = ((level + 100) / 100) ^ 2

        ' The contrast algorithm uses a color value from 0 to 1
        ' Store it in this:
        Dim pixel As Double

        ' loop through all pixels

        For i As Integer = 0 To pixels.Length - 1

                
            ' turn it into a double between 0 and 1
            pixel = pixels(i) / 255.0
            ' pass it through the contrast algorithm
            pixel -= 0.5
            pixel *= contrast
            pixel += 0.5
            pixel *= 255

            ' clip anything that is out of range
            If pixel < 0 Then pixel = 0 Else If pixel > 255 Then pixel = 255

            ' write it back
            pixels(i) = Convert.ToByte(pixel)

        Next

        Copy(pixels, 0, scan0, pixels.Length)
        ' unlock the bits.
        b.UnlockBits(bmd)

        Return b
    End Function

   


End Class