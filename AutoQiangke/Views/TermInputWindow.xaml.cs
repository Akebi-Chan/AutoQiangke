using AutoQiangke.Service;
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
    /// TermInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TermInputWindow : Window, INotifyPropertyChanged
    {
        public TermInputWindow()
        {
            InitData();
            InitializeComponent();
            this.DataContext = this;
        }

        private void InitData()
        {
            Xn = new List<XnModel>();
            Xn.Add(new XnModel("2018-2019", "2018"));
            Xn.Add(new XnModel("2019-2020", "2019"));
            Xn.Add(new XnModel("2020-2021", "2020"));
            Xn.Add(new XnModel("2021-2022", "2021"));
            Xn.Add(new XnModel("2022-2023", "2022"));
            Xn.Add(new XnModel("2023-2024", "2023"));
            Xn.Add(new XnModel("2024-2025", "2024"));
            Xn.Add(new XnModel("2025-2026", "2025"));
            Xq = new List<XqModel>();
            Xq.Add(new XqModel("1", "3"));
            Xq.Add(new XqModel("2", "12"));
            Xq.Add(new XqModel("3", "16"));
        }

        private List<XqModel> xq;

        public List<XqModel> Xq
        {
            get { return xq; }
            set
            {
                xq = value;
                this.RaisePropertyChanged("Xq");
            }
        }

        private List<XnModel> xn;

        public List<XnModel> Xn
        {
            get { return xn; }
            set
            {
                xn = value;
                this.RaisePropertyChanged("Xn");
            }
        }
        private XnModel selectedXn;

        public XnModel SelectedXn
        {
            get { return selectedXn; }
            set
            {
                selectedXn = value;
                this.RaisePropertyChanged("SelectedXn");
            }
        }
        private XqModel selectedXq;

        public XqModel SelectedXq
        {
            get { return selectedXq; }
            set
            {
                selectedXq = value;
                this.RaisePropertyChanged("SelectedXq");
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedXn == null)
            {
                MessageBox.Show("请选择学年");
                return;
            }
            if (SelectedXq == null)
            {
                MessageBox.Show("请选择学期");
                return;
            }
            this.Close();
        }
    }
}
