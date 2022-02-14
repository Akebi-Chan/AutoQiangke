using AutoQiangke.Models;
using AutoQiangke.Service;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoQiangke.Views
{
    /// <summary>
    /// EditTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class EditTaskViewFull : UserControl
    {
        public TaskModel taskModel;
        public EditTaskViewFull()
        {
            InitializeComponent();
        }
        public EditTaskViewFull(TaskModel taskModel)
        {
            InitializeComponent();
            this.taskModel = taskModel;
            this.DataContext = taskModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            JxbCardsControl1.InitCardSize();
            Binding binding = new Binding("Jxbs") { Mode = BindingMode.TwoWay };
            JxbCardsControl1.SetBinding(JxbCardsControl.JxbsProperty, binding);
        }

        private void StartQueryJxbFull()
        {
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Common.RegionSwitchManager.Publish<string>("Task");
        }

        private void ButtonAddJxb_Click(object sender, RoutedEventArgs e)
        {
            AddJxb();
        }

        private void AddJxb()
        {
            var c1 = new BindingList<JxbModel>();
            var jxb = new JxbModel(TextJxb.Text);
            c1.Add(jxb);
            taskModel.Jxbs.Insert(0, c1);
            taskModel.RaiseJxbsChanged();
            //JxbCardsControl1.AddJxb(jxb);
            TextJxb.Text = "";
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += Bgw_DoWork;
            bgw.RunWorkerAsync(jxb);
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            JxbModel jxb = (JxbModel)e.Argument;
            this.Dispatcher.Invoke(StartQueryJxbFull);
            string querytext = jxb.jxbmc;
            var res = Course.PreQueryJxbFull(querytext);
            if (res.success)
            {
                taskModel.UpdateJxbAfterPreQueryFull(jxb, res.jxb);
                taskModel.CheckIsChosenJxb(jxb);
            }
            else
            {
                MessageBox.Show(res.message);
            }
        }

        private void TextJxb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
                AddJxb();
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
