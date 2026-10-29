Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal

Public Class Brightness

    ' Change the brightness
    ' If you change each R, G, B by the same amount you alter the 
    ' brightness.

    'see "inverse" for comments on the way we are reading and writing the data
    'and an expanation of why we do what we do for the indexed colors.
    'see "greyscale" for comments on splitting the color into A,R,G,B values

    Public Shared Function Process(ByVal b As Bitmap, ByVal level As Integer) As Bitmap

        Dim bmd As BitmapData =
        b.LockBits(New Rectangle(0, 0, b.Width, b.Height),
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

            ' calculate the new values by adding the level the user has selected
            blue = blue + level
            green = green + level
            red = red + level

            ' clip anything that is out of range 0 to 255
            If blue < 0 Then blue = 0 Else If blue > 255 Then blue = 255
            If green < 0 Then green = 0 Else If green > 255 Then green = 255
            If red < 0 Then red = 0 Else If red > 255 Then red = 255

            pixels(i) = (255 << 24) _
                      Or (red << 16) _
                      Or (green << 8) _
                      Or blue

        Next

        Copy(pixels, 0, scan0, pixels.Length)
        b.UnlockBits(bmd)
        Return b
    End Function





End Class
