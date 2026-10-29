Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Drawing

Public Class Pixelate2
    Public Shared Function ProcessImage(ByVal bm As Bitmap, ByVal squaresize As Integer) As Bitmap

        Dim x As Integer
        Dim y As Integer
        Dim red, green, blue, alpha As Byte

        Dim bmd As BitmapData = bm.LockBits(New Rectangle(0, 0, bm.Width, bm.Height), _
         System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim pixels(bmd.Stride * bm.Height - 1) As Byte
        Copy(bmd.Scan0, pixels, 0, pixels.Length)

        For y = 0 To bmd.Height - 1 Step squaresize
            For x = 0 To bmd.Width - 1 Step squaresize
                Dim reds As New ArrayList
                Dim greens As New ArrayList
                Dim blues As New ArrayList
                Dim alphas As New ArrayList
                'from the starting pixel x,y
                'GET color values for all the pixels in the
                'square of squaresize
                For yy As Integer = 0 To squaresize - 1
                    For xx As Integer = 0 To squaresize - 1
                        Dim thisPixelX As Integer = x + xx
                        Dim thisPixelY As Integer = y + yy
                        'with some squaresize values, x+xx can go outside the width of the image:
                        If thisPixelX < bm.Width AndAlso thisPixelY < bm.Height Then
                            'get blue in pixel x + xx, y + yy
                            blue = pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)))
                            blues.Add(blue)
                            'get green in pixel x + xx, y + yy
                            green = pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)) + 1)
                            greens.Add(green)
                            'get red in pixel x + xx, y + yy
                            red = pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)) + 2)
                            reds.Add(red)
                            'get alpha in pixel (x+xx, y+yy)
                            alpha = pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)) + 3)
                            alphas.Add(alpha)
                        End If
                    Next
                Next
                'average colors:
                red = averageColor(reds)
                green = averageColor(greens)
                blue = averageColor(blues)
                alpha = averageColor(alphas)
                'from the starting pixel x,y
                'SET color values for all the pixels in the
                'square of squaresize to the average color...
                For xx As Integer = 0 To squaresize - 1
                    For yy As Integer = 0 To squaresize - 1
                        Dim thisPixelX As Integer = x + xx
                        Dim thisPixelY As Integer = y + yy
                        If thisPixelX < bm.Width AndAlso thisPixelY < bm.Height Then
                            'set blue in pixel x + xx, y + yy
                            pixels((bmd.Stride * (y + yy)) + (4 * (x + xx))) = blue
                            'set green in pixel x + xx, y + yy
                            pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)) + 1) = green
                            'set red in pixel x + xx, y + yy
                            pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)) + 2) = red
                            'set alpha
                            pixels((bmd.Stride * (y + yy)) + (4 * (x + xx)) + 3) = alpha
                        End If
                    Next
                Next
            Next
        Next
        Copy(pixels, 0, bmd.Scan0, pixels.Length)
        bm.UnlockBits(bmd)
        Return bm

    End Function

    Public Shared Function averageColor(ByRef colorList As ArrayList) As Byte
        'loop through arraylist items
        'add up all the bytes
        'return the average
        Dim total As Integer
        For i As Integer = 0 To colorList.Count - 1
            total += Convert.ToInt32(colorList(i))
        Next
        Return Convert.ToByte(total / colorList.Count)
    End Function

End Class