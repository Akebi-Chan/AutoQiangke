using AutoQiangke.Models;
using AutoQiangke.Service;
using MaterialDesignThemes.Wpf.Transitions;
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
    public partial class EditTaskView : UserControl
    {
        public TaskModel taskModel;
        string guid;
        public EditTaskView()
        {
            InitializeComponent();
        }
        public EditTaskView(TaskModel taskModel)
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

        private void StartQueryCourse()
        {
            Transitioner.SelectedIndex = 0;
            taskModel.CourseName = "";
            taskModel.IsCourseIdValid = false;
            taskModel.CourseCountText = "";
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
            var jxb = new JxbModel(taskModel.course, TextJxb.Text);
            c1.Add(jxb);
            taskModel.Jxbs.Insert(0, c1);
            if (taskModel.IsCourseIdValid)
            {
                taskModel.UpdateJxbByAdd(jxb);
                taskModel.CheckIsChosenJxb(jxb);
            }
            else
                taskModel.RaiseJxbsChanged();
            //JxbCardsControl1.AddJxb(jxb);
            TextJxb.Text = "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newguid = Guid.NewGuid().ToString();
            guid = newguid;
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += Bgw_DoWork;
            
            bgw.RunWorkerAsync(guid);
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            if ((string)e.Argument != guid) return;
            this.Dispatcher.Invoke(StartQueryCourse);
            string querytext = taskModel.CourseId;
            var res = Course.PreQueryCourse(querytext);
            if ((string)e.Argument != guid) return;
            if (res.success)
            {
                this.Dispatcher.Invoke(() => { Transitioner.SelectedIndex = 1; });
                taskModel.course = res.course;
                taskModel.course.blockInfo = taskModel.BlockInfo;
                taskModel.CourseName = res.course.kcmc;
                taskModel.queryresult = res.jxbs;
                taskModel.CourseCountText = "（共有 " + res.course.jxbcount.ToString() + " 个教学班）";
                taskModel.IsCourseIdValid = true;
                taskModel.UpdateJxbsAfterPreQuery();
                taskModel.CheckIsChosenJxbs();
            }
            else
            {
                this.Dispatcher.Invoke(() => { Transitioner.SelectedIndex = 1; });
                Common.logger.log("PreQueryCourse失败！ " + querytext);
                taskModel.course = null;
                taskModel.queryresult = null;
                taskModel.CourseName = res.message;
                taskModel.CourseCountText = "";
                taskModel.IsCourseIdValid = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (taskModel.course != null)
                taskModel.course.blockInfo = taskModel.BlockInfo;
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
