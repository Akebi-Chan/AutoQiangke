using AutoQiangke.Models;
using AutoQiangke.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// TaskView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskListView : UserControl//, INotifyPropertyChanged
    {
        //public BindingList<TaskModel> Tasks
        //{
        //    get { return Task.tasks; }
        //    set { Task.tasks = value; }
        //}

        public TaskListView()
        {
            InitializeComponent();
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

        private void ButtonAddTask_Click(object sender, RoutedEventArgs e)
        {
            var task = new TaskModel("新建任务", 500, TaskType.Lite);
            Task.tasks.Add(task);
            Common.TaskSwitchMessager.Publish(task);
        }

        private void ButtonAddTaskFull_Click(object sender, RoutedEventArgs e)
        {
            var task = new TaskModel("新建任务", 500, TaskType.Full);
            Task.tasks.Add(task);
            Common.TaskSwitchMessager.Publish(task);
        }
    }
}
