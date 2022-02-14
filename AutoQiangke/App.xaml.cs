using AutoQiangke.Service;
using AutoQiangke;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using AutoQiangke.Helpers;
using AutoQiangke.Views;
using AutoQiangke.Shared;
using AutoQiangke.Models;

namespace AutoQiangke
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SplashWindow s = new SplashWindow();
            s.Message = "读取配置";
            s.Show();
            MainWindow m = new MainWindow();
            this.MainWindow = m;
            //1.读取配置
            base.OnStartup(e);
            ThemeHelper.ChangeHue("#0077d0");
            GlobalSettings.ReadSettings();
            BasicInfo.ReadBaseUri();
            if (BasicInfo.BaseUri == "")
            {
                BaseUriInputWindow inputWindow = new BaseUriInputWindow();
                inputWindow.ShowDialog();
                BasicInfo.BaseUri = inputWindow.BaseUri;
                BasicInfo.SaveBaseUri();
            }
            //2.初始化板块和任务
            s.Message = "初始化板块和任务";
            s.UpdateUI();
            Jac.Init();
            Task.Init();
            Block.Init();
            Storage.ReadData();
            //3.自动登录
            if (GlobalSettings.saveCookie)
            {
                s.Message = "自动登录";
                s.UpdateUI();
                var res = Jac.TryLastCookie();
                if (res)
                {
                    m.OnSwitchView("Welcome");
                    m.OnRecieveMessage(new MySnackBarMessage("自动登录成功（使用Cookie）", TimeSpan.FromSeconds(2)));
                    s.Message = "检查选课是否开始";
                    s.UpdateUI();
                    var res1 = BasicInfo.GetBasicInfo();
                    if (res1.type == BasicInfoResultType.Started)
                    {
                        if (BasicInfo.IsXKSJ)
                            ThemeHelper.ChangeHue("#673ab7");
                        else
                            ThemeHelper.ChangeHue("#0077d0");
                    }
                }
                else
                    m.OnSwitchView("Login");
            }
            //4.设定学年和学期
            if (!BasicInfo.IsXKSJ)
                BasicInfo.ReadXnXq();
            if (BasicInfo.xkxnm == null || BasicInfo.xkxqm == null)
            {
                s.Message = "检查学年和学期设置";
                s.UpdateUI();
                TermInputWindow inputWindow = new TermInputWindow();
                inputWindow.ShowDialog();
                BasicInfo.xkxnm = inputWindow.SelectedXn.value;
                BasicInfo.xkxnmc = inputWindow.SelectedXn.display;
                BasicInfo.xkxqm = inputWindow.SelectedXq.value;
                BasicInfo.xkxqmc = inputWindow.SelectedXq.display;
                BasicInfo.SaveXnXq();
            }
            //5.获取已选课程
            s.Message = "获取已选课程";
            s.UpdateUI();
            if (Jac.islogin)
            {
                var res2 = Course.GetChosenJxbs();
                if (!res2.success)
                    MessageBox.Show("获取已选课程失败！");
            }

            //6.初始化已选课程
            foreach (var task in Task.tasks)
                task.CheckIsChosenJxbs();

            //5.启动
            s.Close();
            m.Show();
        }
    }
}
