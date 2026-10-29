Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Drawing

Public Class EdgeDetectHomogenity

    Public Shared Function ProcessImage(ByVal b As Bitmap, ByVal nThreshold As Integer) As Bitmap

        'Keep a copy of the original to refer to
        Dim bSrc As Bitmap = b.Clone

        'lock bits
        Dim bmDataSrc As BitmapData = bSrc.LockBits(New Rectangle(0, 0, bSrc.Width, bSrc.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
       
        'width of bitmap row in memory
        Dim stride As Integer = bmData.Stride

        'pointers to the start of the bitmap data
        Dim scan0Src As IntPtr = bmDataSrc.Scan0
        Dim scan0 As IntPtr = bmData.Scan0

        'Arrays to hold the bytes of color info
        Dim pixelsSrc(stride * bSrc.Height - 1) As Byte
        Dim pixels(stride * b.Height - 1) As Byte

        'copy data:
        Copy(scan0Src, pixelsSrc, 0, pixelsSrc.Length)
        Copy(scan0, pixels, 0, pixels.Length)

        'don't need the srcBitmap as we have the data in the array now
        bSrc.UnlockBits(bmDataSrc)
        bSrc.Dispose()

        'store position of current color component of the current pixel in the array
        Dim index As Integer

        'store the max difference
        Dim maxDifference As Byte

        'and the current difference
        Dim currentDifference As Byte

        'and the central pixels color value for this byte
        Dim col0 As Integer

        For y As Integer = 1 To b.Height - 2
            For x As Integer = 1 To b.Width - 2
                'loop through the color bytes for a given pixel skipping alpha (BGRA when doing bytes)
                For i As Integer = 0 To 2
                    'position of central pixel's byte in the array:
                    index = (y * stride) + (x * 4) + i
                    'central pixels color
                    col0 = pixelsSrc(index)

                    'Calculate the differences between col0 and the
                    ' corresponding byte in the pixel's 8 neighbours
                    'We want the largest difference so keep a running
                    ' highscore, and test each as against it as we go

                    ' pixel above and left is one stride up, 4 bytes back
                    ' start by setting the highscore
                    maxDifference = Math.Abs(col0 - pixelsSrc(index - stride - 4))

                    ' pixel above is one stride up----------------vvvvvvvvvvvvvv
                    currentDifference = Math.Abs(col0 - pixelsSrc(index - stride))

                    ' set the highest difference from the two so far.
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'third one is the pixel above and right
                    currentDifference = Math.Abs(col0 - pixelsSrc(index - stride + 4))
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'bug in c.g's version, he misses the MR and ML pixels and does
                    'TM and TB twice instead.

                    'fourth one is ML (middle row, left col)
                    currentDifference = Math.Abs(col0 - pixelsSrc(index - 4))
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'MR
                    currentDifference = Math.Abs(col0 - pixelsSrc(index + 4))
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'BL
                    currentDifference = Math.Abs(col0 - pixelsSrc(index + stride - 4))
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'BM
                    currentDifference = Math.Abs(col0 - pixelsSrc(index + stride))
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'BR
                    currentDifference = Math.Abs(col0 - pixelsSrc(index + stride + 4))
                    maxDifference = _
                    IIf(maxDifference > currentDifference, maxDifference, currentDifference)

                    'phew.

                    'If the maxDifference is less than the threshold then zero it
                    If maxDifference < 0 Then maxDifference = 0

                    'Write the pixel to the targer bitmap
                    pixels(index) = maxDifference
                Next
           
            Next
        Next

        'copy array back into bitmap
        Copy(pixels, 0, scan0, pixels.Length)
        b.UnlockBits(bmData)
        Return b

    End Function



End Class