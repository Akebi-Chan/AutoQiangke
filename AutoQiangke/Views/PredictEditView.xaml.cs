using AutoQiangke.Models;
using MaterialDesignThemes.Wpf;
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
    /// PredictEditView.xaml 的交互逻辑
    /// </summary>
    public partial class PredictEditView : UserControl, INotifyPropertyChanged
    {
        public BlockInfo blockinfo;
        public BindingList<VerbDisplay> Verbs { get; set; }

        public bool Rev1
        {
            get { return blockinfo.rule.rev1; }
            set { blockinfo.rule.rev1 = value; }
        }
        public bool Rev2
        {
            get { return blockinfo.rule.rev2; }
            set { blockinfo.rule.rev2 = value; }
        }
        public string Arg1
        {
            get { return blockinfo.rule.arg1; }
            set { blockinfo.rule.arg1 = value; }
        }
        public string Arg2
        {
            get { return blockinfo.rule.arg2; }
            set { blockinfo.rule.arg2 = value; }
        }
        private VerbDisplay verb1;
        public VerbDisplay Verb1
        {
            get { return verb1; }
            set { verb1 = value; blockinfo.rule.verb1 = value.verb; }
        }
        private VerbDisplay verb2;
        public VerbDisplay Verb2
        {
            get { return verb2; }
            set { verb2 = value; blockinfo.rule.verb2 = value.verb; }
        }
        public bool Joiner
        {
            get { return blockinfo.rule.joiner == 0; }
            set { blockinfo.rule.joiner = value ? 0 : 1; }
        }
        public bool RevJoiner
        {
            get { return blockinfo.rule.joiner == 1; }
            set { blockinfo.rule.joiner = value ? 1 : 0; }
        }
        public string Title
        {
            get { return blockinfo.predictname; }
            set { blockinfo.predictname = value; }
        }


        public PredictEditView()
        {
            InitVerbDisplay();
            verb1 = Verbs[0];
            verb2 = Verbs[0];
            this.blockinfo = new BlockInfo(BlockType.Pending, "新建匹配", new BlockMatchRule());
            InitializeComponent();
            this.DataContext = this;
        }

        public PredictEditView(BlockInfo blockinfo)
        {
            this.blockinfo = new BlockInfo(blockinfo.predictname, blockinfo.rule.Clone());
            InitVerbDisplay();
            verb1 = FindVerb(blockinfo.rule.verb1);
            verb2 = FindVerb(blockinfo.rule.verb2);
            InitializeComponent();
            this.DataContext = this;
        }

        private void InitVerbDisplay()
        {
            Verbs = new BindingList<VerbDisplay>();
            var verbtypes = Enum.GetValues<VerbType>();
            foreach (var i in verbtypes)
            {
                var str = "";
                switch (i)
                {
                    case VerbType.None:
                        str = "(无)";
                        break;
                    case VerbType.Regex:
                        str = "正则匹配";
                        break;
                    case VerbType.Equal:
                        str = "等于";
                        break;
                    case VerbType.Contain:
                        str = "包含";
                        break;
                    case VerbType.StartWith:
                        str = "开头是";
                        break;
                    case VerbType.EndWith:
                        str = "结尾是";
                        break;
                }
                Verbs.Add(new VerbDisplay(i, str));
            }
        }

        private VerbDisplay FindVerb(VerbType verbType)
        {
            foreach (var vd in Verbs)
                if (vd.verb == verbType)
                    return vd;
            return null;
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

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (Title=="")
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("标题不能为空", TimeSpan.FromSeconds(2)));
                return;
            }
            if (verb1.verb != VerbType.None && Arg1 == "")
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("参数不能为空", TimeSpan.FromSeconds(2)));
                return;
            }
            if (verb2.verb != VerbType.None && Arg2 == "")
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("参数不能为空", TimeSpan.FromSeconds(2)));
                return;
            }
            if (verb1.verb == VerbType.None && verb2.verb == VerbType.None)
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("请至少设定一个条件", TimeSpan.FromSeconds(2)));
                return;
            }
            if (verb1.verb == VerbType.None && verb2.verb != VerbType.None)
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("请把条件优先写在第一个位置", TimeSpan.FromSeconds(2)));
                return;
            }
            DialogHost.CloseDialogCommand.Execute(true, this);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(false, this);
        }
    }
}
