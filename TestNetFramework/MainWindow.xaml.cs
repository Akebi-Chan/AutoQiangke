using AutoQiangke.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestNetFramework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Login();
        }

        public string Login()
        {
            var url1 = "https://i.sjtu.edu.cn/jaccountlogin";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url1);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            //req.Headers["Accept-Encoding"] = "identity;q=1, *;q=0";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36 Edg/93.0.961.47";
            //req.Referer = "https://courses.sjtu.edu.cn/";
            req.AllowAutoRedirect = false;
            req.Timeout = 10 * 1000;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            string url2 = resp.Headers["Location"];
            CookieCollection cc1;
            if (resp.StatusCode == HttpStatusCode.Redirect)
            {
                url2 = resp.Headers["Location"];
                HttpCookie c;
                HttpCookie.TryParse(resp.Headers["Set-Cookie"], out c);
                
                cc1 = CookieHelper.GetCookiesByHeader(resp.Headers["Set-Cookie"]);
                Text1.Text = cc1.Count + "\n" + resp.Headers["Set-Cookie"];
            }
            return "";
        }
    }
}
