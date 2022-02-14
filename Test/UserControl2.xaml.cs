using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// UserControl2.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public ObservableCollection<DemoItem> Items
        {
            get { return (ObservableCollection<DemoItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<DemoItem>), typeof(UserControl2), new PropertyMetadata(null));

        public UserControl2()
        {
            InitializeComponent();
        }

        private void NewListButton_Click(object sender, RoutedEventArgs e)
        {
            NewList();
        }

        private void NewItemButton_Click(object sender, RoutedEventArgs e)
        {
            NewItem();
        }

        private void EditItemPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            EditItemProperty();
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            EditItem();
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
    }
}
