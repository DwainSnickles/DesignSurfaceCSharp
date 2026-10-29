Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class ColorExtraction


    Public Shared Function ProcessImage(ByRef b As Bitmap, ByVal searchColor As Color,
    ByVal tolerance As Integer) As Bitmap

        'lock bits
        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height),
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        'width of bitmap row in memory
        Dim stride As Integer = bmData.Stride

        'pointers to the start of the bitmap data
        Dim scan0 As IntPtr = bmData.Scan0

        ' array to hold the colors
        Dim pixels(stride * b.Height - 1) As Byte
        Marshal.Copy(scan0, pixels, 0, pixels.Length)

        'store position of current pixel
        Dim position As Integer

        ' store the current pixels RGB:
        Dim R0, G0, B0 As Integer
        ' store the searchColors RGB
        Dim R1, G1, B1 As Integer
        R1 = searchColor.R
        G1 = searchColor.G
        B1 = searchColor.B

        'store the color difference between current pixel and  searchPixel
        Dim colorDifference As Integer

        For y As Integer = 0 To b.Height - 1
            For x As Integer = 0 To b.Width - 1
                'position of current pixel's first byte:
                position = (y * stride) + (x * 4)

                B0 = pixels(position)
                G0 = pixels(position + 1)
                R0 = pixels(position + 2)
                'calculate the color difference

                colorDifference = _
                Math.Sqrt(((R0 - R1) ^ 2) + ((G0 - G1) ^ 2) + ((B0 - B1) ^ 2))

                'If the color is close enough, set it to the search color ( could just leave it if you wanted)
                'if not set it to black

                If colorDifference <= tolerance Then
                    ' set it to the search color
                    pixels(position) = Convert.ToByte(B1)
                    pixels(position + 1) = Convert.ToByte(G1)
                    pixels(position + 2) = Convert.ToByte(R1)
                Else
                    ' set it to black
                    pixels(position) = Convert.ToByte(0)
                    pixels(position + 1) = Convert.ToByte(0)
                    pixels(position + 2) = Convert.ToByte(0)
                End If
            Next
        Next
        Marshal.Copy(pixels, 0, scan0, pixels.Length)

        b.UnlockBits(bmData)

        Return b.Clone

    End Function
End Class
