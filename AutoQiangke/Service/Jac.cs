using AutoQiangke.Helpers;
using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace AutoQiangke.Service
{
    public static class Jac
    {
        public static string mycookie;
        public static string finalJESSIONID, finalkc;
        public static string xh_id;
        public static string name;
        public static bool islogin;

        public static void Init()
        {

        }

        public static LoginResult Login(string jaccount, string password)
        {
            try
            {
                CookieContainer cc = new CookieContainer();

                #region 1. /jaccountlogin
                var url1 = BasicInfo.BaseUri + "/jaccountlogin";
                HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(url1);
                AddCommonHeaders(req1);

                HttpWebResponse resp1 = (HttpWebResponse)req1.GetResponse();

                if (resp1.StatusCode != HttpStatusCode.Redirect)
                    return new LoginResult(LoginState.fail, "1. /jaccountlogin 出错");

                string url2 = resp1.Headers["Location"];
                cc.SetCookies(new Uri("https://" + req1.Host), resp1.Headers["Set-Cookie"]);

                #endregion

                #region 2. /oauth2/authorize
                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(url2);
                AddCommonHeaders(req2);

                HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();


                if (resp2.StatusCode != HttpStatusCode.Redirect)
                    return new LoginResult(LoginState.fail, "2. /oauth2/authorize 错误");

                string url3, uuid;
                url3 = resp2.Headers["Location"];
                uuid = Guid.NewGuid().ToString();
                cc.SetCookies(new Uri("https://" + req2.Host), resp2.Headers["Set-Cookie"]);
                #endregion

                #region 3. /jaccount/jalogin
                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create(url3);
                req3.CookieContainer = cc;
                AddCommonHeaders(req3);

                HttpWebResponse resp3 = (HttpWebResponse)req3.GetResponse();

                if (resp3.StatusCode != HttpStatusCode.OK)
                    return new LoginResult(LoginState.fail, "3. /jaccount/jalogin 失败");

                cc.SetCookies(new Uri("https://" + req3.Host), resp3.Headers["Set-Cookie"]);
                #endregion

                #region 4. /jaccount/captcha
                HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("https://jaccount.sjtu.edu.cn/jaccount/captcha?uuid=" + uuid + "&t=" + Common.GetTimeStamp());
                req4.CookieContainer = cc;
                AddCommonHeaders(req4);

                HttpWebResponse resp4 = (HttpWebResponse)req4.GetResponse();
                Stream streamcaptcha = resp4.GetResponseStream();
                #endregion

                #region 4.5. Captcha-Resize
                Stream outputstream = new MemoryStream();
                using (var image = new ImageMagick.MagickImage(streamcaptcha))
                {
                    image.Resize(new ImageMagick.MagickGeometry(100, 40) { IgnoreAspectRatio = true });
                    image.Write(outputstream, ImageMagick.MagickFormat.Jpeg);
                    
                }
                outputstream.Position = 0;
                #endregion

                #region 5. /captcha-solver
                HttpWebRequest request_captcha = (HttpWebRequest)WebRequest.Create("https://plus.sjtu.edu.cn/captcha-solver/");
                #region 初始化请求对象
                request_captcha.Method = "POST";
                request_captcha.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request_captcha.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
                request_captcha.Timeout = 10 * 1000;
                #endregion

                #region 处理Form表单请求内容
                string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
                request_captcha.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                //请求流
                var postStream = new MemoryStream();

                //文件数据模板
                string fileFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                    "\r\nContent-Type: application/octet-stream" +
                    "\r\n\r\n";

                string formdata = null;
                //上传文件
                formdata = string.Format(
                    fileFormdataTemplate,
                    "image", //表单键
                    "captcha.jpg");

                //统一处理
                byte[] formdataBytes = null;
                //第一行不需要换行
                if (postStream.Length == 0)
                    formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                else
                    formdataBytes = Encoding.UTF8.GetBytes(formdata);
                postStream.Write(formdataBytes, 0, formdataBytes.Length);

                //写入文件内容
                using (var stream = outputstream)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        postStream.Write(buffer, 0, bytesRead);
                    }
                }

                //结尾
                var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                postStream.Write(footer, 0, footer.Length);

                #endregion

                request_captcha.ContentLength = postStream.Length;

                #region 输入二进制流
                if (postStream != null)
                {
                    postStream.Position = 0;
                    //直接写入流
                    Stream requestStream = request_captcha.GetRequestStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }

                    ////debug
                    //postStream.Seek(0, SeekOrigin.Begin);
                    //StreamReader sr = new StreamReader(postStream);
                    //var postStr = sr.ReadToEnd();
                    postStream.Close();//关闭文件访问
                }
                #endregion

                string captcharesultraw = "";
                HttpWebResponse response = (HttpWebResponse)request_captcha.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        captcharesultraw = myStreamReader.ReadToEnd();
                    }
                }

                var captcharesult = Newtonsoft.Json.JsonConvert.DeserializeObject<captcharesult>(captcharesultraw);
                if (captcharesult.error != "succeed")
                    return new LoginResult(LoginState.fail, "captchar识别错误");
                #endregion

                #region 6. /jaccount/ulogin
                Dictionary<string, string> dic = Common.ParseQueryString(url3);

                dic.Add("user", jaccount);
                dic.Add("pass", password);
                dic.Add("captcha", captcharesult.result);
                dic.Add("v", "");
                dic.Add("uuid", uuid);
                dic.Add("g-recaptcha-response", "");

                StringBuilder builder = new StringBuilder();
                if (dic.Count > 0)
                {
                    int i = 0;
                    foreach (var item in dic)
                    {
                        if (i > 0)
                            builder.Append("&");
                        builder.AppendFormat("{0}={1}", item.Key, item.Value);
                        i++;
                    }
                }

                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("https://jaccount.sjtu.edu.cn/jaccount/ulogin");
                req5.Method = "POST";
                AddCommonHeaders(req5);
                req5.ContentType = "application/x-www-form-urlencoded";
                req5.CookieContainer = cc;
                req5.Referer = url3;
                req5.Headers["Cache-Control"] = "max-age=0";
                req5.Headers["Origin"] = "https://jaccount.sjtu.edu.cn";
                req5.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Default);

                #region 添加Post 参数

                byte[] data = Encoding.Default.GetBytes(builder.ToString());
                req5.ContentLength = data.Length;
                using (Stream reqStream = req5.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion

                HttpWebResponse resp5 = (HttpWebResponse)req5.GetResponse();
                if (!(resp5.StatusCode == HttpStatusCode.Redirect))
                    return new LoginResult(LoginState.fail, "7. /jaccount/ulogin 错误");
                string url6 = "https://jaccount.sjtu.edu.cn" + resp5.Headers["Location"];
                if (resp5.Headers.AllKeys.Contains("Set-Cookie"))
                    cc.SetCookies(new Uri("https://" + req5.Host), resp5.Headers["Set-Cookie"]);

                #endregion

                #region 7. 再次 /jaccount/jalogin

                HttpWebRequest req6 = (HttpWebRequest)WebRequest.Create(url6);
                req6.Method = "GET";
                req6.CookieContainer = cc;
                req6.Referer = url6;
                AddCommonHeaders(req6);

                HttpWebResponse resp6 = (HttpWebResponse)req6.GetResponse();

                if (resp6.StatusCode == HttpStatusCode.OK)
                {
                    var body = Web.GetResponseBody(resp6, resp6.GetResponseStream());
                    if (body.success)
                    {
                        if (body.result.Contains("请正确填写你的用户名和密码"))
                        {
                            return new LoginResult(LoginState.fail, "用户名或密码错误，请重试！（注意区分大小写）");
                        }
                        if (body.result.Contains("请正确填写验证码"))
                        {
                            return new LoginResult(LoginState.captchafail, "SJTU-PLUS的自动识别验证码已经连续3次错误……请重试！");
                        }
                    }

                }
                if (!(resp6.StatusCode == HttpStatusCode.Redirect))
                    return new LoginResult(LoginState.fail, "再次 /jaccount/jalogin 错误");
                string url7 = resp6.Headers["Location"];
                #endregion

                #region 8. /oauth2/authorize
                HttpWebRequest req7 = (HttpWebRequest)WebRequest.Create(url7);
                req7.CookieContainer = cc;
                req7.Referer = url6;
                AddCommonHeaders(req7);

                HttpWebResponse resp7 = (HttpWebResponse)req7.GetResponse();
                if (!(resp7.StatusCode == HttpStatusCode.Redirect))
                    return new LoginResult(LoginState.fail, "9. /oauth2/authorize 错误");
                string url8 = resp7.Headers["Location"];
                #endregion

                #region 9. /jaccountlogin
                finalJESSIONID = Common.GetRandomString(32, true, false, true, false, "");
                HttpWebRequest req8 = (HttpWebRequest)WebRequest.Create(url8);
                req8.Headers["cookie"] = "JSESSIONID=" + finalJESSIONID;
                AddCommonHeaders(req8);

                HttpWebResponse resp8 = (HttpWebResponse)req8.GetResponse();
                if (!(resp8.StatusCode == HttpStatusCode.Redirect))
                    return new LoginResult(LoginState.fail, "10. /jaccountlogin 错误");
                string url9 = resp8.Headers["Location"];

                #endregion

                #region 10. 再次/jaccountlogin
                HttpWebRequest req9 = (HttpWebRequest)WebRequest.Create(url9);
                req9.CookieContainer = cc;
                AddCommonHeaders(req9);

                HttpWebResponse resp9 = (HttpWebResponse)req9.GetResponse();
                if (!(resp9.StatusCode == HttpStatusCode.Redirect))
                    return new LoginResult(LoginState.fail, "11. 再次/jaccountlogin 错误");
                string url10 = resp9.Headers["Location"];

                #endregion

                #region 11. 得到Cookie
                if (url10 != "/xtgl/login_slogin.html")
                    return new LoginResult(LoginState.fail, "系统错误！");

                cc.SetCookies(new Uri("https://" + req9.Host), resp9.Headers["Set-Cookie"]);
                finalJESSIONID = cc.GetCookies(new Uri("https://" + req9.Host))["JSESSIONID"].Value;
                finalkc = cc.GetCookies(new Uri("https://" + req9.Host))["kc@" + BasicInfo.Host].Value;
                mycookie = "JSESSIONID=" + finalJESSIONID + "; " + "kc@" + BasicInfo.Host + "=" + finalkc;
                #endregion
            }
            catch (Exception ex)
            {
                return new LoginResult(LoginState.fail, ex.Message);
            }
            
            return new LoginResult(LoginState.success, "登录成功！");
        }

        public static bool ValidateLogin()
        {
            var url = BasicInfo.BaseUri + "/xtgl/index_initMenu.html";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            AddCommonHeaders(req);
            req.Timeout = 5000;
            req.Headers["Cookie"] = mycookie;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode != HttpStatusCode.OK)
                    return false;
                var body = Web.GetResponseBody(resp, resp.GetResponseStream());
                if (!body.success)
                    return false;

                var nameregex = new Regex(@"(?<=<img src=""/zftal-ui-.+/assets/images/user_logo\.jpg"" width="".+"" height="".+""><span><font color=""white"">&nbsp;).+(?=</font></span>)");
                var nameregexresult = nameregex.Match(body.result);
                if (nameregexresult != null)
                    name = nameregexresult.Value;

                var xhregex = new Regex(@"(?<=<input type=""hidden"" id=""sessionUserKey"" value="")[0-9]*(?="" />)");
                var xhregexresult = xhregex.Match(body.result);
                if (xhregexresult != null)
                    xh_id = xhregexresult.Value;

                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        public static bool TryLastCookie()
        {
            mycookie = IniHelper.GetKeyValue("Login", "LastCookie", "", IniHelper.inipath);
            islogin = ValidateLogin();
            if (islogin)
            {
                Application.Current.Resources["stuname"] = "已登录：" + name;
            }
            return islogin;
        }

        public static void AddCommonHeaders(HttpWebRequest req)
        {
            req.KeepAlive = true;
            req.AllowAutoRedirect = false;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36 Edg/93.0.961.47";
            req.Headers["Upgrade-Insecure-Requests"] = "1";
            req.Headers["Sec-Fetch-Site"] = "cross-site";
            req.Headers["Sec-Fetch-Mode"] = "navigate";
            req.Headers["Sec-Fetch-User"] = "?1";
            req.Headers["Sec-Fetch-Dest"] = "document";
            req.Headers["sec-ch-ua"] = "\"Chromium\";v=\"94\", \"Microsoft Edge\";v=\"94\", \"; Not A Brand\";v=\"99\"";
            req.Headers["sec-ch-ua-mobile"] = "?0";
            req.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            req.Headers["Accept-Encoding"] = "gzip, deflate, br";
            req.Headers["Accept-Language"] = "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7";
        }

        public class captcharesult
        {
            public string error;
            public string result;
        }

        public class LoginResult
        {
            public LoginState state;
            public string result;

            public LoginResult() { }
            public LoginResult(LoginState state, string result)
            {
                this.state = state;
                this.result = result;
            }
        }

        public enum LoginState
        {
            success, fail, captchafail
        }
    }
}
