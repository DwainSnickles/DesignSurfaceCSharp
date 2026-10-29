using System;
using System.Drawing;

namespace Microsoft.MSDN.Samples.GraphicObjects
{

    [Serializable()]
    public abstract class ShapeGraphic : GraphicObjects
    {

        protected new float m_lineWidth = 1f;
        protected new Color m_lineColor = Color.Black;
        protected new Color m_fillColor = Color.White;
        protected new bool m_fill = false;
        protected int X1 = 0;
        protected int Y1 = 0;
        protected int Y2 = 0;
        protected int X2 = 0;

        public ShapeGraphic() : base()
        {
        }

        public virtual new float LineWidth
        {
            get
            {
                return m_lineWidth;
            }
            set
            {
                if (value > 0f)
                {
                    m_lineWidth = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("LineWidth", "Line Width must be > 0");
                }
            }
        }

        public override Color LineColor
        {
            get
            {
                return m_lineColor;
            }
            set
            {
                m_lineColor = value;
            }
        }

        public override bool Fill
        {
            get
            {
                return m_fill;
            }
            set
            {
                m_fill = value;
            }
        }

        public override Color FillColor
        {
            get
            {
                return m_fillColor;
            }
            set
            {
                m_fillColor = value;
            }
        }

    }

    [Serializable()]
    public class LineGraphic : ShapeGraphic
    {

        #region Constructors
        public LineGraphic()
        {
        }

        public LineGraphic(Point startPosition) : this()
        {
            SetStartPosition(startPosition);
        }

        public LineGraphic(int posX, int posY) : this(new Point(posX, posY))
        {
        }

        public LineGraphic(Point startPosition, Point endPosition) : this(startPosition)
        {
            SetEndPosition(endPosition);
            AutoSize = false;
        }

        public LineGraphic(int startX, int startY, Point endPosition) : this(new Point(startX, startY), endPosition)
        {
        }

        public LineGraphic(int startX, int startY, int endX, int endY) : this(new Point(startX, startY), new Point(endX, endY))
        {
        }

        public LineGraphic(Point startPosition, Point endPosition, float lineWidth, Color lineColor) : this(startPosition)
        {
            SetEndPosition(endPosition);
            LineWidth = lineWidth;
            LineColor = lineColor;
            AutoSize = false;
        }

        public LineGraphic(int startX, int startY, int endX, int endY, float lineWidth, Color lineColor) : this(new Point(startX, startY), new Point(endX, endY))
        {
            LineWidth = lineWidth;
            LineColor = lineColor;
            AutoSize = false;
        }

        #endregion

        public override bool HitTest(Point pt)
        {
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            var myMatrix = new System.Drawing.Drawing2D.Matrix();
            var myPen = new Pen(LineColor, LineWidth);
            gp.AddLine(X, Y, X + Width, Y + Height);
            myMatrix.RotateAt(Rotation, new PointF(X, Y), System.Drawing.Drawing2D.MatrixOrder.Append);
            gp.Transform(myMatrix);
            return gp.IsOutlineVisible(pt, myPen);
        }

        public Point GetStartPosition()
        {
            return GetPosition();
        }

        public void SetStartPosition(Point Value)
        {
            SetPosition(Value);
        }

        public Point GetEndPosition()
        {
            var endPosition = new Point(m_Position.X, m_Position.Y);
            endPosition.X += m_Size.Width;
            endPosition.Y += m_Size.Height;
            return endPosition;
        }

        public void SetEndPosition(Point Value)
        {
            m_Size.Width = Value.X - m_Position.X;
            m_Size.Height = Value.Y - m_Position.Y;
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
            var myPen = new Pen(LineColor, LineWidth);
            g.DrawLine(myPen, X, Y, X + Width, Y + Height);
            g.EndContainer(gContainer);
        }
    }


    [Serializable()]
    public class RectangleGraphic : ShapeGraphic
    {

        #region Constructors
        public RectangleGraphic()
        {
        }

        public RectangleGraphic(Point graphicPosition) : this()
        {
            SetPosition(graphicPosition);
        }

        public RectangleGraphic(int posX, int posY) : this(new Point(posX, posY))
        {
        }

        public RectangleGraphic(Point graphicPosition, Size graphicSize) : this(graphicPosition)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public RectangleGraphic(int posX, int posY, Size graphicSize) : this(new Point(posX, posY), graphicSize)
        {
        }

        public RectangleGraphic(int posX, int posY, int width, int height) : this(new Point(posX, posY), new Size(width, height))
        {
        }

        public RectangleGraphic(Point graphicPosition, float Rotation) : this()
        {
            SetPosition(graphicPosition);
            this.Rotation = Rotation;
        }

        public RectangleGraphic(int posX, int posY, float Rotation) : this(new Point(posX, posY), Rotation)
        {
        }

        public RectangleGraphic(Point graphicPosition, Size graphicSize, float Rotation) : this(graphicPosition, Rotation)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public RectangleGraphic(int posX, int posY, Size graphicSize, float Rotation) : this(new Point(posX, posY), graphicSize, Rotation)
        {
        }

        public RectangleGraphic(int posX, int posY, int width, int height, float Rotation) : this(new Point(posX, posY), new Size(width, height), Rotation)
        {
        }

        #endregion

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
            var rect = new Rectangle(X, Y, Width, Height);

            if (Fill)
            {
                g.FillRectangle(new SolidBrush(FillColor), rect);
            }
            var myPen = new Pen(LineColor, LineWidth);
            g.DrawRectangle(myPen, rect);

        }
    }

    [Serializable()]
    public class EllipseGraphic : ShapeGraphic
    {

        #region Constructors
        public EllipseGraphic()
        {
        }

        public EllipseGraphic(Point graphicPosition) : this()
        {
            SetPosition(graphicPosition);
        }

        public EllipseGraphic(int posX, int posY) : this(new Point(posX, posY))
        {
        }

        public EllipseGraphic(Point graphicPosition, Size graphicSize) : this(graphicPosition)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public EllipseGraphic(int posX, int posY, Size graphicSize) : this(new Point(posX, posY), graphicSize)
        {
        }

        public EllipseGraphic(int posX, int posY, int width, int height) : this(new Point(posX, posY), new Size(width, height))
        {
        }

        public EllipseGraphic(Point graphicPosition, float Rotation) : this()
        {
            SetPosition(graphicPosition);
            this.Rotation = Rotation;
        }

        public EllipseGraphic(int posX, int posY, float Rotation) : this(new Point(posX, posY), Rotation)
        {
        }

        public EllipseGraphic(Point graphicPosition, Size graphicSize, float Rotation) : this(graphicPosition, Rotation)
        {
            SetSize(graphicSize);
            AutoSize = false;
        }

        public EllipseGraphic(int posX, int posY, Size graphicSize, float Rotation) : this(new Point(posX, posY), graphicSize, Rotation)
        {
        }

        public EllipseGraphic(int posX, int posY, int width, int height, float Rotation) : this(new Point(posX, posY), new Size(width, height), Rotation)
        {
        }

        #endregion

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
            var rect = new Rectangle(X, Y, Width, Height);
            if (Fill)
            {
                g.FillEllipse(new SolidBrush(FillColor), rect);
            }
            var myPen = new Pen(LineColor, LineWidth);
            g.DrawEllipse(myPen, rect);

            g.EndContainer(gContainer);
        }

    }

}