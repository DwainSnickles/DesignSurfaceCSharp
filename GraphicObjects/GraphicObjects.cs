using System;
using System.Collections;
using System.Drawing;
using static System.Math;

namespace Microsoft.MSDN.Samples.GraphicObjects
{

    [Serializable()]
    public class GraphicObjectCollection : CollectionBase
    {

        protected int m_HorizRes = 72;
        protected int m_VertRes = 72;
        protected int DRAG_HANDLE_SIZE = 8;
        protected Graphics ImageGraphic = null;
        protected GraphicObjects m_Gobj;
        public RectangleF[] Drag_Handles = new RectangleF[10];
        public event ObjectChangedEventHandler ObjectChanged;

        public delegate void ObjectChangedEventHandler(GraphicObjects e);
        public RectangleF DragRectangleF = new RectangleF();
        public static float Zoom;
        private RectangleF DHamdles;
        private Point m_MousePosition;
        private Image m_ImageObj;

        public RectangleF DragHandles
        {
            get
            {
                return Drag_Handles[9];
            }
            set
            {
                DHamdles = value;
            }
        }

        public Image ImageObj
        {
            get
            {
                return m_ImageObj;
            }
            set
            {
                m_ImageObj = value;
            }
        }

        protected Rectangle ZoomRectangle(Rectangle originalRect)
        {
            var myNewRect = new Rectangle((int)Round(originalRect.X * Zoom), (int)Round(originalRect.Y * Zoom), (int)Round(originalRect.Width * Zoom), (int)Round(originalRect.Height * Zoom));
            return myNewRect;
        }

        protected Rectangle DeZoomRectangle(Rectangle originalRect)
        {
            var myNewRect = new Rectangle((int)Round(originalRect.X / Zoom), (int)Round(originalRect.Y / Zoom), (int)Round(originalRect.Width / Zoom), (int)Round(originalRect.Height / Zoom));
            return myNewRect;
        }

        protected RectangleF ZoomRectangleF(RectangleF originalRect)
        {
            var myNewRect = new RectangleF(originalRect.X * Zoom, originalRect.Y * Zoom, originalRect.Width * Zoom, originalRect.Height * Zoom);
            return myNewRect;
        }

        protected RectangleF DeZoomRectangleF(RectangleF originalRect)
        {
            var myNewRect = new RectangleF(originalRect.X / Zoom, originalRect.Y / Zoom, originalRect.Width / Zoom, originalRect.Height / Zoom);
            return myNewRect;
        }

        private GraphicObjects GraphicObjectChanged
        {
            set
            {
                m_Gobj = value;
            }
            get
            {
                ObjectChanged?.Invoke(m_Gobj);
                return m_Gobj;
            }
        }

        public GraphicObjectCollection(GraphicObjects gobj)
        {

        }

        public void LoadDrag_Handles()
        {
            int RectHeight = 8;

            if (DragRectangleF.Height < 50f)
            {
                RectHeight = 4;
            }
            else if (DragRectangleF.Height < 25f)
            {
                RectHeight = 2;
            }

            Drag_Handles[0] = new RectangleF((float)((double)DragRectangleF.X - RectHeight / 2d), (float)((double)DragRectangleF.Y - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[1] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width / 2f) - RectHeight / 2d), (float)((double)DragRectangleF.Y - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[2] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), (float)((double)DragRectangleF.Y - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[3] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height / 2f) - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[4] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height) - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[5] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width / 2f) - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height) - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[6] = new RectangleF((float)((double)DragRectangleF.X - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height) - RectHeight / 2d), RectHeight, RectHeight);
            Drag_Handles[7] = new RectangleF((float)((double)DragRectangleF.X - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height / 2f) - RectHeight / 2d), RectHeight, RectHeight);
            // last 2 drag handles are for rotating object
            Drag_Handles[8] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), DragRectangleF.Y + 10f, RectHeight, RectHeight);
            Drag_Handles[9] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), DragRectangleF.Y + DragRectangleF.Height - 20f, RectHeight, RectHeight);

        }

        public void DrawSelectedObject(Graphics g, GraphicObjects selectedObject, float Scale)
        {

            if (selectedObject is null)
            {
                return;
            }

            System.Drawing.Drawing2D.GraphicsContainer gCon1, gCon2;
            gCon1 = g.BeginContainer();
            g.ScaleTransform(Scale, Scale, System.Drawing.Drawing2D.MatrixOrder.Append);
            gCon2 = g.BeginContainer();
            g.PageUnit = GraphicsUnit.Pixel;

            if (selectedObject is not null)
            {

                var selectionPen = new Pen(Color.FromKnownColor(KnownColor.Black));
                selectionPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                selectionPen.Width = 1f;
                Brush GrabHandleBrush = new SolidBrush(Color.Black);
                var HandlePen = new Pen(Color.FromKnownColor(KnownColor.Black));

                if (selectedObject.Rotation != 0f)
                {
                    System.Drawing.Drawing2D.Matrix myMatrix;
                    myMatrix = g.Transform;
                    myMatrix.RotateAt(selectedObject.Rotation, new PointF(selectedObject.X, selectedObject.Y), System.Drawing.Drawing2D.MatrixOrder.Append);
                    g.Transform = myMatrix;
                    // g.TranslateTransform(selectedObject.X, selectedObject.Y, Drawing.Drawing2D.MatrixOrder.Append)
                    // RotatedBitmap = RotateImage(selectedObject.Image, selectedObject.Rotation)
                }

                DragRectangleF = new RectangleF(selectedObject.X, selectedObject.Y, selectedObject.Width, selectedObject.Height);

                int RectHeight = 8;

                if (selectedObject.Height < 50)
                {
                    RectHeight = 4;
                }
                else if (selectedObject.Height < 25)
                {
                    RectHeight = 2;
                }

                Drag_Handles[0] = new RectangleF((float)((double)DragRectangleF.X - RectHeight / 2d), (float)((double)DragRectangleF.Y - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[1] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width / 2f) - RectHeight / 2d), (float)((double)DragRectangleF.Y - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[2] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), (float)((double)DragRectangleF.Y - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[3] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height / 2f) - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[4] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height) - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[5] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width / 2f) - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height) - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[6] = new RectangleF((float)((double)DragRectangleF.X - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height) - RectHeight / 2d), RectHeight, RectHeight);
                Drag_Handles[7] = new RectangleF((float)((double)DragRectangleF.X - RectHeight / 2d), (float)((double)(DragRectangleF.Y + DragRectangleF.Height / 2f) - RectHeight / 2d), RectHeight, RectHeight);
                // last 2 drag handles are for rotating object
                Drag_Handles[8] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), DragRectangleF.Y + 10f, RectHeight, RectHeight);
                Drag_Handles[9] = new RectangleF((float)((double)(DragRectangleF.X + DragRectangleF.Width) - RectHeight / 2d), DragRectangleF.Y + DragRectangleF.Height - 20f, RectHeight, RectHeight);

                g.DrawRectangle(new Pen(Color.Gray), DragRectangleF.X, DragRectangleF.Y, DragRectangleF.Width, DragRectangleF.Height);
                g.FillRectangles(Brushes.Gray, Drag_Handles);
                g.DrawRectangles(Pens.Black, Drag_Handles);

                var RotationDragHandle = new RectangleF();
                RotationDragHandle = Drag_Handles[8];
                g.FillRectangle(Brushes.YellowGreen, RotationDragHandle);

                var RotationDragHandle1 = new RectangleF();
                RotationDragHandle1 = Drag_Handles[9];
                g.FillRectangle(Brushes.YellowGreen, RotationDragHandle1);

            }

            g.EndContainer(gCon2);
            g.EndContainer(gCon1);

        }

        public void DrawObjects(Graphics g, float Scale)
        {
            GraphicObjects drawObj;
            int i;
            System.Drawing.Drawing2D.GraphicsContainer gCon;
            System.Drawing.Drawing2D.Matrix myOriginalMatrix;
            myOriginalMatrix = g.Transform;
            gCon = g.BeginContainer();
            g.PageUnit = GraphicsUnit.Pixel;
            g.ScaleTransform(Scale, Scale);
            if (InnerList is not null && InnerList.Count > 0)
            {
                var loopTo = InnerList.Count - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    drawObj = (GraphicObjects)InnerList[i];
                    drawObj.Draw(g);
                    // ImageGraphic = g
                }
            }
            g.EndContainer(gCon);
            g.Transform = myOriginalMatrix;

        }

        public void PrintObjects(Graphics g)
        {

            GraphicObjects drawObj;
            int i;
            g.PageUnit = GraphicsUnit.Pixel;
            if (InnerList is not null && InnerList.Count > 0)
            {
                var loopTo = InnerList.Count - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    drawObj = (GraphicObjects)InnerList[i];
                    drawObj.Draw(g);
                }
            }
        }

        public Image RotateImage(Image TheImage, float Rotation)
        {
            Image RotatedBM;

            // Copy the output bitmap from the source image.
            var bm_in = new Bitmap(TheImage);

            // Make an array of points defining the
            // image's corners.
            float wid = bm_in.Width;
            float hgt = bm_in.Height;
            Point[] corners = new[] { new Point(0, 0), new Point((int)Round(wid), 0), new Point(0, (int)Round(hgt)), new Point((int)Round(wid), (int)Round(hgt)) };

            // Translate to center the bounding box at the origin.
            float cx = wid / 2f;
            float cy = hgt / 2f;
            long i;
            for (i = 0L; i <= 3L; i++)
            {
                corners[(int)i].X = (int)Round(corners[(int)i].X - cx);
                corners[(int)i].Y = (int)Round(corners[(int)i].Y - cy);
            }

            // Rotate.
            float theta = (float)((double)float.Parse(Rotation.ToString()) * PI / 180.0d);
            float sin_theta = (float)Sin((double)theta);
            float cos_theta = (float)Cos((double)theta);
            float X;
            float Y;
            for (i = 0L; i <= 3L; i++)
            {
                X = corners[(int)i].X;
                Y = corners[(int)i].Y;
                corners[(int)i].X = (int)Round(X * cos_theta + Y * sin_theta);
                corners[(int)i].Y = (int)Round(-X * sin_theta + Y * cos_theta);
            }

            // Translate so X >= 0 and Y >=0 for all corners.
            float xmin = corners[0].X;
            float ymin = corners[0].Y;
            for (i = 1L; i <= 3L; i++)
            {
                if (xmin > corners[(int)i].X)
                    xmin = corners[(int)i].X;
                if (ymin > corners[(int)i].Y)
                    ymin = corners[(int)i].Y;
            }
            for (i = 0L; i <= 3L; i++)
            {
                corners[(int)i].X = (int)Round(corners[(int)i].X - xmin);
                corners[(int)i].Y = (int)Round(corners[(int)i].Y - ymin);
            }

            // Create an output Bitmap and Graphics object.
            var bm_out = new Bitmap((int)Round(-2 * xmin), (int)Round(-2 * ymin));
            var gr_out = Graphics.FromImage(bm_out);

            // Drop the last corner lest we confuse DrawImage, 
            // which expects an array of three corners.
            Array.Resize(ref corners, 3);

            // Draw the result onto the output Bitmap.
            gr_out.DrawImage(bm_in, corners);

            // Display the result.
            RotatedBM = bm_out;

            return RotatedBM;
        }

        public GraphicObjects FindObjectAtPoint(Point pt)
        {
            GraphicObjects drawObj;
            int i;
            if (InnerList is not null && InnerList.Count > 0)
            {
                for (i = InnerList.Count - 1; i >= 0; i -= 1)
                {
                    drawObj = (GraphicObjects)InnerList[i];
                    if (drawObj.HitTest(pt))
                    {
                        return drawObj;
                        break;
                    }
                }
            }
            return null;
        }

        public Image SaveDrawnObjects(Graphics G)
        {
            GraphicObjects drawObj;
            int i;
            var BM = new Bitmap((int)Round(G.DpiX), (int)Round(G.DpiY));
            var gr = Graphics.FromImage(BM);

            G.PageUnit = GraphicsUnit.Pixel;
            gr.PageUnit = GraphicsUnit.Pixel;
            if (InnerList is not null && InnerList.Count > 0)
            {
                var loopTo = InnerList.Count - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    drawObj = (GraphicObjects)InnerList[i];
                    drawObj.Draw(G);
                    drawObj.Draw(gr);
                }
            }
            return (Image)BM.Clone();
        }

        public int HorizontalResolution
        {
            get
            {
                return m_HorizRes;
            }
            set
            {
                m_HorizRes = value;
            }
        }

        public int VerticalResolution
        {
            get
            {
                return m_VertRes;
            }
            set
            {
                m_VertRes = value;
            }
        }
        public GraphicObjectCollection() : base()
        {
        }

        public GraphicObjectCollection(GraphicObjectCollection value) : base()
        {
            AddRange(value);
        }

        public GraphicObjectCollection(GraphicObjects[] value) : base()
        {
            AddRange(value);
        }

        public GraphicObjects this[int index]
        {
            get
            {
                return (GraphicObjects)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public Image RotatedBitmap { get; set; }

        public int Add(GraphicObjects value)
        {
            value.Container = this;

            return List.Add(value);
        }

        public void AddRange(GraphicObjects[] value)
        {
            int i = 0;
            while (i < value.Length)
            {
                Add(value[i]);
                i = i + 1;
            }
        }

        public void AddRange(GraphicObjectCollection value)
        {
            int i = 0;
            while (i < value.Count)
            {
                Add(value[i]);
                i = i + 1;
            }
        }

        public bool Contains(GraphicObjects value)
        {
            return List.Contains(value);
        }

        public void CopyTo(GraphicObjects[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(GraphicObjects value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, GraphicObjects value)
        {
            List.Insert(index, value);
        }

        // See also 'System.Collections.IEnumerator'
        public new GraphicObjectEnumerator GetEnumerator()
        {
            return new GraphicObjectEnumerator(this);
        }

        public void Remove(GraphicObjects value)
        {
            List.Remove(value);
        }

        public class GraphicObjectEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;
            private IEnumerable temp;

            public GraphicObjectEnumerator(GraphicObjectCollection mappings) : base()
            {
                temp = mappings;
                baseEnumerator = temp.GetEnumerator();
            }

            public GraphicObjects Current
            {
                get
                {
                    return (GraphicObjects)baseEnumerator.Current;
                }
            }

            public object IEnumerator_Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            object IEnumerator.Current { get => IEnumerator_Current; }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public bool IEnumerator_MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext() => IEnumerator_MoveNext();

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            public void IEnumerator_Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset() => IEnumerator_Reset();

        }

        // Public Sub MousePosition()
        // Throw New NotImplementedException()
        // End Sub
    }
}