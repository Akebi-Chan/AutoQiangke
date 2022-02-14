using AutoQiangke.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// WelcomeView.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WelcomeText.Text = GetGreetings();
        }

        private string GetGreetings()
        {
            var h = DateTime.Now.Hour;
            if (h < 6)
            {
                return "凌晨好！";
            }
            else if (h < 9)
            {
                return "早上好！";
            }
            else if (h < 12)
            {
                return "中午好！";
            }
            else if (h < 17)
            {
                return "下午好！";
            }
            else if (h < 19)
            {
                return "傍晚好！";
            }
            else if (h < 22)
            {
                return "晚上好！";
            }
            else
            {
                return "深夜好！";
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Common.RegionSwitchManager.Publish("Predict");
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Common.RegionSwitchManager.Publish("Task");
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Common.RegionSwitchManager.Publish("TimeMachine");
        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", BasicInfo.BaseUri);
            Process.Start(startInfo);
        }
    }
}
