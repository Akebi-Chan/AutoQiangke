using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class DemoItem //: INotifyPropertyChanged
    {
        private string str;

        public string Str
        {
            get { return str; }
            set
            {
                str = value;
                //this.RaisePropertyChanged("Str");
            }
        }


        public override string ToString()
        {
            return Str == null ? "null" : Str;
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
