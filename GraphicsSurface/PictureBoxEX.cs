using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.Msdn.Samples.DesignSurface
{



    public class PictureBoxEx : PictureBox
    {

        private const int rectsize = 10;

        private Point mMousePointDown;
        private Point mMousePointDownScrCo;
        private Point mMousePointMove;
        private Point mMousePointUp;

        private float sMousePointDeltaX;
        private int iPicBoxTopWhenMouseDown = 0;
        private int iPicBoxLeftWhenMouseDown = 0;
        private int iPicBoxTopWhenMouseDownSC = 0;
        private int iPicBoxLeftWhenMouseDownSC = 0;
        private int iPicBoxOldHeight;
        private int iPicBoxOldWidth;

        private bool bDrawRect = false;

        private bool bPicBoxMouseDownFlag = false;
        private bool bPicBoxMouseMovedFlag = false;
        private bool bMouseIsOnRectTopLeftFlag = false;
        private bool bMouseIsOnRectTopFlag = false;
        private bool bMouseIsOnRectTopRightFlag = false;
        private bool bMouseIsOnRectRightFlag = false;
        private bool bMouseIsOnRectBottomRightFlag = false;
        private bool bMouseIsOnRectBottomFlag = false;
        private bool bMouseIsOnRectBottomLeftFlag = false;
        private bool bMouseIsOnRectLeftFlag = false;
        private bool bResizingPicBoxFlag = false;
        private bool bCheckDragFlag = false;
        private bool bDragOKFlag = false;

        public PictureBoxEx()
        {
            Click += PictureBoxEx_MouseClick;
            MouseDown += PictureBoxEx_MouseDown;
            MouseLeave += PictureBoxEx_MouseLeave;
            MouseMove += PictureBoxEx_MouseMove;
            MouseUp += PictureBoxEx_MouseUp;
            Paint += PictureBoxEx_Paint;
        }

        private void PictureBoxEx_MouseClick(object sender, EventArgs e)
        {

            PictureBox pb = (PictureBox)sender;

            // switch selection rectangle on and off
            if (bDrawRect == false)
            {
                bDrawRect = true;
                pb.Invalidate();
            }
            else
            {
                bDrawRect = false;
                pb.Invalidate();
            }

        }

        private void PictureBoxEx_MouseDown(object sender, MouseEventArgs e)
        {

            PictureBox pb = (PictureBox)sender;

            // get top and left border of picture box in client coordinates
            pb = (PictureBox)sender;
            iPicBoxTopWhenMouseDown = pb.Top;
            iPicBoxLeftWhenMouseDown = pb.Left;

            // transform to client coordinates
            var myPoint = new Point(pb.Left, pb.Top);
            iPicBoxTopWhenMouseDownSC = PointToScreen(myPoint).Y;
            iPicBoxLeftWhenMouseDownSC = PointToScreen(myPoint).X;

            // set flag that mouse is down on picture box
            bPicBoxMouseDownFlag = true;

            // Reset flag for mouse starts dragging. This flag is set to true in 
            // Sub PictureBox1_MouseMove, when mouse leaves a little rectangle area 
            // around mouse down point.
            bDragOKFlag = false;

            // get client coordinates of mouse point
            mMousePointDown = e.Location;

            // compute screen coordinates of mouse point
            mMousePointDownScrCo = (Point)((dynamic)sender).PointToScreen(mMousePointDown);

            // set picture box to top of z-order
            pb.BringToFront();

            // store height of picture box
            iPicBoxOldHeight = Conversions.ToInteger(((dynamic)sender).Height);

            // store width of picture box
            iPicBoxOldWidth = Conversions.ToInteger(((dynamic)sender).Width);

            // set flag for "mouse is on little rectangle at top border of picture box"
            if (MouseIsOnRectTopLeft(sender, mMousePointDown))
            {
                bMouseIsOnRectTopLeftFlag = true;
            }
            else if (MouseIsOnRectTop(sender, mMousePointDown))
            {
                bMouseIsOnRectTopFlag = true;
            }
            else if (MouseIsOnRectTopRight(sender, mMousePointDown))
            {
                bMouseIsOnRectTopRightFlag = true;
            }
            else if (MouseIsOnRectRight(sender, mMousePointDown))
            {
                bMouseIsOnRectRightFlag = true;
            }
            else if (MouseIsOnRectBottomRight(sender, mMousePointDown))
            {
                bMouseIsOnRectBottomRightFlag = true;
            }
            else if (MouseIsOnRectBottom(sender, mMousePointDown))
            {
                bMouseIsOnRectBottomFlag = true;
            }
            else if (MouseIsOnRectBottomLeft(sender, mMousePointDown))
            {
                bMouseIsOnRectBottomLeftFlag = true;
            }
            else if (MouseIsOnRectLeft(sender, mMousePointDown))
            {
                bMouseIsOnRectLeftFlag = true;
            }
            else
            {
                // set drag cursor when picture box is selected
                Cursor = Cursors.Hand;

                // set flag that drag check (mouse in/out little rectangle
                // around mouse down point?) has started
                bCheckDragFlag = true;
            }

        }

        private void PictureBoxEx_MouseLeave(object sender, EventArgs e)
        {

            PictureBox pb = (PictureBox)sender;

            // reset cursor when mouse leaves picture box
            Cursor = Cursors.Arrow;

            UndrawSelRectAndSetFlags(pb);

        }

        private void PictureBoxEx_MouseMove(object sender, MouseEventArgs e)
        {

            PictureBox pb = (PictureBox)sender;

            float sMousePointDeltaX;
            float sMousePointDeltaY;
            float sPicBoxOldX;
            float sPicBoxOldY;

            Point sPointOnScreen;

            Control sMyParent;
            int sMyParentClientHeight;
            int sMyParentClientWidth;

            // dimensions of parent control do not change while this block is running
            sMyParent = pb.Parent;
            sMyParentClientHeight = sMyParent.ClientRectangle.Height;
            sMyParentClientWidth = sMyParent.ClientRectangle.Width;


            // get location of mouse cursor in client coordinates
            mMousePointMove = e.Location;

            // set DragOKFlag to True when mouse leaves little rectangle around
            // mouse down point
            if ((mMousePointMove.X < mMousePointDown.X - 3 | mMousePointMove.X > mMousePointDown.X + 3 | mMousePointMove.Y < mMousePointDown.Y - 3 | mMousePointMove.Y > mMousePointDown.Y + 3) & bPicBoxMouseDownFlag == true)
            {
                bDragOKFlag = true;
            }

            // compute point with screen coordinates
            sPointOnScreen = (Point)((dynamic)sender).PointToScreen(mMousePointMove);

            // Perform this block when mouse cursor is in the little rectangle on
            // top of the picture box or when mMouseIsOnRectTopFlag is set to True.
            // mMouseIsOnRectTopFlag = True means that the picture box will continue
            // to resize when mouse cursor leaves the little rectangle.
            if ((MouseIsOnRectTopLeft(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectTopLeftFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeNWSE;

                if (bPicBoxMouseDownFlag)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaX = sPointOnScreen.X - mMousePointDownScrCo.X;
                    sMousePointDeltaY = sPointOnScreen.Y - mMousePointDownScrCo.Y;

                    // compute new height and new top of the picture box
                    // make sure, that the picture box does not get to small
                    if (iPicBoxOldHeight - sMousePointDeltaY < 10f)
                    {
                        pb.Height = 10;
                        pb.Top = iPicBoxTopWhenMouseDown + iPicBoxOldHeight - 10;
                        bDrawRect = true;
                    }
                    // Attention: "PictureBox1.Top = ..." makes a redraw of the picture box! Do not use it in If-statement!!
                    else if (iPicBoxTopWhenMouseDown + sMousePointDeltaY < 0f)
                    {
                        // now mouse cursor has left the parent control (Form1)
                        pb.Top = 0;
                        // add the old height to point of top border when mouse down event was entered
                        pb.Height = iPicBoxTopWhenMouseDown + iPicBoxOldHeight;
                    }
                    else
                    {
                        pb.Height = (int)Math.Round(iPicBoxOldHeight - sMousePointDeltaY);
                        pb.Top = (int)Math.Round(iPicBoxTopWhenMouseDown + sMousePointDeltaY);
                    }

                    if (iPicBoxOldWidth - sMousePointDeltaX < 10f)
                    {
                        pb.Width = 10;
                        bDrawRect = true;
                    }
                    // Attention: "PictureBox1.Top = ..." makes a redraw of the picture box! Do not use it in If-statement!!
                    else if (iPicBoxLeftWhenMouseDown + sMousePointDeltaX < 0f)
                    {
                        // now mouse cursor has left the parent control (Form1)
                        pb.Left = 0;
                        // add the old width to point of left border when mouse down event was entered
                        pb.Width = iPicBoxLeftWhenMouseDown + iPicBoxOldWidth;
                    }
                    else
                    {
                        pb.Width = (int)Math.Round(iPicBoxOldWidth - sMousePointDeltaX);
                        pb.Left = (int)Math.Round(iPicBoxLeftWhenMouseDown + sMousePointDeltaX);
                    }

                }
            }

            else if ((MouseIsOnRectTop(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectTopFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeNS;

                if (bPicBoxMouseDownFlag == true)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaY = sPointOnScreen.Y - mMousePointDownScrCo.Y;

                    // compute new height and new top of the picture box make sure, that the picture box does not get to small
                    if (iPicBoxOldHeight - sMousePointDeltaY < 10f)
                    {
                        pb.Height = 10;
                        pb.Top = iPicBoxTopWhenMouseDown + iPicBoxOldHeight - 10;
                        bDrawRect = true;
                    }
                    else if (iPicBoxTopWhenMouseDown + sMousePointDeltaY < 0f)
                    {
                        pb.Height = iPicBoxTopWhenMouseDown + iPicBoxOldHeight;
                        pb.Top = 0;
                    }
                    else
                    {
                        pb.Height = (int)Math.Round(iPicBoxOldHeight - sMousePointDeltaY);
                        pb.Top = (int)Math.Round(iPicBoxTopWhenMouseDown + sMousePointDeltaY);
                    }

                }
            }

            else if ((MouseIsOnRectTopRight(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectTopRightFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeNESW;

                if (bPicBoxMouseDownFlag == true)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaX = sPointOnScreen.X - mMousePointDownScrCo.X;
                    sMousePointDeltaY = sPointOnScreen.Y - mMousePointDownScrCo.Y;

                    // compute new height and width of the picture box. make sure, that the picture box does not get to small
                    if (iPicBoxOldHeight - sMousePointDeltaY < 10f)
                    {
                        pb.Height = 10;
                        pb.Top = iPicBoxTopWhenMouseDown + iPicBoxOldHeight - 10;
                        bDrawRect = true;
                    }
                    else if (iPicBoxTopWhenMouseDown + sMousePointDeltaY < 0f)
                    {
                        pb.Height = iPicBoxTopWhenMouseDown + iPicBoxOldHeight;
                        pb.Top = 0;
                    }
                    else
                    {
                        pb.Height = (int)Math.Round(iPicBoxOldHeight - sMousePointDeltaY);
                        pb.Top = (int)Math.Round(iPicBoxTopWhenMouseDown + sMousePointDeltaY);
                    }

                    if (iPicBoxOldWidth + sMousePointDeltaX < 10f)
                    {
                        pb.Width = 10;
                        bDrawRect = true;
                    }
                    else if (pb.Left + iPicBoxOldWidth + sMousePointDeltaX > sMyParentClientWidth)
                    {
                        pb.Width = sMyParentClientWidth - iPicBoxLeftWhenMouseDown;
                    }
                    else
                    {
                        pb.Width = (int)Math.Round(iPicBoxOldWidth + sMousePointDeltaX);
                    }

                }
            }

            else if ((MouseIsOnRectRight(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectRightFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeWE;

                if (bPicBoxMouseDownFlag == true)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaX = sPointOnScreen.X - mMousePointDownScrCo.X;

                    // compute new width of the picture box. make sure, that the picture box does not get to small
                    if (iPicBoxOldWidth + sMousePointDeltaX < 10f)
                    {
                        pb.Width = 10;
                        bDrawRect = true;
                    }
                    else if (pb.Left + iPicBoxOldWidth + sMousePointDeltaX > sMyParentClientWidth)
                    {
                        pb.Width = sMyParentClientWidth - iPicBoxLeftWhenMouseDown;
                    }
                    else
                    {
                        pb.Width = (int)Math.Round(iPicBoxOldWidth + sMousePointDeltaX);
                    }
                }
            }

            else if ((MouseIsOnRectBottomRight(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectBottomRightFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeNWSE;

                if (bPicBoxMouseDownFlag == true)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaX = sPointOnScreen.X - mMousePointDownScrCo.X;
                    sMousePointDeltaY = sPointOnScreen.Y - mMousePointDownScrCo.Y;

                    // compute new height and width of the picture box. make sure, that the picture box does not get to small
                    if (iPicBoxOldWidth + sMousePointDeltaX < 10f)
                    {
                        pb.Width = 10;
                        bDrawRect = true;
                    }
                    // PictureBox1.Width = mPicBoxOldWidth + sMousePointDeltaX
                    else if (pb.Left + iPicBoxOldWidth + sMousePointDeltaX > sMyParentClientWidth)
                    {
                        pb.Width = sMyParentClientWidth - iPicBoxLeftWhenMouseDown;
                    }
                    else
                    {
                        pb.Width = (int)Math.Round(iPicBoxOldWidth + sMousePointDeltaX);
                    }

                    // picture box top does not change
                    if (iPicBoxOldHeight + sMousePointDeltaY < 10f)
                    {
                        pb.Height = 10;
                        bDrawRect = true;
                    }
                    else if (pb.Top + iPicBoxOldHeight + sMousePointDeltaY > sMyParentClientHeight)
                    {
                        pb.Height = sMyParentClientHeight - iPicBoxTopWhenMouseDown;
                    }
                    else
                    {
                        pb.Height = (int)Math.Round(iPicBoxOldHeight + sMousePointDeltaY);
                    }

                }
            }

            else if ((MouseIsOnRectBottom(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectBottomFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeNS;

                if (bPicBoxMouseDownFlag == true)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaY = sPointOnScreen.Y - mMousePointDownScrCo.Y;

                    // compute new height of the picture box. make sure, that the picture box does not get to small. picture box top does not change
                    if (iPicBoxOldHeight + sMousePointDeltaY < 10f)
                    {
                        pb.Height = 10;
                        bDrawRect = true;
                    }
                    else if (pb.Top + iPicBoxOldHeight + sMousePointDeltaY > sMyParentClientHeight)
                    {
                        pb.Height = sMyParentClientHeight - iPicBoxTopWhenMouseDown;
                    }
                    else
                    {
                        pb.Height = (int)Math.Round(iPicBoxOldHeight + sMousePointDeltaY);
                    }

                }
            }

            else if ((MouseIsOnRectBottomLeft(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectBottomLeftFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeNESW;


                if (bPicBoxMouseDownFlag == true)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaX = sPointOnScreen.X - mMousePointDownScrCo.X;
                    sMousePointDeltaY = sPointOnScreen.Y - mMousePointDownScrCo.Y;

                    // compute new height and width of the picture box. make sure, that the picture box does not get to small. picture box top does not change
                    if (iPicBoxOldHeight + sMousePointDeltaY < 10f)
                    {
                        pb.Height = 10;
                        bDrawRect = true;
                    }
                    else if (pb.Top + iPicBoxOldHeight + sMousePointDeltaY > sMyParentClientHeight)
                    {
                        pb.Height = sMyParentClientHeight - iPicBoxTopWhenMouseDown;
                    }
                    else
                    {
                        pb.Height = (int)Math.Round(iPicBoxOldHeight + sMousePointDeltaY);
                    }

                    if (iPicBoxOldWidth - sMousePointDeltaX < 10f)
                    {
                        pb.Width = 10;
                        bDrawRect = true;
                    }
                    else if (iPicBoxLeftWhenMouseDown + sMousePointDeltaX < 0f)
                    {
                        // now mouse cursor has left the parent control (Form1)
                        pb.Left = 0;
                        // add the old width to point of left border when mouse down event was entered
                        pb.Width = iPicBoxLeftWhenMouseDown + iPicBoxOldWidth;
                    }
                    else
                    {
                        pb.Width = (int)Math.Round(iPicBoxOldWidth - sMousePointDeltaX);
                        pb.Left = (int)Math.Round(iPicBoxLeftWhenMouseDown + sMousePointDeltaX);
                    }

                }
            }

            else if ((MouseIsOnRectLeft(sender, mMousePointMove) & !bPicBoxMouseDownFlag | bMouseIsOnRectLeftFlag) & bDrawRect & !bCheckDragFlag)
            {

                Cursor = Cursors.SizeWE;

                if (bPicBoxMouseDownFlag)
                {

                    UndrawSelRectAndSetFlags(pb);

                    // compute shift
                    sMousePointDeltaX = sPointOnScreen.X - mMousePointDownScrCo.X;

                    // compute new width of the picture box. make sure, that the picture box does not get to small. picture box top does not change
                    if (iPicBoxOldWidth - sMousePointDeltaX < 10f)
                    {
                        pb.Width = 10;
                        bDrawRect = true;
                    }
                    else if (iPicBoxLeftWhenMouseDown + sMousePointDeltaX < 0f)
                    {
                        // now mouse cursor has left the parent control (Form1)
                        pb.Left = 0;
                        // add the old width to point of left border when mouse down event
                        // was entered
                        pb.Width = iPicBoxLeftWhenMouseDown + iPicBoxOldWidth;
                    }
                    else
                    {
                        pb.Width = (int)Math.Round(iPicBoxOldWidth - sMousePointDeltaX);
                        pb.Left = (int)Math.Round(iPicBoxLeftWhenMouseDown + sMousePointDeltaX);
                    }

                }
            }

            else if (!bPicBoxMouseDownFlag)
            {

                Cursor = Cursors.Arrow;
            }

            else if (bDragOKFlag)
            {

                // "mblnDragOKFlag = True" means drag only when mouse leaves. little rectangle around mouse down point

                if (bPicBoxMouseDownFlag & !bMouseIsOnRectTopFlag & !bMouseIsOnRectRightFlag & !bMouseIsOnRectTopRightFlag)
                {

                    // set flag that cursor was moved so that selection rectangle
                    // will stay after releasing mouse button
                    bPicBoxMouseMovedFlag = true;

                    // uncommented code
                    if (bDrawRect)
                    {
                        bDrawRect = false;
                        pb.Invalidate();
                    }

                    // should I better use screen coordinates here?
                    sMousePointDeltaX = mMousePointMove.X - mMousePointDown.X;
                    sMousePointDeltaY = mMousePointMove.Y - mMousePointDown.Y;

                    // store old position
                    sPicBoxOldX = pb.Location.X;
                    sPicBoxOldY = pb.Location.Y;

                    // compute new position. make sure that picture box does not leave the form
                    if (sPicBoxOldX + sMousePointDeltaX < 0f)
                    {
                        pb.Left = 0;
                    }
                    else if (sPicBoxOldX + pb.Width + sMousePointDeltaX > sMyParentClientWidth)
                    {
                        pb.Left = sMyParentClientWidth - pb.Width;
                    }
                    else
                    {
                        pb.Left = (int)Math.Round(sPicBoxOldX + sMousePointDeltaX);
                    }

                    if (sPicBoxOldY + sMousePointDeltaY < 0f)
                    {
                        pb.Top = 0;
                    }
                    else if (sPicBoxOldY + pb.Height + sMousePointDeltaY > sMyParentClientHeight)
                    {
                        pb.Top = sMyParentClientHeight - pb.Height;
                    }
                    else
                    {
                        pb.Top = (int)Math.Round(sPicBoxOldY + sMousePointDeltaY);
                    }

                }

            }

        }

        private void UndrawSelRectAndSetFlags(PictureBox pb)
        {

            // on first entry in this block after pressing left mouse button
            // undraw the selection rectangle
            if (!bResizingPicBoxFlag)
            {
                pb.Invalidate();
            }

            // set flag that picture box is on resizing to avoid dragging
            // of the picture box
            bResizingPicBoxFlag = true;


            // set flag that cursor was moved
            bPicBoxMouseMovedFlag = true;

        }

        private bool MouseIsOnRectTopLeft(object sender, Point MousePoint)
        {
            bool MouseIsOnRectTopLeftRet = default;

            MouseIsOnRectTopLeftRet = false;

            if (MousePoint.X >= 0 & MousePoint.X <= rectsize & MousePoint.Y >= 0 & MousePoint.Y <= rectsize)
            {
                MouseIsOnRectTopLeftRet = true;
            }

            return MouseIsOnRectTopLeftRet;

        }

        private bool MouseIsOnRectTop(object sender, Point MousePoint)
        {
            bool MouseIsOnRectTopRet = default;

            MouseIsOnRectTopRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectGreaterEqual(MousePoint.X, Operators.SubtractObject(Operators.DivideObject(((dynamic)sender).Width, 2), rectsize / 2d), false), Operators.ConditionalCompareObjectLessEqual(MousePoint.X, Operators.AddObject(Operators.DivideObject(((dynamic)sender).Width, 2), rectsize / 2d), false)), MousePoint.Y >= 0), MousePoint.Y <= rectsize)))
            {
                MouseIsOnRectTopRet = true;
            }

            return MouseIsOnRectTopRet;

        }

        private bool MouseIsOnRectTopRight(object sender, Point MousePoint)
        {
            bool MouseIsOnRectTopRightRet = default;

            MouseIsOnRectTopRightRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectGreaterEqual(MousePoint.X, Operators.SubtractObject(((dynamic)sender).Width, rectsize), false), Operators.ConditionalCompareObjectLessEqual(MousePoint.X, ((dynamic)sender).Width, false)), MousePoint.Y >= 0), MousePoint.Y <= rectsize)))
            {
                MouseIsOnRectTopRightRet = true;
            }

            return MouseIsOnRectTopRightRet;

        }

        private bool MouseIsOnRectRight(object sender, Point MousePoint)
        {
            bool MouseIsOnRectRightRet = default;

            MouseIsOnRectRightRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectGreaterEqual(MousePoint.X, Operators.SubtractObject(((dynamic)sender).Width, rectsize), false), Operators.ConditionalCompareObjectLessEqual(MousePoint.X, ((dynamic)sender).Width, false)), Operators.ConditionalCompareObjectGreaterEqual(MousePoint.Y, Operators.SubtractObject(Operators.DivideObject(((dynamic)sender).Height, 2), rectsize / 2d), false)), Operators.ConditionalCompareObjectLessEqual(MousePoint.Y, Operators.AddObject(Operators.DivideObject(((dynamic)sender).Height, 2), rectsize / 2d), false))))
            {
                MouseIsOnRectRightRet = true;
            }

            return MouseIsOnRectRightRet;

        }

        private bool MouseIsOnRectBottomRight(object sender, Point MousePoint)
        {
            bool MouseIsOnRectBottomRightRet = default;

            MouseIsOnRectBottomRightRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectGreaterEqual(MousePoint.X, Operators.SubtractObject(((dynamic)sender).Width, rectsize), false), Operators.ConditionalCompareObjectLessEqual(MousePoint.X, ((dynamic)sender).Width, false)), Operators.ConditionalCompareObjectGreaterEqual(MousePoint.Y, Operators.SubtractObject(((dynamic)sender).Height, rectsize), false)), Operators.ConditionalCompareObjectLessEqual(MousePoint.Y, ((dynamic)sender).Height, false))))
            {
                MouseIsOnRectBottomRightRet = true;
            }

            return MouseIsOnRectBottomRightRet;

        }

        private bool MouseIsOnRectBottom(object sender, Point MousePoint)
        {
            bool MouseIsOnRectBottomRet = default;

            MouseIsOnRectBottomRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectGreaterEqual(MousePoint.X, Operators.SubtractObject(Operators.DivideObject(((dynamic)sender).Width, 2), rectsize / 2d), false), Operators.ConditionalCompareObjectLessEqual(MousePoint.X, Operators.AddObject(Operators.DivideObject(((dynamic)sender).Width, 2), rectsize / 2d), false)), Operators.ConditionalCompareObjectGreaterEqual(MousePoint.Y, Operators.SubtractObject(((dynamic)sender).Height, rectsize), false)), Operators.ConditionalCompareObjectLessEqual(MousePoint.Y, ((dynamic)sender).Height, false))))
            {
                MouseIsOnRectBottomRet = true;
            }

            return MouseIsOnRectBottomRet;

        }

        private bool MouseIsOnRectBottomLeft(object sender, Point MousePoint)
        {
            bool MouseIsOnRectBottomLeftRet = default;

            MouseIsOnRectBottomLeftRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(MousePoint.X >= 0 & MousePoint.X <= rectsize, Operators.ConditionalCompareObjectGreaterEqual(MousePoint.Y, Operators.SubtractObject(((dynamic)sender).Height, rectsize), false)), Operators.ConditionalCompareObjectLessEqual(MousePoint.Y, ((dynamic)sender).Height, false))))
            {
                MouseIsOnRectBottomLeftRet = true;
            }

            return MouseIsOnRectBottomLeftRet;

        }

        private bool MouseIsOnRectLeft(object sender, Point MousePoint)
        {
            bool MouseIsOnRectLeftRet = default;

            MouseIsOnRectLeftRet = false;

            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(MousePoint.X >= 0 & MousePoint.X <= rectsize, Operators.ConditionalCompareObjectGreaterEqual(MousePoint.Y, Operators.SubtractObject(Operators.DivideObject(((dynamic)sender).Height, 2), rectsize / 2d), false)), Operators.ConditionalCompareObjectLessEqual(MousePoint.Y, Operators.AddObject(Operators.DivideObject(((dynamic)sender).Height, 2), rectsize / 2d), false))))
            {
                MouseIsOnRectLeftRet = true;
            }

            return MouseIsOnRectLeftRet;

        }

        private void PictureBoxEx_MouseUp(object sender, MouseEventArgs e)
        {

            PictureBox pb = (PictureBox)sender;

            // reset flags after releasing mouse button

            bMouseIsOnRectTopLeftFlag = false;
            bMouseIsOnRectTopFlag = false;
            bMouseIsOnRectTopRightFlag = false;
            bMouseIsOnRectRightFlag = false;
            bMouseIsOnRectBottomRightFlag = false;
            bMouseIsOnRectBottomFlag = false;
            bMouseIsOnRectBottomLeftFlag = false;
            bMouseIsOnRectLeftFlag = false;

            if (bResizingPicBoxFlag == true)
                bDrawRect = true; // now rectangle will 
                                  // be drawn again when it was drawn before dragging starts
            bResizingPicBoxFlag = false;
            bCheckDragFlag = false;
            if (bDragOKFlag == true)
                bDrawRect = true; // now rectangle will 
                                  // be drawn again when it was drawn before dragging starts
            bDragOKFlag = false;

            // reset cursor from drag to normal
            Cursor = Cursors.Arrow;

            mMousePointUp = e.Location;

            if (bPicBoxMouseMovedFlag == true)
            {
                bDrawRect = true;
                pb.Invalidate();
            }

            // reset flag
            bPicBoxMouseMovedFlag = false;

            // reset flag
            bPicBoxMouseDownFlag = false;

        }

        private void PictureBoxEx_Paint(object sender, PaintEventArgs e)
        {

            // Create pen.
            var blackPen1 = new Pen(Color.Black, 1f);
            var blackPen2 = new Pen(Color.Black, 2f);

            // Create rectangle.
            var rect = new Rectangle(0, 0, Conversions.ToInteger(((dynamic)sender).Width), Conversions.ToInteger(((dynamic)sender).Height));

            float sSenderWidth = Conversions.ToSingle(Operators.DivideObject(((dynamic)sender).Width, 2));
            float sSenderHeight = Conversions.ToSingle(Operators.DivideObject(((dynamic)sender).Height, 2));

            var rectTopLeft = new Rectangle(0, 0, rectsize, rectsize);
            var rectTop = new Rectangle((int)Math.Round((double)sSenderWidth - rectsize / 2d), 0, rectsize, rectsize);
            var rectTopRight = new Rectangle(Conversions.ToInteger(Operators.SubtractObject(((dynamic)sender).Width, rectsize)), 0, rectsize, rectsize);
            var rectRight = new Rectangle(Conversions.ToInteger(Operators.SubtractObject(((dynamic)sender).Width, rectsize)), (int)Math.Round((double)sSenderHeight - rectsize / 2d), rectsize, rectsize);
            var rectBottomRight = new Rectangle((int)Math.Round((double)sSenderWidth - rectsize / 2d), Conversions.ToInteger(Operators.SubtractObject(((dynamic)sender).Height, rectsize)), rectsize, rectsize);
            var rectBottom = new Rectangle(Conversions.ToInteger(Operators.SubtractObject(((dynamic)sender).Width, rectsize)), Conversions.ToInteger(Operators.SubtractObject(((dynamic)sender).Height, rectsize)), rectsize, rectsize);
            var rectBottomLeft = new Rectangle(0, Conversions.ToInteger(Operators.SubtractObject(((dynamic)sender).Height, rectsize)), rectsize, rectsize);
            var rectLeft = new Rectangle(0, (int)Math.Round((double)sSenderHeight - rectsize / 2d), rectsize, rectsize);


            // If mDrawRect = True And mblnResizingPicBoxFlag = False Then
            if (bDrawRect & !bResizingPicBoxFlag & bDrawRect & !bDragOKFlag)
            {

                // Draw rectangle to screen.
                e.Graphics.DrawRectangle(blackPen2, rect);
                e.Graphics.DrawRectangle(blackPen1, rectTopLeft);
                e.Graphics.DrawRectangle(blackPen1, rectTop);
                e.Graphics.DrawRectangle(blackPen1, rectTopRight);
                e.Graphics.DrawRectangle(blackPen1, rectRight);
                e.Graphics.DrawRectangle(blackPen1, rectBottomRight);
                e.Graphics.DrawRectangle(blackPen1, rectBottom);
                e.Graphics.DrawRectangle(blackPen1, rectBottomLeft);
                e.Graphics.DrawRectangle(blackPen1, rectLeft);

            }

        }

    }
}