using AutoQiangke.Models;
using AutoQiangke.Service;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoQiangke.Views
{
    /// <summary>
    /// TaskDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class TaskView : UserControl
    {
        TaskModel taskModel;
        public TaskView()
        {
            InitializeComponent();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            taskModel.ToggleRun();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.taskModel = (TaskModel)this.DataContext;
            taskModel.OnGoAnimation += TaskModel_OnGoAnimation;
            if (taskModel.State == TaskState.Started)
                TaskModel_OnGoAnimation();
        }

        private void TaskModel_OnGoAnimation()
        {
            this.Dispatcher.Invoke(startani);
        }

        private void startani()
        {
            var da = new DoubleAnimation(0, 100, TimeSpan.FromMilliseconds(taskModel.Interval));
            GoButton.BeginAnimation(ButtonProgressAssist.ValueProperty, da);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Task.tasks.Remove(taskModel);
        }

        private void ButtonDetail_Click(object sender, RoutedEventArgs e)
        {
            Common.TaskSwitchMessager.Publish(taskModel);
        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {
            var w = new LogWindow(taskModel.logger);
            w.Show();
        }

        private void JxbSummaryView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Common.TaskSwitchMessager.Publish(taskModel);
        }
    }
}
