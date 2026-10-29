Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms

Public Class EdgeDetectConvolution
    Public Const EDGE_DETECT_KIRSH As Short = 1
    Public Const EDGE_DETECT_PREWITT As Short = 2
    Public Const EDGE_DETECT_SOBEL As Short = 3

    Public Shared Function ProcessImage(ByVal b As Bitmap, ByVal nType As Short, _
    ByVal nThreshold As Byte) As Bitmap

        Dim filt As New convolutionFilters
        Dim m As New convolutionFilters.ConvMatrix
        Dim bTemp As Bitmap = b.Clone

        ' First we run the vertical version of whichever filter it is
        ' and store in one bitmap:

        Select Case nType
            Case EDGE_DETECT_SOBEL
                m.SetAll(0)
                m(0) = 1        '  1  0  -1
                m(2) = -1       '  2  0  -2
                m(3) = 2        '  1  0  -1
                m(5) = -2
                m(6) = 1
                m(8) = -1
                m.Offset = 0
            Case EDGE_DETECT_PREWITT
                m.SetAll(0)
                m(0) = -1       '  -1  0  1
                m(2) = 1        '  -1  0  1
                m(3) = -1       '  -1  0  1
                m(5) = 1
                m(6) = -1
                m(8) = 1
                m.Offset = 0
            Case EDGE_DETECT_KIRSH
                m.SetAll(-3)
                m(4) = 0        '  5  -3  -3
                m(0) = 5        '  5   0  -3
                m(3) = 5        '  5  -3  -3
                m(6) = 5
                m.Offset = 0
        End Select

        ' store in b..
        Dim result As Boolean = False
        result = filt.conv3x3(b, m)
        'wait for a result...
        'otherwise a big image can crash it (
        Do
            Application.DoEvents()
        Loop Until result


        ' Next run the horizontal version of the filter on the other bitmap
        ' which still has the virgin source image.

        Select Case nType
            Case EDGE_DETECT_SOBEL
                m.SetAll(0)
                m(0) = 1         '  1  2  1
                m(1) = 2         '  0  0  0
                m(2) = 1         ' -1 -2 -1
                m(6) = -1
                m(7) = -2
                m(8) = -1
                m.Offset = 0
            Case EDGE_DETECT_PREWITT
                m.SetAll(0)
                m(0) = 1          ' 1  1  1
                m(1) = 1          ' 0  0  0
                m(2) = 1          '-1 -1 -1
                m(6) = -1
                m(7) = -1
                m(8) = -1
                m.Offset = 0
            Case EDGE_DETECT_KIRSH
                m.SetAll(-3)
                m(4) = 0         ' -3 -3 -3
                m(6) = 5         ' -3  0 -3
                m(7) = 5         '  5  5  5
                m(8) = 5
                m.Offset = 0
        End Select
        result = False
        result = filt.conv3x3(bTemp, m)
        Do
            Application.DoEvents()
        Loop Until result

        ' now we merge the two bitmaps with pixel = sqrt(pixel1 * pixel1 + pixel2 * pixel2)

        ' Lock each image:
        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim bmData2 As BitmapData = bTemp.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        ' store the pointers to the start of the pixel data for each bitmap
        Dim scan0 As IntPtr = bmData.Scan0
        Dim scan02 As IntPtr = bmData2.Scan0

        ' arrays to hold the bitmaps color information:
        Dim pixels1(4 * b.Width * b.Height - 1) As Byte
        Dim pixels2(4 * bTemp.Width * bTemp.Height - 1) As Byte

        'fill the arrays
        Marshal.Copy(scan0, pixels1, 0, pixels1.Length)
        Marshal.Copy(scan02, pixels2, 0, pixels2.Length)

        'don't need bTemp as the array has the info
        'do need b as we are going to write to it and return it
        bTemp.UnlockBits(bmData2)
        bTemp.Dispose()

        Dim nPixel As Integer

        For i As Integer = 0 To pixels1.Length - 1
            ' calculate the new value
            nPixel = Convert.ToInt32(Math.Sqrt((pixels1(i) ^ 2) + (pixels2(i) ^ 2)))
            ' make sure it is in range
            If (nPixel < nThreshold) Then nPixel = nThreshold
            If (nPixel > 255) Then nPixel = 255
            ' write it back to the array
            pixels1(i) = Convert.ToByte(nPixel)
        Next

        'write the array back to the bitmap memory data
        Marshal.Copy(pixels1, 0, scan0, pixels1.Length)
        b.UnlockBits(bmData)
        Return b

    End Function

End Class