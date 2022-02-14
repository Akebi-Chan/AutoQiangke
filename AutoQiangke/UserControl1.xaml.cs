using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace AutoQiangke
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl, INotifyPropertyChanged
    {
        public UserControl1()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string kcmc;
        public string Kcmc
        {
            get { return kcmc; }
            set
            {
                kcmc = value;
                this.RaisePropertyChanged("Kcmc");
            }
        }

        private string kch;
        public string Kch
        {
            get { return kch; }
            set
            {
                kch = value;
                this.RaisePropertyChanged("Kch");
            }
        }
        private string jxb_qiang;
        public string Jxb_qiang
        {
            get { return jxb_qiang; }
            set
            {
                jxb_qiang = value;
                this.RaisePropertyChanged("Jxb_qiang");
            }
        }
        private string jxb_tui;
        public string Jxb_tui
        {
            get { return jxb_tui; }
            set
            {
                jxb_tui = value;
                this.RaisePropertyChanged("Jxb_tui");
            }
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
        private int times;

        public int Times
        {
            get { return times; }
            set
            {
                times = value;
                this.RaisePropertyChanged("Times");
            }
        }
        private string textTime;

        public string TextTime
        {
            get { return textTime; }
            set
            {
                textTime = value;
                this.RaisePropertyChanged("TextTime");
            }
        }

        public delegate void PanelCloseHandler(UserControl1 sender);
        public event PanelCloseHandler OnPanelClose;

        private int interval=800;

        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                this.RaisePropertyChanged("Interval");
            }
        }




        List<string> qiang;
        string tui;
        private Timer ti;
        bool initfail;
        public string uuid;

        BackgroundWorker initworker;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GoButton.Content = new PackIcon { Kind = PackIconKind.Play };
        }

        internal void init(string text1, string text2, string text3)
        {
            Kch = text1;
            qiang = text2.Split(',').ToList();
            for (int i = 0; i < qiang.Count; i++) qiang[i] = Kch + "-" + qiang[i];
            tui = text3;
            Message = "正在初始化……";
            GoButton.IsEnabled = false;

            initworker = new BackgroundWorker();
            initworker.DoWork += Initworker_DoWork;
            initworker.RunWorkerCompleted += Initworker_RunWorkerCompleted;
            initworker.RunWorkerAsync();
        }

        private void Initworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (initfail)
            {
                Message = "初始化失败";
                this.Dispatcher.Invoke(() =>
                {
                    GoButton.Content = new PackIcon { Kind = PackIconKind.Refresh };
                    GoButton.IsEnabled = true;
                });
            }
            else
            {
                Message = "准备就绪";
                this.Dispatcher.Invoke(() =>
                {
                    GoButton.Content = new PackIcon { Kind = PackIconKind.Play };
                    GoButton.IsEnabled = true;
                });
            }
        }

        private void Initworker_DoWork(object sender, DoWorkEventArgs e)
        {
            var res = Courses.CourseQuery(tui);
            if (!res.success)
            {
                tui = "";
                Jxb_tui = "无";
            }
            else
                Jxb_tui = tui + "(" + res.jxb[0].jsxx + ")";
            Jxb_tui = "\n" + Jxb_tui;

            Jxb_qiang = "";
            foreach (var j in qiang)
            {
                Jxb_qiang += "\n";
                var res1 = Courses.CourseQuery(j);
                if (res1.success)
                    Jxb_qiang += j + "(" + res1.jxb[0].jsxx + ")";
                else
                    Jxb_qiang += j + "(" + "未知" + ")";
            }

            var res2 = Courses.CourseQuery(Kch);
            if (res2.success)
            {
                Kcmc = res2.jxb[0].kcmc;
            }
                
            else
            {
                initfail = true;
            }
        }

        private void Ti_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ti != null)
                ti.Stop();
            else
                return;
            var res = Go();
            if (!res)
            {
                ti = null;
                this.Dispatcher.Invoke(() =>
                {
                    GoButton.Content = new PackIcon { Kind = PackIconKind.Refresh };
                });
                
                return;
            }
            if (ti != null)
            {
                this.Dispatcher.Invoke(startani);
                ti.Interval = Interval;
                ti.Start();
            }
                
        }

        private bool Go()
        {
            Times++;
            TextTime = DateTime.Now.ToString("T");
            var res = Common.CourseQuery(Kch);
            if (!res.success)
            {
                Message = res.message;
                return false;
            }

            Message = "";
            bool contain = false;
            foreach (var jxb in res.jxb)
            {
                if (qiang.Contains(jxb.jxbmc))
                {
                    contain = true;
                    if (int.Parse(jxb.yxzrs) < int.Parse(jxb.jxbrl))
                    {
                        JxbQueryResult res1 = new JxbQueryResult();
                        if (tui != "")
                        {
                            res1 = Common.CourseQuery(tui);
                            if (!res1.success)
                            {
                                Message = "退课失败\n" + res.message;
                                return false;
                            }
                            var res3 = Common.Go退课(res1.jxb[0]);
                        }

                        var res4 = Common.Go选课(jxb);
                        if (res4 == "{\"flag\":\"1\"}")
                        {
                            Message = "抢课成功";
                            return false;
                        }
                        else
                        {
                            if (tui != "")
                            {
                                var res5 = Common.Go选课(res1.jxb[0]);
                            }
                            Message = "抢课失败\n" + res4;
                            return false;
                        }

                    }
                    else Message = "容量已满";
                }
            }
            if (!contain) Message = Message + (Message == "" ? "" : "/") + "暂未开放";
            return true;
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (initfail)
            {
                Message = "正在初始化……";
                GoButton.IsEnabled = false;
                initworker = new BackgroundWorker();
                initworker.DoWork += Initworker_DoWork;
                initworker.RunWorkerCompleted += Initworker_RunWorkerCompleted;
                initworker.RunWorkerAsync();
                return;
            }
            if (ti==null)
            {
                ti = new Timer();
                ti.Interval = Interval;
                ti.Elapsed += Ti_Elapsed;
                ti.Start();
                startani();
                GoButton.Content = new PackIcon { Kind = PackIconKind.Stop };
            }
            else
            {
                ti = null;
                GoButton.Content = new PackIcon { Kind = PackIconKind.Play };
            }
        }

        private void startani()
        {
            var da = new DoubleAnimation(0, 100, TimeSpan.FromMilliseconds(Interval));
            GoButton.BeginAnimation(ButtonProgressAssist.ValueProperty, da);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            OnPanelClose(this);
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
