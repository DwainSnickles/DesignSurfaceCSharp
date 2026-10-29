Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class convolutionFilters
    
    ' the matrix class
    Public Class ConvMatrix

        Public Factor As Integer = 1
        Public Offset As Integer = 0
        'instead of all those integers, we'll use an array:
        Public grid(8) As Integer
        ' then we can get it in a for... next loop

        Sub New()
            ' fill up with defaults
            For i As Integer = 0 To 8
                grid(i) = 0
                grid(4) = 1
            Next
        End Sub

        Public Sub SetAll(ByVal value As Integer)
            'allows us to set all the  items in grid to the same value
            For i As Integer = 0 To 8
                grid(i) = value
            Next
        End Sub

        Default Property item(ByVal index As Integer) As Integer
            ' default property means we dont have to use the keyword "item"
            ' lets us set the values at grid(value)
            Get
                Return grid(index)
            End Get
            Set(ByVal Value As Integer)
                grid(index) = Value
            End Set
        End Property

    End Class

    Private _grid As ConvMatrix ' for the property:

    ' Allow us to get a grid
    Public ReadOnly Property GetGrid() As ConvMatrix
        Get
            Return _grid
        End Get
    End Property

    Sub New()
        _grid = New ConvMatrix
    End Sub

    Public Function conv3x3(ByVal b As Bitmap, ByVal m As ConvMatrix) As Boolean

        ' this will always be fairly slow as for each pixel we are reading 9 other pixels to get
        ' the final color value. The perpixel filters just read from one pixel.

        'avoid / by 0
        If m.Factor = 0 Then Return True

        ' We are moving through the bitmap, and changing pixels based on their surrounding
        '  pixels. This means that we need a copy of the source bitmap, so that we don't
        '  end up using pixels that we have changed (and so filter based on changed pixels)
        ' So here is a copy of the input bitmap:
        Dim bSrc As Bitmap = b.Clone

        ' So we want Two lockbits now. We lock the source bits
        Dim bmSrc As BitmapData = _
         bSrc.LockBits(New Rectangle(0, 0, bSrc.Width, bSrc.Height), _
         ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        ' And we lock the target bits
        Dim bmData As BitmapData = _
         b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
         ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        ' We need the pointers to the start of both images data:
        Dim Scan0 As IntPtr = bmData.Scan0
        Dim SrcScan0 As IntPtr = bmSrc.Scan0

        ' We are storing all the pixels as int32s in arrays:
        Dim pixels(b.Width * b.Height - 1) As Integer
        Dim pixelsSrc(b.Width * b.Height - 1) As Integer

        ' Marshal copy grabs the pixels
        Marshal.Copy(Scan0, pixels, 0, pixels.Length)
        Marshal.Copy(SrcScan0, pixelsSrc, 0, pixelsSrc.Length)

        ' keep a running total (more later)...
        Dim total(3) As Single ' store sum of the pixels read from the source

        Dim index As Integer

        Dim gridValueDivmFactor As Single
        Dim ninthOfOffset As Single = m.Offset / 9

        ' We are manipulating a pixel based on all its surrounding pixels
        '  the pixels on the edges don't have all the surrounding pixels
        '  so we skip them by staring these loops at 1 and ending at blah -2
        For y As Integer = 1 To b.Height - 2
            For x As Integer = 1 To b.Width - 2

                ' We have defined a 3x3 grid and we apply it to the pixels 
                ' around the current one: (E is current pixel)
                '
                ' pixels         grid
                ' A B C          1 2 3
                ' D E F   with   4 5 6
                ' G H I          7 8 9
                '
                ' (this is not matrix multiplication)
                '
                ' We get: (A*1) + (B*2) + (C*3) + (D*4) + (E*5) + (F*6) + (G*7) + (H*8) + (I*9)
                ' we then divide this by m.factor and add m.offset...
                '
                ' e.g. with the starting grid:
                ' 000
                ' 010
                ' 000 we get the result E = the value of the central pixel so no change to the image
                '  
                ' Our For ... Next loop puts the current pixel at (x,y)
                ' This means we need to get the pixels in the following positions:
                ' (x-1,y-1) ( x ,y-1) (x+1,y-1)
                ' (x-1, y ) ( x , y ) (x+1, y )
                ' (x-1,y+1) ( x ,y+1) (x+1,y+1)
                ' 



                ' Another loop
                ' This one loops through a 3x3 grid:
                For yy As Integer = -1 To +1
                    For xx As Integer = -1 To +1

                        ' the central pixel(x,y) is in the array at this index:
                        ' (y * b.Width) pixels in the rows above + x pixels in the current row
                        ' (y * b.width) + x

                        ' the yyxx for next loop gives us:    (-1,-1)  (0,-1) (1,-1)   
                        '                                     (-1,0)   (0,0)  (1,0)  
                        '                                     (-1,1)   (0,1)  (1,1)
                        '
                        'so... the locaion of the pixels in the loop is (x+xx,y+yy)
                        'and the position in the array is:
                        '
                        index = ((y + yy) * b.Width) + x + xx

                        'we want to total the argb of the current pixel * the grid value for its position
                        'we'll do the factor here too, and add on 1/9 of offset
                        'trying to keep the number of calculations in the loops to a minimum

                        gridValueDivmFactor = m.grid(((yy + 1) * 3) + (xx + 1)) / m.Factor

                        'ignore alpha, it messes up with some filters. will just set it to 255

                        'total(0) += (((pixelsSrc(index) >> 24) And &HFF) * gridValueDivmFactor) + ninthOfOffset
                        total(1) += (((pixelsSrc(index) >> 16) And &HFF) * gridValueDivmFactor) + ninthOfOffset
                        total(2) += (((pixelsSrc(index) >> 8) And &HFF) * gridValueDivmFactor) + ninthOfOffset
                        total(3) += ((pixelsSrc(index) And &HFF) * gridValueDivmFactor) + ninthOfOffset

                    Next
                Next

                For i As Integer = 0 To 3
                    If total(i) > 255 Then total(i) = 255
                    If total(i) < 0 Then total(i) = 0
                Next


                'finally we set the pixel in the destination bitmap

                pixels(y * b.Width + x) = (255 << 24) _
                      Or (Convert.ToInt32(total(1)) << 16) _
                      Or (Convert.ToInt32(total(2)) << 8) _
                      Or Convert.ToInt32(total(3))

                'reset total for next pixel
                Array.Clear(total, 0, 4)

            Next
        Next
        Marshal.Copy(pixels, 0, Scan0, pixels.Length)
        'unlock both bitmaps
        b.UnlockBits(bmData)
        bSrc.UnlockBits(bmSrc)


        Return True

    End Function



End Class



