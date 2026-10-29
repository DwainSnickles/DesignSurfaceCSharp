using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Msdn.Samples.DesignSurface.CustomGrid
{

    /// <summary>
    /// CustomClass (Which is binding to property grid)
    /// </summary>
    public class CustomClass : CollectionBase, ICustomTypeDescriptor
    {

        /// <summary>
        /// Add CustomProperty to Collectionbase List
        /// </summary>
        /// <param name="Value"></param>
        public void Add(CustomProperty Value)
        {
            List.Add(Value);
        }

        /// <summary>
        /// Remove item from List
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Name)
        {
            foreach (CustomProperty prop in List)
            {
                if ((prop.Name ?? "") == (Name ?? ""))
                {
                    List.Remove(prop);
                    return;
                }

            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public CustomProperty this[int index]
        {
            get
            {
                return (CustomProperty)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        #region TypeDescriptor Implementation

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <returns>AttributeCollection</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// GetComponentName
        /// </summary>
        /// <returns>String</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// GetConverter
        /// </summary>
        /// <returns>TypeConverter</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// GetDefaultEvent
        /// </summary>
        /// <returns>EventDescriptor</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// GetDefaultProperty
        /// </summary>
        /// <returns>PropertyDescriptor</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// GetEditor
        /// </summary>
        /// <param name="editorBaseType">editorBaseType</param>
        /// <returns>object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] newProps = new PropertyDescriptor[Count];
            int i = 0;
            while (i < Count)
            {
                var prop = this[i];
                newProps[i] = new CustomPropertyDescriptor(ref prop, attributes);
                i = i + 1;
            }

            return new PropertyDescriptorCollection(newProps);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        private AttributeCollection ICustomTypeDescriptor_GetAttributes()
        {
            throw new NotImplementedException();
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => ICustomTypeDescriptor_GetAttributes();

        private string ICustomTypeDescriptor_GetClassName()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetClassName() => ICustomTypeDescriptor_GetClassName();

        private string ICustomTypeDescriptor_GetComponentName()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetComponentName() => ICustomTypeDescriptor_GetComponentName();

        private TypeConverter ICustomTypeDescriptor_GetConverter()
        {
            throw new NotImplementedException();
        }

        TypeConverter ICustomTypeDescriptor.GetConverter() => ICustomTypeDescriptor_GetConverter();

        private EventDescriptor ICustomTypeDescriptor_GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => ICustomTypeDescriptor_GetDefaultEvent();

        private PropertyDescriptor ICustomTypeDescriptor_GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => ICustomTypeDescriptor_GetDefaultProperty();

        private object ICustomTypeDescriptor_GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => ICustomTypeDescriptor_GetEditor(editorBaseType);

        private EventDescriptorCollection ICustomTypeDescriptor_GetEvents()
        {
            throw new NotImplementedException();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => ICustomTypeDescriptor_GetEvents();

        private EventDescriptorCollection ICustomTypeDescriptor_GetEvents1(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => ICustomTypeDescriptor_GetEvents1(attributes);

        private PropertyDescriptorCollection ICustomTypeDescriptor_GetProperties()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => ICustomTypeDescriptor_GetProperties();

        private PropertyDescriptorCollection ICustomTypeDescriptor_GetProperties1(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) => ICustomTypeDescriptor_GetProperties1(attributes);

        private object ICustomTypeDescriptor_GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => ICustomTypeDescriptor_GetPropertyOwner(pd);
        #endregion
    }

    /// <summary>
    /// Custom property class 
    /// </summary>
    public class CustomProperty
    {

        private string sName = string.Empty;

        private bool bReadOnly = false;

        private bool bVisible = true;

        private object objValue = null;

        public CustomProperty(string sName, object value, bool bReadOnly, bool bVisible) : base()
        {
            this.sName = this.sName;
            objValue = value;
            this.bReadOnly = this.bReadOnly;
            this.bVisible = this.bVisible;
        }

        public bool ReadOnlyProperty
        {
            get
            {
                return bReadOnly;
            }
        }

        public string Name
        {
            get
            {
                return sName;
            }
        }

        public bool Visible
        {
            get
            {
                return bVisible;
            }
        }

        public object Value
        {
            get
            {
                return objValue;
            }
            set
            {
                objValue = value;
            }
        }
    }

    /// <summary>
    /// Custom PropertyDescriptor
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor
    {

        private CustomProperty m_Property;

        public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute[] attrs) : base(myProperty.Name, attrs)
        {
            m_Property = myProperty;
        }
        #region PropertyDescriptor specific

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get
            {
                return null;
            }
        }

        public override object GetValue(object component)
        {
            return m_Property.Value;
        }

        public override string Description
        {
            get
            {
                return m_Property.Name;
            }
        }

        public override string Category
        {
            get
            {
                return string.Empty;
            }
        }

        public override string DisplayName
        {
            get
            {
                return m_Property.Name;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return m_Property.ReadOnlyProperty;
            }
        }

        public override void ResetValue(object component)
        {
            // Have to implement
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override void SetValue(object component, object value)
        {
            m_Property.Value = value;
        }

        public override Type PropertyType
        {
            get
            {
                return m_Property.Value.GetType();
            }
        }
        #endregion
    }
}