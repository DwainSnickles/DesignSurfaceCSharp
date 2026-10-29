Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal


Public Class ColorFilter

    ' For color we add an R/G/B level to the R/G/B for each pixel.

    'see "inverse" for comments on the way we are reading and writing the data
    'and an expanation of why we do what we do for the indexed colors.

    Public Shared Function Process(ByVal b As Bitmap, ByVal levelR As Integer, _
    ByVal levelG As Integer, ByVal levelB As Integer) As Bitmap

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

            ' Here we just add the level selected for each color.
            ' And check they don't go off the top
            red = red + levelR
            green = green + levelG
            blue = blue + levelB

            red = Math.Max(0, red)   ' get 0 or red whichever is bigger
            red = Math.Min(255, red) ' get 255 or red whichever is smaller
            ' so we wont have anything <0 or >255

            green = Math.Max(0, green)
            green = Math.Min(255, green)

            blue = Math.Max(0, blue)
            blue = Math.Min(255, blue)

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