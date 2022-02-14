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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoQiangke.Views
{
    /// <summary>
    /// SplashScreen.xaml 的交互逻辑
    /// </summary>
    public partial class SplashWindow : Window, INotifyPropertyChanged
    {
        public SplashWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private delegate void NoArgDelegate();
        public void UpdateUI()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, (NoArgDelegate)delegate { });
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
