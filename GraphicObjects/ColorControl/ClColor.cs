using System;
// ******************************************************************
// ***************Copyright Bobo Enterprises 2005********************
// ***************Written by MrBobo**********************************
// This code was submitted to Planet Source Code (www.planetsourcecode.com)
// If you downloaded it elsewhere, they stole it and I'll eat them alive


// Class for handling colors - much neater than including
// the code on the form containing controls manipulating colors

// This structure is used to carry both RGB and HSB values to routines
using System.Drawing;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.MSDN.Samples
{

    public struct HSBorRGB
    {
        public decimal HR;
        public decimal SG;
        public decimal BB;
    }

    public class ClColor
    {
        private Color m_Color;
        private int m_Red;
        private int m_Green;
        private int m_Blue;
        private int m_Hue;
        private int m_Saturation;
        private int m_Brightness;
        private decimal m_NETHue;
        private decimal m_NETSaturation;
        private decimal m_NETBrightness;
        public event ColorChangeEventHandler ColorChange;

        public delegate void ColorChangeEventHandler();
        public Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
                // Update all variables
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                m_Hue = (int)Math.Round((decimal)m_Color.GetHue() / 360m * 240m);
                m_Saturation = (int)Math.Round((decimal)m_Color.GetSaturation() * 240m);
                m_Brightness = (int)Math.Round((decimal)m_Color.GetBrightness() * 240m);
                m_NETHue = (decimal)m_Color.GetHue();
                m_NETSaturation = (decimal)m_Color.GetSaturation();
                m_NETBrightness = (decimal)m_Color.GetBrightness();
                ColorChange?.Invoke();
            }
        }
        public int Red
        {
            get
            {
                return m_Red;
            }
            set
            {
                m_Red = value;
                // Update all variables
                Color = Color.FromArgb(m_Red, m_Green, m_Blue);
                ColorChange?.Invoke();
            }
        }
        public int Green
        {
            get
            {
                return m_Green;
            }
            set
            {
                m_Green = value;
                // Update all variables
                Color = Color.FromArgb(m_Red, m_Green, m_Blue);
                ColorChange?.Invoke();
            }
        }
        public int Blue
        {
            get
            {
                return m_Blue;
            }
            set
            {
                m_Blue = value;
                // Update all variables
                Color = Color.FromArgb(m_Red, m_Green, m_Blue);
                ColorChange?.Invoke();
            }
        }
        public int Hue
        {
            get
            {
                return m_Hue;
            }
            set
            {
                m_Hue = value;
                // Update all variables except Saturation and Brightness
                m_Color = ModGen.ColorFromHSB(m_Hue, m_Saturation, m_Brightness);
                m_NETHue = (decimal)m_Color.GetHue();
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                ColorChange?.Invoke();
            }
        }
        public int Saturation
        {
            get
            {
                return m_Saturation;
            }
            set
            {
                m_Saturation = value;
                // Update all variables except Hue and Brightness
                m_Color = ModGen.ColorFromHSB(m_Hue, m_Saturation, m_Brightness);
                m_NETSaturation = (decimal)m_Color.GetSaturation();
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                ColorChange?.Invoke();
            }
        }
        public int Brightness
        {
            get
            {
                return m_Brightness;
            }
            set
            {
                m_Brightness = value;
                // Update all variables except Hue and Saturation
                m_Color = ModGen.ColorFromHSB(m_Hue, m_Saturation, m_Brightness);
                m_NETBrightness = (decimal)m_Color.GetBrightness();
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                ColorChange?.Invoke();
            }
        }
        public decimal NETSaturation
        {
            get
            {
                return m_NETSaturation;
            }
            set
            {
                m_NETSaturation = value;
                // Update all variables except Hue and Brightness
                m_Color = ModGen.ColorFromHSB((float)m_NETHue, (float)m_NETSaturation, (float)m_NETBrightness, true);
                m_Saturation = (int)Math.Round((decimal)m_Color.GetSaturation() * 240m);
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                ColorChange?.Invoke();
            }
        }
        public decimal NETHue
        {
            get
            {
                return m_NETHue;
            }
            set
            {
                m_NETHue = value;
                // Update all variables except Saturation and Brightness
                m_Color = ModGen.ColorFromHSB((float)m_NETHue, (float)m_NETSaturation, (float)m_NETBrightness, true);
                m_Hue = (int)Math.Round((decimal)m_Color.GetHue() / 360m * 240m);
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                ColorChange?.Invoke();
            }
        }
        public decimal NETBrightness
        {
            get
            {
                return m_NETBrightness;
            }
            set
            {
                m_NETBrightness = value;
                // Update all variables except Hue and Saturation
                m_Color = ModGen.ColorFromHSB((float)m_NETHue, (float)m_NETSaturation, (float)m_NETBrightness, true);
                m_Brightness = (int)Math.Round((decimal)m_Color.GetBrightness() * 240m);
                m_Red = m_Color.R;
                m_Green = m_Color.G;
                m_Blue = m_Color.B;
                ColorChange?.Invoke();
            }
        }
        // Used to update HSB all at once instead of raising individual events for
        // Hue, Saturation and Brightness
        public void set_SetHSB(bool UseNETHSB = false, HSBorRGB value = default)
        {
            // Update all variables
            m_Hue = Conversions.ToInteger(Interaction.IIf(UseNETHSB, value.HR * 360m / 240m, value.HR));
            m_Saturation = Conversions.ToInteger(Interaction.IIf(UseNETHSB, value.SG / 240m, value.SG));
            m_Brightness = Conversions.ToInteger(Interaction.IIf(UseNETHSB, value.BB / 240m, value.BB));
            m_NETHue = Conversions.ToDecimal(Interaction.IIf(UseNETHSB, value.HR, value.HR / 360m * 240m));
            m_NETSaturation = Conversions.ToDecimal(Interaction.IIf(UseNETHSB, value.SG, value.SG * 240m));
            m_NETBrightness = Conversions.ToDecimal(Interaction.IIf(UseNETHSB, value.BB, value.BB * 240m));
            m_Color = ModGen.ColorFromHSB((float)value.HR, (float)value.SG, (float)value.BB, UseNETHSB);
            m_Red = m_Color.R;
            m_Green = m_Color.G;
            m_Blue = m_Color.B;
            ColorChange?.Invoke();
        }




    }
}