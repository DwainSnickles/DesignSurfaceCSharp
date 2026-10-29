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

Imports System.Drawing
Imports System.Windows.Forms

Namespace GraphicObjects


    <Serializable()> Public Class TextGraphic
        Inherits GraphicObjects


        Protected Shadows m_Font As Font
        Protected m_Text As String = ""
        Protected m_Color As Color = Color.Black
        Protected m_Color1 As Color = Color.White
        Protected m_Depth As Integer
        Public Shared Fonts As Font
        Public Shared strText As String
        Protected m_TextLabel As Label
        Private Shared m_TextImage As Image
        Private theImage As Image

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal TheLabel As Label)
            Me.New()

            TextLabel = TheLabel
            Text = TheLabel.Text
            Fonts = TheLabel.Font

            Dim BM As New Bitmap(TheLabel.Width, TheLabel.Height)
            Dim brush1 As New SolidBrush(TheLabel.ForeColor)

            Using gr As Graphics = Graphics.FromImage(BM)
                gr.DrawImage(BM, 0, 0, New Rectangle(0, 0, TheLabel.Width, TheLabel.Height), GraphicsUnit.Pixel)
                BM.MakeTransparent()
                gr.DrawString(TheLabel.Text, TheLabel.Font, brush1, New Point(0, 0))
            End Using

            TextImage = BM
            Me.AutoSize = True

        End Sub

        Public Sub New(ByVal ImageText As Image)
            'Me.New()
            'TextLabel = TheLabel
            'm_TextImage = TextToImage(TheLabel)
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal text As String, ByVal textFont As Font, ByVal textColor As Color)
            Me.New()
            Me.SetPosition(graphicPosition)
            Me.Font = textFont
            Me.Text = text
            Me.Color = textColor
        End Sub

        'Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal text As String, ByVal textFont As Font, ByVal textColor As Color)

        '    Me.New(New Point(posX, posY), text, textFont, textColor)

        '    Me.AutoSize = True

        '    Dim lbl As New Label
        '    lbl.AutoSize = True
        '    lbl.Font = textFont
        '    lbl.Text = text
        '    lbl.ForeColor = textColor
        '    lbl.Invalidate()

        '    Dim BM As New Bitmap(lbl.Width, lbl.Height)
        '    Dim brush1 As New SolidBrush(lbl.ForeColor)

        '    Using gr As Graphics = Graphics.FromImage(BM)
        '        gr.DrawImage(BM, 0, 0, New Rectangle(lbl.Location.X, lbl.Location.Y, lbl.Width, lbl.Height), GraphicsUnit.Pixel)
        '        BM.MakeTransparent()
        '        gr.DrawString(text, textFont, brush1, New Point(posX, posY))
        '    End Using
        '    TextImage = BM

        'End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal text As String,
                        ByVal textFont As Font, ByVal textColor As Color, ByVal EmbossColor As Color, ByVal Depth As Integer)

            Me.New(New Point(posX, posY), text, textFont, textColor)
            Me.m_Color1 = EmbossColor
        End Sub

        Public Sub New(ByVal graphicPosition As Point, ByVal text As String, ByVal textFont As Font, ByVal textColor As Color, ByVal Rotation As Single)
            Me.New(graphicPosition, text, textFont, textColor)
            Me.Rotation = Rotation
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal text As String,
                        ByVal textFont As Font, ByVal textColor As Color, ByVal Rotation As Single)

            Me.New(New Point(posX, posY), text, textFont, textColor, Rotation)
        End Sub

        Public Sub New(graphicPosition As Point, theImage As Image)
            MyBase.New()
            Me.theImage = theImage
        End Sub

#End Region

        Public Property TextLabel() As Label
            Get
                Return m_TextLabel
            End Get
            Set(ByVal Value As Label)
                m_TextLabel = Value
                'If m_TextLabel IsNot Nothing Then
                '    TextToImage(m_TextLabel)
                'End If
            End Set
        End Property

        Public Overrides Property Font() As Font
            Get
                Return m_Font
            End Get
            Set(ByVal Value As Font)
                m_Font = Value
                Fonts = Value
            End Set
        End Property

        Public Property LabelText() As String
            Get
                Return m_Text
            End Get
            Set(ByVal Value As String)
                m_Text = Value
            End Set
        End Property

        Public Property Depth() As Integer
            Get
                Return m_Depth
            End Get
            Set(ByVal Value As Integer)
                m_Depth = Value
            End Set
        End Property

        Public Property Color() As System.Drawing.Color
            Get
                Return m_Color
            End Get
            Set(ByVal Value As System.Drawing.Color)
                m_Color = Value
            End Set
        End Property

        Public Property EmbossColor() As System.Drawing.Color
            Get
                Return m_Color1
            End Get
            Set(ByVal Value As System.Drawing.Color)
                m_Color1 = Value
            End Set
        End Property

        Public Property TextImage() As Image
            Get
                Return m_TextImage
            End Get
            Set(ByVal Value As Image)
                m_TextImage = Value
            End Set
        End Property

        Public Function TextToImage(ByVal label As Label) As Image

            Dim BM As New Bitmap(label.Width, label.Height)
            Dim brush1 As New SolidBrush(label.ForeColor)

            Using gr As Graphics = Graphics.FromImage(BM)
                gr.DrawImage(BM, 0, 0, New Rectangle(label.Location.X, label.Location.Y, label.Width, label.Height), GraphicsUnit.Pixel)
                BM.MakeTransparent()
                gr.DrawString(label.Text, label.Font, brush1, New Point(0, 0))
            End Using

            Return BM
        End Function

        Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
            Dim gContainer As System.Drawing.Drawing2D.GraphicsContainer
            Dim myMatrix As Drawing2D.Matrix
            gContainer = g.BeginContainer()
            myMatrix = g.Transform()
            If m_Rotation <> 0 Then
                myMatrix.RotateAt(m_Rotation, New PointF(X, Y), Drawing.Drawing2D.MatrixOrder.Append)
                g.Transform = myMatrix
            End If
            If Me.AutoSize Then
                Dim mySize As SizeF = g.MeasureString(m_Text, m_Font)
                Me.Width = CInt(mySize.Width)
                Me.Height = CInt(mySize.Height)
                g.DrawString(m_Text, m_Font, New SolidBrush(m_Color), X, Y)
            Else
                Dim rect As New System.Drawing.RectangleF(X, Y, Me.Width, Me.Height)
                g.DrawString(m_Text, m_Font, New SolidBrush(m_Color), rect)
            End If
            g.EndContainer(gContainer)
        End Sub

    End Class

End Namespace