'******************************************************************
'***************Copyright Bobo Enterprises 2005********************
'***************Written by MrBobo**********************************
'This code was submitted to Planet Source Code (www.planetsourcecode.com)
'If you downloaded it elsewhere, they stole it and I'll eat them alive


'Class for handling colors - much neater than including
'the code on the form containing controls manipulating colors

'This structure is used to carry both RGB and HSB values to routines
Imports System.Drawing

Public Structure HSBorRGB
    Dim HR As Decimal
    Dim SG As Decimal
    Dim BB As Decimal
End Structure
Public Class ClColor
    Private m_Color As Color
    Private m_Red As Integer
    Private m_Green As Integer
    Private m_Blue As Integer
    Private m_Hue As Integer
    Private m_Saturation As Integer
    Private m_Brightness As Integer
    Private m_NETHue As Decimal
    Private m_NETSaturation As Decimal
    Private m_NETBrightness As Decimal
    Public Event ColorChange()
    Public Property Color() As Color
        Get
            Return m_Color
        End Get
        Set(ByVal Value As Color)
            m_Color = Value
            'Update all variables
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            m_Hue = CDec(m_Color.GetHue) / 360 * 240
            m_Saturation = CDec(m_Color.GetSaturation) * 240
            m_Brightness = CDec(m_Color.GetBrightness) * 240
            m_NETHue = m_Color.GetHue
            m_NETSaturation = m_Color.GetSaturation
            m_NETBrightness = m_Color.GetBrightness
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property Red() As Integer
        Get
            Return m_Red
        End Get
        Set(ByVal Value As Integer)
            m_Red = Value
            'Update all variables
            Color = Color.FromArgb(m_Red, m_Green, m_Blue)
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property Green() As Integer
        Get
            Return m_Green
        End Get
        Set(ByVal Value As Integer)
            m_Green = Value
            'Update all variables
            Color = Color.FromArgb(m_Red, m_Green, m_Blue)
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property Blue() As Integer
        Get
            Return m_Blue
        End Get
        Set(ByVal Value As Integer)
            m_Blue = Value
            'Update all variables
            Color = Color.FromArgb(m_Red, m_Green, m_Blue)
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property Hue() As Integer
        Get
            Return m_Hue
        End Get
        Set(ByVal Value As Integer)
            m_Hue = Value
            'Update all variables except Saturation and Brightness
            m_Color = ColorFromHSB(m_Hue, m_Saturation, m_Brightness)
            m_NETHue = m_Color.GetHue
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property Saturation() As Integer
        Get
            Return m_Saturation
        End Get
        Set(ByVal Value As Integer)
            m_Saturation = Value
            'Update all variables except Hue and Brightness
            m_Color = ColorFromHSB(m_Hue, m_Saturation, m_Brightness)
            m_NETSaturation = m_Color.GetSaturation
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property Brightness() As Integer
        Get
            Return m_Brightness
        End Get
        Set(ByVal Value As Integer)
            m_Brightness = Value
            'Update all variables except Hue and Saturation
            m_Color = ColorFromHSB(m_Hue, m_Saturation, m_Brightness)
            m_NETBrightness = m_Color.GetBrightness
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property NETSaturation() As Decimal
        Get
            Return m_NETSaturation
        End Get
        Set(ByVal Value As Decimal)
            m_NETSaturation = Value
            'Update all variables except Hue and Brightness
            m_Color = ColorFromHSB(m_NETHue, m_NETSaturation, m_NETBrightness, True)
            m_Saturation = CDec(m_Color.GetSaturation) * 240
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property NETHue() As Decimal
        Get
            Return m_NETHue
        End Get
        Set(ByVal Value As Decimal)
            m_NETHue = Value
            'Update all variables except Saturation and Brightness
            m_Color = ColorFromHSB(m_NETHue, m_NETSaturation, m_NETBrightness, True)
            m_Hue = CDec(m_Color.GetHue) / 360 * 240
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property
    Public Property NETBrightness() As Decimal
        Get
            Return m_NETBrightness
        End Get
        Set(ByVal Value As Decimal)
            m_NETBrightness = Value
            'Update all variables except Hue and Saturation
            m_Color = ColorFromHSB(m_NETHue, m_NETSaturation, m_NETBrightness, True)
            m_Brightness = CDec(m_Color.GetBrightness) * 240
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property
    'Used to update HSB all at once instead of raising individual events for
    'Hue, Saturation and Brightness
    Public WriteOnly Property SetHSB(Optional ByVal UseNETHSB As Boolean = False) As HSBorRGB
        Set(ByVal value As HSBorRGB)
            'Update all variables
            m_Hue = IIf(UseNETHSB, value.HR * 360 / 240, value.HR)
            m_Saturation = IIf(UseNETHSB, value.SG / 240, value.SG)
            m_Brightness = IIf(UseNETHSB, value.BB / 240, value.BB)
            m_NETHue = IIf(UseNETHSB, value.HR, value.HR / 360 * 240)
            m_NETSaturation = IIf(UseNETHSB, value.SG, value.SG * 240)
            m_NETBrightness = IIf(UseNETHSB, value.BB, value.BB * 240)
            m_Color = ColorFromHSB(value.HR, value.SG, value.BB, UseNETHSB)
            m_Red = m_Color.R
            m_Green = m_Color.G
            m_Blue = m_Color.B
            RaiseEvent ColorChange()
        End Set
    End Property




End Class
