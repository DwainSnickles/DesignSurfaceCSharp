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

namespace Microsoft.MSDN.Samples.GraphicObjects
{

    [Serializable()]
    public abstract class ImageGraphic : GraphicObjects
    {
        protected ImageGraphic() : base()
        {
        }
        public abstract Image GetImage();

    }

    [Serializable()]
    public class LinkedImageGraphic : ImageGraphic
    {
        // Protected m_ImagePath As String
        [NonSerialized()]
        protected new Image m_Image;

        #region Constructors
        public LinkedImageGraphic() : base()
        {
        }

        public LinkedImageGraphic(Point graphicPosition, string ImagePath) : this()
        {
            SetPosition(graphicPosition);
            this.ImagePath = ImagePath;
        }

        public LinkedImageGraphic(int posX, int posY, string ImagePath) : this(new Point(posX, posY), ImagePath)
        {
        }

        public LinkedImageGraphic(Point graphicPosition, Size graphicSize, string ImagePath) : this(graphicPosition, ImagePath)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public LinkedImageGraphic(int posX, int posY, Size graphicSize, string ImagePath) : this(new Point(posX, posY), graphicSize, ImagePath)
        {
        }

        public LinkedImageGraphic(int posX, int posY, int width, int height, string ImagePath) : this(new Point(posX, posY), new Size(width, height), ImagePath)
        {
        }

        public LinkedImageGraphic(Point graphicPosition, float Rotation, string ImagePath) : this(graphicPosition, ImagePath)
        {
            this.Rotation = Rotation;
        }

        public LinkedImageGraphic(int posX, int posY, float Rotation, string ImagePath) : this(new Point(posX, posY), Rotation, ImagePath)
        {
        }

        public LinkedImageGraphic(Point graphicPosition, Size graphicSize, float Rotation, string ImagePath) : this(graphicPosition, Rotation, ImagePath)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public LinkedImageGraphic(int posX, int posY, Size graphicSize, float Rotation, string ImagePath) : this(new Point(posX, posY), graphicSize, Rotation, ImagePath)
        {
        }

        public LinkedImageGraphic(int posX, int posY, int width, int height, float Rotation, string ImagePath) : this(new Point(posX, posY), new Size(width, height), Rotation, ImagePath)
        {
        }

        #endregion

        private GraphicObjects m_AddImageGraphic;

        public void AddImageGraphic(GraphicObjects gobj)
        {

        }

        public override Image GetImage()
        {
            try
            {
                if (m_Image is null)
                {
                    m_Image = new Bitmap(m_ImagePath);
                }
                return m_Image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override void Draw(Graphics g)
        {

            System.Drawing.Drawing2D.GraphicsContainer container;
            System.Drawing.Drawing2D.Matrix myMatrix;
            container = g.BeginContainer();
            myMatrix = g.Transform;
            if (m_Rotation != 0f)
            {
                myMatrix.RotateAt(m_Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.Transform = myMatrix;
            }

            var myImage = GetImage();

            if (myImage is not null)
            {
                if (AutoSize)
                {
                    int myNewWidth = (int)Math.Round(myImage.Width / myImage.HorizontalResolution * Container.HorizontalResolution);
                    int myNewHeight = (int)Math.Round(myImage.Height / myImage.VerticalResolution * Container.VerticalResolution);
                    Height = myNewHeight;
                    Width = myNewWidth;
                }
                g.DrawImage(myImage, X, Y, Width, Height);
                // modStart.CurrentImageGraphic = g
            }
            g.EndContainer(container);
        }

        public void ReDrawLinkedImageGraphic(GraphicObjects gobj)
        {

            Graphics g = null;
            System.Drawing.Drawing2D.GraphicsContainer container;
            System.Drawing.Drawing2D.Matrix myMatrix;
            container = g.BeginContainer();
            myMatrix = g.Transform;
            m_Rotation = gobj.Rotation;

            if (m_Rotation != 0f)
            {
                myMatrix.RotateAt(m_Rotation, new PointF(gobj.X, gobj.Y), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.Transform = myMatrix;
            }

            var myImage = Image.FromFile(gobj.ImagePath);

            if (myImage is not null)
            {

                if (AutoSize)
                {
                    int myNewWidth = (int)Math.Round(gobj.Width / myImage.HorizontalResolution * Container.HorizontalResolution);
                    int myNewHeight = (int)Math.Round(gobj.Height / myImage.VerticalResolution * Container.VerticalResolution);
                    Height = myNewHeight;
                    Width = myNewWidth;
                }

                g.DrawImage(myImage, gobj.X, gobj.Y, gobj.Width, gobj.Height);

            }
            g.EndContainer(container);
        }

        // Public Overloads Sub ReDrawImage(ByVal W As Integer, ByVal H As Integer)
        // If ImageGraphic Is Nothing Then Exit Sub
        // 'Dim g As Graphics
        // Dim container As System.Drawing.Drawing2D.GraphicsContainer
        // Dim myMatrix As Drawing2D.Matrix
        // container = ImageGraphic.BeginContainer()
        // myMatrix = ImageGraphic.Transform()

        // If m_Rotation <> 0 Then
        // myMatrix.RotateAt(m_Rotation, New PointF(X, Y), Drawing.Drawing2D.MatrixOrder.Append)
        // ImageGraphic.Transform = myMatrix
        // End If

        // Dim myImage As Image
        // myImage = Me.GetImage

        // If Not myImage Is Nothing Then
        // If Me.AutoSize Then
        // Dim myNewWidth As Integer = (myImage.Width / myImage.HorizontalResolution) * Me.Container.HorizontalResolution
        // Dim myNewHeight As Integer = (myImage.Height / myImage.VerticalResolution) * Me.Container.VerticalResolution
        // Me.Height = myNewHeight
        // Me.Width = myNewWidth
        // End If

        // ImageGraphic.DrawImage(myImage, X, Y, W, H)
        // End If
        // ImageGraphic.EndContainer(container)
        // End Sub

    }


    [Serializable()]
    public class EmbeddedImageGraphic : ImageGraphic
    {
        protected new Image m_Image;

        #region Constructors
        public EmbeddedImageGraphic() : base()
        {
        }

        public EmbeddedImageGraphic(Point graphicPosition, Image startingImage) : this()
        {
            SetPosition(graphicPosition);
            Image = startingImage;
        }

        public EmbeddedImageGraphic(int posX, int posY, Image startingImage) : this(new Point(posX, posY), startingImage)
        {
        }

        public EmbeddedImageGraphic(Point graphicPosition, Size graphicSize, Image startingImage) : this(graphicPosition, startingImage)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public EmbeddedImageGraphic(int posX, int posY, Size graphicSize, Image startingImage) : this(new Point(posX, posY), graphicSize, startingImage)
        {
        }

        public EmbeddedImageGraphic(int posX, int posY, int width, int height, Image startingImage) : this(new Point(posX, posY), new Size(width, height), startingImage)
        {
        }

        public EmbeddedImageGraphic(Point graphicPosition, float Rotation, Image startingImage) : this(graphicPosition, startingImage)
        {
            this.Rotation = Rotation;
        }

        public EmbeddedImageGraphic(int posX, int posY, float Rotation, Image startingImage) : this(new Point(posX, posY), Rotation, startingImage)
        {
        }

        public EmbeddedImageGraphic(Point graphicPosition, Size graphicSize, float Rotation, Image startingImage) : this(graphicPosition, Rotation, startingImage)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public EmbeddedImageGraphic(int posX, int posY, Size graphicSize, float Rotation, Image startingImage) : this(new Point(posX, posY), graphicSize, Rotation, startingImage)
        {
        }

        public EmbeddedImageGraphic(int posX, int posY, int width, int height, float Rotation, Image startingImage) : this(new Point(posX, posY), new Size(width, height), Rotation, startingImage)
        {
        }

        #endregion

        public new Image Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                m_Image = value;
            }
        }

        public override Image GetImage()
        {
            return Image;
        }

        public override void Draw(Graphics g)
        {
            System.Drawing.Drawing2D.GraphicsContainer container;
            System.Drawing.Drawing2D.Matrix myMatrix;
            container = g.BeginContainer();
            myMatrix = g.Transform;
            if (m_Rotation != 0f)
            {
                myMatrix.RotateAt(m_Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.Transform = myMatrix;
            }
            if (m_Image is not null)
            {
                if (AutoSize)
                {
                    int myNewWidth = (int)Math.Round(m_Image.Width / m_Image.HorizontalResolution * Container.HorizontalResolution);
                    int myNewHeight = (int)Math.Round(m_Image.Height / m_Image.VerticalResolution * Container.VerticalResolution);
                    Height = myNewHeight;
                    Width = myNewWidth;
                }
                g.DrawImage(m_Image, X, Y, Width, Height);
            }
            g.EndContainer(container);
        }
    }

    [Serializable()]
    public class BackGroundImageGraphic : ImageGraphic
    {
        // Protected m_ImagePath As String
        [NonSerialized()]
        protected new Image m_Image;

        #region Constructors
        public BackGroundImageGraphic() : base()
        {
        }

        public BackGroundImageGraphic(Point graphicPosition, string ImagePath) : this()
        {
            SetPosition(graphicPosition);
            this.ImagePath = ImagePath;
        }

        public BackGroundImageGraphic(int posX, int posY, string ImagePath) : this(new Point(posX, posY), ImagePath)
        {
        }

        // Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal Image As Image)
        // m_Image = Image
        // Me.New(New Point(posX, posY),)
        // End Sub

        public BackGroundImageGraphic(Point graphicPosition, Size graphicSize, string ImagePath) : this(graphicPosition, ImagePath)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public BackGroundImageGraphic(int posX, int posY, Size graphicSize, string ImagePath) : this(new Point(posX, posY), graphicSize, ImagePath)
        {
        }

        public BackGroundImageGraphic(int posX, int posY, int width, int height, string ImagePath) : this(new Point(posX, posY), new Size(width, height), ImagePath)
        {
        }

        public BackGroundImageGraphic(Point graphicPosition, float Rotation, string ImagePath) : this(graphicPosition, ImagePath)
        {
            this.Rotation = Rotation;
        }

        public BackGroundImageGraphic(int posX, int posY, float Rotation, string ImagePath) : this(new Point(posX, posY), Rotation, ImagePath)
        {
        }

        public BackGroundImageGraphic(Point graphicPosition, Size graphicSize, float Rotation, string ImagePath) : this(graphicPosition, Rotation, ImagePath)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public BackGroundImageGraphic(int posX, int posY, Size graphicSize, float Rotation, string ImagePath) : this(new Point(posX, posY), graphicSize, Rotation, ImagePath)
        {
        }

        public BackGroundImageGraphic(int posX, int posY, int width, int height, float Rotation, string ImagePath) : this(new Point(posX, posY), new Size(width, height), Rotation, ImagePath)
        {
        }

        #endregion

        public override Image GetImage()
        {
            try
            {
                if (m_Image is null)
                {
                    m_Image = new Bitmap(m_ImagePath);
                }
                return m_Image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override void Draw(Graphics g)
        {
            System.Drawing.Drawing2D.GraphicsContainer container;
            System.Drawing.Drawing2D.Matrix myMatrix;
            container = g.BeginContainer();
            myMatrix = g.Transform;
            if (m_Rotation != 0f)
            {
                myMatrix.RotateAt(m_Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.Transform = myMatrix;
            }
            Image myImage;
            myImage = GetImage();
            if (myImage is not null)
            {
                if (AutoSize)
                {
                    int myNewWidth = (int)Math.Round(myImage.Width / myImage.HorizontalResolution * Container.HorizontalResolution);
                    int myNewHeight = (int)Math.Round(myImage.Height / myImage.VerticalResolution * Container.VerticalResolution);
                    Height = myNewHeight;
                    Width = myNewWidth;
                }
                g.DrawImage(myImage, X, Y, Width, Height);
            }
            g.EndContainer(container);
        }
    }

}