Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing

Public Class greyScale2

    ' Remove the color from the image
    ' Do it with a colormatrix

    Public Shared Function ProcessImage(ByRef _sourceBitmap As Bitmap) As Bitmap

        Dim _resultBitmap As Bitmap
        Dim ia As New ImageAttributes
        Dim colorMatrixElements As Single()() = { _
           New Single() {0.299, 0.299, 0.299, 0, 0}, _
           New Single() {0.587, 0.587, 0.587, 0, 0}, _
           New Single() {0.114, 0.114, 0.114, 0, 0}, _
           New Single() {0, 0, 0, 1, 0}, _
           New Single() {0, 0, 0, 0, 1}}

        Dim colorMatrix As New ColorMatrix(colorMatrixElements)
        ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Bitmap)
        _resultBitmap = New Bitmap(_sourceBitmap.Width, _sourceBitmap.Height)
        Dim g As Graphics = Graphics.FromImage(_resultBitmap)

        ia.SetColorMatrix(colorMatrix)
        g.DrawImage(_sourceBitmap, New Rectangle(0, 0, _sourceBitmap.Width, _sourceBitmap.Height), _
        0, 0, _sourceBitmap.Width, _sourceBitmap.Height, GraphicsUnit.Pixel, ia)
        g.Dispose()
        Return _resultBitmap

    End Function

End Class