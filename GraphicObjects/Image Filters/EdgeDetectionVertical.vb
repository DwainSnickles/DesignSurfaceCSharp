Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class EdgeDetectionVertical

    Public Shared Function ProcessImage(ByVal b As Bitmap) As Bitmap

        ' make a copy of the source
        Dim bSrc As Bitmap = b.Clone

        ' Now its lockbits time again.
        ' Lock each image:
        Dim bmDataSrc As BitmapData = bSrc.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        ' stride is the width of a row in memory
        Dim stride As Integer = bmData.Stride

        ' store the pointers to the start of the pixel data for each bitmap
        Dim scan0Src As IntPtr = bmDataSrc.Scan0
        Dim scan0 As IntPtr = bmData.Scan0

        'arrays to hold the color values
        Dim pixelsSrc(stride * bSrc.Height - 1) As Byte
        Dim pixels(stride * b.Height - 1) As Byte

        'fill the arrays
        Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length)
        Marshal.Copy(scan0, pixels, 0, pixels.Length)

        'don't need the srcBitmap as we have the data in the array now
        bSrc.UnlockBits(bmDataSrc)
        bSrc.Dispose()

        ' Sum the color values as we move through a grid
        Dim total As Integer

        ' yy loop:
        ' first row is row 0 
        ' can't do top 3 pixels so start at row 4 (y=3)
        ' can't do last 3 pixels so stop at height - 3
        ' first row is 0, so remove another 1 from height:
        For y As Integer = 3 To b.Height - 4

            ' xx loop:
            'looping through pixels in a row

            For x As Integer = 0 To b.Width - 3
                For colorBytes As Integer = 0 To 2 ' loop through blue, green, red, skip alpha
                    '
                    '
                    ' this time the 7x3 grid looks like this: 
                    ' 
                    ' .--------------.
                    ' | -1 |  0 |  1 |
                    ' |--------------|
                    ' | -1 |  0 |  1 |
                    ' |--------------|
                    ' | -1 |  0 |  1 |
                    ' |--------------|
                    ' | -1 |  0 |  1 |
                    ' |--------------|
                    ' | -1 |  0 |  1 |
                    ' |--------------|
                    ' | -1 |  0 |  1 |
                    ' |--------------|
                    ' | -1 |  0 |  1 |
                    ' '--------------'
                    '
                    ' We start by setting x,y to (0,3) = blue in the first pixel, fourth row
                    '  
                    '
                    For yy As Integer = -3 To 3 ' rows in the 3x7 grid relative to our position
                        '                          in the fourth row 

                        For xx As Integer = -1 To 1 Step 2
                            ' xx will loop -1, +1 but add 1 to it later
                            ' middle cells in row are 0 so we can skip them

                            ' Read the color information:

                            ' Current position from scan0 (start of the bitmap memory):
                            Dim position As Integer
                            ' add the number of rows down * width of row
                            position = (y + yy) * stride
                            ' add distance along the current row
                            ' xx needs to refer to 0 or 2 so add 1

                            position += (x + xx + 1) * 4
                            position += colorBytes
                            Dim col1 As Integer = pixelsSrc(position)
                            ' now if we are in the left 3x7 column then xx  = -1 
                            ' and we want to subtract the color value from the total
                            ' if we are in the right column then xx = 1
                            ' and we want to add the color value to the total
                            ' So:
                            total = total + (xx * col1)
                        Next
                    Next

                    If total > 255 Then total = 255
                    If total < 0 Then total = 0
                    pixels((y * stride) + ((x + 1) * 4) + colorBytes) = total

                    ' reset the total for the next byte of color info:
                    ' (interesting effect if you comment this out)
                    total = 0
                Next colorBytes
            Next x
        Next y

        Marshal.Copy(pixels, 0, scan0, pixels.Length)

        b.UnlockBits(bmData)

        Return b

    End Function

End Class