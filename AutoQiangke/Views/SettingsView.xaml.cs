using AutoQiangke.Service;
using AutoQiangke.Shared;
using System;
using System.Collections.Generic;
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

namespace AutoQiangke.Views
{
    /// <summary>
    /// SettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        #region Fields
        public bool? SaveCookie
        {
            get { return GlobalSettings.saveCookie; }
            set
            {
                GlobalSettings.saveCookie = value.Value;
                this.RaisePropertyChanged("SaveCookie");
            }
        }
        public bool? SaveFullBlockInfos
        {
            get { return GlobalSettings.saveFullBlockInfos; }
            set
            {
                GlobalSettings.saveFullBlockInfos = value.Value;
                this.RaisePropertyChanged("SaveFullBlockInfos");
            }
        }
        public bool? DisableTK
        {
            get { return GlobalSettings.disableTK; }
            set
            {
                GlobalSettings.disableTK = value.Value;
                this.RaisePropertyChanged("DisableTK");
            }
        }
        public bool? XkWithFullArgs
        {
            get { return GlobalSettings.xkWithFullArgs; }
            set
            {
                GlobalSettings.xkWithFullArgs = value.Value;
                this.RaisePropertyChanged("XkWithFullArgs");
            }
        }
        #endregion

        public SettingsView()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storage.ReadData();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Storage.SaveData();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GlobalSettings.SaveSettings();
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
