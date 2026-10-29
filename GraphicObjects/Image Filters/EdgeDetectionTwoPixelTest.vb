Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class EdgeDetectionTwoPixelTest

    ' see
    ' http://www.gamedev.net/reference/articles/article2007.asp
    ' This is algorithm1 - edge detection.
    ' Here we get each pixels RGB
    ' and the RGB for its R hand neighbour
    ' and the RGB for its downstairs neighbour
    ' We then calculate the COLOR DIFFERENCE
    ' and if it is above a certain threshold we have an edge
    ' if it is an edge we make it white
    ' otherwise black.


    Public Shared Function ProcessImage(ByVal b As Bitmap, ByVal nThreshold As Integer) As Bitmap

        ' Draw the new bitmap in this:
        Dim bTarget As New Bitmap(b.Width, b.Height)

        'set the background to black
        Dim g As Graphics = Graphics.FromImage(bTarget)
        g.Clear(Color.Black)
        g.Dispose()

        'lock bits
        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmTarget As BitmapData = bTarget.LockBits(New Rectangle(0, 0, bTarget.Width, bTarget.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        'width of bitmap row in memory
        Dim stride As Integer = bmData.Stride

        'pointers to the start of the bitmap data
        Dim scan0Src As IntPtr = bmData.Scan0
        Dim scan0Target As IntPtr = bmTarget.Scan0

        'arrays to hold the color values
        Dim pixelsSrc(stride * b.Height - 1) As Byte
        Dim pixelsTarget(stride * bTarget.Height - 1) As Byte

        'fill the arrays
        Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length)
        Marshal.Copy(scan0Target, pixelsTarget, 0, pixelsTarget.Length)

        'don't need the srcBitmap as we have the data in the array now
        b.UnlockBits(bmData)
        b.Dispose()

        'store position of current pixel
        Dim position As Integer

        ' store the current pixels ARGB:
        Dim R0, G0, B0 As Integer
        ' R hand neighbour:
        Dim RRight, GRight, BRight As Integer
        ' Pixel Below:
        Dim RBelow, GBelow, BBelow As Integer

        'store the color difference between current pixel and R pixel
        Dim colorDifferenceR As Integer
        ' and the pixel below
        Dim colorDifferenceB As Integer

        For y As Integer = 0 To bTarget.Height - 2
            ' Get the first pixels RGB in the outer loop
            ' This will only happen at the start of a row, we get the other pixels R0, G0, B0 later

            B0 = pixelsSrc(y * stride)
            G0 = pixelsSrc((y * stride) + 1)
            R0 = pixelsSrc((y * stride) + 2)
            For x As Integer = 0 To bTarget.Width - 2

                'position of current pixel's first byte:
                position = (y * stride) + (x * 4)

                'colors for the Right pixel
                BRight = pixelsSrc(position + 4)
                GRight = pixelsSrc(position + 5)
                RRight = pixelsSrc(position + 6)

                'colors for the pixel beneath
                BBelow = pixelsSrc(position + stride)
                GBelow = pixelsSrc(position + stride + 1)
                RBelow = pixelsSrc(position + stride + 2)

                colorDifferenceR = _
                Math.Sqrt(((R0 - RRight) ^ 2) + ((G0 - GRight) ^ 2) + ((B0 - BRight) ^ 2))

                colorDifferenceB = _
                Math.Sqrt(((R0 - RBelow) ^ 2) + ((G0 - GBelow) ^ 2) + ((B0 - BBelow) ^ 2))

                If colorDifferenceR > nThreshold OrElse colorDifferenceB > nThreshold Then
                    ' make the pixel white
                    pixelsTarget(position) = 255
                    pixelsTarget(position + 1) = 255
                    pixelsTarget(position + 2) = 255
                End If

                ' On the next pixel, its R0 is the current RR
                ' so Set it now and we wont have to Get it later
                R0 = RRight
                G0 = GRight
                B0 = BRight
            Next
        Next

        Marshal.Copy(pixelsTarget, 0, scan0Target, pixelsTarget.Length)
        bTarget.UnlockBits(bmTarget)
        Return bTarget

    End Function
End Class
