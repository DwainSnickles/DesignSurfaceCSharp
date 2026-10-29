Imports System.Drawing.Imaging
Imports System.Drawing

Public Class ResizeSetPixel


    Public Shared Function ProcessImage(ByVal b As Bitmap, _
    ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bBilinear As Boolean) As Bitmap

        ' this is just a straight copy of c.g's code...

        Dim bTemp As Bitmap = b.Clone
        b = New Bitmap(nWidth, nHeight, bTemp.PixelFormat)
        Dim nXFactor As Double = bTemp.Width / nWidth
        Dim nYFactor As Double = bTemp.Height / nHeight

        If Not bBilinear Then
            ' The not very good way:
            ' We are looping through the new image size
            ' and grabbing the pixel color from a real pixel
            ' in the source bitmap
            ' The color comes from the pixel found by scaling again
            '  and rounding the result.
            For x As Integer = 0 To b.Width - 1
                For y As Integer = 0 To b.Height - 1
                    b.SetPixel(x, y, bTemp.GetPixel(Convert.ToInt32(Math.Floor(x * nXFactor)), _
                          Convert.ToInt32(Math.Floor(y * nYFactor))))
                Next
            Next

            Return b
        Else
            ' The Better Way:
            Dim fraction_x, fraction_y, one_minus_x, one_minus_y As Double
            Dim ceil_x, ceil_y, floor_x, floor_y As Integer
            Dim c1 As Color = New Color
            Dim c2 As Color = New Color
            Dim c3 As Color = New Color
            Dim c4 As Color = New Color
            Dim red, green, blue As Byte
            Dim b1, b2 As Byte
            For x As Integer = 0 To b.Width - 1
                For y As Integer = 0 To b.Height - 1
                    'Setup
                    floor_x = Convert.ToInt32(Math.Floor(x * nXFactor))
                    floor_y = Convert.ToInt32(Math.Floor(y * nYFactor))
                    ceil_x = floor_x + 1
                    If (ceil_x >= bTemp.Width) Then ceil_x = floor_x
                    ceil_y = floor_y + 1
                    If (ceil_y >= bTemp.Height) Then ceil_y = floor_y
                    fraction_x = x * nXFactor - floor_x
                    fraction_y = y * nYFactor - floor_y
                    one_minus_x = 1.0 - fraction_x
                    one_minus_y = 1.0 - fraction_y
                    c1 = bTemp.GetPixel(floor_x, floor_y)
                    c2 = bTemp.GetPixel(ceil_x, floor_y)
                    c3 = bTemp.GetPixel(floor_x, ceil_y)
                    c4 = bTemp.GetPixel(ceil_x, ceil_y)

                    'Blue
                    b1 = Convert.ToByte(one_minus_x * c1.B + fraction_x * c2.B)
                    b2 = Convert.ToByte(one_minus_x * c3.B + fraction_x * c4.B)
                    blue = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)
                    'Green
                    b1 = Convert.ToByte(one_minus_x * c1.G + fraction_x * c2.G)
                    b2 = Convert.ToByte(one_minus_x * c3.G + fraction_x * c4.G)
                    green = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)
                    'Red
                    b1 = Convert.ToByte(one_minus_x * c1.R + fraction_x * c2.R)
                    b2 = Convert.ToByte(one_minus_x * c3.R + fraction_x * c4.R)
                    red = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)
                    b.SetPixel(x, y, System.Drawing.Color.FromArgb(255, red, green, blue))
                Next
            Next
            Return b
        End If

    End Function

End Class