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
Imports System.Windows.Forms.ControlPaint

Namespace GraphicObjects

    <Serializable()> Public MustInherit Class GraphicObjects
        Protected m_Position As New Point(0, 0)
        Protected m_Size As New Size(0, 0)
        Protected m_Location As New Size(0, 0)
        Protected m_Rotation As Single = 0
        Protected m_AutoSize As Boolean = True
        Protected m_Container As GraphicObjectCollection
        Protected m_Type As ObjectTypes
        Protected m_ImageNumber As Integer = 0
        Protected m_ImageTag As String = Nothing
        Protected m_ImagePath As String = Nothing
        Protected m_ImageName As String = Nothing
        Protected m_ImageText As String = Nothing
        Protected m_BackGroundColor As Color = Nothing
        Protected m_BackGroundImage As Image = Nothing
        Protected m_Font As Font = Nothing
        Protected m_FontColor As Color = Nothing
        Protected m_FontSize As Integer = Nothing
        Protected m_FontStyle As FontStyle = Nothing
        Protected m_FontUnderlined As Boolean = Nothing
        Protected m_FontStrikeOut As Boolean = Nothing
        Protected M_FontScript As Integer = Nothing
        Protected m_LineWidth As Integer = Nothing
        Protected m_FillColor As Color = Nothing
        Protected m_Fill As Boolean = False
        Protected m_LineColor As Color = Nothing
        Protected m_Graphics As Graphics = Nothing
        'Protected m_RectangleF As RectangleF
        Private Drag_Handles(7) As RectangleF
        Protected m_MinimumSize As Integer
        Protected m_MaximumSize As Integer
        Protected m_MousePosition As New Point

        'Holds the font script selected value using the index Number
        Private Enum FontScripts
            Western = 1
            Hebrew = 2
            Arabic = 3
            Greek = 4
            Turkish = 5
            Baltic = 6
            CentralEuropean = 7
            Cryllic = 8
            Thai = 9
            Vietnamese = 10
        End Enum

        Enum ObjectTypes
            Linked = 1 'Handles image with known paths
            Background = 2
            Text = 3
            Image = 7
            CircularTextImage = 8
            EmbeddedImageGraphic = 9 'Handles images without paths
            Fill = 4
            ClonedObject = 5
            DeletedObject = 6
        End Enum

        ' Send the object to the back of the stack.
        Public Sub SendToBack(ByVal dr As GraphicObjects)
            If Not (dr Is Nothing) Then
                m_Container.Remove(dr)
                m_Container.Insert(0, dr)
            End If
        End Sub

        ' Bring the object to the front of the stack.
        Public Sub BringToFront(ByVal dr As GraphicObjects)
            If Not (dr Is Nothing) Then
                m_Container.Remove(dr)
                m_Container.Insert(m_Container.Count, dr)
            End If
        End Sub

        'Save Images Background color property
        Public Overridable Property BackGroundColor() As Color
            Get
                Return m_BackGroundColor
            End Get
            Set(ByVal Value As Color)
                m_BackGroundColor = Value
            End Set
        End Property

        'Save Images Text property
        Public Overridable Property Text() As String
            Get
                Return m_ImageText
            End Get
            Set(ByVal Value As String)
                m_ImageText = Value
            End Set
        End Property

        'Save Images Font property
        Public Overridable Property FontUnderlined() As Boolean
            Get
                Return m_FontUnderlined
            End Get
            Set(ByVal Value As Boolean)
                m_FontUnderlined = Value
            End Set
        End Property

        'Save Images Font property
        Public Overridable Property FontStrikeOut() As Boolean
            Get
                Return m_FontStrikeOut
            End Get
            Set(ByVal Value As Boolean)
                m_FontStrikeOut = Value
            End Set
        End Property

        'Save Images FontStyle property
        Public Overridable Property FontStyle() As FontStyle
            Get
                Return m_FontStyle
            End Get
            Set(ByVal Value As FontStyle)
                m_FontStyle = Value
            End Set
        End Property

        'Save Images Font property
        Public Overridable Property FontColor() As Color
            Get
                Return m_FontColor
            End Get
            Set(ByVal Value As Color)
                m_FontColor = Value
            End Set
        End Property

        'Save Images Font property
        Public Overridable Property FontSize() As Integer
            Get
                Return m_FontSize
            End Get
            Set(ByVal Value As Integer)
                m_FontSize = Value
            End Set
        End Property

        'Save Images Font property
        Public Overridable Property Font() As Font
            Get
                Return m_Font
            End Get
            Set(ByVal Value As Font)
                m_Font = Value
            End Set
        End Property

        'Save Images FontScript property by passing the indexNumber
        Public Overridable Property FontScript() As Integer
            Get
                Return M_FontScript
            End Get
            Set(ByVal Value As Integer)
                Select Case FontScript
                    Case Value = 1
                        M_FontScript = FontScripts.Western 'Value
                    Case Value = 2
                        M_FontScript = FontScripts.Hebrew
                    Case Value = 3
                        M_FontScript = FontScripts.Arabic
                    Case Value = 4
                        M_FontScript = FontScripts.Greek
                    Case Value = 5
                        M_FontScript = FontScripts.Turkish
                    Case Value = 6
                        M_FontScript = FontScripts.Baltic
                    Case Value = 7
                        M_FontScript = FontScripts.CentralEuropean
                    Case Value = 8
                        M_FontScript = FontScripts.Cryllic
                    Case Value = 9
                        M_FontScript = FontScripts.Thai
                    Case Value = 1
                        M_FontScript = FontScripts.Vietnamese
                End Select

            End Set
        End Property

        'Save Images FillColor property
        Public Overridable Property LineColor() As Color
            Get
                Return m_LineColor
            End Get
            Set(ByVal Value As Color)
                m_LineColor = Value
            End Set
        End Property

        'Save Images FillColor property
        Public Overridable Property FillColor() As Color
            Get
                Return m_FillColor
            End Get
            Set(ByVal Value As Color)
                m_FillColor = Value
            End Set
        End Property

        'Save Images Fill property
        Public Overridable Property Fill() As Boolean
            Get
                Return m_Fill
            End Get
            Set(ByVal Value As Boolean)
                m_Fill = Value
            End Set
        End Property

        'Save Images Linewidth property
        Public Overridable Property LineWidth() As Integer
            Get
                Return m_LineWidth
            End Get
            Set(ByVal Value As Integer)
                m_LineWidth = Value
            End Set
        End Property

        Public Overridable Property ImageName() As String
            Get
                Return m_ImageName
            End Get
            Set(ByVal Value As String)
                m_ImageName = Value
            End Set
        End Property

        Public Overridable Property ImagePath() As String
            Get
                Return m_ImagePath
            End Get
            Set(ByVal Value As String)
                m_ImagePath = Value
            End Set
        End Property

        Public Overridable Property ImageNumber() As Integer
            Get
                Return m_ImageNumber
            End Get
            Set(ByVal Value As Integer)
                m_ImageNumber = Value
            End Set
        End Property

        Public Overridable Property objType() As ObjectTypes
            Get
                Return m_Type
            End Get
            Set(ByVal Value As ObjectTypes)
                m_Type = Value
            End Set
        End Property

        Public Overridable Property Container() As GraphicObjectCollection
            Get
                Return m_Container
            End Get
            Set(ByVal Value As GraphicObjectCollection)
                m_Container = Value
            End Set
        End Property

        Public Property MousePosition() As Point
            Get
                Return m_MousePosition
            End Get
            Set(ByVal value As Point)
                m_MousePosition = value
            End Set
        End Property

        Public Overridable Overloads Function HitTest(ByVal pt As System.Drawing.Point) As Boolean
            Dim gp As New Drawing2D.GraphicsPath
            Dim myMatrix As New Drawing2D.Matrix

            gp.AddRectangle(New Rectangle(X, Y, Width, Height))
            If Me.Rotation <> 0 Then
                myMatrix.RotateAt(Me.Rotation, New PointF(Me.X, Me.Y),
                    Drawing.Drawing2D.MatrixOrder.Append)
            End If

            gp.Transform(myMatrix)
            Return gp.IsVisible(pt)
        End Function

        Public Overridable Overloads Function HitTest(ByVal rect As RectangleF) As Boolean
            'is this object contained within the supplied rectangle
            Dim gp As New Drawing2D.GraphicsPath
            Dim myMatrix As New Drawing2D.Matrix

            gp.AddRectangle(New Rectangle(X, Y, Width, Height))
            If Me.Rotation <> 0 Then
                myMatrix.RotateAt(Me.Rotation, New PointF(Me.X, Me.Y),
                    Drawing.Drawing2D.MatrixOrder.Append)
            End If
            gp.Transform(myMatrix)
            Dim gpRect As Rectangle = Rectangle.Round(gp.GetBounds)
            Return rect.Contains(gpRect)
        End Function

#Region "Constructors"
        Protected Sub New()
        End Sub

        Protected Sub New(ByVal graphicPosition As Point)
            Me.New()
            Me.SetPosition(graphicPosition)
        End Sub

        Protected Sub New(ByVal posX As Integer, ByVal posY As Integer)
            Me.New(New Point(posX, posY))
        End Sub


        Protected Sub New(ByVal graphicPosition As Point, ByVal graphicSize As Size)
            Me.New(graphicPosition)
            Me.SetSize(graphicSize)
            Me.AutoSize = False
        End Sub

        Protected Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal graphicSize As Size)
            Me.New(New Point(posX, posY), graphicSize)
        End Sub

        Protected Sub New(ByVal posX As Integer, ByVal posY As Integer,
                ByVal width As Integer, ByVal height As Integer)
            Me.New(New Point(posX, posY), New Size(width, height))
        End Sub

        Protected Sub New(ByVal graphicPosition As Point, ByVal Rotation As Single)
            Me.New()
            Me.SetPosition(graphicPosition)
            Me.Rotation = Rotation
        End Sub

        Protected Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), Rotation)
        End Sub

        Protected Sub New(ByVal graphicPosition As Point, ByVal graphicSize As Size, ByVal Rotation As Single)
            Me.New(graphicPosition, Rotation)
            Me.SetSize(graphicSize)
            Me.AutoSize = False
        End Sub

        Protected Sub New(ByVal posX As Integer, ByVal posY As Integer,
            ByVal graphicSize As Size, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), graphicSize, Rotation)
        End Sub

        Protected Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal width As Integer,
                               ByVal height As Integer, ByVal Rotation As Single)
            Me.New(New Point(posX, posY), New Size(width, height), Rotation)
        End Sub

#End Region

        Public Property Image As Image

        Public Overridable Property AutoSize() As Boolean
            Get
                Return m_AutoSize
            End Get
            Set(ByVal Value As Boolean)
                If Value <> m_AutoSize Then
                    m_AutoSize = Value
                End If
            End Set
        End Property

        Public Overridable Property MinimumSize() As Integer
            Get
                Return m_MinimumSize
            End Get
            Set(ByVal Value As Integer)
                m_MinimumSize = Value
            End Set
        End Property

        Public Overridable Property MaximumSize() As Integer
            Get
                Return m_MaximumSize
            End Get
            Set(ByVal Value As Integer)
                m_MaximumSize = Value
            End Set
        End Property

        Public Overridable Property X() As Integer
            Get
                Return m_Position.X
            End Get
            Set(ByVal Value As Integer)
                m_Position.X = Value
            End Set
        End Property

        Public Overridable Property Y() As Integer
            Get
                Return m_Position.Y
            End Get
            Set(ByVal Value As Integer)
                m_Position.Y = Value
            End Set
        End Property

        Public Overridable Function GetPosition() As Point
            Dim myPosition As New Point(m_Position.X + 10, m_Position.Y + 10)
            Return myPosition
        End Function

        Public Overridable Sub SetPosition(ByVal Value As Point)
            'any value is currently ok,
            'but I might want to add validation later.
            m_Position = Value
        End Sub

        Public Overridable Property Height() As Integer
            Get
                Return m_Size.Height
            End Get
            Set(ByVal Value As Integer)
                m_Size.Height = Value
            End Set
        End Property

        Public Overridable Property Width() As Integer
            Get
                Return m_Size.Width
            End Get
            Set(ByVal Value As Integer)
                m_Size.Width = Value
            End Set
        End Property

        Public Overridable Sub SetSize(ByVal Value As Size)
            Me.Width = Value.Width
            Me.Height = Value.Height
            m_Size = Value
        End Sub

        Public Overridable Sub ResizeObject(ByVal Value As Rectangle)
            'Me.Height = 
            Me.SetSize(New Point(Value.Width, Value.Height))

        End Sub

        Public Overridable Function GetSize() As Size
            Dim mySize As New Size(m_Size.Width + 5, m_Size.Height + 15)
            Return mySize
        End Function

        Public Overridable Property Rotation() As Single
            Get
                Return m_Rotation
            End Get

            Set(ByVal Value As Single)
                m_Rotation = Value
            End Set
        End Property


        Public MustOverride Sub Draw(ByVal g As Graphics)

        Public Sub ObjChanged()
            MsgBox("Object Has Changed")
        End Sub

        Public Function Left() As Object
            Return Me.X
        End Function

        Public Function Top() As Object
            Return Me.Y
        End Function

        Public Function Location() As Rectangle
            Dim r As New Rectangle(Me.X, Me.Y, Me.Width + Me.X, Me.Height + Me.Y)
            Return (r)
        End Function

        Public Shared Narrowing Operator CType(v As GraphicObjects) As Control
            Throw New NotImplementedException()
        End Operator
    End Class

End Namespace

