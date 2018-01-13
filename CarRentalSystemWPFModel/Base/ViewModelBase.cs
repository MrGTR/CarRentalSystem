using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using WPFBiz;

namespace Base.WPFModel
{
    abstract public class ViewModelBase : PropertyBase
    {
        public ViewModelBase()
        {
        }
        public ViewModelBase(string url) :base()
        {
            this.DbFactory = new DataManager(url);
        }
        public ViewModelBase(DataManager manager) : base()
        {
            this.DbFactory = manager;
        }
        private DataManager _dManager;
        public DataManager DbFactory
        {
            get
            {
                return _dManager;
            }
            set
            {
                this._dManager = value;
            }
        }
    }
}
