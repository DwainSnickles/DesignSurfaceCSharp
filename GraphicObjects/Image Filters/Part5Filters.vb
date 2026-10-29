Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.Marshal
Imports System.Windows.Forms
Imports System.Drawing

Public Class Part5Filters

    Public Shared Function OffsetFilterAbs(ByRef b As Bitmap, ByVal offset As Point(,)) As Boolean

        'absolute...

        ' takes an array of points called offset()
        ' and a bitmap in.
        ' The offset array has the same dimensions as the bitmap
        ' say that the first point stored in the offset array 
        ' is (10,10). 
        ' i.e. offset(0,0) stores point(10,10)
        ' we look at point (10,10) in the source bitmap, get the color
        ' and then set point(0,0) in the target bitmap.

        Dim bSrc As Bitmap = b.Clone

        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmDataSrc As BitmapData = bSrc.LockBits(New Rectangle(0, 0, bSrc.Width, bSrc.Height), _
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim Scan0 As IntPtr = bmData.Scan0
        Dim SrcScan0 As IntPtr = bmDataSrc.Scan0

        Dim pixels(b.Width * b.Height - 1) As Integer
        Dim srcPixels(bSrc.Width * bSrc.Height - 1) As Integer

        Copy(Scan0, pixels, 0, pixels.Length)
        Copy(SrcScan0, srcPixels, 0, srcPixels.Length)

        bSrc.UnlockBits(bmDataSrc)
        bSrc.Dispose()

        Dim offsetx, offsety As Integer

        For y As Integer = 0 To b.Height - 1
            For x As Integer = 0 To b.Width - 1

                offsetx = offset(x, y).X
                offsety = offset(x, y).Y

                'index could go out of range

                If offsetx > 0 AndAlso offsetx < b.Width - 1 AndAlso offsety > 0 AndAlso offsety < b.Height - 1 Then
                    pixels(y * b.Width + x) = srcPixels(offsetx + (offsety * b.Width))
                End If
            Next
        Next

        Copy(pixels, 0, Scan0, pixels.Length)
        b.UnlockBits(bmData)

        Return True

    End Function

    Public Shared Function OffsetFilter(ByRef b As Bitmap, ByVal offset As Point(,)) As Boolean

        ' similar to the function above, but the values in offset are
        ' relative to the index of offset. blurgh, by example:
        ' if the values in offset(10,10) are (2, -8) then you
        ' get the colors from (10 + 2, 10 - 8) and write them
        ' into (10,10)

        Dim bSrc As Bitmap = b.Clone

        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmDataSrc As BitmapData = bSrc.LockBits(New Rectangle(0, 0, bSrc.Width, bSrc.Height), _
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim Scan0 As IntPtr = bmData.Scan0
        Dim SrcScan0 As IntPtr = bmDataSrc.Scan0

        Dim pixels(b.Width * b.Height - 1) As Integer
        Dim srcPixels(bSrc.Width * bSrc.Height - 1) As Integer

        Copy(Scan0, pixels, 0, pixels.Length)
        Copy(SrcScan0, srcPixels, 0, srcPixels.Length)

        bSrc.UnlockBits(bmDataSrc)
        bSrc.Dispose()

        Dim offsetx, offsety As Integer

        For y As Integer = 0 To b.Height - 1
            For x As Integer = 0 To b.Width - 1
                ' random jitter can make the value go out of range
                ' skip the test if your implementation keeps values in range...
                offsetx = x + offset(x, y).X
                offsety = y + offset(x, y).Y
                If offsetx > 0 AndAlso offsetx < b.Width - 1 AndAlso offsety > 0 AndAlso offsety < b.Height - 1 Then
                    pixels(y * b.Width + x) = srcPixels(offsetx + (b.Width * offsety))
                End If
            Next
        Next

        Copy(pixels, 0, Scan0, pixels.Length)
        b.UnlockBits(bmData)

        Return True
    End Function

    Public Structure FloatPoint
        Public X As Double
        Public Y As Double
    End Structure

    Public Shared Function OffsetFilterAntiAlias(ByRef b As Bitmap, ByVal fp As FloatPoint(,)) _
    As Boolean

        'this one is absolute, and uses the bilinear filter. 

        Dim bSrc As Bitmap = b.Clone

        Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmDataSrc As BitmapData = bSrc.LockBits(New Rectangle(0, 0, bSrc.Width, bSrc.Height), _
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim stride As Integer = bmData.Stride
        Dim Scan0 As IntPtr = bmData.Scan0
        Dim SrcScan0 As IntPtr = bmDataSrc.Scan0

        Dim pixels(stride * b.Height - 1) As Byte
        Dim srcPixels(stride * bSrc.Height - 1) As Byte

        Copy(Scan0, pixels, 0, pixels.Length)
        Copy(SrcScan0, srcPixels, 0, srcPixels.Length)

        bSrc.UnlockBits(bmDataSrc)
        bSrc.Dispose()

        Dim xoffset, yoffset As Integer

        Dim fraction_x, fraction_y, one_minus_x, one_minus_y As Double

        Dim ceil_x, ceil_y, floor_x, floor_y As Integer
        Dim p1, p2 As Byte

        Dim color1, color2 As Byte

        For x As Integer = 0 To b.Width - 1
            For y As Integer = 0 To b.Height - 1

                xoffset = fp(x, y).X
                yoffset = fp(x, y).Y

                ' Setup

                floor_x = Math.Floor(xoffset)
                floor_y = Math.Floor(yoffset)
                ceil_x = floor_x + 1
                ceil_y = floor_y + 1
                fraction_x = xoffset - floor_x
                fraction_y = yoffset - floor_y
                one_minus_x = 1.0 - fraction_x
                one_minus_y = 1.0 - fraction_y

                If (floor_y >= 0 AndAlso ceil_y < b.Height AndAlso floor_x >= 0 _
                AndAlso ceil_x < b.Width) Then


                    'blue
                    color1 = srcPixels((floor_y * stride) + (floor_x * 4))
                    color2 = srcPixels((floor_y * stride) + (ceil_x * 4))
                    p1 = Convert.ToByte((one_minus_x * color1) + (fraction_x * color2))

                    color1 = srcPixels((ceil_y * stride) + (floor_x * 4))
                    color2 = srcPixels((ceil_y * stride) + (ceil_x * 4))
                    p2 = Convert.ToByte((one_minus_x * color1) + (fraction_x * color2))

                    pixels((y * stride) + (x * 4)) = Convert.ToByte((one_minus_y * p1) + (fraction_y * p2))

                    'green
                    color1 = srcPixels((floor_y * stride) + (floor_x * 4) + 1)
                    color2 = srcPixels((floor_y * stride) + (ceil_x * 4) + 1)
                    p1 = Convert.ToByte((one_minus_x * color1) + (fraction_x * color2))

                    color1 = srcPixels((ceil_y * stride) + (floor_x * 4) + 1)
                    color2 = srcPixels((ceil_y * stride) + (ceil_x * 4) + 1)
                    p2 = Convert.ToByte((one_minus_x * color1) + (fraction_x * color2))

                    pixels((y * stride) + (x * 4) + 1) = Convert.ToByte((one_minus_y * p1) + (fraction_y * p2))

                    'Red
                    color1 = srcPixels((floor_y * stride) + (floor_x * 4) + 2)
                    color2 = srcPixels((floor_y * stride) + (ceil_x * 4) + 2)
                    p1 = Convert.ToByte((one_minus_x * color1) + (fraction_x * color2))

                    color1 = srcPixels((ceil_y * stride) + (floor_x * 4) + 2)
                    color2 = srcPixels((ceil_y * stride) + (ceil_x * 4) + 2)
                    p2 = Convert.ToByte((one_minus_x * color1) + (fraction_x * color2))

                    pixels((y * stride) + (x * 4) + 2) = Convert.ToByte((one_minus_y * p1) + (fraction_y * p2))
                End If
            Next
        Next
        Copy(pixels, 0, Scan0, pixels.Length)
        b.UnlockBits(bmData)
        Return True
    End Function

    Public Shared Function Flip(ByVal b As Bitmap, ByVal bHorz As Boolean, ByVal bVert As Boolean) _
    As Bitmap

        ' fill up a point array with the co-ords of the flipped points

        Dim ptFlip(b.Width, b.Height) As Point
        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height
        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1
                ptFlip(x, y).X = IIf((bHorz), nWidth - (x + 1), x)
                ptFlip(x, y).Y = IIf((bVert), nHeight - (y + 1), y)
            Next
        Next

        Dim result As Boolean = False
        result = OffsetFilterAbs(b, ptFlip) ' run bitmap b through the offset filter abs
        ' so that all of b's pixels get moved to the values we just stored in ptFlip.
        Do
            'waiting for calculation to finish before returning.
            Application.DoEvents()
        Loop Until result
        Return b

    End Function

    Public Shared Function RandomJitter(ByRef b As Bitmap, ByVal nDegree As Short) As Bitmap

        ' fill the point array up with random distaces 
        ' between (-nDegree\2) and (+nDegree\2)

        Dim ptRandomJitter(b.Width, b.Height) As Point

        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height

        Dim newX, newY As Integer

        Dim nHalf As Short = CShort(Math.Floor(nDegree / 2))
        Dim rand As Random = New Random
        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1
                newX = rand.Next(nDegree) - nHalf
                If ((x + newX) > 0) AndAlso ((x + newX) < nWidth) Then
                    ptRandomJitter(x, y).X = newX
                Else
                    ptRandomJitter(x, y).X = 0
                End If
                newY = rand.Next(nDegree) - nHalf

                If ((y + newY) > 0) AndAlso ((y + newY) < nWidth) Then
                    ptRandomJitter(x, y).Y = newY
                Else
                    ptRandomJitter(x, y).Y = 0
                End If
            Next

        Next

        Dim result As Boolean = False
        ' we use the offset filter, so the point in b gets moved by the random 
        ' distances we have put into ptRandomJitter
        result = OffsetFilter(b, ptRandomJitter)
        Do
            Application.DoEvents()
        Loop Until result

        Return b
    End Function

    Public Shared Function Swirl(ByVal b As Bitmap, ByVal fDegree As Double, _
    ByVal bSmoothing As Boolean) As Bitmap

        ' complicated 
        ' I've just translated it...

        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height

        Dim fp(nWidth, nHeight) As FloatPoint
        Dim pt(nWidth, nHeight) As Point

        Dim midl As New Point
        midl.X = nWidth / 2
        midl.Y = nHeight / 2

        Dim theta, radius, newx, newy As Double
        Dim trueX, trueY As Integer

        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1
                trueX = x - midl.X
                trueY = y - midl.Y
                theta = Math.Atan2((trueY), (trueX))
                radius = Math.Sqrt((trueX * trueX) + (trueY * trueY))
                newx = midl.X + (radius * Math.Cos(theta + (fDegree * radius)))
                If (newx > 0) AndAlso newx < (nWidth - 1) Then
                    fp(x, y).X = newx
                    pt(x, y).X = Convert.ToInt32(newx)
                Else
                    fp(x, y).X = x
                    pt(x, y).X = x
                End If
                newy = midl.Y + (radius * Math.Sin(theta + (fDegree * radius)))
                If (newy > 0) AndAlso newy < (nHeight - 1) Then
                    fp(x, y).Y = newy
                    pt(x, y).Y = Convert.ToInt32(newy)
                Else
                    fp(x, y).Y = y
                    pt(x, y).Y = y
                End If
            Next
        Next

        Dim result As Boolean = False

        If (bSmoothing) Then
            result = OffsetFilterAntiAlias(b, fp)
        Else
            result = OffsetFilterAbs(b, pt)
        End If

        Do
            Application.DoEvents()
        Loop Until result

        Return b

    End Function

    Public Shared Function Sphere(ByVal b As Bitmap, ByVal bSmoothing As Boolean) As Bitmap

        ' complicated 
        ' I've just translated it...

        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height

        Dim fp(nWidth, nHeight) As FloatPoint
        Dim pt(nWidth, nHeight) As Point

        Dim midl As New Point
        midl.X = nWidth / 2
        midl.Y = nHeight / 2

        Dim theta, radius, newx, newy, newRadius As Double
        Dim trueX, trueY As Integer

        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1
                trueX = x - midl.X
                trueY = y - midl.Y
                theta = Math.Atan2((trueY), (trueX))
                radius = Math.Sqrt(trueX * trueX + trueY * trueY)
                newRadius = radius * radius / (Math.Max(midl.X, midl.Y))
                newx = midl.X + (newRadius * Math.Cos(theta))
                If newx > 0 AndAlso newx < nWidth Then
                    fp(x, y).X = newx
                    pt(x, y).X = Convert.ToInt32(newx)
                Else
                    fp(x, y).X = 0.0
                    fp(x, y).Y = 0.0
                    pt(0, 0).X = 0
                    pt(0, 0).Y = 0
                End If
                newy = midl.Y + (newRadius * Math.Sin(theta))
                If newy > 0 AndAlso newy < nHeight AndAlso newx > 0 AndAlso newx < nWidth Then
                    fp(x, y).Y = newy
                    pt(x, y).Y = Convert.ToInt32(newy)
                Else
                    fp(x, y).X = 0.0
                    fp(x, y).Y = 0.0
                    pt(x, y).X = 0
                    pt(x, y).Y = 0
                End If
            Next
        Next

        Dim result As Boolean = False

        If (bSmoothing) Then
            result = OffsetFilterAbs(b, pt)
        Else
            result = OffsetFilterAntiAlias(b, fp)
        End If

        Do
            Application.DoEvents()
        Loop Until result

        Return b
    End Function


    Public Shared Function TimeWarp(ByVal b As Bitmap, ByVal factor As Byte, _
    ByVal bSmoothing As Boolean) As Bitmap

        ' complicated 
        ' I've just translated it...

        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height

        Dim fp(nWidth, nHeight) As FloatPoint
        Dim pt(nWidth, nHeight) As Point

        Dim midl As New Point
        midl.X = nWidth / 2
        midl.Y = nHeight / 2

        Dim theta, radius, newx, newy, newRadius As Double
        Dim trueX, trueY As Integer

        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1
                trueX = x - midl.X
                trueY = y - midl.Y
                theta = Math.Atan2((trueY), (trueX))
                radius = Math.Sqrt(trueX * trueX + trueY * trueY)
                newRadius = Math.Sqrt(radius) * factor
                newx = midl.X + (newRadius * Math.Cos(theta))
                If newx > 0 AndAlso newx < nWidth Then
                    fp(x, y).X = newx
                    pt(x, y).X = Convert.ToInt32(newx)
                Else
                    fp(x, y).X = 0.0
                    pt(0, 0).X = 0
                End If
                newy = midl.Y + (newRadius * Math.Sin(theta))
                If newy > 0 AndAlso newy < nHeight Then
                    fp(x, y).Y = newy
                    pt(x, y).Y = Convert.ToInt32(newy)
                Else
                    fp(x, y).Y = 0.0
                    pt(x, y).Y = 0
                End If

            Next
        Next

        Dim result As Boolean = False

        If (bSmoothing) Then
            result = OffsetFilterAbs(b, pt)
        Else
            result = OffsetFilterAntiAlias(b, fp)
        End If

        Do
            Application.DoEvents()
        Loop Until result

        Return b

    End Function

    Public Shared Function Moire(ByVal b As Bitmap, ByVal fDegree As Double) As Bitmap

        ' complicated 
        ' I've just translated it...

        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height

        Dim pt(nWidth, nHeight) As Point

        Dim midl As New Point
        midl.X = nWidth / 2
        midl.Y = nHeight / 2

        Dim theta, radius, newx, newy As Double
        Dim trueX, trueY As Integer

        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1
                trueX = x - midl.X
                trueY = y - midl.Y
                theta = Math.Atan2((trueX), (trueY))
                radius = Math.Sqrt((trueX * trueX) + (trueY * trueY))
                newx = Convert.ToInt32(radius * Math.Sin(theta + (fDegree * radius)))

                If newx > 0 AndAlso newx < nWidth Then
                    pt(x, y).X = Convert.ToInt32(newx)
                Else
                    pt(0, 0).X = 0
                End If
                newy = Convert.ToInt32(radius * Math.Sin(theta + (fDegree * radius)))
                If newy > 0 AndAlso newy < nHeight Then
                    pt(x, y).Y = Convert.ToInt32(newy)
                Else
                    pt(x, y).Y = 0
                End If

            Next
        Next

        Dim result As Boolean = False
        result = OffsetFilterAbs(b, pt)
        Do
            Application.DoEvents()
        Loop Until result
        Return b

    End Function

    Public Shared Function Water(ByVal b As Bitmap, ByVal nWave As Short, _
    ByVal bSmoothing As Boolean) As Bitmap

        ' bit easier
        ' but, I've just translated it...


        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height
        Dim fp(nWidth, nHeight) As FloatPoint
        Dim pt(nWidth, nHeight) As Point

        Dim midl As New Point
        midl.X = nWidth / 2
        midl.Y = nHeight / 2

        Dim newx, newy, xo, yo As Double

        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1

                xo = (nWave * Math.Sin(2.0 * Math.PI * (y / 128.0)))
                yo = (nWave * Math.Cos((2.0 * Math.PI) * (x / 128.0)))
                newx = (x + xo)
                newy = (y + yo)

                If newx > 0 AndAlso newx < nWidth Then
                    fp(x, y).X = newx
                    pt(x, y).X = Convert.ToInt32(newx)
                Else
                    fp(x, y).X = 0.0
                    pt(x, y).X = 0
                End If
                If newy > 0 AndAlso newy < nHeight Then
                    fp(x, y).Y = newy
                    pt(x, y).Y = Convert.ToInt32(newy)
                Else
                    fp(x, y).Y = 0.0
                    pt(x, y).Y = 0
                End If
            Next
        Next

        Dim result As Boolean = False

        If (bSmoothing) Then
            result = OffsetFilterAbs(b, pt)
        Else
            result = OffsetFilterAntiAlias(b, fp)
        End If

        Do
            Application.DoEvents()
        Loop Until result

        Return b

    End Function

    Public Shared Function Pixelate(ByVal b As Bitmap, ByVal pixel As Short, ByVal bGrid As Boolean) As Bitmap

        'grid can look pretty weird as the color is variable on the grid lines
        'I thought it was borked for a while but it was my source image.
        'I'll leave it as it is, but you could use gdi to draw a grid instead.

        'This one stores the offset to the pixel in the top left corner of the
        ' current small square. The pixelate2 filter  calculates the
        ' average color of pixels in the small square.

        Dim nWidth As Integer = b.Width
        Dim nHeight As Integer = b.Height

        Dim pt(nWidth, nHeight) As Point

        Dim newx, newy As Integer

        For x As Integer = 0 To nWidth - 1
            For y As Integer = 0 To nHeight - 1

                newx = pixel - (x Mod pixel)

                If (bGrid AndAlso newx = pixel) Then
                    pt(x, y).X = -x
                ElseIf (x + newx > 0 AndAlso x + newx < nWidth) Then
                    pt(x, y).X = newx
                Else
                    pt(x, y).X = 0
                End If

                newy = pixel - y Mod pixel

                If (bGrid AndAlso newy = pixel) Then
                    pt(x, y).Y = -y
                ElseIf (y + newy > 0 AndAlso y + newy < nHeight) Then
                    pt(x, y).Y = newy
                Else
                    pt(x, y).Y = 0
                End If
            Next
        Next

        Dim result As Boolean = False
        result = OffsetFilter(b, pt)
        Do
            Application.DoEvents()
        Loop Until result
        Return b

    End Function




End Class

