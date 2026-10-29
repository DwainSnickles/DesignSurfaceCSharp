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
Imports System.Xml.Serialization
Imports System.Drawing
Imports System.Math

Namespace GraphicObjects

    <Serializable()> Public MustInherit Class ShapeGraphic
        Inherits GraphicObjects

        Protected Shadows m_lineWidth As Single = 1
        Protected Shadows m_lineColor As Color = Color.Black
        Protected Shadows m_fillColor As Color = Color.White
        Protected Shadows m_fill As Boolean = False
        Protected X1 As Integer = 0
        Protected Y1 As Integer = 0
        Protected Y2 As Integer = 0
        Protected X2 As Integer = 0

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overridable Shadows Property LineWidth() As Single
            Get
                Return m_lineWidth
            End Get
            Set(ByVal Value As Single)
                If Value > 0 Then
                    m_lineWidth = Value
                Else
                    Throw New ArgumentOutOfRangeException("LineWidth", "Line Width must be > 0")
                End If
            End Set
        End Property

        Public Overrides Property LineColor() As Color
            Get
                Return m_lineColor
            End Get
            Set(ByVal Value As Color)
                m_lineColor = Value
            End Set
        End Property

        Public Overrides Property Fill() As Boolean
            Get
                Return m_fill
            End Get
            Set(ByVal Value As Boolean)
                m_fill = Value
            End Set
        End Property

        Public Overrides Property FillColor() As Color
            Get
                Return m_fillColor
            End Get
            Set(ByVal Value As Color)
                m_fillColor = Value
            End Set
        End Property

    End Class

    <Serializable()> Public Class LineGraphic
        Inherits ShapeGraphic

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal startPosition As Point)
            Me.New()
            Me.SetStartPosition(startPosition)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer)
            Me.New(New Point(posX, posY))
        End Sub

        Public Sub New(ByVal startPosition As Point, ByVal endPosition As Point)
            Me.New(startPosition)
            Me.SetEndPosition(endPosition)
            Me.AutoSize = False
        End Sub

        Public Sub New(ByVal startX As Integer, ByVal startY As Integer, ByVal endPosition As Point)
            Me.New(New Point(startX, startY), endPosition)
        End Sub

        Public Sub New(ByVal startX As Integer, ByVal startY As Integer, ByVal endX As Integer, ByVal endY As Integer)
            Me.New(New Point(startX, startY), New Point(endX, endY))
        End Sub

        Public Sub New(ByVal startPosition As Point, ByVal endPosition As Point, ByVal lineWidth As Single, ByVal lineColor As Color)
            Me.New(startPosition)
            Me.SetEndPosition(endPosition)
            Me.LineWidth = lineWidth
            Me.LineColor = lineColor
            Me.AutoSize = False
        End Sub

        Public Sub New(ByVal startX As Integer, ByVal startY As Integer, ByVal endX As Integer, ByVal endY As Integer, ByVal lineWidth As Single, ByVal lineColor As Color)
            Me.New(New Point(startX, startY), New Point(endX, endY))
            Me.LineWidth = lineWidth
            Me.LineColor = lineColor
            Me.AutoSize = False
        End Sub

#End Region

        Public Overloads Overrides Function HitTest(ByVal pt As System.Drawing.Point) As Boolean
            Dim gp As New Drawing2D.GraphicsPath
            Dim myMatrix As New Drawing2D.Matrix
            Dim myPen As New Pen(Me.LineColor, Me.LineWidth)
            gp.AddLine(X, Y, X + Width, Y + Height)
            myMatrix.RotateAt(Me.Rotation, New PointF(Me.X, Me.Y), Drawing.Drawing2D.MatrixOrder.Append)
            gp.Transform(myMatrix)
            Return gp.IsOutlineVisible(pt, myPen)
        End Function

        Public Function GetStartPosition() As Point
            Return Me.GetPosition()
        End Function

        Public Sub SetStartPosition(ByVal Value As Point)
            Me.SetPosition(Value)
        End Sub

        Public Function GetEndPosition() As Point
            Dim endPosition As New Point(Me.m_Position.X, Me.m_Position.Y)
            endPosition.X += Me.m_Size.Width
            endPosition.Y += Me.m_Size.Height
            Return endPosition
        End Function

        Public Sub SetEndPosition(ByVal Value As Point)
            m_Size.Width = Value.X - Me.m_Position.X
            m_Size.Height = Value.Y - Me.m_Position.Y
        End Sub

        Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
            Dim gContainer As System.Drawing.Drawing2D.GraphicsContainer
            Dim myMatrix As Drawing2D.Matrix
            gContainer = g.BeginContainer()
            myMatrix = g.Transform()
            If m_Rotation <> 0 Then
                myMatrix.RotateAt(m_rotation, New PointF(X, Y), Drawing.Drawing2D.MatrixOrder.Append)
                g.Transform = myMatrix
            End If
            Dim myPen As New Pen(Me.LineColor, Me.LineWidth)
            g.DrawLine(myPen, X, Y, X + Width, Y + Height)
            g.EndContainer(gContainer)
        End Sub
    End Class


    <Serializable()> Public Class RectangleGraphic
        Inherits ShapeGraphic

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal graphicPosition As Point)
            Me.New()
            Me.SetPosition(graphicPosition)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer)
            Me.New(New Point(posX, posY))
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal graphicSize As Size)
            Me.New(graphicPosition)
            Me.SetSize(graphicSize)
            Me.AutoSize = False
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal graphicSize As Size)
            Me.New(New Point(posX, posY), graphicSize)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal width As Integer, ByVal height As Integer)
            Me.New(New Point(posX, posY), New Size(width, height))
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal Rotation As Single)
            Me.New()
            Me.SetPosition(graphicPosition)
            Me.Rotation = Rotation
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), Rotation)
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal graphicSize As Size, ByVal Rotation As Single)
            Me.New(graphicPosition, Rotation)
            Me.SetSize(graphicSize)
            Me.AutoSize = False
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal graphicSize As Size, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), graphicSize, Rotation)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal width As Integer, _
                               ByVal height As Integer, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), New Size(width, height), Rotation)
        End Sub

#End Region

        Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
            Dim gContainer As System.Drawing.Drawing2D.GraphicsContainer
            Dim myMatrix As Drawing2D.Matrix
            gContainer = g.BeginContainer()
            myMatrix = g.Transform()
            If m_Rotation <> 0 Then
                myMatrix.RotateAt(m_rotation, New PointF(X, Y), _
                    Drawing.Drawing2D.MatrixOrder.Append)
                g.Transform = myMatrix
            End If
            Dim rect As New Rectangle(X, Y, Width, Height)

            If Me.Fill Then
                g.FillRectangle(New SolidBrush(Me.FillColor), rect)
            End If
            Dim myPen As New Pen(Me.LineColor, Me.LineWidth)
            g.DrawRectangle(myPen, rect)

        End Sub
    End Class

    <Serializable()> Public Class EllipseGraphic
        Inherits ShapeGraphic

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal graphicPosition As Point)
            Me.New()
            Me.SetPosition(graphicPosition)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer)
            Me.New(New Point(posX, posY))
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal graphicSize As Size)
            Me.New(graphicPosition)
            Me.SetSize(graphicSize)
            Me.AutoSize = False
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal graphicSize As Size)
            Me.New(New Point(posX, posY), graphicSize)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal width As Integer, ByVal height As Integer)
            Me.New(New Point(posX, posY), New Size(width, height))
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal Rotation As Single)
            Me.New()
            Me.SetPosition(graphicPosition)
            Me.Rotation = Rotation
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), Rotation)
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal graphicSize As Size, ByVal Rotation As Single)
            Me.New(graphicPosition, Rotation)
            Me.SetSize(graphicSize)
            Me.AutoSize = False
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal graphicSize As Size, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), graphicSize, Rotation)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal width As Integer, _
                               ByVal height As Integer, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), New Size(width, height), Rotation)
        End Sub

#End Region

        Public Overrides Sub Draw(ByVal g As Graphics)
            Dim gContainer As Drawing2D.GraphicsContainer
            Dim myMatrix As Drawing2D.Matrix
            gContainer = g.BeginContainer()
            myMatrix = g.Transform()
            If m_Rotation <> 0 Then
                myMatrix.RotateAt(m_rotation, New PointF(X, Y), _
                    Drawing2D.MatrixOrder.Append)
                g.Transform = myMatrix
            End If
            Dim rect As New Rectangle(X, Y, Width, Height)
            If Me.Fill Then
                g.FillEllipse(New SolidBrush(Me.FillColor), rect)
            End If
            Dim myPen As New Pen(Me.LineColor, Me.LineWidth)
            g.DrawEllipse(myPen, rect)

            g.EndContainer(gContainer)
        End Sub

    End Class

End Namespace


