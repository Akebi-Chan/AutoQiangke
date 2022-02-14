using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl, INotifyPropertyChanged
    {
        public UserControl1()
        {
            InitializeComponent();
            this.Log = new BindingList<string>();
            list1.ItemsSource = Log;
            //this.DataContext = this;
        }

        private BindingList<string> log;

        public BindingList<string> Log
        {
            get { return log; }
            set
            {
                log = value;
                this.RaisePropertyChanged("Log");
            }
        }

        public ObservableCollection<DemoItem> Items
        {
            get { return (ObservableCollection<DemoItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<DemoItem>), typeof(UserControl1), new PropertyMetadata(null, propertyChangedCallback, coerceValueCallback));

        private static object coerceValueCallback(DependencyObject d, object baseValue)
        {
            var control = (UserControl1)d;
            Debug.WriteLine("coerceValueCallback");
            control.Log.Add("coerceValueCallback");
            control.RaisePropertyChanged("Log");
            return baseValue;
        }

        private static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (UserControl1)d;
            Debug.WriteLine("propertyChangedCallback");
            control.Log.Add("propertyChangedCallback");
            control.RaisePropertyChanged("Log");
        }

        #region Main业务
        public void NewList()
        {
            Items = new ObservableCollection<DemoItem>();
        }

        public void NewItem()
        {
            Items.Add(new DemoItem() { Str = "新建测试" + Common.GetRandomString(4, true, true, true, false, "")});
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
