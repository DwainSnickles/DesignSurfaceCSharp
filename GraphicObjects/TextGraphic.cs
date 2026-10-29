using System;
// Copyright (C) 2002 Microsoft Corporation
// All rights reserved.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER
// EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
// MERCHANTIBILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// 
// Date: June 2002
// Author: Duncan Mackenzie
// 
// Requires the release version of .NET Framework

using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.MSDN.Samples.GraphicObjects
{


    [Serializable()]
    public class TextGraphic : GraphicObjects
    {


        protected new Font m_Font;
        protected string m_Text = "";
        protected Color m_Color = Color.Black;
        protected Color m_Color1 = Color.White;
        protected int m_Depth;
        public static Font Fonts;
        public static string strText;
        protected Label m_TextLabel;
        private static Image m_TextImage;
        private Image theImage;

        #region Constructors

        public TextGraphic() : base()
        {
        }

        public TextGraphic(Label TheLabel) : this()
        {

            TextLabel = TheLabel;
            Text = TheLabel.Text;
            Fonts = TheLabel.Font;

            var BM = new Bitmap(TheLabel.Width, TheLabel.Height);
            var brush1 = new SolidBrush(TheLabel.ForeColor);

            using (var gr = Graphics.FromImage(BM))
            {
                gr.DrawImage(BM, 0, 0, new Rectangle(0, 0, TheLabel.Width, TheLabel.Height), GraphicsUnit.Pixel);
                BM.MakeTransparent();
                gr.DrawString(TheLabel.Text, TheLabel.Font, brush1, new Point(0, 0));
            }

            TextImage = BM;
            AutoSize = true;

        }

        public TextGraphic(Image ImageText)
        {
            // Me.New()
            // TextLabel = TheLabel
            // m_TextImage = TextToImage(TheLabel)
        }

        public TextGraphic(Point graphicPosition, string text, Font textFont, Color textColor) : this()
        {
            SetPosition(graphicPosition);
            Font = textFont;
            Text = text;
            Color = textColor;
        }

        // Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal text As String, ByVal textFont As Font, ByVal textColor As Color)

        // Me.New(New Point(posX, posY), text, textFont, textColor)

        // Me.AutoSize = True

        // Dim lbl As New Label
        // lbl.AutoSize = True
        // lbl.Font = textFont
        // lbl.Text = text
        // lbl.ForeColor = textColor
        // lbl.Invalidate()

        // Dim BM As New Bitmap(lbl.Width, lbl.Height)
        // Dim brush1 As New SolidBrush(lbl.ForeColor)

        // Using gr As Graphics = Graphics.FromImage(BM)
        // gr.DrawImage(BM, 0, 0, New Rectangle(lbl.Location.X, lbl.Location.Y, lbl.Width, lbl.Height), GraphicsUnit.Pixel)
        // BM.MakeTransparent()
        // gr.DrawString(text, textFont, brush1, New Point(posX, posY))
        // End Using
        // TextImage = BM

        // End Sub


        public TextGraphic(int posX, int posY, string text, Font textFont, Color textColor, Color EmbossColor, int Depth) : this(new Point(posX, posY), text, textFont, textColor)
        {
            m_Color1 = EmbossColor;
        }

        public TextGraphic(Point graphicPosition, string text, Font textFont, Color textColor, float Rotation) : this(graphicPosition, text, textFont, textColor)
        {
            this.Rotation = Rotation;
        }


        public TextGraphic(int posX, int posY, string text, Font textFont, Color textColor, float Rotation) : this(new Point(posX, posY), text, textFont, textColor, Rotation)
        {
        }

        public TextGraphic(Point graphicPosition, Image theImage) : base()
        {
            this.theImage = theImage;
        }

        #endregion

        public Label TextLabel
        {
            get
            {
                return m_TextLabel;
            }
            set
            {
                m_TextLabel = value;
                // If m_TextLabel IsNot Nothing Then
                // TextToImage(m_TextLabel)
                // End If
            }
        }

        public override Font Font
        {
            get
            {
                return m_Font;
            }
            set
            {
                m_Font = value;
                Fonts = value;
            }
        }

        public string LabelText
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }

        public int Depth
        {
            get
            {
                return m_Depth;
            }
            set
            {
                m_Depth = value;
            }
        }

        public Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
            }
        }

        public Color EmbossColor
        {
            get
            {
                return m_Color1;
            }
            set
            {
                m_Color1 = value;
            }
        }

        public Image TextImage
        {
            get
            {
                return m_TextImage;
            }
            set
            {
                m_TextImage = value;
            }
        }

        public Image TextToImage(Label label)
        {

            var BM = new Bitmap(label.Width, label.Height);
            var brush1 = new SolidBrush(label.ForeColor);

            using (var gr = Graphics.FromImage(BM))
            {
                gr.DrawImage(BM, 0, 0, new Rectangle(label.Location.X, label.Location.Y, label.Width, label.Height), GraphicsUnit.Pixel);
                BM.MakeTransparent();
                gr.DrawString(label.Text, label.Font, brush1, new Point(0, 0));
            }

            return BM;
        }

        public override void Draw(Graphics g)
        {
            System.Drawing.Drawing2D.GraphicsContainer gContainer;
            System.Drawing.Drawing2D.Matrix myMatrix;
            gContainer = g.BeginContainer();
            myMatrix = g.Transform;
            if (m_Rotation != 0f)
            {
                myMatrix.RotateAt(m_Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.Transform = myMatrix;
            }
            if (AutoSize)
            {
                var mySize = g.MeasureString(m_Text, m_Font);
                Width = (int)Math.Round(mySize.Width);
                Height = (int)Math.Round(mySize.Height);
                g.DrawString(m_Text, m_Font, new SolidBrush(m_Color), X, Y);
            }
            else
            {
                var rect = new RectangleF(X, Y, Width, Height);
                g.DrawString(m_Text, m_Font, new SolidBrush(m_Color), rect);
            }
            g.EndContainer(gContainer);
        }

    }

}