Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Drawing

Public Class Gamma

    'Change each pixel to a new color taken from a gamma table

    'see "inverse" for comments on the way we are reading and writing the data
    'and an expanation of why we do what we do for the indexed colors.

    Public Shared Function Process(ByVal b As Bitmap, _
    ByVal levelR As Double, ByVal levelG As Double, ByVal levelB As Double) As Bitmap

        ' First we build up the gamma map 
        ' http://www.codeproject.com/cs/media/csharpgraphicfilterspixelformat.Format32bppArgb11.asp
        Dim redGamma(255) As Byte
        Dim greenGamma(255) As Byte
        Dim blueGamma(255) As Byte
        For i As Integer = 0 To 255

            redGamma(i) = _
            Convert.ToByte(Math.Min(255, Convert.ToInt32((255.0 * Math.Pow(i / 255.0, 1.0 / levelR)) + 0.5)))

            greenGamma(i) = _
            Convert.ToByte(Math.Min(255, Convert.ToInt32((255.0 * Math.Pow(i / 255.0, 1.0 / levelG)) + 0.5)))

            blueGamma(i) = _
            Convert.ToByte(Math.Min(255, Convert.ToInt32((255.0 * Math.Pow(i / 255.0, 1.0 / levelB)) + 0.5)))

        Next

        'specify the same pixelformat as the bitmap
        Dim bmd As BitmapData = _
        b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim scan0 As IntPtr = bmd.Scan0
        Dim pixels(b.Width * b.Height - 1) As Integer
        Copy(scan0, pixels, 0, pixels.Length)
        Dim alpha, red, green, blue As Integer

        ' loop through all pixels

        For i As Integer = 0 To pixels.Length - 1

            alpha = (pixels(i) >> 24) And &HFF
            red = (pixels(i) >> 16) And &HFF
            green = (pixels(i) >> 8) And &HFF
            blue = pixels(i) And &HFF

            'Write the gamma version of the byte
            blue = blueGamma(blue)
            green = greenGamma(green)
            red = redGamma(red)

            pixels(i) = (255 << 24) _
                      Or (red << 16) _
                      Or (green << 8) _
                      Or blue
        Next

        Copy(pixels, 0, scan0, pixels.Length)
        ' unlock the bits.
        b.UnlockBits(bmd)
        Return b
    End Function

 

End Class