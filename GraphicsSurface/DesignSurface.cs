using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using static System.Math;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static Microsoft.MSDN.Samples.GraphicObjects.GraphicObjects;
using Microsoft.VisualBasic;

namespace Microsoft.Msdn.Samples.DesignSurface
{

    public class DesignSurface : UserControl
    {

        #region  Windows Form Designer generated code 

        public DesignSurface() : base()
        {
            gobjc = new MSDN.Samples.GraphicObjects.GraphicObjectCollection();

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            MouseDown += GraphicsSurface_MouseDown;
            MouseUp += GraphicsSurface_MouseUp;
            MouseMove += GraphicsSurface_MouseMove;
            KeyDown += DesignSurface_KeyDown;
            Load += DesignSurface_Load;
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
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        private ContextMenu _DefaultMenu;

        internal virtual ContextMenu DefaultMenu
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _DefaultMenu;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _DefaultMenu = value;
            }
        }
        private MenuItem _ds_mnuCut;

        internal virtual MenuItem ds_mnuCut
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ds_mnuCut;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ds_mnuCut = value;
            }
        }
        private MenuItem _ds_mnuCopy;

        internal virtual MenuItem ds_mnuCopy
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ds_mnuCopy;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ds_mnuCopy = value;
            }
        }
        private MenuItem _ds_mnuPaste;

        internal virtual MenuItem ds_mnuPaste
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ds_mnuPaste;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ds_mnuPaste = value;
            }
        }
        private MenuItem _ds_mmuBringToFront;

        internal virtual MenuItem ds_mmuBringToFront
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ds_mmuBringToFront;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ds_mmuBringToFront = value;
            }
        }
        private MenuItem _ds_mnuSendToBack;

        internal virtual MenuItem ds_mnuSendToBack
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ds_mnuSendToBack;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ds_mnuSendToBack = value;
            }
        }
        private PrintDialog _setupPrint;

        internal virtual PrintDialog setupPrint
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _setupPrint;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _setupPrint = value;
            }
        }
        private System.Drawing.Printing.PrintDocument _designSurfacePrintDocument;

        internal virtual System.Drawing.Printing.PrintDocument designSurfacePrintDocument
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _designSurfacePrintDocument;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _designSurfacePrintDocument = value;
            }
        }
        private FontDialog _fontChooser;

        internal virtual FontDialog fontChooser
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _fontChooser;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _fontChooser = value;
            }
        }
        private ColorDialog _colorPicker;

        internal virtual ColorDialog colorPicker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _colorPicker;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _colorPicker = value;
            }
        }
        private OpenFileDialog _ofdImage;

        internal virtual OpenFileDialog ofdImage
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ofdImage;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ofdImage = value;
            }
        }
        private SaveFileDialog _sfdImage;

        internal virtual SaveFileDialog sfdImage
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _sfdImage;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _sfdImage = value;
            }
        }
        private PrintPreviewDialog _ppdPreview;

        internal virtual PrintPreviewDialog ppdPreview
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ppdPreview;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ppdPreview = value;
            }
        }
        private MenuItem _ds_mnuEditSelected;

        internal virtual MenuItem ds_mnuEditSelected
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ds_mnuEditSelected;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _ds_mnuEditSelected = value;
            }
        }
        private PropertyGrid _PropertyGrid1;

        internal virtual PropertyGrid PropertyGrid1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _PropertyGrid1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _PropertyGrid1 = value;
            }
        }
        private PictureBox _pbCropped;

        internal virtual PictureBox pbCropped
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _pbCropped;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_pbCropped != null)
                {
                    _pbCropped.Click -= pbCropped_Click;
                }

                _pbCropped = value;
                if (_pbCropped != null)
                {
                    _pbCropped.Click += pbCropped_Click;
                }
            }
        }
        private int DRAG_HANDLE_SIZE = 7;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignSurface));
            _DefaultMenu = new ContextMenu();
            _ds_mnuCut = new MenuItem();
            _ds_mnuCopy = new MenuItem();
            _ds_mnuPaste = new MenuItem();
            _ds_mmuBringToFront = new MenuItem();
            _ds_mnuSendToBack = new MenuItem();
            _ds_mnuEditSelected = new MenuItem();
            _setupPrint = new PrintDialog();
            _designSurfacePrintDocument = new System.Drawing.Printing.PrintDocument();
            _fontChooser = new FontDialog();
            _colorPicker = new ColorDialog();
            _ofdImage = new OpenFileDialog();
            _sfdImage = new SaveFileDialog();
            _ppdPreview = new PrintPreviewDialog();
            _PropertyGrid1 = new PropertyGrid();
            _pbCropped = new PictureBox();
            _pbCropped.Click += new EventHandler(pbCropped_Click);
            ((System.ComponentModel.ISupportInitialize)_pbCropped).BeginInit();
            SuspendLayout();
            // 
            // DefaultMenu
            // 
            _DefaultMenu.MenuItems.AddRange(new MenuItem[] { _ds_mnuCut, _ds_mnuCopy, _ds_mnuPaste, _ds_mmuBringToFront, _ds_mnuSendToBack, _ds_mnuEditSelected });
            // 
            // ds_mnuCut
            // 
            _ds_mnuCut.Index = 0;
            _ds_mnuCut.Text = "&Cut";
            // 
            // ds_mnuCopy
            // 
            _ds_mnuCopy.Index = 1;
            _ds_mnuCopy.Text = "C&opy";
            // 
            // ds_mnuPaste
            // 
            _ds_mnuPaste.Index = 2;
            _ds_mnuPaste.Text = "&Paste";
            // 
            // ds_mmuBringToFront
            // 
            _ds_mmuBringToFront.Index = 3;
            _ds_mmuBringToFront.Text = "&BringToFront";
            // 
            // ds_mnuSendToBack
            // 
            _ds_mnuSendToBack.Index = 4;
            _ds_mnuSendToBack.Text = "&Send To Back";
            // 
            // ds_mnuEditSelected
            // 
            _ds_mnuEditSelected.Index = 5;
            _ds_mnuEditSelected.Text = "&Edit Selected Image";
            // 
            // setupPrint
            // 
            _setupPrint.Document = _designSurfacePrintDocument;
            // 
            // sfdImage
            // 
            _sfdImage.FileName = "Untitled";
            // 
            // ppdPreview
            // 
            _ppdPreview.AutoScrollMargin = new Size(0, 0);
            _ppdPreview.AutoScrollMinSize = new Size(0, 0);
            _ppdPreview.ClientSize = new Size(400, 300);
            _ppdPreview.Document = _designSurfacePrintDocument;
            _ppdPreview.Enabled = true;
            _ppdPreview.Icon = (Icon)resources.GetObject("ppdPreview.Icon");
            _ppdPreview.Name = "_ppdPreview";
            _ppdPreview.UseAntiAlias = true;
            _ppdPreview.Visible = false;
            // 
            // PropertyGrid1
            // 
            _PropertyGrid1.Dock = DockStyle.Right;
            _PropertyGrid1.Location = new Point(403, 0);
            _PropertyGrid1.Name = "_PropertyGrid1";
            _PropertyGrid1.Size = new Size(105, 620);
            _PropertyGrid1.TabIndex = 0;
            // 
            // pbCropped
            // 
            _pbCropped.BackColor = Color.Transparent;
            _pbCropped.Location = new Point(297, 44);
            _pbCropped.Name = "_pbCropped";
            _pbCropped.Size = new Size(100, 50);
            _pbCropped.TabIndex = 1;
            _pbCropped.TabStop = false;
            // 
            // DesignSurface
            // 
            AutoScroll = true;
            BackColor = SystemColors.Window;
            Controls.Add(_pbCropped);
            Controls.Add(_PropertyGrid1);
            Name = "DesignSurface";
            Size = new Size(508, 620);
            ((System.ComponentModel.ISupportInitialize)_pbCropped).EndInit();
            ResumeLayout(false);

        }

        #endregion

        #region Events

        public event SelectionChangedEventHandler SelectionChanged;

        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
        public event StatusUpdateEventHandler StatusUpdate;

        public delegate void StatusUpdateEventHandler(object sender, StatusUpdateEventArgs e);
        #endregion

        private const float MinimumGridSize = 20f;
        private Point dragOffset = new Point(0, 0);
        private bool selectionDragging = false;
        private bool dragging = false;
        private bool wasdragging = false;
        private bool rotating = false;
        private float startingRotation = 0f;
        private float originalRotation = 0f;
        private Rectangle selectionRect;
        private Rectangle m_SurfaceBounds = new Rectangle(0, 0, 850, 1100);
        private Rectangle m_SurfaceMargins = new Rectangle(10, 10, 840, 1090);
        private int m_HorizRes = 300;
        private int m_VertRes = 300;
        private bool m_ShowGrid = true;
        private bool m_ShowMargins = true;
        private Color m_NonPrintingAreaColor = Color.Gray;
        private Color m_GridColor = Color.LightBlue;
        private float m_GridSize = 50f;
        private int m_GridLineWidth = 1;
        private int m_MarginLineWidth = 1;
        private Color m_MarginColor = Color.Green;
        private MSDN.Samples.GraphicObjects.GraphicObjects m_SelectedObject, m_CutSelectedObject, m_CopySelectedObject, m_PasteSelectedObject, m_UndoSelectedObject, m_RedoSelectedObject, MouseDragStart;
        private float m_Zoom = 0.5f;
        private ContextMenu m_ContextMenu;
        private MSDN.Samples.GraphicObjects.GraphicObjectCollection m_drawingObjects = new MSDN.Samples.GraphicObjects.GraphicObjectCollection();
        private Image m_SelectedImage;
        private int m_RectStartX, m_RectStartY, m_RectEndX, m_RectEndY, m_ImageObjWidth, m_ImageObjHeight, m_Drag_Y, m_Drag_X;
        // Private ReadOnly RedoList As New List(Of GraphicObjects.GraphicObjects)
        // Private ReadOnly UndoList As New List(Of GraphicObjects.GraphicObjects)
        private int intObjectCount;
        private bool allObjectsDeleted = false;
        private string strImagePath;
        private bool blnLandscape, CanCutObject;
        private MSDN.Samples.GraphicObjects.GraphicObjects ClonedTextObject;
        private MSDN.Samples.GraphicObjects.GraphicObjects ClonedObject, CutObject;
        private MSDN.Samples.GraphicObjects.GraphicObjects CurrentGraphicImage;
        private Rectangle SelectRectangle = new Rectangle();
        private RectangleF StartlRectangle = new RectangleF();

        // declare different points
        private Point mMousePointDown;
        private Point mMousePointDownScrCo;
        private Point mMousePointMove;
        private Point mMousePointUp;

        // declare some coordinate variables
        private float mMousePointDeltaX;
        private int mPicBoxTopWhenMouseDown = 0;
        private int mPicBoxLeftWhenMouseDown = 0;
        private int mPicBoxTopWhenMouseDownSC = 0;
        private int mPicBoxLeftWhenMouseDownSC = 0;
        private int mPicBoxOldHeight;
        private int mPicBoxOldWidth;
        private bool MouseInRect = false;
        private bool RectDragging = false;
        private bool RectResizing = false;
        private int ActiveDrag = -1;
        public int RotationOffSet_X = 0;
        public int RotationOffSet_Y = 0;
        private Point MousePos;
        private SolidBrush BackBrush;
        private Pen[] LinePen = new Pen[2];
        private MSDN.Samples.GraphicObjects.GraphicObjectCollection _gobjc;

        // Runtime drag/hit-test state. It is a settable collection, so without these attributes the
        // WinForms designer tries to code-generate AddRange calls for it and fails on the
        // System.Drawing.Graphics members inside (not serializable).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual MSDN.Samples.GraphicObjects.GraphicObjectCollection gobjc
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _gobjc;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _gobjc = value;
            }
        }

        public Stack<MSDN.Samples.GraphicObjects.GraphicObjects> UndoStack = new Stack<MSDN.Samples.GraphicObjects.GraphicObjects>();
        public Stack<MSDN.Samples.GraphicObjects.GraphicObjects> RedoStack = new Stack<MSDN.Samples.GraphicObjects.GraphicObjects>();

        public DesignSurface(MSDN.Samples.GraphicObjects.GraphicObjects gobj)
        {
            gobjc = new MSDN.Samples.GraphicObjects.GraphicObjectCollection();
            MouseDown += GraphicsSurface_MouseDown;
            MouseUp += GraphicsSurface_MouseUp;
            MouseMove += GraphicsSurface_MouseMove;
            KeyDown += DesignSurface_KeyDown;
            Load += DesignSurface_Load;

        }

        #region Saving / Loading

        public virtual void LoadFromFile(string fileName)
        {
            // load objects from a binary file
            try
            {
                var myBinarySerializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                m_drawingObjects = (MSDN.Samples.GraphicObjects.GraphicObjectCollection)myBinarySerializer.Deserialize(new FileStream(fileName, FileMode.Open));
                SelectedObject = null;
                StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.FileLoaded, null, string.Format("file {0} loaded into the Graphic Surface", fileName), default, (float)default));
                Invalidate();
            }
            catch (Exception e)
            {
                throw new ApplicationException("File failed to load.", e);
            }

        }

        public virtual void SaveToFile(string fileName)
        {
            // save objects to a binary file
            try
            {
                string settingsPath = fileName;
                var myBinarySerializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                myBinarySerializer.Serialize(new FileStream(settingsPath, FileMode.Create), drawingObjects);
                StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.FileSaved, m_SelectedObject, string.Format("Graphic Surface saved to file {0}", fileName), default, (float)default));
            }
            catch (Exception e)
            {
                throw new ApplicationException("Save Failed.", e);
            }
        }

        // if undo = true pop it from undo and push it to redo and Vise Versa
        // Public Overridable Sub UnDo_Redo_Stack(ByVal Undo As Boolean, ByVal gobj As GraphicObjects.GraphicObjects)
        // If Undo = True Then
        // RedoStack.Push(gobj)
        // If UndoStack.Count > 0 Then
        // UndoStack.Pop()
        // End If
        // Else
        // UndoStack.Push(gobj)
        // If UndoStack.Count > 0 Then
        // RedoStack.Pop()
        // End If
        // End If
        // End Sub

        // USAGE
        // ConvertBMP("C:\test.bmp", ImageFormat.Jpeg)
        // ConvertBMP("C:\test.bmp", ImageFormat.Emf)
        // ConvertBMP("C:\test.bmp", ImageFormat.Exif)
        // ConvertBMP("C:\test.bmp", ImageFormat.Gif)
        // ConvertBMP("C:\test.bmp", ImageFormat.Icon)
        // ConvertBMP("C:\test.bmp", ImageFormat.MemoryBmp)
        // ConvertBMP("C:\test.bmp", ImageFormat.Png)
        // ConvertBMP("C:\test.bmp", ImageFormat.Tiff)
        // ConvertBMP("C:\test.bmp", ImageFormat.Wmf)
        public enum ImageSaveType
        {
            None,
            jpeg,
            png,
            gif,
            emf,
            exif,
            ico,
            bmp,
            wmf,
            tiff
        }

        public void SaveAsBitmap()
        {
            try
            {
                sfdImage.Filter = "Images|*.png;*.bmp;*.jpg;*.emf;*.gif;*.exif;*.tiff;*.wmf;*.ico";
                sfdImage.InitialDirectory = ((int)Environment.SpecialFolder.MyPictures).ToString();
                sfdImage.ShowDialog();
                if (string.IsNullOrEmpty(sfdImage.FileName))
                    return;

                if (drawingObjects.Count >= 1)
                {
                    Color MC, GC;
                    MC = MarginColor;
                    GC = GridColor;
                    // get the instance of the graphics from the control
                    var G = CreateGraphics();
                    MarginColor = BackColor;
                    GridColor = BackColor;
                    SelectedObject = null;
                    // Cycle through all drawrin objects to get min and max size using objects
                    int YMin = default, YMax = default, XMin = default, XMax = default, Count = default;
                    foreach (MSDN.Samples.GraphicObjects.GraphicObjects DrawnObject in drawingObjects)
                    {
                        if (Count == 0) // First Drawn Object Check for Min and Max
                        {
                            YMin = DrawnObject.Y;
                            YMax = DrawnObject.Y + DrawnObject.Height;
                            XMin = DrawnObject.X;
                            XMax = DrawnObject.X + DrawnObject.Width;
                            Count = 1;
                        }
                        else // Compare positions to obtain min max etc
                        {
                            if (DrawnObject.Y < YMin)
                            {
                                YMin = DrawnObject.Y;
                            }
                            if (DrawnObject.Y + DrawnObject.Height > YMax)
                            {
                                YMax = DrawnObject.Y + DrawnObject.Height;
                            }
                            if (DrawnObject.X < XMin)
                            {
                                XMin = DrawnObject.X;
                            }
                            if (DrawnObject.X + DrawnObject.Width > XMax)
                            {
                                XMax = DrawnObject.X + DrawnObject.Width;
                            }
                        }
                    }
                    // new bitmap object to save the image        Bitmap bmp = new Bitmap(control.Width, control.Height);
                    if (drawingObjects.Count > 1)
                    {

                    }
                    var BMP = new Bitmap(XMax - XMin - 10, YMax - YMin - 10);
                    // //Drawing control to the bitmap        control.DrawToBitmap(bmp, new Rectangle(0, 0, control.Width, control.Height));
                    DrawToBitmap(BMP, new Rectangle(0, 0, BMP.Width, BMP.Height));

                    var SaveImage = new Bitmap(BMP);
                    string fileName = sfdImage.FileName;
                    var fi = new FileInfo(fileName);
                    string extn = fi.Extension;

                    if (extn == ".emf")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Emf);
                    }

                    if (extn == ".bmp")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Bmp);
                    }

                    if (extn == ".exif")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Exif);
                    }

                    if (extn == ".gif")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Gif);
                    }

                    if (extn == ".ico")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Icon);
                    }

                    if (extn == ".jpg")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Jpeg);
                    }

                    if (extn == ".png")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Png);
                    }

                    if (extn == ".tiff")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Tiff);
                    }

                    if (extn == ".wmf")
                    {
                        SaveImage.Save(sfdImage.FileName, ImageFormat.Wmf);
                    }


                    MarginColor = MC;
                    GridColor = GC;

                    SaveImage.Dispose();

                }
            }

            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }


        }
        #endregion
        #region Mouse Rectangles For Drag Drop Operatios

        // Private Sub UndrawSelRectAndSetFlags()

        // ' on first entry in this block after pressing left mouse button
        // ' undraw the selection rectangle
        // If Not mblnResizingPicBoxFlag Then
        // Me.Invalidate()
        // End If

        // ' set flag that picture box is on resizing to avoid dragging
        // ' of the picture box
        // mblnResizingPicBoxFlag = True


        // ' set flag that cursor was moved
        // mPicBoxMouseMovedFlag = True

        // ' TODO - needed?
        // '' make sure that no drag happens when resizing is running
        // 'mblnDragOKFlag = False

        // End Sub

        // Private Function MouseIsOn_NW(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.NW_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.NW_P

        // MouseIsOn_NW = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then
        // MouseIsOn_NW = True
        // 'Me.Cursor = Cursors.PanNW
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'North Rectangle
        // Private Function mouseIsOn_N(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.N_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.N_P
        // mouseIsOn_N = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then

        // mouseIsOn_N = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'NE Rectangle
        // Private Function mouseIsOn_NE(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.NE_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.NE_P
        // mouseIsOn_NE = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then

        // 'Me.Cursor = Cursors.PanNE
        // mouseIsOn_NE = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'East
        // Private Function mouseIsOn_E(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.E_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.E_P
        // mouseIsOn_E = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then

        // mouseIsOn_E = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'SE
        // Private Function mouseIsOn_SE(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.SE_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.SE_P
        // mouseIsOn_SE = False

        // If MousePoint.X >= PT.X - 10 And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y - 10 And MousePoint.Y <= PT.Y + 10 Then

        // mouseIsOn_SE = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'South
        // Private Function mouseIsOn_S(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.S_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.S_P
        // mouseIsOn_S = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then

        // 'Me.Cursor = Cursors.PanNE
        // mouseIsOn_S = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'SW
        // Private Function mouseIsOn_SW(ByVal sender As Object, ByVal MousePoint As Point) As Boolean

        // Dim Rect As Rectangle = GraphicObjectCollection.SW_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.SW_P
        // mouseIsOn_SW = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then

        // Me.Cursor = Cursors.Hand
        // mouseIsOn_SW = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'West
        // Private Function mouseIsOn_W(ByVal sender As Object, ByVal MousePoint As Point) As Boolean
        // Dim Rect As Rectangle = GraphicObjectCollection.W_R

        // If SelectedObject Is Nothing Or Rect.IsEmpty Then
        // Exit Function
        // End If

        // Dim PT As New Point()
        // PT = GraphicObjectCollection.W_P
        // mouseIsOn_W = False

        // If MousePoint.X >= PT.X And MousePoint.X <= PT.X + 10 And MousePoint.Y >= PT.Y And MousePoint.Y <= PT.Y + 10 Then

        // mouseIsOn_W = True
        // Resizing_Flag = True
        // Else
        // Resizing_Flag = False
        // End If

        // End Function

        // 'XXXXXXXXX--Start Cursor Rectangles--XXXXXX
        // Private Function mouseIsOn_gobjc.DragRectangleFangle(ByVal MousePoint As Point) As Boolean

        // If SelectedObject Is Nothing Then
        // Exit Function
        // End If

        // mouseIsOn_gobjc.DragRectangleFangle = False
        // MousePoint = DeZoomPoint(MousePoint)

        // Dim P As Point = New Point(SelectedObject.X, SelectedObject.Y)
        // 'Debug.WriteLine("Mouse " & MousePoint.ToString)
        // 'Debug.WriteLine("Object Location " + SelectedObject.Location.ToString)
        // 'Debug.WriteLine("Drag Rectangle " + P.ToString)
        // 'Subtract 20 From Widht to allow for grabhandles
        // If MousePoint.X >= P.X And MousePoint.X <= SelectedObject.Width + SelectedObject.X - 20 And MousePoint.Y >= P.Y And MousePoint.Y <= SelectedObject.Height + SelectedObject.Y Then
        // mouseIsOn_gobjc.DragRectangleFangle = True
        // Drag_Flag = True
        // Else
        // Drag_Flag = False
        // End If

        // End Function

        // Private Function mouseIsOn_RotateRectangle(ByVal MousePoint As Point) As Boolean

        // If SelectedObject Is Nothing Then
        // Exit Function
        // End If

        // Dim P As Point = New Point(SelectedObject.X, SelectedObject.Y)
        // MousePoint = DeZoomPoint(MousePoint)

        // mouseIsOn_RotateRectangle = False

        // Debug.WriteLine("Mouse " & MousePoint.ToString)
        // Debug.WriteLine("Object Location " + SelectedObject.Location.ToString)

        // If MousePoint.X >= SelectedObject.Width + SelectedObject.X - 25 And MousePoint.X <= SelectedObject.Width + SelectedObject.X And MousePoint.Y >= P.Y And MousePoint.Y <= SelectedObject.Height + SelectedObject.Y Then

        // mouseIsOn_RotateRectangle = True
        // Rotate_Flag = True
        // Else
        // Rotate_Flag = False
        // End If

        // End Function

        #endregion
        #region Design Surface Proprties
        public object GrObjectChanged { get; set; }

        // Public Property LabelText() As String
        // Get
        // Return m_Text
        // End Get
        // Set(ByVal Value As String)
        // m_Text = Value
        // End Set
        // End Property

        public virtual MSDN.Samples.GraphicObjects.GraphicObjects CopyGraphicObject
        {
            get
            {
                return CurrentGraphicImage;
            }
            set
            {
                CurrentGraphicImage = value;
            }
        }

        public virtual MSDN.Samples.GraphicObjects.GraphicObjects PasteGraphicImage
        {
            get
            {
                return CurrentGraphicImage;
            }
            set
            {
                if (value is null)
                    return;
                // Me.AddObject(CurrentGraphicImage, CurrentGraphicImage.ImageType)
            }
        }

        public virtual ContextMenu ContxtMenu
        {
            get
            {
                return m_ContextMenu;
            }
            set
            {
                m_ContextMenu = value;
                Invalidate();
            }
        }

        public virtual Rectangle SurfaceBounds
        {
            // bounds in 1/100ths of an inch, just like the printer objects use
            get
            {
                return m_SurfaceBounds;
            }
            set
            {
                m_SurfaceBounds = value;
                Invalidate();
            }
        }

        public virtual Rectangle SurfaceMargins
        {
            // bounds in 1/100ths of an inch, just like the printer objects use
            get
            {
                return m_SurfaceMargins;
            }
            set
            {
                m_SurfaceMargins = value;
                Invalidate();
            }
        }

        public float Zoom
        {
            get
            {
                return m_Zoom;
            }
            set
            {
                if ((double)value > 0.05d)
                {
                    m_Zoom = value;
                }
                else
                {
                    m_Zoom = 0.05f;
                }
                Invalidate();
            }
        }

        public virtual MSDN.Samples.GraphicObjects.GraphicObjectCollection drawingObjects
        {
            get
            {
                // since this is a reference type, users can add/remove/manipulate its contents
                // even though it is read only. they can't change m_drawingobjects itself though
                // ie. graphicssurface1.drawingobjects = Nothing would fail.
                return m_drawingObjects;
            }
        }

        public virtual Color MarginColor
        {
            get
            {
                return m_MarginColor;
            }
            set
            {
                m_MarginColor = value;
                Invalidate();
            }
        }

        public virtual int MarginLineWidth
        {
            get
            {
                return m_MarginLineWidth;
            }
            set
            {
                if (value > 0)
                {
                    m_MarginLineWidth = value;
                    Invalidate();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MarginLineWidth", "MarginLineWidth must be greater than zero");
                }
            }
        }

        public virtual bool GridShown
        {
            get
            {
                return m_ShowGrid;
            }
            set
            {
                m_ShowGrid = false;
                Invalidate();
            }
        }

        public virtual bool GridMarginsShown
        {
            get
            {
                return m_ShowMargins;
            }
            set
            {
                m_ShowMargins = false;
                Invalidate();
            }
        }

        public virtual Color GridColor
        {
            get
            {
                return m_GridColor;
            }
            set
            {
                m_GridColor = value;
                Invalidate();
            }
        }

        public virtual Color NonPrintingAreaColor
        {
            get
            {
                return m_NonPrintingAreaColor;
            }
            set
            {
                m_NonPrintingAreaColor = value;
            }
        }

        public virtual int GridLineWidth
        {
            get
            {
                return m_GridLineWidth;
            }
            set
            {
                if (value > 0)
                {
                    m_GridLineWidth = value;
                    Invalidate();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("GridLineWidth", "GridLineWidth must be greater than zero");
                }
            }
        }

        public virtual float GridSize
        {
            // in 100ths of an inch
            get
            {
                return m_GridSize;
            }
            set
            {
                if (value > MinimumGridSize)
                {
                    m_GridSize = value;
                    Invalidate();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("GridSize", string.Format("Grid Size must be greater than {0}", MinimumGridSize));
                }
            }
        }

        public virtual int ImageObjHeight
        {
            get
            {
                return m_ImageObjHeight;
            }
            set
            {
                if (SelectedObject is not null)
                {
                    m_ImageObjHeight = value;
                    Invalidate();
                }

                // GraphicObject.Height = m_ImageObjHeight
            }
        }

        public virtual int ImageObjWidth
        {
            get
            {
                return m_ImageObjWidth;
            }
            set
            {
                m_ImageObjWidth = value;
                Invalidate();
            }
        }

        #endregion

        protected float ConvertToHPixels(float value)
        {
            return value * m_HorizRes;
        }

        protected float ConvertToVPixels(float value)
        {
            return value * m_VertRes;
        }

        protected Rectangle ConvertToPixels(Rectangle rect)
        {
            // convert from 100ths of an inch to pixels
            Rectangle Bounds;
            Bounds = new Rectangle((int)Round(ConvertToHPixels((float)(rect.X / 100d))), (int)Round(ConvertToVPixels((float)(rect.Y / 100d))), (int)Round(ConvertToHPixels((float)(rect.Width / 100d))), (int)Round(ConvertToVPixels((float)(rect.Height / 100d))));
            return Bounds;
        }

        protected int ZoomInteger(int @int)
        {
            int MyZoom = (int)Round(@int * Zoom);
            return MyZoom;
        }

        protected Point ZoomPoint(Point PT)
        {
            var MyZoom = new Point((int)Round(PT.X * Zoom), (int)Round(PT.Y * Zoom));
            return MyZoom;
        }

        protected Size ZoomSize(Size SZ)
        {
            var MyZoom = new Size((int)Round(SZ.Width * Zoom), (int)Round(SZ.Width * Zoom));
            return MyZoom;
        }

        protected Size DeZoomSize(Size SZ)
        {
            var MyZoom = new Size((int)Round(SZ.Width / Zoom), (int)Round(SZ.Width / Zoom));
            return MyZoom;
        }

        protected Point DeZoomPoint(Point PT)
        {
            var MyZoom = new Point((int)Round(PT.X / Zoom), (int)Round(PT.Y / Zoom));
            return MyZoom;
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

        protected virtual void DrawGrid(Graphics g)
        {
            Rectangle bounds;
            int horizGridSize = (int)Round(ConvertToHPixels(GridSize / 100f) * Zoom);
            int vertGridSize = (int)Round(ConvertToVPixels(GridSize / 100f) * Zoom);
            bounds = ConvertToPixels(SurfaceBounds);
            bounds = ZoomRectangle(bounds);
            if (AutoScrollMinSize.Height != bounds.Height && AutoScrollMinSize.Width != bounds.Width)
            {
                AutoScrollMinSize = new Size(bounds.Width, bounds.Height);
            }

            g.Clear(m_NonPrintingAreaColor);
            g.FillRectangle(new SolidBrush(BackColor), bounds);

            var gridPen = new Pen(GridColor, m_GridLineWidth);
            int i;
            var loopTo = bounds.Height;
            for (i = vertGridSize; vertGridSize >= 0 ? i <= loopTo : i >= loopTo; i += vertGridSize)
                g.DrawLine(gridPen, 0, i, bounds.Width, i);
            var loopTo1 = bounds.Width;
            for (i = horizGridSize; horizGridSize >= 0 ? i <= loopTo1 : i >= loopTo1; i += horizGridSize)
                g.DrawLine(gridPen, i, 0, i, bounds.Height);

        }

        protected virtual void DrawMargins(Graphics g)
        {
            var margins = ZoomRectangle(ConvertToPixels(m_SurfaceMargins));
            var marginPen = new Pen(m_MarginColor);
            marginPen.DashStyle = DashStyle.Dash;
            marginPen.Width = m_MarginLineWidth;
            g.DrawRectangle(marginPen, margins);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {

                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // get the dpi settings of the graphics context,
                // for example; 96dpi on screen, 600dpi for the printer
                // used to adjust grid and margin sizing.
                m_HorizRes = (int)Round(g.DpiX);
                m_VertRes = (int)Round(g.DpiY);

                // handle the possibility that the viewport is scrolled,
                // adjust my origin coordintates to compensate
                var pt = AutoScrollPosition;
                g.TranslateTransform(pt.X, pt.Y);

                if (m_ShowGrid)
                {
                    DrawGrid(g);
                }


                // draw the actual objects onto the page, on top of the grid
                {
                    var withBlock = drawingObjects;
                    // pass the graphics resolution onto the objects
                    // so that images and other objects can be sized
                    // correct taking the dpi into consideration.
                    withBlock.HorizontalResolution = (int)Round(g.DpiX);
                    withBlock.VerticalResolution = (int)Round(g.DpiY);
                    withBlock.DrawObjects(g, Zoom);
                    // doesn't really draw the selected object, but instead the
                    // selection indicator, a dotted outline around the selected object
                    withBlock.DrawSelectedObject(g, m_SelectedObject, Zoom);
                }

                // Draw dashed line margin indicators, over top of objects
                if (GridMarginsShown)
                {
                    DrawMargins(g);
                }

                if (SelectedObject is null)
                    return;
                gobjc.DragRectangleF = new RectangleF(SelectedObject.X, SelectedObject.Y, SelectedObject.Width, SelectedObject.Height);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new ApplicationException("Error Drawing Graphics Surface", ex);
            }

        }

        private Point gscTogoc(Point gsPT)
        {
            var myNewPoint = default(Point);
            myNewPoint.X = (int)Round((gsPT.X - AutoScrollPosition.X) / Zoom);
            myNewPoint.Y = (int)Round((gsPT.Y - AutoScrollPosition.Y) / Zoom);
            return myNewPoint;
        }

        private Point gscTogoc(int X, int Y)
        {
            var myNewPoint = default(Point);
            myNewPoint.X = (int)Round((X - AutoScrollPosition.X) / Zoom);
            myNewPoint.Y = (int)Round((Y - AutoScrollPosition.Y) / Zoom);
            return myNewPoint;
        }

        private Point gocTogsc(Point goPT)
        {
            // note that there is no need for a New for my Point
            // before I can assign values to X and Y
            // as it is a structure not an object
            var myNewPoint = default(Point);
            myNewPoint.X = (int)Round(goPT.X * Zoom);
            myNewPoint.Y = (int)Round(goPT.Y * Zoom);

            return myNewPoint;
        }

        private Point gocTogsc(int X, int Y)
        {
            var myNewPoint = default(Point);
            myNewPoint.X = (int)Round(X * Zoom);
            myNewPoint.Y = (int)Round(Y * Zoom);
            return myNewPoint;
        }

        private void SetDragCursor()
        {
            if (ActiveDrag != -1)
            {
                rotating = false;
                switch (ActiveDrag)
                {
                    case 0:
                        {
                            Cursor = Cursors.SizeNWSE;
                            break;
                        }
                    case 1:
                        {
                            Cursor = Cursors.SizeNS;
                            break;
                        }
                    case 2:
                        {
                            Cursor = Cursors.SizeNESW;
                            break;
                        }
                    case 3:
                        {
                            Cursor = Cursors.SizeWE;
                            break;
                        }
                    case 4:
                        {
                            Cursor = Cursors.SizeNWSE;
                            break;
                        }
                    case 5:
                        {
                            Cursor = Cursors.SizeNS;
                            break;
                        }
                    case 6:
                        {
                            Cursor = Cursors.SizeNESW;
                            break;
                        }
                    case 7:
                        {
                            Cursor = Cursors.SizeWE;
                            break;
                        }
                    case 8:
                        {
                            var img = new Bitmap(My.Resources.Resources.RotateHand);
                            img.MakeTransparent(Color.White);
                            Cursor = CustomCursor.CreateCursor(img, 8, 8);
                            rotating = true;
                            break;
                        }
                    case 9:
                        {
                            var img = new Bitmap(My.Resources.Resources.RotateHand);
                            img.MakeTransparent(Color.White);
                            Cursor = CustomCursor.CreateCursor(img, 8, 8);
                            rotating = true;
                            break;
                        }

                    default:
                        {
                            Cursor = Cursors.Default;
                            break;
                        }
                }
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        public Image RotateImage(Image TheImage, float Rotation)
        {
            Image RotatedBM;
            // = DeZoomRectangle(TheImage.X, TheImage.Y, TheImage.Width, TheImage.Height)

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
            float theta = (float)((double)float.Parse(Rotation.ToString()) * PI / -180.0d);
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

        private void CenterAndZoomAndRotatedText(Graphics gr, string txt, Point pos, Size Size, float Rotatation) // ByVal x As Single, ByVal y As Single)
        {
            var displayFontFam = new FontFamily("Rockwell Extra Bold");
            var displayFont = new Font(displayFontFam, 36f, FontStyle.Bold, GraphicsUnit.Pixel);

            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            gr.CompositingQuality = CompositingQuality.HighQuality;

            // Zoom += 1
            // If Zoom = 0 Then Zoom = 0.01
            int wp = (int)Round(Size.Width / 2d);
            int hp = (int)Round(Size.Height / 2d);
            var rectF = new RectangleF(pos.X - wp, pos.Y - hp, Size.Width, Size.Height);

            var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            var StringSize = new SizeF();
            StringSize = gr.MeasureString(txt, displayFont);
            int méret = (int)Round(StringSize.Width);
            var br = new SolidBrush(Color.FromArgb(30, SystemColors.Highlight));
            gr.ResetTransform();
            gr.TranslateTransform(-pos.X, -pos.Y, MatrixOrder.Append);
            gr.ScaleTransform(Zoom, Zoom, MatrixOrder.Append);
            gr.RotateTransform(Rotatation, MatrixOrder.Append);
            gr.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);
            gr.FillRectangle(br, rectF);

            gr.DrawRectangle(new Pen(Color.Blue, 2f), new Rectangle((int)Round(rectF.X), (int)Round(rectF.Y), (int)Round(rectF.Width), (int)Round(rectF.Height)));

            gr.DrawString(txt, displayFont, Brushes.Blue, rectF, sf);
            gr.ResetTransform();

            gr.DrawLine(Pens.Red, pos.X - 10, pos.Y, pos.X + 10, pos.Y);
            gr.DrawLine(Pens.Red, pos.X, pos.Y - 10, pos.X, pos.Y + 10);

            sf.Dispose();
            displayFontFam.Dispose();
            displayFont.Dispose();
        }

        private void GraphicsSurface_MouseDown(object sender, MouseEventArgs e)
        {

            // If e.Button And MouseButtons.Right Then
            // ContextMenu = Me.ContxtMenu
            // 
            // originalRotation = m_SelectedObject.Rotation
            // wasdragging = False
            // End If

            if (e.Button == MouseButtons.Left)
            {

                // Dim rectf As Rectangle = Rectangle.Round(gobjc.DragRectangleFangle)
                gobjc.DragRectangleF = new RectangleF(SelectedObject.X, SelectedObject.Y, SelectedObject.Width, SelectedObject.Height);
                StartlRectangle = gobjc.DragRectangleF;
                SelectedObject.AutoSize = false;
                MousePos = new Point(e.X, e.Y);
                MousePos = PointToScreen(MousePos);

                if (MouseInRect & ActiveDrag == -1)
                {
                    RectDragging = true;
                }
                // OriginalRectangle = gobjc.DragRectangleFangle
                else if (ActiveDrag != -1)
                {
                    RectResizing = true;
                }
                else if (ActiveDrag > 7)
                {
                    rotating = true;
                }
            }
        }

        private void GraphicsSurface_MouseUp(object sender, MouseEventArgs e)
        {

            if (rotating == true)
            {
                rotating = false;
                // SelectedObject = SelectedObject
                // SelectedObject.Image = gobjc.RotatedBitmap
                Refresh();
                Invalidate();
                gobjc.LoadDrag_Handles();
            }

            if (e.Button == MouseButtons.Left)
            {
                RectDragging = false;
                RectResizing = false;
                Invalidate();
                gobjc.LoadDrag_Handles();
                PropertyGrid1.Refresh();
            }
        }

        private void GraphicsSurface_MouseMove(object sender, MouseEventArgs e)
        {

            var CurrMouse = new Point(e.X, e.Y);

            CurrMouse.X = Max(0, Min(CurrMouse.X, ClientSize.Width));
            CurrMouse.Y = Max(0, Min(CurrMouse.Y, ClientSize.Height));

            CurrMouse = PointToScreen(CurrMouse);

            if (e.Button == MouseButtons.Left)
            {

                var dragPoint = gscTogoc(e.X, e.Y);

                if (rotating)
                {

                    float currentRotation;

                    currentRotation = AngleToPoint(m_SelectedObject.GetPosition(), dragPoint);

                    currentRotation = (int)Round((currentRotation - startingRotation + originalRotation) % 360f);

                    m_SelectedObject.Rotation = currentRotation;

                    StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.ObjectRotated, SelectedObject, string.Format("Object Rotated to {0} degrees", currentRotation), default, currentRotation));

                    Invalidate();
                    // Return
                }

                if (RectDragging)
                {

                    if (Abs(CurrMouse.X - MousePos.X) > 2 | Abs(CurrMouse.Y - MousePos.Y) > 2)
                    {
                        gobjc.DragRectangleF.Offset(CurrMouse.X - MousePos.X, CurrMouse.Y - MousePos.Y);

                        gobjc.DragRectangleF.X = Max(0f, Min(gobjc.DragRectangleF.X, ClientSize.Width - gobjc.DragRectangleF.Width));
                        gobjc.DragRectangleF.Y = Max(0f, Min(gobjc.DragRectangleF.Y, ClientSize.Height - gobjc.DragRectangleF.Height));

                        MousePos = new Point(e.X, e.Y);

                        MousePos.X = Max(0, Min(MousePos.X, ClientSize.Width));
                        MousePos.Y = Max(0, Min(MousePos.Y, ClientSize.Height));

                        SelectedObject.X = MousePos.X;
                        SelectedObject.Y = MousePos.Y;

                        MousePos = PointToScreen(MousePos);

                        Invalidate();

                        StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.ObjectRotated, SelectedObject, string.Format("Object Dragged", null), MousePos, (float)default));
                    }
                }

                else if (RectResizing)
                {

                    if (ActiveDrag != -1)
                    {

                        if (Abs(CurrMouse.X - MousePos.X) > 2 | Abs(CurrMouse.Y - MousePos.Y) > 2)
                        {

                            switch (ActiveDrag)
                            {
                                case 0: // NW
                                    {

                                        SelectedObject.AutoSize = false;

                                        StartlRectangle.Height -= CurrMouse.Y - MousePos.Y;

                                        if (!(gobjc.DragRectangleF.Height <= 20f))
                                        {
                                            gobjc.DragRectangleF.Y -= StartlRectangle.Y;
                                            SelectedObject.Y = (int)Round(StartlRectangle.Y);
                                            SelectedObject.SetPosition(new Point((int)Round(StartlRectangle.X), (int)Round(StartlRectangle.Y)));
                                            SelectedObject.Height = (int)Round(StartlRectangle.Height - SelectedObject.Y);
                                        }

                                        StartlRectangle.Width -= CurrMouse.X - MousePos.X;

                                        if (!(StartlRectangle.Width < 20f))
                                        {
                                            gobjc.DragRectangleF.X -= -CurrMouse.X + MousePos.X;
                                            gobjc.DragRectangleF.Width -= -CurrMouse.Y + MousePos.Y;
                                            SelectedObject.X = (int)Round(StartlRectangle.X);
                                            SelectedObject.SetPosition(new Point((int)Round(StartlRectangle.X), (int)Round(StartlRectangle.Y)));
                                            SelectedObject.Width = (int)Round(StartlRectangle.Width - SelectedObject.X);
                                        }
                                        else
                                        {
                                            gobjc.DragRectangleF.Width = 20f;
                                        }

                                        break;
                                    }

                                case 1: // North
                                    {

                                        SelectedObject.AutoSize = false;

                                        StartlRectangle.Height -= CurrMouse.Y - MousePos.Y;

                                        if (!(gobjc.DragRectangleF.Height < 20f))
                                        {
                                            gobjc.DragRectangleF.Y -= StartlRectangle.Y;
                                            SelectedObject.Y = (int)Round(StartlRectangle.Y);
                                            SelectedObject.Height = (int)Round(StartlRectangle.Height);
                                        }

                                        break;
                                    }

                                case 2: // NE
                                    {

                                        SelectedObject.AutoSize = false;

                                        StartlRectangle.Height -= CurrMouse.Y - MousePos.Y;
                                        StartlRectangle.Width -= CurrMouse.Y - MousePos.Y;

                                        if (!(StartlRectangle.Height < 20f))
                                        {
                                            gobjc.DragRectangleF.Y = StartlRectangle.Y;
                                            SelectedObject.Y = (int)Round(gobjc.DragRectangleF.Y);
                                            SelectedObject.Height = (int)Round(StartlRectangle.Height);
                                        }


                                        if (!(StartlRectangle.Width <= 20f))
                                        {
                                            gobjc.DragRectangleF.X = StartlRectangle.X;
                                            SelectedObject.X = (int)Round(StartlRectangle.X);
                                            SelectedObject.Width = (int)Round(StartlRectangle.Width);
                                        }

                                        break;
                                    }

                                case 3: // East
                                    {

                                        SelectedObject.AutoSize = false;

                                        StartlRectangle.Width += CurrMouse.X - MousePos.X;

                                        if (StartlRectangle.Width >= 20f)
                                        {
                                            gobjc.DragRectangleF.Width = StartlRectangle.Width;
                                            SelectedObject.Width = (int)Round(StartlRectangle.Width);
                                        }

                                        break;
                                    }

                                case 4: // SE
                                    {

                                        SelectedObject.AutoSize = false;

                                        StartlRectangle.Height += CurrMouse.Y - MousePos.Y;
                                        StartlRectangle.Width += CurrMouse.Y - MousePos.Y;

                                        if (!(StartlRectangle.Height < 20f))
                                        {
                                            gobjc.DragRectangleF.Y = StartlRectangle.Y;
                                            SelectedObject.Y = (int)Round(gobjc.DragRectangleF.Y);
                                            SelectedObject.Height = (int)Round(StartlRectangle.Height);
                                        }


                                        if (!(StartlRectangle.Width <= 20f))
                                        {
                                            gobjc.DragRectangleF.X = StartlRectangle.X;
                                            SelectedObject.X = (int)Round(StartlRectangle.X);
                                            SelectedObject.Width = (int)Round(StartlRectangle.Width);
                                        }

                                        break;
                                    }

                                case 5: // South
                                    {

                                        SelectedObject.AutoSize = false;

                                        if (gobjc.DragRectangleF.Height >= 20f)
                                        {
                                            gobjc.DragRectangleF.Height += CurrMouse.Y - MousePos.Y;
                                            gobjc.DragRectangleF.Height = Max(20f, gobjc.DragRectangleF.Height);
                                            SelectedObject.Height = (int)Round(gobjc.DragRectangleF.Height);
                                        }

                                        Refresh();
                                        break;
                                    }

                                case 6: // SW
                                    {

                                        SelectedObject.AutoSize = false;

                                        StartlRectangle.Height += CurrMouse.Y - MousePos.Y;
                                        StartlRectangle.Width += CurrMouse.X - MousePos.X;

                                        if (!(StartlRectangle.Height < 20f))
                                        {
                                            gobjc.DragRectangleF.Y = StartlRectangle.Y;
                                            SelectedObject.Y = (int)Round(gobjc.DragRectangleF.Y);
                                            SelectedObject.Height = (int)Round(StartlRectangle.Height);
                                        }


                                        if (!(StartlRectangle.Width <= 20f))
                                        {
                                            gobjc.DragRectangleF.X = StartlRectangle.X;
                                            SelectedObject.X = (int)Round(StartlRectangle.X);
                                            SelectedObject.Width = (int)Round(StartlRectangle.Width);
                                        }

                                        break;
                                    }

                                case 7: // West
                                    {

                                        SelectedObject.AutoSize = false;

                                        gobjc.DragRectangleF.Width -= CurrMouse.X - MousePos.X;

                                        if (!(gobjc.DragRectangleF.Width <= 20f))
                                        {
                                            gobjc.DragRectangleF.X -= -CurrMouse.X + MousePos.X;
                                            SelectedObject.X = (int)Round(gobjc.DragRectangleF.X);
                                            SelectedObject.Width = (int)Round(gobjc.DragRectangleF.Width);
                                        }

                                        Refresh();
                                        break;
                                    }

                            }

                            MousePos = new Point(e.X, e.Y);

                            MousePos.X = Max(0, Min(MousePos.X, ClientSize.Width));
                            MousePos.Y = Max(0, Min(MousePos.Y, ClientSize.Height));

                            MousePos = PointToScreen(MousePos);
                            Invalidate();
                            StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.ObjectRotated, SelectedObject, string.Format("Object Resized", null), MousePos, (float)default));
                        }
                    }
                }
            }

            else
            {

                if (SelectedObject is null)
                    return;

                MouseInRect = false;
                ActiveDrag = -1;
                SelectedObject.MousePosition = MousePos;
                PropertyGrid1.SelectedObject = SelectedObject;
                PropertyGrid1.Refresh();
                gobjc.LoadDrag_Handles();
                int i = 0;
                var R = new RectangleF();
                gobjc.LoadDrag_Handles();
                // Dim OffSetX As Single = 0
                // Dim OffSetY As Single = 0

                foreach (var tmprect in gobjc.Drag_Handles)
                {
                    if (tmprect.Contains(e.X / Zoom, e.Y / Zoom))
                    {
                        MouseInRect = true;
                        ActiveDrag = i;
                        SetDragCursor();
                        Invalidate();
                        return;
                    }
                    i += 1;
                }

                if (gobjc.DragRectangleF.Contains(e.X / Zoom, e.Y / Zoom))
                {
                    MouseInRect = true;
                    Cursor = Cursors.Hand;
                    Invalidate();
                    return;
                }

                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta != 0) // has wheel been moved?
            {
                // The .NET docs suggest that e.Delta returns the actual number of notches
                // the mouse wheel has been rotated, but in actuality each roll of the mouse wheel
                // returns a value of +/- 120 (depending on the direction of rotation).
                // 120 is actually a system constant, and therefore there is a possibility that it could
                // change to allow for higher-resolution mouse wheels.

                // check out <http://msdn.microsoft.com/library/en-us/winui/winui/windowsuserinterface/userinput/mouseinput/mouseinputreference/mouseinputmessages/wm_mousewheel.asp>
                // for more information
                if (SelectedObject is null)
                {
                    int detents = Abs(e.Delta / 120);
                    int i;
                    bool zoomIn = e.Delta < 0;
                    int zoomPercentage = (int)Round(Zoom * 100f);
                    var loopTo = detents;
                    for (i = 1; i <= loopTo; i++)
                    {
                        if (zoomPercentage <= 100)
                        {
                            if (zoomIn)
                            {
                                zoomPercentage = zoomPercentage / 2;
                            }
                            else
                            {
                                zoomPercentage = zoomPercentage * 2;
                            }
                        }
                        else if (zoomIn)
                        {
                            zoomPercentage = zoomPercentage - 50;
                        }
                        else
                        {
                            zoomPercentage = zoomPercentage + 50;
                        }
                    }
                    Zoom = (float)(zoomPercentage / 100d);
                    StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.SurfaceZoomChanged, SelectedObject, string.Format("Zoom set to {0}", Zoom * 100f), default, Zoom));
                    Invalidate();
                }

                MSDN.Samples.GraphicObjects.GraphicObjectCollection.Zoom = Zoom;
                // Dim IG As GraphicObjects.ImageGraphic
                // IG.Draw(Nothing)
            }

        }

        // TODO: Rewrite to handle multiple selected objects
        public MSDN.Samples.GraphicObjects.GraphicObjects SelectedObject
        {
            get
            {
                return m_SelectedObject;
            }
            set
            {
                if (!ReferenceEquals(value, m_SelectedObject))
                {
                    if (m_drawingObjects.Contains(value) || value is null)
                    {
                        m_SelectedObject = value;
                        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(value));
                        if (value is null)
                        {
                            StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.SelectionChanged, value, "No Object Selected", default, 0f));
                        }
                        else
                        {
                            StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(StatusUpdateType.SelectionChanged, value, "Selected Object Changed", value.GetPosition(), 0f));
                        }
                        Invalidate();
                    }
                }
            }
        }

        public void DeleteSelectedObject()
        {
            if (SelectedObject is null)
                return;
            var objectToDelete = SelectedObject;
            var gobj = SelectedObject;
            if (objectToDelete is not null)
            {
                if (drawingObjects.Contains(objectToDelete))
                {
                    // UndoList.Remove(gobj)
                    gobj.objType = ObjectTypes.DeletedObject;
                    // RedoList.Add(gobj)
                    drawingObjects.Remove(objectToDelete);
                    SelectedObject = null;
                }
            }
            intObjectCount -= 1;
            // Me.lblUndo.Text = "Redo = " & RedoList.Count.ToString
            // Me.lblUndo.Text = "Undo = " & UndoList.Count.ToString
            // MsgBox("DeleteSelectedObject Undo = " & UndoList.Count & Environment.NewLine & "Redodo = " & RedoList.Count)
        }

        public void DeleteAllObjects()
        {
            // add all deleted objects to list so it can be undone
            foreach (MSDN.Samples.GraphicObjects.GraphicObjects gobj in drawingObjects)
                // Me.RedoList.Add(gobj)
                gobj.objType = ObjectTypes.DeletedObject;
            // UndoList.Clear()
            drawingObjects.Clear();
            intObjectCount = 0;
            allObjectsDeleted = true;
            // Me.lblUndo.Text = "Redo = " & RedoList.Count.ToString
            // Me.lblUndo.Text = "Undo = " & UndoList.Count.ToString
            Invalidate();
        }

        private float AngleToPoint(Point Origin, Point Target)
        {
            // a cool little utility function, 
            // given two points finds the angle between them....
            // forced me to recall my highschool math, 
            // but the task is made easier by a special overload to
            // Atan that takes X,Y co-ordinates.
            float Angle;
            Target.X = Target.X - Origin.X;
            Target.Y = Target.Y - Origin.Y;
            Angle = (float)(Atan2(Target.Y, Target.X) / (PI / 180d));
            return Angle;
        }

        private void DesignSurface_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Delete:
                    {
                        if (SelectedObject is not null)
                        {
                            DeleteSelectedObject();
                        }

                        break;
                    }
            }
        }

        // This is where most of the action takes place any item added to design surface must 
        // be done through this sub procedure 
        // If Path is known set image path 
        // If image has no path set theImage
        public void AddObject(ObjectTypes ObjectType, string ImagePath = "", Image TheImage = null)
        {

            Cursor = Cursors.WaitCursor;
            // Create a new graphic object
            MSDN.Samples.GraphicObjects.GraphicObjects gObj = null;
            // Add 1 To image count to track images
            intObjectCount += 1;
            Bitmap BMImage = (Bitmap)TheImage;
            BMImage.MakeTransparent();
            pbCropped.Width = BMImage.Width;
            pbCropped.Height = BMImage.Height;
            var ReduceBMImageSize = new Bitmap(TheImage, ZoomSize(TheImage.Size));

            pbCropped.Image = ReduceBMImageSize;


            switch (ObjectType)
            {

                case ObjectTypes.Linked:
                    {

                        var myLinkedImage = new MSDN.Samples.GraphicObjects.LinkedImageGraphic(50, 50, strImagePath);
                        BMImage = new Bitmap(strImagePath);
                        gObj.Image = BMImage;
                        gObj = myLinkedImage;
                        gObj.AutoSize = true;
                        break;
                    }

                case ObjectTypes.Text:
                    {

                        var MyNewImage = new MSDN.Samples.GraphicObjects.TextGraphic(new Point(50, 50), TheImage);
                        gObj = MyNewImage;
                        gObj.AutoSize = true;
                        gObj.Image = TheImage;
                        break;
                    }
                // Dim myTextObject As New TextGraphic(10, 10, Text, Font, ForeColor)
                // gObj = myTextObject
                // gObj.AutoSize = True

                case ObjectTypes.EmbeddedImageGraphic: // Allows images without paths
                    {

                        var MyNewImage = new MSDN.Samples.GraphicObjects.EmbeddedImageGraphic(new Point(50, 50), TheImage);
                        gObj = MyNewImage;
                        gObj.AutoSize = true;
                        gObj.Image = TheImage;
                        break;
                    }

                case ObjectTypes.ClonedObject:
                    {
                        break;
                    }
                // gObj = ClonedObject

                case ObjectTypes.Fill: // Allow the background to be colored
                    {

                        try
                        {
                            var myRectangle = new MSDN.Samples.GraphicObjects.RectangleGraphic(SurfaceMargins.X, SurfaceMargins.Y, SurfaceMargins.Width, SurfaceMargins.Height);
                            myRectangle.Fill = true;
                            colorPicker.ShowDialog();
                            myRectangle.FillColor = colorPicker.Color;
                            gObj = myRectangle;
                        }

                        catch (Exception ex)
                        {

                        }

                        break;
                    }

                case ObjectTypes.Background: // Allow the background to have an image fit to Surface
                    {

                        try
                        {
                            if (TheImage is null)
                            {

                                ofdImage.ShowDialog();

                                if (ofdImage.FileName is not null)
                                {

                                    strImagePath = ofdImage.FileName;
                                    var myBackgroundImage = new MSDN.Samples.GraphicObjects.BackGroundImageGraphic(0, 0, strImagePath);
                                    gObj = myBackgroundImage;

                                    // This sets the new image Size
                                    gObj.Height = SurfaceBounds.Height;
                                    gObj.Width = SurfaceBounds.Width;
                                    gObj.AutoSize = false;

                                }
                            }
                            else
                            {
                                BackgroundImage = TheImage;
                            }
                        }

                        catch (Exception ex)
                        {

                        }

                        break;
                    }

                default:
                    {
                        break;
                    }

            }

            if (gObj is not null)
            {
                // Add the object to the designer
                drawingObjects.Add(gObj);
                Update();
                SelectedObject = gObj;
                SelectedObject.objType = ObjectType;
                SelectedObject.ImageName = ObjectType.ToString() + intObjectCount.ToString();
                SelectedObject.ImageNumber = intObjectCount;
                SelectedObject.ImagePath = strImagePath;
                PropertyGrid1.SelectedObject = SelectedObject;
                gobjc.DragRectangleF = new RectangleF(SelectedObject.X, SelectedObject.Y, SelectedObject.Width, SelectedObject.Height);

                // Add The object to the Undo List
                Undo();
                // listUndo.Items.Add(gObj)
                Refresh();
            }

            // ClonedObject = Nothing

            if (intObjectCount > 0)
            {
                CanCutObject = true;
            }
            else
            {
                CanCutObject = false;
            }
            // PropertyGrid1.SelectedObject = dsSurface.SelectedObject
            Cursor = Cursors.Default;
        }

        public void AddImageObject()
        {
            try
            {

                // Call the openfile dialog and get information
                {
                    var withBlock = ofdImage;
                    withBlock.CheckFileExists = true;
                    withBlock.CheckPathExists = true;
                    withBlock.Title = "Select Image File";
                    withBlock.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
                    // "Bitmap Image(*.bmp)|*.bmp|Jpeg Image(*.jpg)|*.jpg|GIF Image(*.gif)|*.gif|PNG Image(*.png)|*.png|All files (*.*)|*.*"
                    // '"JPEG files (*.jpg)|*.jpg|GIF files (*.gif)|*.gif|All files (*.*)|*.*"
                    withBlock.AddExtension = true;
                    withBlock.Multiselect = false;
                    withBlock.RestoreDirectory = true;
                    withBlock.ShowDialog();
                    // Hold the path for use later
                    strImagePath = withBlock.FileName;
                    if (withBlock.FileName is null)
                    {
                        return;
                    }
                    AddObject(ObjectTypes.Linked, strImagePath);
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
        }

        public void AddBackGroundImage()
        {
            {
                var withBlock = ofdImage;
                AddObject(ObjectTypes.Background, withBlock.FileName);
            }
            SelectedObject.SendToBack(SelectedObject);
            Invalidate();
        }

        public void AddImage(Image img)
        {
            {
                var withBlock = ofdImage;
                AddObject(ObjectTypes.EmbeddedImageGraphic, null, img);
            }
            // Me.SelectedObject.SendToBack(Me.SelectedObject)
            Invalidate();
        }

        public void FillBackColor()
        {
            AddObject(ObjectTypes.Fill, null);
        }

        public void ClearGridObjects()
        {
            DialogResult dr;
            dr = MessageBox.Show("Are you sure you want to remove all the graphic elements from the design surface?", "Confirm Clear All Elements", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                DeleteAllObjects();
                SelectedObject = null;
                // Reset ImageCount to 0
                intObjectCount = 0;
                // sbObjectCount.Text = Me.intObjectCount.ToString & "  Images Loaded"
                Invalidate();
            }
        }

        private void pbCropped_Click(object sender, EventArgs e)
        {
            // pbCropped.Image is only assigned once an object has been added through
            // AddObject(), so this preview box can still be clicked while it is empty.
            if (pbCropped.Image == null)
                return;

            MessageBox.Show(pbCropped.Image.Size.ToString());
        }

        public void GridLandscape()
        {
            if (intObjectCount != 0)
            {
                Interaction.MsgBox("Changing the orientation will not change the objects on the surface and should be set up on empty surface.", MsgBoxStyle.Information);
            }
            blnLandscape = true;
            SurfaceBounds = new Rectangle(0, 0, 1100, 860);
            SurfaceMargins = new Rectangle(10, 10, 1080, 830);
            // sbImageSize.Text = "Current Print Size = " & "11" & " X " & "8.5" & " " & "Inches"
            // sbOrientation.Text = "Landscape"
            Invalidate();
            Invalidate();
        }

        public void GridPortrait()
        {
            if (intObjectCount != 0)
            {
                Interaction.MsgBox("Changing the orientation will not change the objects on the surface and should be set up on empty surface.", MsgBoxStyle.Information);
            }
            blnLandscape = false;
            SurfaceBounds = new Rectangle(0, 0, 860, 1100);
            SurfaceMargins = new Rectangle(10, 10, 830, 1080);
            // sbImageSize.Text = "Current Print Size = " & "8.5" & " X " & "11" & " " & "Inches"
            // sbOrientation.Text = "Portrait"
            Invalidate();
            Invalidate();
        }

        public void CopyObject(MSDN.Samples.GraphicObjects.GraphicObjects gobj)
        {
            // ClonedObject = Nothing
            // If Me.SelectedObject Is Nothing Then Exit Sub
            // ClonedObject = SelectedObject

        }

        public void PasteObject()
        {
            // If ClonedObject Is Nothing Then Exit Sub
            // AddObject(ClonedObject.ImageType, Nothing)
        }

        public void Undo()
        {

            // If SelectedObject Is Nothing Then
            // SelectedObject = UndoStack.Pop
            // RedoStack.Push(SelectedObject)
            // Else
            // UndoStack.Push(gobj)
            // End If
            // Dim gobj As MustInherit GraphicObjects.GraphicObjects
            // gobj = UndoStack.Pop
            // If UndoList.Count < 1 Then Exit Sub
            // If UndoList.Count = 1 And SelectedObject Is Nothing Then
            // RedoList.Add(SelectedObject)
            // DeleteSelectedObject()
            // 'Me.lblUndo.Text = "Redo = " & RedoList.Count.ToString
            // 'Me.lblUndo.Text = "Undo = " & UndoList.Count.ToString
            // Exit Sub
            // End If

            // ClonedObject = UndoList.Item(UndoList.Count - 1)

            // ClonedObject.objType = ObjectTypes.ClonedObject
            // RedoList.Add(ClonedObject)
            // UndoList.RemoveAt(UndoList.Count - 1)
            // AddObject(ClonedObject.objType, Nothing)
            // 'Me.lblUndo.Text = "Redo = " & RedoList.Count.ToString
            // 'Me.lblUndo.Text = "Undo = " & UndoList.Count.ToString

        }

        public void Redo()
        {
            // If UndoStack.Count > 0 Then
            // UndoStack.Push(RedoStack.Pop)
            // End If


            // If RedoList.Count < 1 Then Exit Sub
            // ClonedObject = RedoList.Item(RedoList.Count - 1)
            // UndoList.Add(ClonedObject)
            // RedoList.RemoveAt(RedoList.Count - 1)
            // If Not ClonedObject.objType = ObjectTypes.DeletedObject Then
            // AddObject(ClonedObject.objType, Nothing)
            // End If

            // 'Me.lblUndo.Text = "Redo = " & RedoList.Count.ToString
            // 'Me.lblUndo.Text = "Undo = " & UndoList.Count.ToString
        }

        public bool ConvertBMP(string BMPFullPath, ImageFormat imgFormat)
        {

            bool bAns;
            string sNewFile;

            try
            {
                // bitmap class in system.drawing.imaging
                var objBmp = new Bitmap(BMPFullPath);

                // below 2 functions in system.io.path
                sNewFile = Path.GetDirectoryName(BMPFullPath);
                sNewFile += Path.GetFileNameWithoutExtension(BMPFullPath);

                sNewFile += "." + imgFormat.ToString();
                objBmp.Save(sNewFile, imgFormat);

                bAns = true; // return true on success
            }
            catch
            {
                bAns = false; // return false on error
            }
            return bAns;

        }

        // Update the propertie grid
        public void SelectedObjectChanged(MSDN.Samples.GraphicObjects.GraphicObjects gobj = null)
        {
            if (gobj is null)
            {
            }
            // PropertyGrid1.SelectedObject = Nothing
            else
            {
                Interaction.MsgBox("Object Changed");
            }
        }

        public void OpenGraphic()
        {
            {
                var withBlock = ofdImage;
                withBlock.AddExtension = true;
                withBlock.CheckPathExists = true;
                withBlock.Filter = "Data Files(*.dat)|*.dat|All files (*.*)|*.*";
                withBlock.DefaultExt = "dat";
                withBlock.InitialDirectory = Application.StartupPath + @"\"; // Set the object directory
                withBlock.Title = "Load Graphic Surface";
                withBlock.RestoreDirectory = true;
                if (withBlock.ShowDialog() == DialogResult.OK)
                {
                    string fileName = withBlock.FileName;
                    try
                    {
                        LoadFromFile(fileName);
                        MessageBox.Show("File Loaded Successfully", "Design Surface", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        intObjectCount += 1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Constants.vbCrLf + "Current Object is already Open", "File Failed To Load");
                    }
                }
            }
            Invalidate();
        }

        public void SaveGraphics()
        {
            if (intObjectCount <= 0)
            {
                Interaction.MsgBox("Nothing to save", MsgBoxStyle.Critical, "Save Graphic Error");
                return; // no known object to be saved
            }
            foreach (MSDN.Samples.GraphicObjects.GraphicObjects dobj in drawingObjects)
                Debug.WriteLine(string.Format("{0},{1}", dobj.X, dobj.Y));

            {
                var withBlock = sfdImage;
                withBlock.AddExtension = true;
                withBlock.CheckPathExists = true;
                withBlock.CreatePrompt = true;
                withBlock.OverwritePrompt = true;
                withBlock.Filter = "Data Files (*.dat)|*.dat|All files (*.*)|*.*";
                withBlock.DefaultExt = "dat";
                withBlock.FileName = "GraphicObjects.dat";
                withBlock.InitialDirectory = Application.StartupPath + @"\";
                withBlock.Title = "Save Current Graphic Surface";
                withBlock.RestoreDirectory = true;
                if (withBlock.ShowDialog() == DialogResult.OK)
                {
                    string fileName = withBlock.FileName;
                    SaveToFile(fileName);
                    MessageBox.Show("File Saved Successfully.", "Graphics Surface", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void PrintPreview()
        {
            try
            {
                if (intObjectCount <= 0)
                {
                    Interaction.MsgBox("Nothing to preview", MsgBoxStyle.Critical, "Preview Graphic Error");
                    return; // no known object to be saved
                }
                var ppd = new PrintPreviewDialog();
                ppd.TopMost = true;
                ppd.Document = designSurfacePrintDocument;
                ppd.UseAntiAlias = true;
                ppd.Show();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.ToString());
            }
        }

        public void PrintGraphics()
        {
            if (intObjectCount <= 0)
            {
                Interaction.MsgBox("Nothing to print", MsgBoxStyle.Critical, "Print Graphic Error");
                return; // no known object to be saved
            }
            setupPrint.ShowDialog();
        }

        public void LoadImageObjects()
        {
            {
                var withBlock = ofdImage;
                withBlock.AddExtension = true;
                withBlock.CheckPathExists = true;
                withBlock.Filter = "Data Files(*.dat)|*.dat|All files (*.*)|*.*";
                withBlock.DefaultExt = "dat";
                withBlock.InitialDirectory = Application.StartupPath + @"\";
                withBlock.Title = "Load Graphic Surface";
                withBlock.RestoreDirectory = true;
                if (withBlock.ShowDialog() == DialogResult.OK)
                {
                    string fileName = withBlock.FileName;
                    try
                    {
                        LoadFromFile(fileName);
                        MessageBox.Show("File Loaded Successfully", "Design Surface", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "File Failed To Load");
                    }
                }
            }
        }

        private void btnHideGrabHandles_Click(MSDN.Samples.GraphicObjects.GraphicObjects gobj, EventArgs e)
        {
            // DrawControlBorder(gobj, False)
            Invalidate();
        }

        private void DesignSurface_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            MSDN.Samples.GraphicObjects.GraphicObjectCollection.Zoom = Zoom;
            BackBrush = new SolidBrush(Color.LightGray);
            LinePen[0] = new Pen(ForeColor);
            LinePen[1] = new Pen(ForeColor);
            LinePen[1].DashPattern = new float[] { 5f, 3f };

            // LoadDrag_Handles()

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }



    }
}