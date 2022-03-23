using AutoQiangke.Helpers;
using DownKyi.Core.Downloader;
using Easy.MessageHub;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        MainViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainViewModel();
            this.DataContext = vm;
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

        private void NewListButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckBox1.IsChecked.Value)
                mycontrol.NewList();
            else
                vm.NewList();
        }

        private void NewItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckBox1.IsChecked.Value)
                mycontrol.NewItem();
            else
                vm.NewItem();
        }

        private void EditItemPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckBox1.IsChecked.Value)
                mycontrol.EditItemProperty();
            else
                vm.EditItemProperty();
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckBox1.IsChecked.Value)
                mycontrol.EditItem();
            else
                vm.EditItem();
        }

        private void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window1();
            w.DataContext = vm;
            w.Show();
        }

        private void RaisePropertyChangedButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RaisePropertyChanged("Items");
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            //MultiThreadDownloader d = new MultiThreadDownloader("https://dldir1.qq.com/qqfile/qq/PCQQ9.5.8/QQ9.5.8.28186.exe", "D:\\qq.exe", 4);
            //d.Start();
            WebClient client = new WebClient();
            client.DownloadString("https://www.baidu.com/");
            client.DownloadString("https://www.baidu.com/more/");  
        }
    }

}
