Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Drawing

Public Class Inverse

    ' Invert all color in the image

    Public Shared Function Process(ByVal b As Bitmap) As Bitmap


        ' 32 bit per pixel
        ' 1 byte per color component per pixel (A, R, G, B).
        ' we don't want to change the alpha as normal images would become invisible

        ' The lockbits method locks the bitmap in memory and it won't be garbage collected until unlocked
        ' We get a pointer called scan0 that is the address of the start of the data. 
        ' The first 4 bytes are the color of the top left pixel in the bitmap 
        ' I have been fussy about using 32bpp ARGB to ensure the image is laid out in a predictable way. 
        ' It is also the easiest to use...

        Dim bmd As BitmapData = _
        b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim scan0 As IntPtr = bmd.Scan0
        Dim stride As Integer = bmd.Stride

        ' Here's the speedier method.
        ' define an array to store each pixels color as an int32 
        Dim pixels(b.Width * b.Height - 1) As Integer

        ' Old method used readbyte for each color component of each pixel in a loop.
        ' Instead we copy all the data at once into the array and work with that
        ' this is much faster. (almost as fast as using c#, lockbits, unsafe and pointers

        ' this is system.runtime.interopservices.marshall.copy
        Copy(scan0, pixels, 0, pixels.Length)

        ' loop through all pixels and invert

        For i As Integer = 0 To pixels.Length - 1

            ' calculate the inverse:
            pixels(i) = (Not pixels(i) And &HFFFFFF) Or (pixels(i) And &HFF000000)

        Next

        ' Copy the data back from the array to the locked memory
        Copy(pixels, 0, scan0, pixels.Length)

        ' finally we unlock the bits. 
        b.UnlockBits(bmd)
        Return b

    End Function



End Class