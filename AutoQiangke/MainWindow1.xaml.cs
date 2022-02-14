using AutoQiangke.Helpers;
using AutoQiangke.Models;
using AutoQiangke.Service;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AutoQiangke
{
    /// <summary>
    /// AutoQiangke.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
        }

        List<TaskStorage> tasks=new List<TaskStorage>();

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new UserControl1();
            tmp.uuid = Common.GetRandomString(6, true, true, false, false, "");
            MainStackPanel.Children.Add(tmp);
            tmp.init(Text_kch.Text, Text_jxb.Text, Text_tui.Text);
            tmp.OnPanelClose += Tmp_OnPanelClose;

            var ts = new TaskStorage();
            ts.uuid = tmp.uuid;
            ts.kch = Text_kch.Text;
            ts.jxb = Text_jxb.Text;
            ts.tui = Text_tui.Text;
            tasks.Add(ts);

            IniHelper.SetKeyValue("main", "tasks", Get_tasks_string(), IniHelper.inipath);
            IniHelper.SetKeyValue(tmp.uuid, "kch", Text_kch.Text, IniHelper.inipath);
            IniHelper.SetKeyValue(tmp.uuid, "jxb", Text_jxb.Text, IniHelper.inipath);
            IniHelper.SetKeyValue(tmp.uuid, "tui", Text_tui.Text, IniHelper.inipath);

            Text_kch.Text = "";
            Text_jxb.Text = "";
            Text_tui.Text = "";
        }

        private void Tmp_OnPanelClose(UserControl1 sender)
        {
            MainStackPanel.Children.Remove(sender);
            foreach (var task in tasks)
                if (task.uuid == sender.uuid)
                {
                    tasks.Remove(task);
                    break;
                }
            IniHelper.SetKeyValue("main", "tasks", Get_tasks_string(), IniHelper.inipath);
            IniHelper.SetKeyValue(sender.uuid, "", "", IniHelper.inipath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var l = IniHelper.GetKeyValue("main", "tasks", "", IniHelper.inipath).Split(',');
            foreach (var uuid in l)
            {
                if (uuid == "") continue;
                var ts = new TaskStorage();
                ts.uuid = uuid;
                ts.kch= IniHelper.GetKeyValue(uuid, "kch", "", IniHelper.inipath);
                ts.jxb = IniHelper.GetKeyValue(uuid, "jxb", "", IniHelper.inipath);
                ts.tui = IniHelper.GetKeyValue(uuid, "tui", "", IniHelper.inipath);
                tasks.Add(ts);

                var tmp = new UserControl1();
                tmp.uuid = uuid;
                MainStackPanel.Children.Add(tmp);
                tmp.init(ts.kch, ts.jxb, ts.tui);
                tmp.OnPanelClose += Tmp_OnPanelClose;
            }
        }

        private string Get_tasks_string()
        {
            var res = "";
            for (int i = 0; i < tasks.Count; i++)
                res += tasks[i].uuid + (i == tasks.Count - 1 ? "" : ",");
            return res;
        }

        private void ButtonXuanke_Click(object sender, RoutedEventArgs e)
        {
            var j = new Jxb() { do_jxb_id = Text_jxb_ids.Text, kch_id = Text_kch_id.Text };
            var res= Courses.Go选课(j);
            //MessageBox.Show(res);
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            //Jac.Login(Text_account.Text, Text_password.Password);
        }

        private void ButtonCookieLogin_Click(object sender, RoutedEventArgs e)
        {
            //Common.mycookie=Text_cookie.Text;
        }

        private void Button_tiyu_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            d.Add("Accept-Encoding", "gzip, deflate, br");
            d.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            d.Add("Cache-Control", "no-cache");
            d.Add("Connection", "keep-alive");
            d.Add("Cookie", Jac.mycookie);
            d.Add("Host", "i.sjtu.edu.cn");
            d.Add("Origin", "https://i.sjtu.edu.cn");
            d.Add("Pragma", "no-cache");
            d.Add("Referer", "https://i.sjtu.edu.cn/xtgl/index_initMenu.html?jsdm=xs&_t=1640708657934");
            d.Add("sec-ch-ua", "\"Chromium\";v=\"94\", \"Microsoft Edge\";v=\"94\", \"; Not A Brand\";v=\"99\"");
            d.Add("sec-ch-ua-mobile", "?0");
            d.Add("sec-ch-ua-platform", "\"Windows\"");
            d.Add("Sec-Fetch-Dest", "document");
            d.Add("Sec-Fetch-Mode", "navigate");
            d.Add("Sec-Fetch-Site", "same-origin");
            d.Add("Sec-Fetch-User", "1");
            d.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.9 Safari/537.36");
            d.Add("X-Requested-With", "XMLHttpRequest");
            var res = Web.Get("https://i.sjtu.edu.cn/xsxk/zzxkyzb_cxZzxkYzbIndex.html?gnmkdm=N253512&layout=default&su=521021910900", d);
            if (res.success)
            {
                var k = 0;
                List<string> list = new List<string>();
                k = res.result.IndexOf("queryCourse(");
                while (k != -1)
                {
                    var k1 = res.result.IndexOf("</a>", k);
                    list.Add(res.result.Substring(k, k1 - k));
                    k = res.result.IndexOf("queryCourse(", k1 + 1);
                }
                foreach (var i in list)
                    if(i.Contains("体育"))
                    {
                        var k3 = i.IndexOf("(");
                        var k4 = i.IndexOf(")");
                        var tmp = i.Substring(k3 + 1, k4 - k3 - 1);
                        var tmp1 = tmp.Replace("'","").Split(',');
                        Jac.xkkz_id = tmp1[2];

                        d.Clear();
                        d.Add("Accept", "text/html, */*; q=0.01");
                        d.Add("Accept-Encoding", "gzip, deflate, br");
                        d.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
                        d.Add("Cache-Control", "no-cache");
                        d.Add("Connection", "keep-alive");
                        d.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                        d.Add("Cookie", Jac.mycookie);
                        d.Add("Host", "i.sjtu.edu.cn");
                        d.Add("Origin", "https://i.sjtu.edu.cn");
                        d.Add("Pragma", "no-cache");
                        d.Add("Referer", "https://i.sjtu.edu.cn/xsxk/zzxkyzb_cxZzxkYzbIndex.html?gnmkdm=N253512&layout=default&su=521021910900");
                        d.Add("sec-ch-ua", "\"Chromium\";v=\"94\", \"Microsoft Edge\";v=\"94\", \"; Not A Brand\";v=\"99\"");
                        d.Add("sec-ch-ua-mobile", "?0");
                        d.Add("sec-ch-ua-platform", "\"Windows\"");
                        d.Add("Sec-Fetch-Dest", "empty");
                        d.Add("Sec-Fetch-Mode", "cors");
                        d.Add("Sec-Fetch-Site", "same-origin");
                        d.Add("Sec-Fetch-User", "1");
                        d.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.9 Safari/537.36");
                        d.Add("X-Requested-With", "XMLHttpRequest");
                        var res1 = Web.Post("https://i.sjtu.edu.cn/xsxk/zzxkyzb_cxZzxkYzbDisplay.html?gnmkdm=N253512&su=521021910900", d , "xkkz_id=" + Jac.xkkz_id  + "&xszxzt=1&kspage=0&jspage=0", false);
                        if (res1.success)
                        {
                            var kk = 0;
                            kk = res1.result.IndexOf("id=\"bklx_id\"");
                            var kk1 = res1.result.IndexOf("/>", kk);
                            var tmp2= res1.result.Substring(kk, kk1 - kk+1);
                            var p = tmp2.IndexOf("value=");
                            var q= tmp2.IndexOf("/");
                            var tmp3 = tmp2.Substring(p + 7, q - p - 8);
                            Jac.bklx_id = tmp3;
                            
                        }
                    }
            }

        }
    }
    public class TaskStorage
    {
        public string uuid;
        public string kch;
        public string jxb;
        public string tui;
    }
}
