using AutoQiangke.Helpers;
using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace AutoQiangke.Service
{
    public static class BasicInfo
    {
        #region fields
        public static string BaseUri;

        public static string xqh_id;
        public static string njdm_id_1;
        public static string zyh_id_1;
        public static string zyh_id;
        public static string zyfx_id;
        public static string njdm_id;
        public static string bh_id;
        public static string xbm;
        public static string xslbdm;
        public static string ccdm;
        public static string xz;
        public static string xsbj;
        public static string xkxnm;
        public static string xkxqm;
        public static string xkxnmc;
        public static string xkxqmc;
        public static string jg_id;//jg_id_1
        public static string iskxk;//是否是选课时间
        public static string xszxzt;//仅用于查询板块
        #endregion

        public static bool IsXKSJ => iskxk == "1";

        public static string Host
        {
            get {
                Uri uri = new Uri(BasicInfo.BaseUri);
                return uri.Host; 
            }
        }


        #region 蹲守开始
        private static Timer ti;
        public static bool isstart;
        public static int interval = 1000;

        public delegate void OnStopWaitHandler();
        public static event OnStopWaitHandler OnStopWait;
        public delegate void OnGoAnimationHandler();
        public static event OnGoAnimationHandler OnGoAnimation;

        public static CommonResult ReadBaseUri()
        {
            BaseUri = IniHelper.GetKeyValue("Main", "BaseUri", "", IniHelper.inipath);
            return new CommonResult(true, "");
        }
        public static CommonResult SaveBaseUri()
        {
            IniHelper.SetKeyValue("Main", "BaseUri", BaseUri, IniHelper.inipath);
            return new CommonResult(true, "");
        }

        public static void ReadXnXq()
        {
            var lastsavetime = IniHelper.GetKeyValue("XnXq", "lastsavetime", null, IniHelper.inipath);
            if (lastsavetime == null || lastsavetime == "") 
                return;
            if (Common.GetTimeStampInt64() - long.Parse(lastsavetime) > TimeSpan.FromDays(5).TotalSeconds)
                return;
            xkxnm = IniHelper.GetKeyValue("XnXq", "xkxnm", null, IniHelper.inipath);
            xkxnmc = IniHelper.GetKeyValue("XnXq", "xkxnmc", null, IniHelper.inipath);
            xkxqm = IniHelper.GetKeyValue("XnXq", "xkxqm", null, IniHelper.inipath);
            xkxqmc = IniHelper.GetKeyValue("XnXq", "xkxqmc", null, IniHelper.inipath);
        }

        public static void SaveXnXq()
        {
            IniHelper.SetKeyValue("XnXq", "lastsavetime", Common.GetTimeStampInt64().ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("XnXq", "xkxnm", xkxnm, IniHelper.inipath);
            IniHelper.SetKeyValue("XnXq", "xkxnmc", xkxnmc, IniHelper.inipath);
            IniHelper.SetKeyValue("XnXq", "xkxqm", xkxqm, IniHelper.inipath);
            IniHelper.SetKeyValue("XnXq", "xkxqmc", xkxqmc, IniHelper.inipath);
        }

        public static BasicInfoResult GetBasicInfo()
        {
            if (!Jac.islogin)
            {
                return new BasicInfoResult(BasicInfoResultType.NotLogin, "请先登录");
            }
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Connection", "keep-alive");
            d.Add("Pragma", "no-cache");
            d.Add("Cache-Control", "no-cache");
            d.Add("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"96\", \"Microsoft Edge\";v=\"96\"");
            d.Add("sec-ch-ua-mobile", "?0");
            d.Add("sec-ch-ua-platform", "\"Windows\"");
            d.Add("Upgrade-Insecure-Requests", "1");
            d.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.9 Safari/537.36");
            d.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            d.Add("Sec-Fetch-Site", "same-origin");
            d.Add("Sec-Fetch-Mode", "navigate");
            d.Add("Sec-Fetch-User", "?1");
            d.Add("Sec-Fetch-Dest", "document");
            d.Add("Referer", BasicInfo.BaseUri + "/xtgl/index_initMenu.html");
            d.Add("Accept-Encoding", "gzip, deflate, br");
            d.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            d.Add("Cookie", Jac.mycookie);
            var getres = Web.Get(BasicInfo.BaseUri + "/xsxk/zzxkyzb_cxZzxkYzbIndex.html?gnmkdm=N253512&layout=default&su=" + Jac.xh_id, d);

            if (!getres.success)
            {
                return new BasicInfoResult(BasicInfoResultType.Fail, "获取基本信息失败\n" + getres.result);
            }

            var reg = new Regex(@"(?<=<input type="".*"" name="")(.*)"" id=""(.*)"" value=""(.*)(?=""/>)");
            var regres = reg.Matches(getres.result);
            foreach (Match i in regres)
            {
                if (i.Groups[1].Value == "xqh_id") xqh_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "njdm_id_1") njdm_id_1 = i.Groups[3].Value;
                if (i.Groups[1].Value == "zyh_id_1") zyh_id_1 = i.Groups[3].Value;
                if (i.Groups[1].Value == "zyh_id") zyh_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "zyfx_id") zyfx_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "njdm_id") njdm_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "bh_id") bh_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "xbm") xbm = i.Groups[3].Value;
                if (i.Groups[1].Value == "xslbdm") xslbdm = i.Groups[3].Value;
                if (i.Groups[1].Value == "ccdm") ccdm = i.Groups[3].Value;
                if (i.Groups[1].Value == "xz") xz = i.Groups[3].Value;
                if (i.Groups[1].Value == "xsbj") xsbj = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkxnm") xkxnm = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkxnmc") xkxnmc = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkxqm") xkxqm = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkxqmc") xkxqmc = i.Groups[3].Value;
                if (i.Groups[1].Value == "jg_id_1") jg_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "iskxk") iskxk = i.Groups[3].Value;
                if (i.Groups[1].Value == "xszxzt") xszxzt = i.Groups[3].Value;
            }

            if (iskxk != "1")
                return new BasicInfoResult(BasicInfoResultType.NotBegin, "当前不是选课时间");

            var blockreg = new Regex(@"(?<=<a href=""javascript:void\(0\)"" onclick=""queryCourse\(this,)'(.+)','(.+)','(.+)','(.+)'\)"" role=""tab"" data-toggle=""tab"">(.+)(?=</a>)");
            var blockregres = blockreg.Matches(getres.result);
            List<BlockInfo> list = new List<BlockInfo>();
            foreach (Match i in blockregres)
            {
                list.Add(new BlockInfo(BlockType.Original,i.Groups[5].Value, i.Groups[1].Value, i.Groups[2].Value));
            }


            SaveXnXq();
            Application.Current.Resources["xkxnmc"] = xkxnmc;
            Application.Current.Resources["xkxqmc"] = xkxqmc;
            Common.BlockMessager.Publish(list);
            //Block.MergeBlocks(list);

            return new BasicInfoResult(BasicInfoResultType.Started, "操作成功！");
        }

        public static void StopWait()
        {
            ti = null;
            isstart = false;
            if (IsXKSJ)
                ThemeHelper.ChangeHue("#673ab7");
            else
                ThemeHelper.ChangeHue("#0077d0");
            if (OnStopWait != null) OnStopWait.Invoke();
        }
        private static void Ti_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ti != null)
                ti.Stop();
            else
                return;
            var res = Go();
            Application.Current.Resources["Time"] = DateTime.Now.ToString("G");
            if (!res)
            {
                ti = null;
                StartTasks();
                StopWait();
                return;
            }
            if (ti != null)
            {
                ti.Interval = interval;
                ti.Start();
                OnGoAnimation.Invoke();
            }
        }
        private static void StartTasks()
        {
            foreach (TaskModel t in Task.tasks)
                if (t.State == TaskState.Wait)
                    t.StartRun();
        }
        private static bool Go()
        {
            var res=GetBasicInfo();
            if (res.type != BasicInfoResultType.Started)
                return true;
            else
                return false;
        }
        public static void StartWait()
        {
            isstart = true;
            ti = new Timer();
            ti.Interval = interval;
            ti.Elapsed += Ti_Elapsed;
            ti.Start();
            OnGoAnimation.Invoke();
            ThemeHelper.ChangeHue("#ff5722");
        }
        public static void ToggleWait()
        {
            if (!isstart)
            {
                StartWait();
            }
            else
            {
                StopWait();
            }
        }
        #endregion
    }
    public class BasicInfoResult
    {
        public BasicInfoResultType type;
        public string result;

        public BasicInfoResult(BasicInfoResultType type, string result)
        {
            this.type = type;
            this.result = result;
        }
    }

    public enum BasicInfoResultType
    {
        NotLogin, Fail, NotBegin, Started
    }

    public class XnModel
    {
        public string display { get; set; }
        public string value { get; set; }

        public XnModel(string display, string value)
        {
            this.display = display;
            this.value = value;
        }
    }
    public class XqModel
    {
        public string display { get; set; }
        public string value { get; set; }

        public XqModel(string display, string value)
        {
            this.display = display;
            this.value = value;
        }
    }
}
