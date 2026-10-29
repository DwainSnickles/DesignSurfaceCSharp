using System;
using System.Drawing;

namespace Microsoft.Msdn.Samples.DesignSurface
{

    public class SelectionChangedEventArgs : EventArgs
    {
        private MSDN.Samples.GraphicObjects.GraphicObjects m_SelectedObject;

        public SelectionChangedEventArgs(MSDN.Samples.GraphicObjects.GraphicObjects selectedObject)
        {
            m_SelectedObject = selectedObject;
        }

        public MSDN.Samples.GraphicObjects.GraphicObjects SelectedObject
        {
            get
            {
                return m_SelectedObject;
            }
        }
    }


    public enum StatusUpdateType
    {
        ObjectRotated = 0,
        ObjectMoved = 1,
        ObjectDeleted = 2,
        SurfaceZoomChanged = 3,
        FileLoaded = 4,
        FileSaved = 5,
        SelectionChanged = 6,
        ObjectSizeChanged = 7
    }

    public class StatusUpdateEventArgs : EventArgs
    {
        private StatusUpdateType m_UpdateType;
        private MSDN.Samples.GraphicObjects.GraphicObjects m_SelectedObject;
        private string m_Message;
        private Point m_Coord;
        private float m_Amount;

        public StatusUpdateEventArgs(StatusUpdateType UpdateType, MSDN.Samples.GraphicObjects.GraphicObjects Selection, string StatusMessage, Point Coord, float Amt)
        {

            m_UpdateType = UpdateType;
            m_SelectedObject = Selection;
            m_Message = StatusMessage;
            m_Coord = Coord;
            m_Amount = Amt;
        }

        public StatusUpdateType Type
        {
            get
            {
                return m_UpdateType;
            }
        }
        public MSDN.Samples.GraphicObjects.GraphicObjects SelectedObject
        {
            get
            {
                return m_SelectedObject;
            }
        }

        public string Message
        {
            get
            {
                return m_Message;
            }
        }

        public Point Coordinates
        {
            get
            {
                return m_Coord;
            }
        }

        public float Amount
        {
            get
            {
                return m_Amount;
            }
        }

    }
}