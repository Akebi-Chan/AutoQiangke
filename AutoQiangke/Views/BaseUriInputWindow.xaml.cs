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

namespace AutoQiangke.Views
{
    /// <summary>
    /// BaseUriInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BaseUriInputWindow : Window, INotifyPropertyChanged
    {
        public BaseUriInputWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private string baseUri;

        public string BaseUri
        {
            get { return baseUri; }
            set
            {
                baseUri = value;
                this.RaisePropertyChanged("BaseUri");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
