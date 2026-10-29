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
using Microsoft.VisualBasic;

namespace Microsoft.MSDN.Samples.GraphicObjects
{

    [Serializable()]
    public abstract class GraphicObjects
    {
        protected Point m_Position = new Point(0, 0);
        protected Size m_Size = new Size(0, 0);
        protected Size m_Location = new Size(0, 0);
        protected float m_Rotation = 0f;
        protected bool m_AutoSize = true;
        protected GraphicObjectCollection m_Container;
        protected ObjectTypes m_Type;
        protected int m_ImageNumber = 0;
        protected string m_ImageTag = null;
        protected string m_ImagePath = null;
        protected string m_ImageName = null;
        protected string m_ImageText = null;
        protected Color m_BackGroundColor = default;
        protected Image m_BackGroundImage = null;
        protected Font m_Font = null;
        protected Color m_FontColor = default;
        protected int m_FontSize = default;
        protected FontStyle m_FontStyle = default;
        protected bool m_FontUnderlined = default;
        protected bool m_FontStrikeOut = default;
        protected int M_FontScript = default;
        protected int m_LineWidth = default;
        protected Color m_FillColor = default;
        protected bool m_Fill = false;
        protected Color m_LineColor = default;
        protected Graphics m_Graphics = null;
        // Protected m_RectangleF As RectangleF
        private RectangleF[] Drag_Handles = new RectangleF[8];
        protected int m_MinimumSize;
        protected int m_MaximumSize;
        protected Point m_MousePosition = new Point();

        // Holds the font script selected value using the index Number
        private enum FontScripts
        {
            Western = 1,
            Hebrew = 2,
            Arabic = 3,
            Greek = 4,
            Turkish = 5,
            Baltic = 6,
            CentralEuropean = 7,
            Cryllic = 8,
            Thai = 9,
            Vietnamese = 10
        }

        public enum ObjectTypes
        {
            Linked = 1, // Handles image with known paths
            Background = 2,
            Text = 3,
            Image = 7,
            CircularTextImage = 8,
            EmbeddedImageGraphic = 9, // Handles images without paths
            Fill = 4,
            ClonedObject = 5,
            DeletedObject = 6
        }

        // Send the object to the back of the stack.
        public void SendToBack(GraphicObjects dr)
        {
            if (dr is not null)
            {
                m_Container.Remove(dr);
                m_Container.Insert(0, dr);
            }
        }

        // Bring the object to the front of the stack.
        public void BringToFront(GraphicObjects dr)
        {
            if (dr is not null)
            {
                m_Container.Remove(dr);
                m_Container.Insert(m_Container.Count, dr);
            }
        }

        // Save Images Background color property
        public virtual Color BackGroundColor
        {
            get
            {
                return m_BackGroundColor;
            }
            set
            {
                m_BackGroundColor = value;
            }
        }

        // Save Images Text property
        public virtual string Text
        {
            get
            {
                return m_ImageText;
            }
            set
            {
                m_ImageText = value;
            }
        }

        // Save Images Font property
        public virtual bool FontUnderlined
        {
            get
            {
                return m_FontUnderlined;
            }
            set
            {
                m_FontUnderlined = value;
            }
        }

        // Save Images Font property
        public virtual bool FontStrikeOut
        {
            get
            {
                return m_FontStrikeOut;
            }
            set
            {
                m_FontStrikeOut = value;
            }
        }

        // Save Images FontStyle property
        public virtual FontStyle FontStyle
        {
            get
            {
                return m_FontStyle;
            }
            set
            {
                m_FontStyle = value;
            }
        }

        // Save Images Font property
        public virtual Color FontColor
        {
            get
            {
                return m_FontColor;
            }
            set
            {
                m_FontColor = value;
            }
        }

        // Save Images Font property
        public virtual int FontSize
        {
            get
            {
                return m_FontSize;
            }
            set
            {
                m_FontSize = value;
            }
        }

        // Save Images Font property
        public virtual Font Font
        {
            get
            {
                return m_Font;
            }
            set
            {
                m_Font = value;
            }
        }

        // Save Images FontScript property by passing the indexNumber
        public virtual int FontScript
        {
            get
            {
                return M_FontScript;
            }
            set
            {
                switch (FontScript)
                {
                    case object _ when value == 1:
                        {
                            M_FontScript = (int)FontScripts.Western; // Value
                            break;
                        }
                    case object _ when value == 2:
                        {
                            M_FontScript = (int)FontScripts.Hebrew;
                            break;
                        }
                    case object _ when value == 3:
                        {
                            M_FontScript = (int)FontScripts.Arabic;
                            break;
                        }
                    case object _ when value == 4:
                        {
                            M_FontScript = (int)FontScripts.Greek;
                            break;
                        }
                    case object _ when value == 5:
                        {
                            M_FontScript = (int)FontScripts.Turkish;
                            break;
                        }
                    case object _ when value == 6:
                        {
                            M_FontScript = (int)FontScripts.Baltic;
                            break;
                        }
                    case object _ when value == 7:
                        {
                            M_FontScript = (int)FontScripts.CentralEuropean;
                            break;
                        }
                    case object _ when value == 8:
                        {
                            M_FontScript = (int)FontScripts.Cryllic;
                            break;
                        }
                    case object _ when value == 9:
                        {
                            M_FontScript = (int)FontScripts.Thai;
                            break;
                        }
                    case object _ when value == 1:
                        {
                            M_FontScript = (int)FontScripts.Vietnamese;
                            break;
                        }
                }

            }
        }

        // Save Images FillColor property
        public virtual Color LineColor
        {
            get
            {
                return m_LineColor;
            }
            set
            {
                m_LineColor = value;
            }
        }

        // Save Images FillColor property
        public virtual Color FillColor
        {
            get
            {
                return m_FillColor;
            }
            set
            {
                m_FillColor = value;
            }
        }

        // Save Images Fill property
        public virtual bool Fill
        {
            get
            {
                return m_Fill;
            }
            set
            {
                m_Fill = value;
            }
        }

        // Save Images Linewidth property
        public virtual int LineWidth
        {
            get
            {
                return m_LineWidth;
            }
            set
            {
                m_LineWidth = value;
            }
        }

        public virtual string ImageName
        {
            get
            {
                return m_ImageName;
            }
            set
            {
                m_ImageName = value;
            }
        }

        public virtual string ImagePath
        {
            get
            {
                return m_ImagePath;
            }
            set
            {
                m_ImagePath = value;
            }
        }

        public virtual int ImageNumber
        {
            get
            {
                return m_ImageNumber;
            }
            set
            {
                m_ImageNumber = value;
            }
        }

        public virtual ObjectTypes objType
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        public virtual GraphicObjectCollection Container
        {
            get
            {
                return m_Container;
            }
            set
            {
                m_Container = value;
            }
        }

        public Point MousePosition
        {
            get
            {
                return m_MousePosition;
            }
            set
            {
                m_MousePosition = value;
            }
        }

        public virtual bool HitTest(Point pt)
        {
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            var myMatrix = new System.Drawing.Drawing2D.Matrix();

            gp.AddRectangle(new Rectangle(X, Y, Width, Height));
            if (Rotation != 0f)
            {
                myMatrix.RotateAt(Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
            }

            gp.Transform(myMatrix);
            return gp.IsVisible(pt);
        }

        public virtual bool HitTest(RectangleF rect)
        {
            // is this object contained within the supplied rectangle
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            var myMatrix = new System.Drawing.Drawing2D.Matrix();

            gp.AddRectangle(new Rectangle(X, Y, Width, Height));
            if (Rotation != 0f)
            {
                myMatrix.RotateAt(Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
            }
            gp.Transform(myMatrix);
            var gpRect = Rectangle.Round(gp.GetBounds());
            return rect.Contains(gpRect);
        }

        #region Constructors
        protected GraphicObjects()
        {
        }

        protected GraphicObjects(Point graphicPosition) : this()
        {
            SetPosition(graphicPosition);
        }

        protected GraphicObjects(int posX, int posY) : this(new Point(posX, posY))
        {
        }


        protected GraphicObjects(Point graphicPosition, Size graphicSize) : this(graphicPosition)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        protected GraphicObjects(int posX, int posY, Size graphicSize) : this(new Point(posX, posY), graphicSize)
        {
        }

        protected GraphicObjects(int posX, int posY, int width, int height) : this(new Point(posX, posY), new Size(width, height))
        {
        }

        protected GraphicObjects(Point graphicPosition, float Rotation) : this()
        {
            SetPosition(graphicPosition);
            this.Rotation = Rotation;
        }

        protected GraphicObjects(int posX, int posY, float Rotation) : this(new Point(posX, posY), Rotation)
        {
        }

        protected GraphicObjects(Point graphicPosition, Size graphicSize, float Rotation) : this(graphicPosition, Rotation)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        protected GraphicObjects(int posX, int posY, Size graphicSize, float Rotation) : this(new Point(posX, posY), graphicSize, Rotation)
        {
        }

        protected GraphicObjects(int posX, int posY, int width, int height, float Rotation) : this(new Point(posX, posY), new Size(width, height), Rotation)
        {
        }

        #endregion

        public Image Image { get; set; }

        public virtual bool AutoSize
        {
            get
            {
                return m_AutoSize;
            }
            set
            {
                if (value != m_AutoSize)
                {
                    m_AutoSize = value;
                }
            }
        }

        public virtual int MinimumSize
        {
            get
            {
                return m_MinimumSize;
            }
            set
            {
                m_MinimumSize = value;
            }
        }

        public virtual int MaximumSize
        {
            get
            {
                return m_MaximumSize;
            }
            set
            {
                m_MaximumSize = value;
            }
        }

        public virtual int X
        {
            get
            {
                return m_Position.X;
            }
            set
            {
                m_Position.X = value;
            }
        }

        public virtual int Y
        {
            get
            {
                return m_Position.Y;
            }
            set
            {
                m_Position.Y = value;
            }
        }

        public virtual Point GetPosition()
        {
            var myPosition = new Point(m_Position.X + 10, m_Position.Y + 10);
            return myPosition;
        }

        public virtual void SetPosition(Point Value)
        {
            // any value is currently ok,
            // but I might want to add validation later.
            m_Position = Value;
        }

        public virtual int Height
        {
            get
            {
                return m_Size.Height;
            }
            set
            {
                m_Size.Height = value;
            }
        }

        public virtual int Width
        {
            get
            {
                return m_Size.Width;
            }
            set
            {
                m_Size.Width = value;
            }
        }

        public virtual void SetSize(Size Value)
        {
            Width = Value.Width;
            Height = Value.Height;
            m_Size = Value;
        }

        public virtual void ResizeObject(Rectangle Value)
        {
            // Me.Height = 
            SetSize((Size)new Point(Value.Width, Value.Height));

        }

        public virtual Size GetSize()
        {
            var mySize = new Size(m_Size.Width + 5, m_Size.Height + 15);
            return mySize;
        }

        public virtual float Rotation
        {
            get
            {
                return m_Rotation;
            }

            set
            {
                m_Rotation = value;
            }
        }


        public abstract void Draw(Graphics g);

        public void ObjChanged()
        {
            Interaction.MsgBox("Object Has Changed");
        }

        public object Left()
        {
            return X;
        }

        public object Top()
        {
            return Y;
        }

        public Rectangle Location()
        {
            var r = new Rectangle(X, Y, Width + X, Height + Y);
            return r;
        }

        public static explicit operator Control(GraphicObjects v)
        {
            throw new NotImplementedException();
        }
    }

}