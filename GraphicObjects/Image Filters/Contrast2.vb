Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class Contrast2


    ' Change the Contrast
    ' Using another algorithm 
    ' seen on http://www.gamedev.net/reference/articles/article2007.asp

    Public Shared Function ProcessImage(ByRef source As Bitmap, ByVal level As Integer) As Bitmap

        Dim bmd As BitmapData = _
        source.LockBits(New Rectangle(0, 0, source.Width, source.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        ' scan0 is a pointer to the first pixel in the bitmapData
        Dim scan0 As IntPtr = bmd.Scan0

        Dim pixels(bmd.Stride * source.Height - 1) As Byte
        Marshal.Copy(scan0, pixels, 0, pixels.Length)

        'the contrast level coming in is from 0 - 90 degrees
        ' we want it in radians

        Dim contrast As Double = level * (Math.PI / 180)

        'Position of the current byte
        Dim position As Integer

        Dim input As Double

        ' loop through all bytes:
        For y As Integer = 0 To bmd.Height - 1
            For x As Integer = 0 To bmd.Width - 1
                For thisByte As Integer = 0 To 2 ' skip alpha
                    position = (y * bmd.Stride) + (x * 4) + thisByte

                    'change it to an double so we can do some maths
                    input = pixels(position)

                    'this is from the webpage mentioned at the top
                    'it produces a slightly different image to contrast1
                    Dim temp As Double = 128.0F * Math.Tan(contrast)
                    If input < (temp + 128.0F) AndAlso input > (128.0F - temp) Then
                        input = (input - 128) / Math.Tan(contrast) + 128
                    Else
                        If input >= (temp + 128.0F) Then
                            input = 255
                        Else
                            input = 0
                        End If
                    End If
                    ' write the new value
                    pixels(position) = Convert.ToByte(input)

                Next thisByte
            Next x
        Next y
        Marshal.Copy(pixels, 0, scan0, pixels.Length)
        ' unlock the bits
        source.UnlockBits(bmd)
        Return source
    End Function

End Class