using AutoQiangke.Helpers;
using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQiangke.Shared
{
    public static class GlobalSettings
    {
        public static bool saveCookie = true;
        public static bool saveFullBlockInfos = false;
        public static bool disableTK = false;
        public static bool xkWithFullArgs = false;

        public static CommonResult ReadSettings()
        {
            saveCookie = bool.Parse(IniHelper.GetKeyValue("Main", "SaveCookie", "true", IniHelper.inipath));
            saveFullBlockInfos = bool.Parse(IniHelper.GetKeyValue("Main", "SaveFullBlockInfos", "false", IniHelper.inipath));
            disableTK = bool.Parse(IniHelper.GetKeyValue("Main", "DisableTK", "false", IniHelper.inipath));
            xkWithFullArgs = bool.Parse(IniHelper.GetKeyValue("Main", "XkWithFullArgs", "false", IniHelper.inipath));
            return new CommonResult(true, "");
        }
        public static CommonResult SaveSettings()
        {
            IniHelper.SetKeyValue("Main", "SaveCookie", saveCookie.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("Main", "SaveFullBlockInfos", saveFullBlockInfos.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("Main", "DisableTK", disableTK.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("Main", "XkWithFullArgs", xkWithFullArgs.ToString(), IniHelper.inipath);
            return new CommonResult(true, "");
        }
    }
}
