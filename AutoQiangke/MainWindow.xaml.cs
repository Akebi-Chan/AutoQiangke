using AutoQiangke.Models;
using AutoQiangke.Service;
using AutoQiangke.Shared;
using AutoQiangke.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoQiangke
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            MainTreeView.DataContext = this;
            Common.RegionSwitchManager.Subscribe<string>(OnSwitchView);
            Common.SnackbarManager.Subscribe<MySnackBarMessage>(OnRecieveMessage);
            Common.BlockMessager.Subscribe<List<BlockInfo>>(OnMergeBlocks);
            Common.TaskSwitchMessager.Subscribe<TaskModel>(OnSwitchTask);
            BasicInfo.OnStopWait += BasicInfo_OnStopWait;
            BasicInfo.OnGoAnimation += BasicInfo_OnGoAnimation;
            //CreateTasks();
        }

        #region Window Event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
        #endregion

        #region Region Switch
        private void OnSwitchTask(TaskModel task)
        {
            TaskListBox.SelectedIndex = Task.tasks.IndexOf(task);
        }

        public void OnRecieveMessage(MySnackBarMessage obj)
        {
            this.Dispatcher.Invoke(() => { MainSnackbar.MessageQueue.Enqueue(obj.message, null, null, null, false, true, obj.duration); });
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null || e.NewValue is ListBox)
                return;

            TaskListBox.SelectedItem = null;
            var item = (TreeViewItem)e.NewValue;
            if (item.Tag is UserControl u)
            {
                MainPage.Content = u;
                return;
            }
            switch (item.Tag)
            {
                case "Welcome":
                    MainPage.Content = new WelcomeView();
                    item.Tag = MainPage.Content;
                    break;
                case "Login":
                    MainPage.Content = new LoginView();
                    item.Tag = MainPage.Content;
                    break;
                case "Predict":
                    MainPage.Content = new PredictView();
                    item.Tag = MainPage.Content;
                    break;
                case "TimeMachine":
                    MainPage.Content = new TimeMachineView();
                    item.Tag = MainPage.Content;
                    break;
                case "Task":
                    MainPage.Content = new TaskListView();
                    item.Tag = MainPage.Content;
                    break;
                case "Settings":
                    MainPage.Content = new SettingsView();
                    item.Tag = MainPage.Content;
                    break;
                case "About":
                    MainPage.Content = new AboutView();
                    item.Tag = MainPage.Content;
                    break;
            }
        }

        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskListBox.SelectedItem == null)
                return;
            if (TaskListBox.SelectedItem is TaskModel task)
            {
                var olditem = (TreeViewItem)MainTreeView.SelectedItem;
                if (olditem != null)
                    olditem.IsSelected = false;
                if(task.view != null)
                {
                    MainPage.Content = task.view;
                    return;
                }
                task.view = task.type == TaskType.Lite ? new EditTaskView(task) : new EditTaskViewFull(task);
                MainPage.Content = task.view;
            }
        }

        public void OnSwitchView(string view)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (view)
                {
                    case "Welcome":
                        WelcomeTreeItem.IsSelected = true;
                        break;
                    case "Login":
                        LoginTreeItem.IsSelected = true;
                        break;
                    case "Predict":
                        PredictTreeItem.IsSelected = true;
                        break;
                    case "TimeMachine":
                        TimeMachineTreeItem.IsSelected = true;
                        break;
                    case "Task":
                        TaskTreeItem.IsSelected = true;
                        break;
                    case "Settings":
                        SettingsTreeItem.IsSelected = true;
                        break;
                    case "About":
                        AboutTreeItem.IsSelected = true;
                        break;
                }
            });
        }
        #endregion

        #region UI task
        private void OnMergeBlocks(List<BlockInfo> list)
        {
            this.Dispatcher.Invoke(new Action<List<BlockInfo>>(Block.MergeBlocks), list);
        }

        private void BasicInfo_OnStopWait()
        {
            this.Dispatcher.Invoke(() => { ButtonWait.Content = "开始蹲守"; });
        }
        #endregion

        #region 蹲守选课
        public int Interval
        {
            get { return BasicInfo.interval; }
            set
            {
                BasicInfo.interval = value;
                this.RaisePropertyChanged("Interval");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Jac.islogin)
            {
                MessageBox.Show("请先登录");
                return;
            }
            BasicInfo.ToggleWait();
            if (BasicInfo.isstart)
                this.Dispatcher.Invoke(() => { ButtonWait.Content = "停止蹲守"; });
        }
        private void BasicInfo_OnGoAnimation()
        {
            this.Dispatcher.Invoke(startani);
        }
        private void startani()
        {
            var da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(Interval));
            ProgressBar1.BeginAnimation(ProgressBar.ValueProperty, da);
        }

        #endregion

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
