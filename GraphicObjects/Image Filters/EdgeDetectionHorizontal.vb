Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class EdgeDetectionHorizontal

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

        ' The pixel we set is the one in the middle
        ' So when we put the grid in the top left corner of the bitmap
        '  we are setting pixel(3,1)
        ' This means that there will be 3 pixels untouched on the left and right edges
        '  and 1 untouched at the top and bottom edges.

        ' y loop:
        ' can't do the first row (row 0) so start at 1
        ' b.height is one to big when we start at 0, so remove 1 (b.height -1)
        ' and another -1 as we cant do the last row:
        For y As Integer = 1 To b.Height - 2

            ' x loop:
            ' we need to stop before the end of the row, 
            ' to account for the width of the grid.
            ' looping through each pixel.

            For x As Integer = 0 To b.Width - 7

                ' c.g.'s version starts at byte (9,2) 
                ' 
                ' For the fisrt run through the loop he adds / subtracts the  
                '  color values of pixels like so:
                '
                '   (0,2) + (3,2) + (6,2) + (9,2) + (12,2) + (15,2) + (18,2)  
                ' - (0,0) - (3,0) - (6,0) - (9,0) - (12,0) - (15,0) - (18,0)  
                ' 
                ' These are multiples of three as we want to get every third byte
                '  so that it is the same color component. BGRBGRBGRBGRBGRBGRBGR 
                '                                          ^  ^  ^  ^  ^  ^  ^   
                ' which in effect is using this grid: 
                ' 
                ' .----------------------------------.
                ' | -1 | -1 | -1 | -1 | -1 | -1 | -1 |
                ' |----------------------------------|
                ' |  0 |  0 |  0 |  0 |  0 |  0 |  0 |
                ' |----------------------------------|
                ' |  1 |  1 |  1 |  1 |  1 |  1 |  1 |
                ' '----------------------------------'
                '
                ' and moving it through each byte.
                '
                ' WE start by setting x,y to (0,1) = blue in the first pixel, second row
                '  
                '
                'loop though the B G and R
                For colorByte As Integer = 0 To 2
                    '
                    ' yy loop:
                    ' rows in the 7x3 grid (missing out the middle row)
                    ' relative to the middle row:
                    For yy As Integer = -1 To 1 Step 2
                        ' xx loop:
                        ' across the cells in a 7x3 row
                        ' start at 0 so end at 6

                        For xx As Integer = 0 To 6

                            ' Read the color information:

                            ' Current position from scan0 (start of the bitmap memory):
                            Dim position As Integer
                            position = (y + yy) * stride ' add the number of rows down * width of row
                            position += (x + xx) * 4  ' add the cell position along the current row
                            position += colorByte ' and the current byte
                            Dim col1 As Integer = pixelsSrc(position)
                            ' now if we are in the top 7x3 row then yy  = -1 
                            ' and we want to subtract the color value from the total
                            ' if we are in the bottom row then yy = 1
                            ' and we want to add the color value to the total
                            ' So
                            total = total + (yy * col1)
                        Next
                    Next

                    If total > 255 Then total = 255
                    If total < 0 Then total = 0
                    ' Now total has the value for the grid starting at (x,y) 
                    '  The actual pixel we want to set is the one in the middle of
                    '  We are starting on line 1 anyway, so the y value is fine
                    '  but we need to bump x along by 3 pixels 

                    pixels((y * stride) + ((x + 3) * 4) + colorByte) = total

                    ' reset the total for the next byte of color info:
                    ' (btw: interesting effect if you comment this out)
                    total = 0
                Next
            Next
        Next

        Marshal.Copy(pixels, 0, scan0, pixels.Length)

        b.UnlockBits(bmData)

        Return b

    End Function

End Class