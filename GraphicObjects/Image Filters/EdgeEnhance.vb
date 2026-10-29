Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class EdgeEnhance
    'the bulk is the same as edgeDetectDifference

    Public Shared Function ProcessImage(ByVal b As Bitmap, ByVal nThreshold As Integer) As Bitmap

        'Keep a copy of the original to refer to
        Dim bSrc As Bitmap = b.Clone

        'lock bits
        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmDataSrc As BitmapData = bSrc.LockBits(New Rectangle(0, 0, bSrc.Width, bSrc.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        'width of bitmap row in memory
        Dim stride As Integer = bmData.Stride

        'pointers to the start of the bitmap data
        Dim scan0 As IntPtr = bmData.Scan0
        Dim scan0Src As IntPtr = bmDataSrc.Scan0

        'arrays to hold the color values
        Dim pixelsSrc(stride * bSrc.Height - 1) As Byte
        Dim pixels(stride * b.Height - 1) As Byte

        'fill the arrays
        Marshal.Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length)
        Marshal.Copy(scan0, pixels, 0, pixels.Length)

        'don't need the srcBitmap as we have the data in the array now
        bSrc.UnlockBits(bmDataSrc)
        bSrc.Dispose()

        Dim index As Integer ' position in the array of pixel at (x,y) in the following loop


        'store the max difference
        Dim maxDifference As Byte

        'and the current difference
        Dim currentDifference As Byte

        
        'and the test pixels colors
        Dim testColor1 As Integer
        Dim testcolor2 As Integer

        For y As Integer = 1 To b.Height - 2
            For x As Integer = 1 To b.Width - 2
                'Do loop through the color bytes for a given pixel
                For i As Integer = 0 To 2
                    'position of central pixel's byte:
                    index = (y * stride) + (x * 4) + i

                    ' 0 and 8
                    '0
                    testColor1 = pixelsSrc(index - stride - 4) ' 1 row up, 1 pixel left
                    '8
                    testcolor2 = pixelsSrc(index + stride + 4) ' 1 row down, 1 pixel right

                    'start by setting maxDifference. anything bigger will overwrite later
                    maxDifference = Math.Abs(testColor1 - testcolor2)

                    ' 1 and 7
                    ' 1
                    testColor1 = pixelsSrc(index - stride)  ' 1 row up
                    ' 7
                    testcolor2 = pixelsSrc(index + stride)  '1 row down
                    currentDifference = Math.Abs(testColor1 - testcolor2)
                    ' set max to current if current is bigger
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    ' 2 and 6
                    ' 2
                    testColor1 = pixelsSrc(index - stride + 4)  ' 1 row up, 1 pixel right
                    ' 6
                    testcolor2 = pixelsSrc(index + stride - 4) ' 1 row down, 1 pixel left
                    currentDifference = Math.Abs(testColor1 - testcolor2)
                    ' set max to current if current is bigger
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    ' 3 and 5
                    ' 3
                    testColor1 = pixelsSrc(index - 4)   ' 1 pixel left
                    ' 5 
                    testcolor2 = pixelsSrc(index + 4) ' 1 pixel right
                    currentDifference = Math.Abs(testColor1 - testcolor2)
                    ' set max to current if current is bigger
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)
                    'Here is the difference. 
                    'If maxDifference is bigger than the threshold, and bigger
                    ' than the original value of the central pixel
                    ' then set the pixel to maxDifference.

                    If maxDifference > nThreshold AndAlso maxDifference > pixelsSrc(index) Then
                        pixels(index) = maxDifference
                    End If

                Next
            Next
        Next

        Marshal.Copy(pixels, 0, scan0, pixels.Length)
        b.UnlockBits(bmData)

        Return b

    End Function
End Class