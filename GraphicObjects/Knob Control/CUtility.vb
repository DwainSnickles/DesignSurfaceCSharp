Imports System
Imports System.Drawing

Public Class CUtility

    Public Function getDarkColor(ByVal c As Color, ByVal d As Byte) As Color
        Dim r As Byte = 0
        Dim g As Byte = 0
        Dim b As Byte = 0

        If (c.R > d) Then r = (c.R - d)
        If (c.G > d) Then g = (c.G - d)
        If (c.B > d) Then b = (c.B - d)

        Dim c1 As Color = Color.FromArgb(r, g, b)
        Return c1
    End Function

    Public Function getLightColor(ByVal c As Color, ByVal d As Byte) As Color
        Dim r As Byte = 255
        Dim g As Byte = 255
        Dim b As Byte = 255

        If (CInt(c.R) + CInt(d) <= 255) Then r = (c.R + d)
        If (CInt(c.G) + CInt(d) <= 255) Then g = (c.G + d)
        If (CInt(c.B) + CInt(d) <= 255) Then b = (c.B + d)

        Dim c2 As Color = Color.FromArgb(r, g, b)
        Return c2
    End Function

    ' Method which checks is particular point is in rectangle
    ' <param name="p">Point to be Chaecked</param>
    ' <param name="r">Rectangle</param>
    ' <returns>true is Point is in rectangle, else false</returns>
    Public Function isPointinRectangle(ByVal p As Point, ByVal r As Rectangle) As Boolean
        Dim flag As Boolean = False
        If (p.X > r.X And p.X < r.X + r.Width And p.Y > r.Y And p.Y < r.Y + r.Height) Then
            flag = True
        End If
        Return flag
    End Function

    Public Sub DrawInsetCircle(ByRef g As Graphics, ByRef r As Rectangle, ByVal p As Pen)
        Dim i As Integer
        Dim p1 As Pen = New Pen(getDarkColor(p.Color, 50))
        Dim p2 As Pen = New Pen(getLightColor(p.Color, 50))

        For i = 0 To p.Width
            Dim r1 As Rectangle = New Rectangle(r.X + i, r.Y + i, r.Width - i * 2, r.Height - i * 2)
            g.DrawArc(p2, r1, -45, 180)
            g.DrawArc(p1, r1, 135, 180)
        Next
    End Sub

End Class

