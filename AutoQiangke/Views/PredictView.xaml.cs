using AutoQiangke.Models;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using System.Globalization;

namespace AutoQiangke.Views
{
    /// <summary>
    /// PredictView.xaml 的交互逻辑
    /// </summary>
    public partial class PredictView : UserControl, INotifyPropertyChanged
    {
        public PredictView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        //private void ButtonGetBlockInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    var res = BasicInfo.GetBasicInfo();
        //    MessageBox.Show(res.result);
        //}

        private async void ButtonAdd_ClickAsync(object sender, RoutedEventArgs e)
        {
            var dialog=new PredictEditView();
            var rawres = await DialogHost.Show(dialog, "Root");
            if (rawres!=null && rawres is bool && (bool)rawres==true)
            {
                Block.AddBlock(dialog.blockinfo);
            }
        }

        private async void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (BlockListView.SelectedItem==null)
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("请选择一条数据", TimeSpan.FromSeconds(3)));
                return;
            }
            var item = (BlockInfo)BlockListView.SelectedItem;
            if (item.type == BlockType.Original)
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("本身存在的板块不可编辑！", TimeSpan.FromSeconds(3)));
                return;
            }
            var dialog = new PredictEditView((BlockInfo)BlockListView.SelectedItem);
            var rawres = await DialogHost.Show(dialog, "Root");
            if (rawres != null && rawres is bool && (bool)rawres == true)
            {
                Block.EditBlock(item, dialog.blockinfo);
                //item.rule = dialog.blockinfo.rule;
                //item.Predictname = dialog.blockinfo.Predictname;
                //item.Refresh();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            //删除确认
            if (BlockListView.SelectedItems.Count == 0)
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("请至少选择一条数据", TimeSpan.FromSeconds(3)));
                return;
            }
            if (MessageBox.Show("确定删除 " + BlockListView.SelectedItems.Count + " 条数据？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            List<BlockInfo> list = new List<BlockInfo>();
            foreach (var i in BlockListView.SelectedItems) list.Add((BlockInfo)i);
            foreach (var i in list) Block.DeleteBlock(i);
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Block.ClearBlocks();
            Block.AddBlock(new BlockInfo("主修课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "主修" }));
            Block.AddBlock(new BlockInfo("交叉课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "交叉" }));
            Block.AddBlock(new BlockInfo("任选课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "任选" }));
            Block.AddBlock(new BlockInfo("民族课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "民族" }));
            Block.AddBlock(new BlockInfo("留学生课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "留学生" }));
            Block.AddBlock(new BlockInfo("体育", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "体育" }));
            Block.AddBlock(new BlockInfo("通识课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "通识" }));
            Block.AddBlock(new BlockInfo("新生研讨课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "新生研讨课" }));
            Block.AddBlock(new BlockInfo("公共选修课", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "公共选修" }));
            Block.AddBlock(new BlockInfo("英语", new BlockMatchRule() { verb1 = VerbType.Contain, arg1 = "英语" }));
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
