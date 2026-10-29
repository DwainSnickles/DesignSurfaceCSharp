Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Drawing

Public Class ResizeLockBits


    Public Shared Function ProcessImage(ByVal b As Bitmap, _
    ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bBilinear As Boolean) As Bitmap

 

        Dim bTemp As Bitmap = b.Clone
        b = New Bitmap(nWidth, nHeight, PixelFormat.Format32bppArgb)
        Dim nXFactor As Double = bTemp.Width / nWidth
        Dim nYFactor As Double = bTemp.Height / nHeight

        Dim bTempData As BitmapData = bTemp.LockBits _
        (New Rectangle(0, 0, bTemp.Width, bTemp.Height), _
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim scan0Temp As IntPtr = bTempData.Scan0
        Dim strideTemp As Integer = bTempData.Stride ' these will vary as the sizes do

        Dim tempPixels(strideTemp * bTemp.Height - 1) As Byte
        Copy(scan0Temp, tempPixels, 0, tempPixels.Length)

        

        Dim bData As BitmapData = b.LockBits _
        (New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim scan0b As IntPtr = bData.Scan0
        Dim strideb As Integer = bData.Stride

        Dim bPixels(strideb * b.Height - 1) As Byte
        Copy(scan0b, bPixels, 0, bPixels.Length)

        Dim position As Integer

        If Not bBilinear Then

            Dim alpha, red, green, blue As Byte
            Dim xTemp, yTemp As Integer

            For x As Integer = 0 To b.Width - 1
                For y As Integer = 0 To b.Height - 1

                    'set the position in bTemp where we are grabbing the pixel
                    xTemp = Convert.ToInt32(Math.Floor(x * nXFactor))
                    yTemp = Convert.ToInt32(Math.Floor(y * nYFactor))
                    'position in btemp then is
                    position = (yTemp * strideTemp) + (xTemp * 4)
                    'get colors from btemp
                    blue = tempPixels(position)
                    green = tempPixels(position + 1)
                    red = tempPixels(position + 2)
                    alpha = tempPixels(position + 3)
                    'reset position to refer to the target bitmap location
                    position = (y * strideb) + (x * 4)
                    'write to b
                    bPixels(position) = blue
                    bPixels(position + 1) = green
                    bPixels(position + 2) = red
                    bPixels(position + 3) = alpha
                Next
            Next
            bTemp.UnlockBits(bTempData)
            bTemp.Dispose()
            Copy(bPixels, 0, scan0b, bPixels.Length)
            b.UnlockBits(bData)

            Return b
        Else
            ' The Better Way:
            Dim fraction_x, fraction_y, one_minus_x, one_minus_y As Double
            Dim ceil_x, ceil_y, floor_x, floor_y As Integer
            'store ARGB info in these:
            Dim c1(4) As Integer
            Dim c2(4) As Integer
            Dim c3(4) As Integer
            Dim c4(4) As Integer
            Dim red, green, blue, alpha As Byte
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

                    'ugly - we need the argb's for four pixels...
                    position = (floor_y * strideTemp) + (floor_x * 4)
                    c1(0) = tempPixels(position)
                    c1(1) = tempPixels(position + 1)
                    c1(2) = tempPixels(position + 2)
                    c1(3) = tempPixels(position + 3)
                    position = (floor_y * strideTemp) + (ceil_x * 4)
                    c2(0) = tempPixels(position)
                    c2(1) = tempPixels(position + 1)
                    c2(2) = tempPixels(position + 2)
                    c2(3) = tempPixels(position + 3)
                    position = (ceil_y * strideTemp) + (floor_x * 4)
                    c3(0) = tempPixels(position)
                    c3(1) = tempPixels(position + 1)
                    c3(2) = tempPixels(position + 2)
                    c3(3) = tempPixels(position + 3)
                    position = (ceil_y * strideTemp) + (ceil_x * 4)
                    c4(0) = tempPixels(position)
                    c4(1) = tempPixels(position + 1)
                    c4(2) = tempPixels(position + 2)
                    c4(3) = tempPixels(position + 3)

                    'Blue
                    b1 = Convert.ToByte(one_minus_x * c1(0) + fraction_x * c2(0))
                    b2 = Convert.ToByte(one_minus_x * c3(0) + fraction_x * c4(0))
                    blue = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)
                    'Green
                    b1 = Convert.ToByte(one_minus_x * c1(1) + fraction_x * c2(1))
                    b2 = Convert.ToByte(one_minus_x * c3(1) + fraction_x * c4(1))
                    green = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)
                    'Red
                    b1 = Convert.ToByte(one_minus_x * c1(2) + fraction_x * c2(2))
                    b2 = Convert.ToByte(one_minus_x * c3(2) + fraction_x * c4(2))
                    red = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)
                    'Alpha ? unsure what to do.

                    b1 = Convert.ToByte(one_minus_x * c1(3) + fraction_x * c2(3))
                    b2 = Convert.ToByte(one_minus_x * c3(3) + fraction_x * c4(3))
                    alpha = Convert.ToByte(one_minus_y * b1 + fraction_y * b2)

                    position = (y * strideb) + (x * 4)
                    bPixels(position) = blue
                    bPixels(position + 1) = green
                    bPixels(position + 2) = red
                    bPixels(position + 3) = alpha
                Next
            Next
            bTemp.UnlockBits(bTempData)
            bTemp.Dispose()
            Copy(bPixels, 0, scan0b, bPixels.Length)
            b.UnlockBits(bData)
            Return b
        End If

    End Function


End Class