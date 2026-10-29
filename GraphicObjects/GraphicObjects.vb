'Copyright (C) 2002 Microsoft Corporation
'All rights reserved.
'
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER
'EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
'MERCHANTIBILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'
'Date: June 2002
'Author: Duncan Mackenzie
'
'Requires the release version of .NET Framework
Imports Microsoft.MSDN.Samples
Imports System.Drawing
Imports System.Math

Namespace GraphicObjects

    <Serializable()>
    Public Class GraphicObjectCollection
        Inherits CollectionBase

        Protected m_HorizRes As Integer = 72
        Protected m_VertRes As Integer = 72
        Protected DRAG_HANDLE_SIZE As Integer = 8
        Protected ImageGraphic As Graphics = Nothing
        Protected m_Gobj As GraphicObjects
        Public Drag_Handles(9) As RectangleF
        Public Event ObjectChanged(e As GraphicObjects)
        Public DragRectangleF As New RectangleF()
        Public Shared Zoom As Single
        Private DHamdles As RectangleF
        Private m_MousePosition As Point
        Private m_ImageObj As Image

        Public Property DragHandles() As RectangleF
            Get
                Return Drag_Handles(9)
            End Get
            Set(ByVal value As RectangleF)
                DHamdles = value
            End Set
        End Property

        Public Property ImageObj() As Image
            Get
                Return m_ImageObj
            End Get
            Set(ByVal value As Image)
                m_ImageObj = value
            End Set
        End Property

        Protected Function ZoomRectangle(ByVal originalRect As Rectangle) As Rectangle
            Dim myNewRect As New Rectangle(originalRect.X * Zoom,
                originalRect.Y * Zoom,
                originalRect.Width * Zoom,
                originalRect.Height * Zoom)
            Return myNewRect
        End Function

        Protected Function DeZoomRectangle(ByVal originalRect As Rectangle) As Rectangle
            Dim myNewRect As New Rectangle(originalRect.X / Zoom,
                originalRect.Y / Zoom,
                originalRect.Width / Zoom,
                originalRect.Height / Zoom)
            Return myNewRect
        End Function

        Protected Function ZoomRectangleF(ByVal originalRect As RectangleF) As RectangleF
            Dim myNewRect As New RectangleF(originalRect.X * Zoom,
                originalRect.Y * Zoom,
                originalRect.Width * Zoom,
                originalRect.Height * Zoom)
            Return myNewRect
        End Function

        Protected Function DeZoomRectangleF(ByVal originalRect As RectangleF) As RectangleF
            Dim myNewRect As New RectangleF(originalRect.X / Zoom,
                originalRect.Y / Zoom,
                originalRect.Width / Zoom,
                originalRect.Height / Zoom)
            Return myNewRect
        End Function

        Private Property GraphicObjectChanged As GraphicObjects
            Set(value As GraphicObjects)
                m_Gobj = value
            End Set
            Get
                RaiseEvent ObjectChanged(m_Gobj)
                Return m_Gobj
            End Get
        End Property

        Sub New(gobj As GraphicObjects)

        End Sub

        Public Sub LoadDrag_Handles()
            Dim RectHeight As Integer = 8

            If DragRectangleF.Height < 50 Then
                RectHeight = 4
            ElseIf DragRectangleF.Height < 25 Then
                RectHeight = 2
            End If

            Drag_Handles(0) = New RectangleF(DragRectangleF.X - RectHeight / 2, DragRectangleF.Y - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(1) = New RectangleF(DragRectangleF.X + DragRectangleF.Width / 2 - RectHeight / 2, DragRectangleF.Y - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(2) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(3) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height / 2 - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(4) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(5) = New RectangleF(DragRectangleF.X + DragRectangleF.Width / 2 - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(6) = New RectangleF(DragRectangleF.X - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - RectHeight / 2, RectHeight, RectHeight)
            Drag_Handles(7) = New RectangleF(DragRectangleF.X - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height / 2 - RectHeight / 2, RectHeight, RectHeight)
            'last 2 drag handles are for rotating object
            Drag_Handles(8) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + 10, RectHeight, RectHeight)
            Drag_Handles(9) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - 20, RectHeight, RectHeight)

        End Sub

        Public Sub DrawSelectedObject(ByVal g As Graphics, ByVal selectedObject As GraphicObjects, ByVal Scale As Single)

            If selectedObject Is Nothing Then
                Exit Sub
            End If

            Dim gCon1, gCon2 As Drawing2D.GraphicsContainer
            gCon1 = g.BeginContainer
            g.ScaleTransform(Scale, Scale,
                 Drawing.Drawing2D.MatrixOrder.Append)
            gCon2 = g.BeginContainer
            g.PageUnit = GraphicsUnit.Pixel

            If Not selectedObject Is Nothing Then

                Dim selectionPen As New Pen(Color.FromKnownColor(KnownColor.Black))
                selectionPen.DashStyle = Drawing2D.DashStyle.Solid
                selectionPen.Width = 1
                Dim GrabHandleBrush As Brush = New SolidBrush(Color.Black)
                Dim HandlePen As New Pen(Color.FromKnownColor(KnownColor.Black))

                If selectedObject.Rotation <> 0 Then
                    Dim myMatrix As Drawing2D.Matrix
                    myMatrix = g.Transform()
                    myMatrix.RotateAt(selectedObject.Rotation, New PointF(selectedObject.X, selectedObject.Y), Drawing.Drawing2D.MatrixOrder.Append)
                    g.Transform = myMatrix
                    'g.TranslateTransform(selectedObject.X, selectedObject.Y, Drawing.Drawing2D.MatrixOrder.Append)
                    'RotatedBitmap = RotateImage(selectedObject.Image, selectedObject.Rotation)
                End If

                DragRectangleF = New RectangleF(selectedObject.X, selectedObject.Y, selectedObject.Width, selectedObject.Height)

                Dim RectHeight As Integer = 8

                If selectedObject.Height < 50 Then
                    RectHeight = 4
                ElseIf selectedObject.Height < 25 Then
                    RectHeight = 2
                End If

                Drag_Handles(0) = New RectangleF(DragRectangleF.X - RectHeight / 2, DragRectangleF.Y - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(1) = New RectangleF(DragRectangleF.X + DragRectangleF.Width / 2 - RectHeight / 2, DragRectangleF.Y - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(2) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(3) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height / 2 - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(4) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(5) = New RectangleF(DragRectangleF.X + DragRectangleF.Width / 2 - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(6) = New RectangleF(DragRectangleF.X - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - RectHeight / 2, RectHeight, RectHeight)
                Drag_Handles(7) = New RectangleF(DragRectangleF.X - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height / 2 - RectHeight / 2, RectHeight, RectHeight)
                'last 2 drag handles are for rotating object
                Drag_Handles(8) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + 10, RectHeight, RectHeight)
                Drag_Handles(9) = New RectangleF(DragRectangleF.X + DragRectangleF.Width - RectHeight / 2, DragRectangleF.Y + DragRectangleF.Height - 20, RectHeight, RectHeight)

                g.DrawRectangle(New Pen(Color.Gray), DragRectangleF.X, DragRectangleF.Y, DragRectangleF.Width, DragRectangleF.Height)
                g.FillRectangles(Brushes.Gray, Drag_Handles)
                g.DrawRectangles(Pens.Black, Drag_Handles)

                Dim RotationDragHandle As New RectangleF()
                RotationDragHandle = Drag_Handles(8)
                g.FillRectangle(Brushes.YellowGreen, RotationDragHandle)

                Dim RotationDragHandle1 As New RectangleF()
                RotationDragHandle1 = Drag_Handles(9)
                g.FillRectangle(Brushes.YellowGreen, RotationDragHandle1)

            End If

            g.EndContainer(gCon2)
            g.EndContainer(gCon1)

        End Sub

        Public Sub DrawObjects(ByVal g As Graphics, ByVal Scale As Single)
            Dim drawObj As GraphicObjects
            Dim i As Integer
            Dim gCon As Drawing2D.GraphicsContainer
            Dim myOriginalMatrix As Drawing2D.Matrix
            myOriginalMatrix = g.Transform()
            gCon = g.BeginContainer
            g.PageUnit = GraphicsUnit.Pixel
            g.ScaleTransform(Scale, Scale)
            If Not Me.InnerList Is Nothing AndAlso Me.InnerList.Count > 0 Then
                For i = 0 To Me.InnerList.Count - 1
                    drawObj = CType(Me.InnerList(i), GraphicObjects)
                    drawObj.Draw(g)
                    'ImageGraphic = g
                Next
            End If
            g.EndContainer(gCon)
            g.Transform = myOriginalMatrix

        End Sub

        Public Sub PrintObjects(ByVal g As Graphics)

            Dim drawObj As GraphicObjects
            Dim i As Integer
            g.PageUnit = GraphicsUnit.Pixel
            If Not Me.InnerList Is Nothing AndAlso Me.InnerList.Count > 0 Then
                For i = 0 To Me.InnerList.Count - 1
                    drawObj = CType(Me.InnerList(i), GraphicObjects)
                    drawObj.Draw(g)
                Next
            End If
        End Sub

        Public Function RotateImage(TheImage As Image, Rotation As Single) As Image
            Dim RotatedBM As Image

            ' Copy the output bitmap from the source image.
            Dim bm_in As New Bitmap(TheImage)

            ' Make an array of points defining the
            ' image's corners.
            Dim wid As Single = bm_in.Width
            Dim hgt As Single = bm_in.Height
            Dim corners As Point() = {New Point(0, 0), New Point(wid, 0), New Point(0, hgt), New Point(wid, hgt)}

            ' Translate to center the bounding box at the origin.
            Dim cx As Single = wid / 2
            Dim cy As Single = hgt / 2
            Dim i As Long
            For i = 0 To 3
                corners(i).X -= cx
                corners(i).Y -= cy
            Next i

            ' Rotate.
            Dim theta As Single = Single.Parse(Rotation) * PI / 180.0
            Dim sin_theta As Single = Sin(theta)
            Dim cos_theta As Single = Cos(theta)
            Dim X As Single
            Dim Y As Single
            For i = 0 To 3
                X = corners(i).X
                Y = corners(i).Y
                corners(i).X = X * cos_theta + Y * sin_theta
                corners(i).Y = -X * sin_theta + Y * cos_theta
            Next i

            ' Translate so X >= 0 and Y >=0 for all corners.
            Dim xmin As Single = corners(0).X
            Dim ymin As Single = corners(0).Y
            For i = 1 To 3
                If xmin > corners(i).X Then xmin = corners(i).X
                If ymin > corners(i).Y Then ymin = corners(i).Y
            Next i
            For i = 0 To 3
                corners(i).X -= xmin
                corners(i).Y -= ymin
            Next i

            ' Create an output Bitmap and Graphics object.
            Dim bm_out As New Bitmap(CInt(-2 * xmin), CInt(-2 * ymin))
            Dim gr_out As Graphics = Graphics.FromImage(bm_out)

            ' Drop the last corner lest we confuse DrawImage, 
            ' which expects an array of three corners.
            ReDim Preserve corners(2)

            ' Draw the result onto the output Bitmap.
            gr_out.DrawImage(bm_in, corners)

            ' Display the result.
            RotatedBM = bm_out

            Return RotatedBM
        End Function

        Public Function FindObjectAtPoint(ByVal pt As Point) As GraphicObjects
            Dim drawObj As GraphicObjects
            Dim i As Integer
            If Not Me.InnerList Is Nothing AndAlso
                    Me.InnerList.Count > 0 Then
                For i = Me.InnerList.Count - 1 To 0 Step -1
                    drawObj = CType(Me.InnerList(i), GraphicObjects)
                    If drawObj.HitTest(pt) Then
                        Return drawObj
                        Exit For
                    End If
                Next
            End If
            Return Nothing
        End Function

        Public Function SaveDrawnObjects(ByVal G As Graphics) As Image
            Dim drawObj As GraphicObjects
            Dim i As Integer
            Dim BM As Bitmap = New Bitmap(CInt(G.DpiX), CInt(G.DpiY))
            Dim gr As Graphics = Graphics.FromImage(BM)

            G.PageUnit = GraphicsUnit.Pixel
            gr.PageUnit = GraphicsUnit.Pixel
            If Not Me.InnerList Is Nothing AndAlso Me.InnerList.Count > 0 Then
                For i = 0 To Me.InnerList.Count - 1
                    drawObj = CType(Me.InnerList(i), GraphicObjects)
                    drawObj.Draw(G)
                    drawObj.Draw(gr)
                Next
            End If
            Return BM.Clone
        End Function

        Public Property HorizontalResolution() As Integer
            Get
                Return m_HorizRes
            End Get
            Set(ByVal Value As Integer)
                m_HorizRes = Value
            End Set
        End Property

        Public Property VerticalResolution() As Integer
            Get
                Return m_VertRes
            End Get
            Set(ByVal Value As Integer)
                m_VertRes = Value
            End Set
        End Property
        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal value As GraphicObjectCollection)
            MyBase.New()
            Me.AddRange(value)
        End Sub

        Public Sub New(ByVal value() As GraphicObjects)
            MyBase.New()
            Me.AddRange(value)
        End Sub

        Default Public Property Item(ByVal index As Integer) As GraphicObjects
            Get
                Return CType(List(index), GraphicObjects)
            End Get
            Set(ByVal Value As GraphicObjects)
                List(index) = Value
            End Set
        End Property

        Public Property RotatedBitmap As Image

        Public Function Add(ByVal value As GraphicObjects) As Integer
            value.Container = Me

            Return List.Add(value)
        End Function

        Public Overloads Sub AddRange(ByVal value() As GraphicObjects)
            Dim i As Integer = 0
            Do While (i < value.Length)
                Me.Add(value(i))
                i = (i + 1)
            Loop
        End Sub

        Public Overloads Sub AddRange(ByVal value As GraphicObjectCollection)
            Dim i As Integer = 0
            Do While (i < value.Count)
                Me.Add(value(i))
                i = (i + 1)
            Loop
        End Sub

        Public Function Contains(ByVal value As GraphicObjects) As Boolean
            Return List.Contains(value)
        End Function

        Public Sub CopyTo(ByVal array() As GraphicObjects, ByVal index As Integer)
            List.CopyTo(array, index)
        End Sub

        Public Function IndexOf(ByVal value As GraphicObjects) As Integer
            Return List.IndexOf(value)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal value As GraphicObjects)
            List.Insert(index, value)
        End Sub

        'See also 'System.Collections.IEnumerator'
        Public Shadows Function GetEnumerator() As GraphicObjectEnumerator
            Return New GraphicObjectEnumerator(Me)
        End Function

        Public Sub Remove(ByVal value As GraphicObjects)
            List.Remove(value)
        End Sub

        Public Class GraphicObjectEnumerator
            Inherits Object
            Implements IEnumerator

            Private baseEnumerator As IEnumerator
            Private temp As IEnumerable

            Public Sub New(ByVal mappings As GraphicObjectCollection)
                MyBase.New()
                Me.temp = CType(mappings, IEnumerable)
                Me.baseEnumerator = temp.GetEnumerator
            End Sub

            Public ReadOnly Property Current() As GraphicObjects
                Get
                    Return CType(baseEnumerator.Current, GraphicObjects)
                End Get
            End Property

            ReadOnly Property IEnumerator_Current() As Object Implements IEnumerator.Current
                Get
                    Return baseEnumerator.Current
                End Get
            End Property

            Public Function MoveNext() As Boolean
                Return baseEnumerator.MoveNext
            End Function

            Function IEnumerator_MoveNext() As Boolean Implements IEnumerator.MoveNext
                Return baseEnumerator.MoveNext
            End Function

            Public Sub Reset()
                baseEnumerator.Reset()
            End Sub

            Sub IEnumerator_Reset() Implements IEnumerator.Reset
                baseEnumerator.Reset()
            End Sub

        End Class

        'Public Sub MousePosition()
        '    Throw New NotImplementedException()
        'End Sub
    End Class
End Namespace
