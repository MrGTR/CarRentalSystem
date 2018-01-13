using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Base.WPFModel
{
    abstract public class PropertyBase : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public PropertyBase()
        {
        }

        #region INotifyPropertyChanging Members
        private PropertyChangingEventHandler _propertyChanging;
        private PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { this._propertyChanging += value; }
            remove { this._propertyChanging -= value; }
        }
        public virtual void OnPropertyChanging(string info)
        {
            if (this._propertyChanging != null)
                this._propertyChanging(this, new PropertyChangingEventArgs(info));
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { this._propertyChanged += value; }
            remove { this._propertyChanged -= value; }
        }

        public virtual void OnPropertyChanged(string info)
        {
            if (this._propertyChanged != null)
                this._propertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
