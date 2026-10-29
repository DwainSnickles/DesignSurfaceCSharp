// Original Author:  Jigar Desai
// Original Language: C#  
// Original Release Date: 05/13/2002   
// Revision Author:  Bryan Long
// Revision Language: VB.NET  
// Revision Release Date: 07/09/2002 

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.MSDN.Samples
{

    // Delegate type for hooking up ValueChanged notifications. 
    public delegate void ValueChangedEventHandler(object Sender);

    public class KnobControlType : UserControl
    {

        private CUtility Utility;
        private int _Minimum = 0;
        private int _Maximum = 25;
        private int _LargeChange = 5;
        private int _SmallChange = 1;
        private bool _ShowSmallScale = false;
        private bool _ShowLargeScale = true;
        private bool _isFocused = false;
        private Color _KnobColor;
        private int _Value = 0;
        private bool isKnobRotating = false;
        private Rectangle rKnob;
        private Point pKnob;
        private Rectangle rScale;
        private Pen DottedPen;

        private Brush bKnob;
        private Brush bKnobPoint;

        // declare Off screen image and Offscreen graphics       
        private Image OffScreenImage;
        private Graphics gOffScreen;

        // An event that clients can use to be notified whenever 
        // the Value is Changed.                                 
        public event ValueChangedEventHandler ValueChanged;

        public delegate void ValueChangedEventHandler(object ValueChangedEventHandler);

        #region  Windows Form Designer generated code 

        public KnobControlType() : base()
        {
            _KnobColor = BackColor;

            // This call is required by the Windows Form Designer.
            KnobControl();
            InitializeComponent();

            // Knob Control
            ImeMode = ImeMode.On;
            Name = "Knob";
            Resize += Knob_Resize;

        }

        // UserControl1 overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        // Required by the Windows Form Designer
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // KnobControl
            // 
            BackColor = Color.White;
            Name = "KnobControl";
            ResumeLayout(false);

        }

        #endregion

        #region  Public Properties 


        // Shows Small Scale marking.
        public bool ShowSmallScale
        {
            get
            {
                return _ShowSmallScale;
            }
            set
            {
                _ShowSmallScale = value;
                // Need to redraw 
                Refresh();
            }
        }


        // Shows Large Scale marking
        public bool ShowLargeScale
        {
            get
            {
                return _ShowLargeScale;
            }
            set
            {
                _ShowLargeScale = value;
                // Need to redraw
                Refresh();
            }
        }


        // Minimum Value for knob Control
        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                _Minimum = value;
            }
        }


        // Maximum value for knob control
        public int Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                _Maximum = value;
            }
        }


        // value set for large change
        public int LargeChange
        {
            get
            {
                return _LargeChange;
            }
            set
            {
                _LargeChange = value;
                Refresh();
            }
        }


        // value set for small change.
        public int SmallChange
        {
            get
            {
                return _SmallChange;
            }
            set
            {
                _SmallChange = value;
                Refresh();
            }
        }

        // Current Value of knob control
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = (int)Math.Round(value);
                if (_Value > 0)
                {
                    // Call delegate  
                    OnValueChanged(Value);
                    Refresh();
                }
            }
        }

        // Set Color of knob control
        public Color KnobColor
        {
            get
            {
                return _KnobColor;
            }
            set
            {
                _KnobColor = value;
                // Refresh Colors
                Refresh();
            }
        }

        #endregion

        protected void OnValueChanged(object sender)
        {
            ValueChanged?.Invoke(sender);
        }

        public void KnobControl()
        {
            Utility = new CUtility();
            DottedPen = new Pen(Utility.getDarkColor(BackColor, 40));
            DottedPen.DashStyle = DashStyle.Dot;
            DottedPen.DashCap = DashCap.Round;
            setDimensions();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // create LinearGradientBrush for creating knob            
            bKnob = new LinearGradientBrush(rKnob, Utility.getLightColor(KnobColor, 55), Utility.getDarkColor(KnobColor, 55), LinearGradientMode.ForwardDiagonal);


            // create LinearGradientBrush for knobPoint                
            bKnobPoint = new LinearGradientBrush(rKnob, Utility.getLightColor(BackColor, 55), Utility.getDarkColor(base.BackColor, 55), LinearGradientMode.ForwardDiagonal);



            // Set background color of Image...            
            gOffScreen.Clear(BackColor);
            // Fill knob Background to give knob effect....
            gOffScreen.FillEllipse(bKnob, rKnob);
            // Set antialias effect on                     
            gOffScreen.SmoothingMode = SmoothingMode.AntiAlias;
            // Draw border of knob                         
            gOffScreen.DrawEllipse(new Pen(BackColor), rKnob);

            // if control is focused 
            if (_isFocused)
            {
                gOffScreen.DrawEllipse(DottedPen, rKnob);
            }
            // get current position of pointer             
            var Arrow = getKnobPosition();
            // Draw pointer arrow that shows knob position 
            var argr = new Rectangle(Arrow.X - 3, Arrow.Y - 3, 6, 6);
            Utility.DrawInsetCircle(ref gOffScreen, ref argr, new Pen(BackColor));

            // Draw small and large scale                  
            int i = Minimum;
            if (_ShowSmallScale)
            {
                var loopTo = Maximum;
                for (i = Minimum; i <= loopTo; i++)
                    gOffScreen.DrawLine(new Pen(ForeColor), getMarkerPoint(0, i), getMarkerPoint(3, i));
            }

            if (_ShowLargeScale)
            {
                var loopTo1 = Maximum;
                for (i = Minimum; i <= loopTo1; i++)
                    gOffScreen.DrawLine(new Pen(ForeColor), getMarkerPoint(0, i), getMarkerPoint(5, i));
            }

            // Drawimage on screen                    
            g.DrawImage(OffScreenImage, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Empty To avoid Flickering due do background Drawing
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Utility.isPointinRectangle(new Point(e.X, e.Y), rKnob))
            {
                // Start Rotation of knob         
                isKnobRotating = true;
            }
        }

        // ----------------------------------------------------------
        // we need to override IsInputKey method to allow user to   
        // use up, down, right and bottom keys other wise using me
        // keys will change focus from current object to another    
        // object on the form                                       
        // ----------------------------------------------------------
        protected override bool IsInputKey(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
            return base.IsInputKey(key);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Stop rotation                   
            isKnobRotating = false;
            if (Utility.isPointinRectangle(new Point(e.X, e.Y), rKnob))
            {
                // get value                   
                Value = getValueFromPosition(new Point(e.X, e.Y));
            }
            Cursor = Cursors.Default;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // --------------------------------------
            // Following Handles Knob Rotating     
            // --------------------------------------
            if (isKnobRotating == true)
            {
                Cursor = Cursors.Hand;
                var p = new Point(e.X, e.Y);
                int posVal = getValueFromPosition(p);
                Value = posVal;
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            _isFocused = true;
            Refresh();
            base.OnEnter(new EventArgs());
        }

        protected override void OnLeave(EventArgs e)
        {
            _isFocused = false;
            Refresh();
            base.OnLeave(new EventArgs());
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // --------------------------------------------------------
            // Handles knob rotation with up,down,left and right keys 
            // --------------------------------------------------------
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Right)
            {
                if (_Value < Maximum)
                {
                    Value = _Value + 1;
                    Refresh();
                }
            }
            else if (e.KeyCode == Keys.Down | e.KeyCode == Keys.Left)
            {
                if (_Value > Minimum)
                {
                    Value = _Value - 1;
                    Refresh();
                }
            }
        }

        private void setDimensions()
        {
            // get smaller from height and width
            int size = Width;
            if (Width > Height)
            {
                size = Height;
            }
            // allow 10% gap on all side to determine size of knob    
            rKnob = new Rectangle((int)Math.Round(size * 0.1d), (int)Math.Round(size * 0.1d), (int)Math.Round(size * 0.8d), (int)Math.Round(size * 0.8d));
            rScale = new Rectangle(2, 2, size - 4, size - 4);
            pKnob = new Point((int)Math.Round(rKnob.X + rKnob.Width / 2d), (int)Math.Round(rKnob.Y + rKnob.Height / 2d));
            // create offscreen image                                 
            OffScreenImage = new Bitmap(Width, Height);
            // create offscreen graphics                              
            gOffScreen = Graphics.FromImage(OffScreenImage);
        }

        private void Knob_Resize(object sender, EventArgs e)
        {
            setDimensions();
            Refresh();
        }


        // gets knob position that is to be drawn on control.
        private Point getKnobPosition()
        {
            double degree = 270d * Value / (Maximum - Minimum);
            degree = (degree + 135d) * Math.PI / 180d;

            var Pos = new Point(0, 0);
            Pos.X = (int)Math.Round(Math.Cos(degree) * (rKnob.Width / 2d - 10d) + rKnob.X + rKnob.Width / 2d);
            Pos.Y = (int)Math.Round(Math.Sin(degree) * (rKnob.Width / 2d - 10d) + rKnob.Y + rKnob.Height / 2d);
            return Pos;
        }


        // gets marker point required to draw scale marker.
        // <param name="length">distance from center</param>
        // <param name="Value">value that is to be marked</param>
        // <returns>Point that describes marker position</returns>
        private Point getMarkerPoint(int length, int Value)
        {
            double degree = 270 * Value / (double)(Maximum - Minimum);
            degree = (degree + 135d) * Math.PI / 180d;

            var Pos = new Point(0, 0);
            Pos.X = (int)Math.Round(Math.Cos(degree) * (rKnob.Width / 2d - length + 7d) + rKnob.X + rKnob.Width / 2d);
            Pos.Y = (int)Math.Round(Math.Sin(degree) * (rKnob.Width / 2d - length + 7d) + rKnob.Y + rKnob.Height / 2d);
            return Pos;
        }


        // converts geomatrical position in to value..
        // <param name="p">Point that is to be converted</param>
        // <returns>Value derived from position</returns>
        private int getValueFromPosition(Point p)
        {
            double degree = 0.0d;
            int v = 0;
            if (p.X <= pKnob.X)
            {
                degree = (pKnob.Y - p.Y) / (double)(pKnob.X - p.X);
                degree = Math.Atan(degree);
                degree = degree * (180d / Math.PI) + 45d;
                v = (int)Math.Round(degree * (Maximum - Minimum) / 270d);
            }
            else if (p.X > pKnob.X)
            {
                degree = (p.Y - pKnob.Y) / (double)(p.X - pKnob.X);
                degree = Math.Atan(degree);
                degree = 225d + degree * (180d / Math.PI);
                v = (int)Math.Round(degree * (Maximum - Minimum) / 270d);
            }

            if (v > Maximum)
                v = Maximum;
            if (v < Minimum)
                v = Minimum;
            return v;
        }

    }
}