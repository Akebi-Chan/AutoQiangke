using AutoQiangke.Helpers;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AutoQiangke.Views
{
    /// <summary>
    /// LogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogWindow : Window
    {
        public Logger logger;
        public LogWindow()
        {
            InitializeComponent();
        }

        public LogWindow(Logger logger)
        {
            InitializeComponent();
            this.logger = logger;
            ListBox1.ItemsSource = logger.list;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Refresh();
        }
    }
}
