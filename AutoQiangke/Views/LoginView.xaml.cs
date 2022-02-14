using AutoQiangke.Helpers;
using AutoQiangke.Models;
using AutoQiangke.Service;
using AutoQiangke.Shared;
using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoQiangke.Views
{
    /// <summary>
    /// Welcome.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl, INotifyPropertyChanged
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                this.RaisePropertyChanged("Account");
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.RaisePropertyChanged("Password");
            }
        }
        private int methodIndex;
        public int MethodIndex
        {
            get { return methodIndex; }
            set
            {
                methodIndex = value;
                this.RaisePropertyChanged("MethodIndex");
            }
        }
        private bool isSaveInfo;
        public bool IsSaveInfo
        {
            get { return isSaveInfo; }
            set
            {
                isSaveInfo = value;
                this.RaisePropertyChanged("IsSaveInfo");
            }
        }
        private bool isAutoLogin;
        public bool IsAutoLogin
        {
            get { return isAutoLogin; }
            set
            {
                isAutoLogin = value;
                this.RaisePropertyChanged("IsAutoLogin");
            }
        }
        private string errorText;
        public string ErrorText
        {
            get { return errorText; }
            set
            {
                errorText = value;
                this.RaisePropertyChanged("ErrorText");
            }
        }
        private string cookie;
        public string Cookie
        {
            get { return cookie; }
            set
            {
                cookie = value;
                this.RaisePropertyChanged("Cookie");
            }
        }
        public bool Islogin
        {
            get { return Jac.islogin; }
            set
            {
                Jac.islogin = value;
                this.RaisePropertyChanged("Islogin");
            }
        }
        public string StuName
        {
            get { return Jac.name; }
            set
            {
                Jac.name = value;
                this.RaisePropertyChanged("StuName");
            }
        }
        public string Xh
        {
            get { return Jac.xh_id; }
            set
            {
                Jac.xh_id = value;
                this.RaisePropertyChanged("Xh");
            }
        }


        private BackgroundWorker worker;

        private void TextAPIHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MethodIndex = int.Parse(IniHelper.GetKeyValue("Login", "MethodIndex", "0", IniHelper.inipath));
            IsSaveInfo = Boolean.Parse(IniHelper.GetKeyValue("Login", "IsSaveInfo", "false", IniHelper.inipath));
            IsAutoLogin = Boolean.Parse(IniHelper.GetKeyValue("Login", "IsAutoLogin", "false", IniHelper.inipath));

            if (isSaveInfo)
            {
                Account = IniHelper.GetKeyValue("Login", "Account", "", IniHelper.inipath);
                Password = IniHelper.GetKeyValue("Login", "Password", "", IniHelper.inipath);
                Cookie = IniHelper.GetKeyValue("Login", "Cookie", "", IniHelper.inipath);
            }

            worker = new BackgroundWorker();
            worker.DoWork += Login_DoWork;
            worker.RunWorkerCompleted += Login_RunWorkerCompleted;

            if (IsAutoLogin)
            {
                worker.RunWorkerAsync();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        private void Login_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(() => {
                if (MethodIndex == 0)
                {
                    ButtonProgressAssist.SetValue(AccountLoginButton, -1);
                    ButtonProgressAssist.SetIsIndicatorVisible(AccountLoginButton, true);
                    ButtonProgressAssist.SetIsIndeterminate(AccountLoginButton, true);
                    AccountLoginButton.Content = "正在登录...";
                }
                if (MethodIndex == 1)
                {
                    ButtonProgressAssist.SetValue(CookieLoginButton, -1);
                    ButtonProgressAssist.SetIsIndicatorVisible(CookieLoginButton, true);
                    ButtonProgressAssist.SetIsIndeterminate(CookieLoginButton, true);
                    CookieLoginButton.Content = "正在登录...";

                }
                this.IsEnabled = false;
            });

            SaveSettings();
            e.Result = DoLogin();
        }

        private void Login_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var res = (bool)e.Result;
            this.Dispatcher.Invoke(()=> {
                ButtonProgressAssist.SetIsIndicatorVisible(CookieLoginButton, false);
                ButtonProgressAssist.SetIsIndicatorVisible(AccountLoginButton, false);
                CookieLoginButton.Content = "登录";
                AccountLoginButton.Content = "登录";
            });
            this.IsEnabled = true;
            if (res)
            {
                SaveInfo();
                Xh = Jac.xh_id;
                StuName = Jac.name;
                Application.Current.Resources["stuname"] = "已登录：" + StuName;
                Islogin = true;
                Thread t = new Thread(new ThreadStart(GetChosen));
                t.Start();
                Common.RegionSwitchManager.Publish<string>("Welcome");
                Common.SnackbarManager.Publish(new MySnackBarMessage("登录成功！",TimeSpan.FromSeconds(3)));
            }
        }

        public bool DoLogin()
        {
            if (MethodIndex == 0)
            {
                Jac.LoginResult res = new Jac.LoginResult();
                int i = 0;
                do
                {
                    res = Jac.Login(Account, Password);
                    i++;
                } while (res.state == Jac.LoginState.captchafail && i < 3);
                
                if (res.state != Jac.LoginState.success)
                {
                    ErrorText = res.result;
                    Common.SnackbarManager.Publish(new MySnackBarMessage(ErrorText, TimeSpan.FromSeconds(3)));
                    return false;
                }
                var res1 = Jac.ValidateLogin();
                if (res1)
                    return true;
                else
                {
                    ErrorText = "奇怪，登录成功，获取网页失败";
                    return false;
                }
            }
            else
            {
                Jac.mycookie = Cookie;
                var res1 = Jac.ValidateLogin();
                if (res1)
                    return true;
                else
                {
                    ErrorText = "获取信息失败，请检查Cookie是否有效";
                    Common.SnackbarManager.Publish(new MySnackBarMessage(ErrorText, TimeSpan.FromSeconds(3)));
                    return false;
                }
            }
        }

        private void GetChosen()
        {
            var res = Course.GetChosenJxbs();
            if (!res.success)
                MessageBox.Show("获取已选课程失败！");
            else
                foreach (var task in Task.tasks)
                    task.CheckIsChosenJxbs();
        }

        public void SaveInfo()
        {
            if (isSaveInfo)
            {
                IniHelper.SetKeyValue("Login", "Account", Account, IniHelper.inipath);
                IniHelper.SetKeyValue("Login", "Password", Password, IniHelper.inipath);
                IniHelper.SetKeyValue("Login", "Cookie", Cookie, IniHelper.inipath);
            }
            if (GlobalSettings.saveCookie)
            {
                IniHelper.SetKeyValue("Login", "LastCookie", Jac.mycookie, IniHelper.inipath);
            }
        }

        public void SaveSettings()
        {
            IniHelper.SetKeyValue("Login", "MethodIndex", MethodIndex.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("Login", "IsSaveInfo", IsSaveInfo.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("Login", "IsAutoLogin", IsAutoLogin.ToString(), IniHelper.inipath);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Islogin = false;
            Jac.islogin = false;
            Jac.mycookie = "";
            Application.Current.Resources["stuname"] = "未登录";
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
