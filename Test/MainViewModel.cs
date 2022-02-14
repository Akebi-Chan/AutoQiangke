using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DemoItem> items;

        public ObservableCollection<DemoItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                this.RaisePropertyChanged("Items");
            }
        }

        public MainViewModel()
        {
            //Items = new ObservableCollection<DemoItem>();
        }

        #region Main业务
        public void NewList()
        {
            Items = new ObservableCollection<DemoItem>();
        }

        public void NewItem()
        {
            Items.Add(new DemoItem() { Str = "新建测试" + Common.GetRandomString(4, true, true, true, false, "") });
        }

        public void EditItemProperty()
        {
            if (Items.Count == 0) return;
            Items[0].Str = "修改测试" + Common.GetRandomString(4, true, true, true, false, "");
        }
        public void EditItem()
        {
            if (Items.Count == 0) return;
            var item = Items[0];
            item = new DemoItem() { Str = "替换测试" + Common.GetRandomString(4, true, true, true, false, "") };
        }
        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
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
