'******************************************************************
'***************Copyright Bobo Enterprises 2005********************
'***************Written by MrBobo**********************************
'This code was submitted to Planet Source Code (www.planetsourcecode.com)
'If you downloaded it elsewhere, they stole it and I'll eat them alive


Imports System.Drawing
Imports System.Windows.Forms

Module ModGen
    'API used to create an offscreen bitmap
    Private Declare Function CreateDC Lib "gdi32" Alias "CreateDCA" (ByVal lpDriverName As String, ByVal lpDeviceName As String, ByVal lpOutput As String, ByVal lpInitData As String) As Integer
    Private Declare Function CreateCompatibleDC Lib "GDI32" (ByVal hDC As Integer) As Integer
    Private Declare Function CreateCompatibleBitmap Lib "GDI32" (ByVal hDC As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer) As Integer
    Private Declare Function SelectObject Lib "GDI32" (ByVal hDC As Integer, ByVal hObject As Integer) As Integer
    Private Declare Function DeleteObject Lib "GDI32" (ByVal hObj As Integer) As Integer
    Private Declare Function BitBlt Lib "GDI32" (ByVal hDestDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal SrcX As Integer, ByVal SrcY As Integer, ByVal Rop As Integer) As Integer
    Private Declare Function DeleteDC Lib "GDI32" (ByVal hDC As Integer) As Integer
    Private Const SRCCOPY As Integer = &HCC0020
    'Draw a pallette - like the commondialog color dialog
    Public Function BuildPalette() As Bitmap
        Dim zx As Integer
        Dim zy As Integer
        Dim bmp As New Bitmap(240, 240)
        For zx = 0 To 239
            For zy = 239 To 0 Step -1
                bmp.SetPixel(zx, zy, ColorFromHSB(zx, 239 - zy, 120))
            Next
        Next
        Return bmp
    End Function
    Public Function BuildScreen() As Bitmap
        'Create an off screen bitmap of the current desktop
        'Borrowed code - cant remember where - Microsoft ?
        Dim hDesktopDC As Integer
        Dim hOffscreenDC As Integer
        Dim hBitmap As Integer
        Dim hOldBmp As Integer
        Dim bmp As Bitmap = Nothing
        hDesktopDC = CreateDC("DISPLAY", "", "", "")
        hOffscreenDC = CreateCompatibleDC(hDesktopDC)
        If hDesktopDC Then
            hOffscreenDC = CreateCompatibleDC(hDesktopDC)
            If hOffscreenDC Then
                hBitmap = CreateCompatibleBitmap(hDesktopDC, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
                If hBitmap Then
                    hOldBmp = SelectObject(hOffscreenDC, hBitmap)
                    BitBlt(hOffscreenDC, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, hDesktopDC, 0, 0, SRCCOPY)
                    bmp = Bitmap.FromHbitmap(New IntPtr(hBitmap))
                    DeleteObject(SelectObject(hOffscreenDC, hOldBmp))
                End If
                DeleteDC(hOffscreenDC)
            End If
            DeleteDC(hDesktopDC)
        End If
        Return bmp
    End Function

    Public Sub VerticalLabel(ByVal TargetBox As PictureBox, ByVal lblText As String)
        'Printing text vertically
        Dim TextBrush As New SolidBrush(System.Drawing.SystemColors.MenuText)
        TargetBox.Image = New Bitmap(TargetBox.Width, TargetBox.Height)
        Dim g As Graphics = Graphics.FromImage(TargetBox.Image)
        Dim sFormat As New StringFormat
        sFormat.FormatFlags = StringFormatFlags.DirectionVertical
        g.DrawString(lblText, TargetBox.Font, TextBrush, 0, 0, sFormat)
        TargetBox.Invalidate()
        TextBrush.Dispose()
        g.Dispose()
        sFormat.Dispose()
    End Sub
    Public Function ColorFromHSB(ByVal Hue As Single, ByVal Saturation As Single, ByVal Brightness As Single, Optional ByVal UseNETHSB As Boolean = False) As Color
        'This is a slightly modified version of code submitted by Morten Kvistgaard
        'I found this routine more efficient than the one I was using previously
        If Not UseNETHSB Then
            Hue *= 360 / 240
            Saturation /= 240
            Brightness /= 240
        End If
        If Brightness = 0 Then
            Return Color.Black
        Else
            If Saturation = 0 Then
                Return Color.FromArgb(CInt(255 * Brightness), CInt(255 * Brightness), CInt(255 * Brightness))
            Else
                Dim i As Integer
                Dim temp1 As Double
                Dim temp2 As Double
                Hue /= 360
                Dim t3() As Double = {Hue + 1.0 / 3.0, Hue, Hue - 1.0 / 3.0}
                Dim clr() As Double = {0, 0, 0}
                If Brightness <= 0.5 Then
                    temp2 = Brightness * (1.0 + Saturation)
                Else
                    temp2 = Brightness + Saturation - Brightness * Saturation
                End If
                temp1 = 2.0 * Brightness - temp2
                For i = 0 To 2
                    If t3(i) < 0 Then t3(i) += 1.0
                    If t3(i) > 1 Then t3(i) -= 1.0
                    If 6.0 * t3(i) < 1.0 Then
                        clr(i) = temp1 + (temp2 - temp1) * t3(i) * 6.0
                    ElseIf 2.0 * t3(i) < 1.0 Then
                        clr(i) = temp2
                    ElseIf 3.0 * t3(i) < 2.0 Then
                        clr(i) = temp1 + (temp2 - temp1) * (2.0 / 3.0 - t3(i)) * 6.0
                    Else
                        clr(i) = temp1
                    End If
                Next i
                Return Color.FromArgb(CInt(255 * clr(0)), CInt(255 * clr(1)), CInt(255 * clr(2)))
            End If
        End If
    End Function

End Module
