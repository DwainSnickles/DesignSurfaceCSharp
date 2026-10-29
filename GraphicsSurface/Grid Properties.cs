using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Msdn.Samples.DesignSurface
{
    /// 
    [DefaultProperty("Title")]
    public class GridProperties
    {
        /// 
        private string _Title = "Title";
        private bool _Show = true;
        private short _Number = 345;
        /// 
        [Category("Application")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Bindable(false)]
        [DefaultValue("")]
        [DesignOnly(false)]
        [Description("Enter Title for the application")]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }
        /// 
        [Category("Application")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Bindable(false)]
        [DefaultValue("True")]
        [DesignOnly(false)]
        [Description("Show option")]
        public bool Show
        {
            get
            {
                return _Show;
            }
            set
            {
                _Show = value;
            }
        }
        /// 
        [Category("Application")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Bindable(false)]
        [DefaultValue("0")]
        [DesignOnly(false)]
        [Description("Enter a number")]
        public short Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
            }
        }

        private Size _ApplicationSize;
        private Point _ApplicationLocation;
        private Font _ApplicationFont;
        private Color _FontColor;
        private Icon _ApplicationIcon;
        private Cursor _ApplicationCursor;
        private System.Globalization.CultureInfo _ApplicationLangugae;
        /// 
        [Category("Design")]
        [DefaultValue("")]
        [Description("Enter Application size")]
        public Size ApplicationSize
        {
            get
            {
                return _ApplicationSize;
            }
            set
            {
                _ApplicationSize = value;
            }
        }

        [Category("Design")]
        [DefaultValue("")]
        [Description("Enter Application Location")]
        public Point ApplicationLocation
        {
            get
            {
                Point ApplicationLocationRet = default;
                return ApplicationLocationRet;
            }
            set
            {
                _ApplicationLocation = value;
            }
        }

        [Category("Design")]
        [DefaultValue("")]
        [Description("Select font for the application")]
        public Font ApplicationFont
        {
            get
            {
                return _ApplicationFont;
            }
            set
            {
                _ApplicationFont = value;
            }
        }

        [Category("Design")]
        [DefaultValue("")]
        [Description("Select font color")]
        public Color FontColor
        {
            get
            {
                return _FontColor;
            }
            set
            {
                _FontColor = value;
            }
        }

        [Category("Design")]
        [DefaultValue("")]
        [Description("Select icon for application")]
        public Icon ApplicationIcon
        {
            get
            {
                return _ApplicationIcon;
            }
            set
            {
                _ApplicationIcon = value;
            }
        }

        [Category("Design")]
        [DefaultValue("")]
        [Description("Select application's cursor")]
        public Cursor ApplicationCursor
        {
            get
            {
                return _ApplicationCursor;
            }
            set
            {
                _ApplicationCursor = value;
            }
        }

        [Category("Design")]
        [DefaultValue("")]
        [Description("Select User language")]
        public System.Globalization.CultureInfo ApplicationLangugae
        {
            get
            {
                return _ApplicationLangugae;
            }
            set
            {
                _ApplicationLangugae = value;
            }
        }

        public GridProperties()
        {
            Title = "Title";
            Show = true;
            _Number = 345;
        }
    }
}