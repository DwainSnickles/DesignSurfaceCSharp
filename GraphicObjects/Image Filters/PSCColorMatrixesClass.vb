Imports System.Drawing
Imports System.Drawing.Imaging
Public Class ColorMatrixes

    'A compilation of uses for the ColorMatrix class
    'in Visual Basic .Net 2003
    'by John Harald Apeland
    'email jh@johnharald.net
    'May 2005

    'Note that the subject is still being researched, and the class may therefore be updated in future.

    'The .Net framework ColorMatrix class provide easy linear color filters. 
    'There exist a scant methodology for some common uses of this to modify color, 
    'best known are scaling and translation with their common contrast and brightness adjustments. 
    'Applying the ColorMatrix as an imageattribute is really fast for image editing. 

    'Best use in image editor is to provide user input in 100-precision, 
    'then take input and divide by 100 before using the functions below. 
    'For rotation input is 0-360 in degrees. 
    'A couple functions is just a plain convertion with no settings, image in - image out. 
    'Limits and defaults are commented in class and also displayed by the example app. 
    'Error handling seems unnecessary since this is an 'internal' class, 
    ' and settings limits/defaults are always handled by the programmer. 
    '
    'For instance a -100 to 100 value brightness scroll, defaults 100, uservalue 110: 
    'picbox.image = classinstanceCM.brightness(freshimage,uservalue/100). 
    'This will apply a syncronized scale of value 1.1 returning a brighter image. 
    'See example app how this is done in all cases of using this class. 

#Region " general purpose "

    Public Function apply_cm(ByVal img As Image, ByVal cm As ColorMatrix) As Image
        'return new image with given ImageAttributes set
        Dim bm As Bitmap = New Bitmap(img.Width, img.Height)
        Dim g As Graphics = Graphics.FromImage(bm)
        Dim ia As ImageAttributes = New ImageAttributes
        ia.SetColorMatrix(cm)
        g.DrawImage(img, New Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia)
        g.Dispose()
        Return bm
    End Function

    'returns degrees in radians
    Private Function de_ra(ByVal de As Double) As Double
        Return de * (Math.PI / 180)
    End Function
    Private Function ra_de(ByVal ra As Double) As Double
        Return ra * (180 / Math.PI)
    End Function

#End Region

#Region " translate "
    'settings -1 to 1, default 0

    Public Function translate(ByVal img As Image, ByVal a As Single, ByVal r As Single, ByVal g As Single, ByVal b As Single) As Image

        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
            {New Single() {1, 0, 0, 0, 0}, _
            New Single() {0, 1, 0, 0, 0}, _
            New Single() {0, 0, 1, 0, 0}, _
            New Single() {0, 0, 0, 1, 0}, _
            New Single() {r, g, b, a, 1}})

        Return apply_cm(img, cm)
    End Function

    Public Function brightness(ByVal img As Image, ByVal bri As Single) As Image
        'brightness is a syncronized translation
        Return translate(img, 1, bri, bri, bri)
    End Function

#End Region

#Region " scale "
    'settings 0 to 2, default 1

    Public Function scale(ByVal img As Image, ByVal a As Single, ByVal r As Single, ByVal g As Single, ByVal b As Single) As Image

        'There is one small problem with this theory and that is the ColorMatrix has a nasty habit of causing 
        'arithmetic overflows in the images colour values such that simply scaling will cause an undesirable 
        'colourization of the image. Any red, green or blue colour elements having a zero value can overflow 
        'to some large value which causes the pixel to take on a totally different colour. To fix this, and it 
        'is a blatant fudge, we simply need to shift the colours minutely in one direction or another and the 
        'problem goes away. The fudge in the code below offsets the image colours by a measly 0.001, just a tenth 
        'of one percent. This means that our final matrix looks like this... (Bob Powell)

        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
            {New Single() {r, 0, 0, 0, 0}, _
            New Single() {0, g, 0, 0, 0}, _
            New Single() {0, 0, b, 0, 0}, _
            New Single() {0, 0, 0, a, 0}, _
            New Single() {0.001, 0.001, 0.001, 0, 1}})

        Return apply_cm(img, cm)
    End Function


    Public Function contrast(ByVal img As Image, ByVal con As Single) As Image
        'contrast is a syncronized scale
        Return scale(img, 1, con, con, con)
    End Function

    Public Function invert(ByVal img As Image) As Image
        'is a scale, but needs the non-translation (1,1,1) for some special zero colors)
        'a "transpose" ?
        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
            {New Single() {-1, 0, 0, 0, 0}, _
            New Single() {0, -1, 0, 0, 0}, _
            New Single() {0, 0, -1, 0, 0}, _
            New Single() {0, 0, 0, 1, 0}, _
            New Single() {1, 1, 1, 0, 1}})

        Return apply_cm(img, cm)
    End Function

#End Region

#Region " saturation "
    'sat(0) img blackwhite (greyscale)
    'sat(1) img unchanged
    'sat(>0-1<) interpolating
    'sat(<0-1>) extrapolating 

    Public Function blackwhite(ByVal img As Image) As Image
        'using saturation formula
        Return saturation(img, 0)
    End Function

    Public Function complimentary(ByVal img As Image) As Image
        'using saturation formula
        Return saturation(img, -1)
    End Function

    Public Function saturation(ByVal img As Image, ByVal s As Single) As Image
        'http://www.sgi.com/misc/grafica/matrix/
        '// The luminance weight factors for the RGB color space.
        '// These values are actually preferable to the better known factors of
        '// Y = 0.30R + 0.59G + 0.11B, the formula which is used in color television technique.
        Const lumR As Single = 0.3086
        Const lumG As Single = 0.6094
        Const lumB As Single = 0.082
        Dim a As Single = (1.0 - s) * lumR + s
        Dim b As Single = (1.0 - s) * lumR
        Dim c As Single = (1.0 - s) * lumR
        Dim d As Single = (1.0 - s) * lumG
        Dim e As Single = (1.0 - s) * lumG + s
        Dim f As Single = (1.0 - s) * lumG
        Dim g As Single = (1.0 - s) * lumB
        Dim h As Single = (1.0 - s) * lumB
        Dim i As Single = (1.0 - s) * lumB + s

        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
                    {New Single() {a, b, c, 0, 0}, _
                    New Single() {d, e, f, 0, 0}, _
                    New Single() {g, h, i, 0, 0}, _
                    New Single() {0, 0, 0, 1, 0}, _
                    New Single() {0, 0, 0, 0, 1}})

        Return apply_cm(img, cm)
    End Function

#End Region

#Region " rotate "
    'http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdicpp/GDIPlus/usingGDIPlus/recoloring/rotatingcolors.asp

    'r = 0-360 degrees, also known as X rotation
    Public Function rotateR(ByVal img As Image, ByVal r As Single) As Image
        Dim sin As Single = Math.Sin(de_ra(r))
        Dim cos As Single = Math.Cos(de_ra(r))
        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
                            {New Single() {1, 0, 0, 0, 0}, _
                            New Single() {0, cos, sin, 0, 0}, _
                            New Single() {0, -sin, cos, 0, 0}, _
                            New Single() {0, 0, 0, 1, 0}, _
                            New Single() {0, 0, 0, 0, 1}})

        Return apply_cm(img, cm)
    End Function

    'r = 0-360 degrees, also known as Y rotation
    Public Function rotateG(ByVal img As Image, ByVal r As Single) As Image
        Dim sin As Single = Math.Sin(de_ra(r))
        Dim cos As Single = Math.Cos(de_ra(r))
        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
                            {New Single() {cos, 0, -sin, 0, 0}, _
                            New Single() {0, 1, 0, 0, 0}, _
                            New Single() {sin, 0, cos, 0, 0}, _
                            New Single() {0, 0, 0, 1, 0}, _
                            New Single() {0, 0, 0, 0, 1}})

        Return apply_cm(img, cm)
    End Function

    'r = 0 to 360 degrees, also known as Z rotation
    Public Function rotateB(ByVal img As Image, ByVal r As Single) As Image
        Dim sin As Single = Math.Sin(de_ra(r))
        Dim cos As Single = Math.Cos(de_ra(r))
        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
                           {New Single() {cos, sin, 0, 0, 0}, _
                            New Single() {-sin, cos, 0, 0, 0}, _
                            New Single() {0, 0, 1, 0, 0}, _
                            New Single() {0, 0, 0, 1, 0}, _
                            New Single() {0, 0, 0, 0, 1}})

        Return apply_cm(img, cm)
    End Function

#End Region

End Class
