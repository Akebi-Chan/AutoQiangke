using AutoQiangke.Models;
using AutoQiangke.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static AutoQiangke.Service.Web;

namespace AutoQiangke.Service
{
    public static class Course
    {
        public static List<ChosenJxbDto> ChosenJxbs;
        public static Dictionary<string, ChosenJxbDto> ChosenJxbsDic;
        public static CommonResult Go退课(JxbModel jxb)
        {
            if (GlobalSettings.disableTK)
                return new CommonResult(true, "禁止退课");
            if (!BasicInfo.IsXKSJ)
                if (MessageBox.Show("当前不是选课时间！！！！\n在非选课时间，退课可能仍然有效！！\n——————————————————\n您确定要继续退掉课程 " + jxb.course.kcmc + " 吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Stop) != MessageBoxResult.Yes)
                    return new CommonResult(false, "用户取消");
            var data = "kch_id=" + jxb.course.kch_id + "&jxb_ids=" + jxb.do_jxb_id + "&xkxnm=" + BasicInfo.xkxnm + "&xkxqm=" + BasicInfo.xkxqm;//+ "&txbsfrl=" + jxb.course.blockInfo.txbsfrl;
            Dictionary<string, string> d = Web.PostCommonHeaders();
            var res = Post(BasicInfo.BaseUri + "/xsxk/zzxkyzb_tuikBcZzxkYzb.html?gnmkdm=N253512&su=" + Jac.xh_id, d, data, false);
            if (!res.success)
                return new CommonResult(false, "退课失败");

            if (res.result != "\"1\"")
                return new CommonResult(false, "退课失败");
            return new CommonResult(true, "退课成功");
        }

        public static CommonResult Go选课(JxbModel jxb)
        {
            if (GlobalSettings.xkWithFullArgs)
                return Go选课_full(jxb);
            //kch_id：课程号ID
            //jxb_ids：256位课号ID，似乎是实时获取的
            //xkkz_id：未知ID，和选课轮数有关，同一轮，同一课程类型为定值。
            //sxbj：未知定值1(是否已选上 ?)
            //qz：未知定值0
            //njdm_id：年级级数。
            //zyh_id：专业号ID。
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("kch_id", jxb.course.kch_id);
            dic.Add("jxb_ids", jxb.do_jxb_id);
            dic.Add("sxbj", "1");
            dic.Add("qz", "0");
            dic.Add("njdm_id", BasicInfo.njdm_id);
            dic.Add("zyh_id", BasicInfo.zyh_id);

            Dictionary<string, string> d = Web.PostCommonHeaders();

            var res = Post(BasicInfo.BaseUri + "/xsxk/zzxkyzbjk_xkBcZyZzxkYzb.html?gnmkdm=N253512&su=" + Jac.xh_id, d, dic, false);

            if (!res.success) 
                return new CommonResult(false, "选课失败");

            XkJsonDto json1;
            try
            {
                json1 = Newtonsoft.Json.JsonConvert.DeserializeObject<XkJsonDto>(res.result);
            }
            catch //(Exception ex)
            {
                return new CommonResult(false, "反序列化失败");
            }
            if (json1.flag != "1")
                return new CommonResult(false, json1.msg);

            return new CommonResult(true, "选课成功");
        }

        public static CommonResult Go选课_full(JxbModel jxb)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("jxb_ids", jxb.do_jxb_id);
            dic.Add("kch_id", jxb.course.kch_id);
            dic.Add("rwlx", jxb.course.blockInfo.rwlx);
            dic.Add("rlkz", jxb.course.blockInfo.rlkz);
            dic.Add("rlzlkz", jxb.course.blockInfo.rlzlkz);
            dic.Add("sxbj", "1");//xx标记（未知）
            dic.Add("xxkbj", jxb.course.xxkbj);
            dic.Add("qz", "0");//权重（未知）
            dic.Add("cxbj", jxb.course.cxbj);
            dic.Add("xkkz_id", jxb.course.blockInfo.xkkz_id);
            dic.Add("njdm_id", BasicInfo.njdm_id);
            dic.Add("zyh_id", BasicInfo.zyh_id);
            dic.Add("kklxdm", jxb.course.blockInfo.kklxdm);
            dic.Add("xklc", jxb.course.blockInfo.xklc);
            dic.Add("xkxnm", BasicInfo.xkxnm);
            dic.Add("xkxqm", BasicInfo.xkxqm);

            Dictionary<string, string> d = Web.PostCommonHeaders();

            var res = Post(BasicInfo.BaseUri + "/xsxk/zzxkyzbjk_xkBcZyZzxkYzb.html?gnmkdm=N253512&su=" + Jac.xh_id, d, dic, false);

            if (!res.success) 
                return new CommonResult(false, "选课失败");

            XkJsonDto json1;
            try
            {
                json1 = Newtonsoft.Json.JsonConvert.DeserializeObject<XkJsonDto>(res.result);
            }
            catch //(Exception ex)
            {
                return new CommonResult(false, "反序列化失败");
            }
            if (json1.flag != "1")
                return new CommonResult(false, json1.msg);

            return new CommonResult(true, "选课成功");
        }

        public static JxbQueryResult Query(string querytext, BlockInfo blockInfo)
        {
            var formdata = CreateCourseQueryString(querytext, blockInfo);
            Dictionary<string, string> d = Web.PostCommonHeaders();
            var res = Post(BasicInfo.BaseUri + "/xsxk/zzxkyzb_cxZzxkYzbPartDisplay.html?gnmkdm=N253512&su=" + Jac.xh_id, d, formdata, false);
            if (!res.success) return new JxbQueryResult(false, "Post查询课程失败");

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<CourseQueryJsonDto>(res.result);
            if (json.tmpList == null || json.tmpList.Count == 0)
            {
                return new JxbQueryResult(false, "课程列表为空");
            }
            var formdata1 = CreateJxbQueryString(querytext, blockInfo, json.tmpList[0]);
            Dictionary<string, string> d1 = Web.PostCommonHeaders();
            res = Post(BasicInfo.BaseUri + "/xsxk/zzxkyzbjk_cxJxbWithKchZzxkYzb.html?gnmkdm=N253512&su=" + Jac.xh_id, d1, formdata1, false);
            if (!res.success) return new JxbQueryResult(false, "查询教学班失败");
            List<JxbQueryDto> json1;
            try
            {
                json1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JxbQueryDto>>(res.result);
            }
            catch //(Exception ex)
            {
                return new JxbQueryResult(false, "反序列化失败");
            }
            if (json1.Count != json.tmpList.Count)
            {
                return new JxbQueryResult(false, "个数不相等");
            }
            var jxbs = new List<JxbModel>();
            CourseModel c = null;
            for (int i = 0; i < json1.Count; i++)
                for (int j = 0; j < json.tmpList.Count; j++)
                    if (json1[i].jxb_id == json.tmpList[j].jxb_id)
                    {
                        if (c == null) 
                            c = new CourseModel(blockInfo, json.tmpList[j]);
                        if (c.kch != json.tmpList[j].kch)
                            return new JxbQueryResult(false, "课程混杂");
                        jxbs.Add(new JxbModel(json1[i], json.tmpList[j], c));
                        break;
                    }
            return new JxbQueryResult(jxbs, c, "成功");
        }

        public static PreQueryJxbFullResult PreQueryJxbFull(string querytext)
        {
            if (!Jac.islogin)
            {
                return new PreQueryJxbFullResult(false, "请先登录");
            }
            var headers = Web.PostCommonHeaders();
            headers["Referer"] = BasicInfo.BaseUri + "/design/viewFunc_cxDesignFuncPageIndex.html?gnmkdm=N2199113&layout=default&su=" + Jac.xh_id;
            var formdata = "xnm=" + BasicInfo.xkxnm + "&xqm=" + BasicInfo.xkxqm + "&jxbmc=" + querytext + "&_search=false&nd=" + Common.GetTimeStampMilli() + "&queryModel.showCount=5&queryModel.currentPage=1&queryModel.sortName=&queryModel.sortOrder=asc";
            var res = Web.Post(BasicInfo.BaseUri + "/design/funcData_cxFuncDataList.html?func_widget_guid=8B04B7BBB49C4455E0530200A8C06482&gnmkdm=N2199113&su=" + Jac.xh_id, headers, formdata, false);
            if (!res.success)
                return new PreQueryJxbFullResult(false, res.result);

            PreQueryCourseJsonDto json = null;
            try
            {
                json = Newtonsoft.Json.JsonConvert.DeserializeObject<PreQueryCourseJsonDto>(res.result);
            }
            catch (Exception ex)
            {
                return new PreQueryJxbFullResult(false, ex.Message);
            }
            if (json == null)
                return new PreQueryJxbFullResult(false, "找不到教学班");
            if (json.items.Count == 0)
                return new PreQueryJxbFullResult(false, "找不到教学班");

            PreQueryCourseDto onejxb = null;

            foreach (var dto in json.items)
                if (dto.jxbmc == querytext)
                {
                    if (onejxb == null)
                        onejxb = dto;
                    else
                        return new PreQueryJxbFullResult(false, "教学班不唯一，请检查教学班代码是否正确");
                }
            if (onejxb == null)
                return new PreQueryJxbFullResult(false, "找不到教学班");
            CourseModel c = new CourseModel();
            c.kch = onejxb.kcdm;
            c.jxbcount = json.totalCount;
            c.kcmc = onejxb.kcmc;

            return new PreQueryJxbFullResult(true, new JxbModel(onejxb, c));
        }

        public static PreQueryCourseResult PreQueryCourse(string querytext)
        {
            if (!Jac.islogin)
            {
                return new PreQueryCourseResult(false, "请先登录");
            }
            var headers = Web.PostCommonHeaders();
            headers["Referer"] = BasicInfo.BaseUri + "/design/viewFunc_cxDesignFuncPageIndex.html?gnmkdm=N2199113&layout=default&su=" + Jac.xh_id;
            var formdata = "xnm=" + BasicInfo.xkxnm + "&xqm=" + BasicInfo.xkxqm + "&kcdm=" + querytext + "&_search=false&nd=" + Common.GetTimeStampMilli() + "&queryModel.showCount=100&queryModel.currentPage=1&queryModel.sortName=&queryModel.sortOrder=asc";
            var res = Web.Post(BasicInfo.BaseUri + "/design/funcData_cxFuncDataList.html?func_widget_guid=8B04B7BBB49C4455E0530200A8C06482&gnmkdm=N2199113&su=" + Jac.xh_id, headers, formdata, false);
            if (!res.success)
                return new PreQueryCourseResult(false, res.result);

            PreQueryCourseJsonDto json = null;
            try
            {
                json = Newtonsoft.Json.JsonConvert.DeserializeObject<PreQueryCourseJsonDto>(res.result);
            }
            catch (Exception ex)
            {
                return new PreQueryCourseResult(false, ex.Message);
            }
            if (json == null)
                return new PreQueryCourseResult(false, "找不到教学班");

            if (json.items.Count==0)
                return new PreQueryCourseResult(false, "找不到教学班");

            var jxbs = new List<JxbModel>();
            CourseModel c = null;
            foreach (var res1 in json.items)
                if (res1.kcdm == querytext)
                {
                    if (c==null)
                    {
                        c = new CourseModel();
                        c.kch = res1.kcdm;
                        c.jxbcount = json.totalCount;
                        c.kcmc = res1.kcmc;
                    }
                    jxbs.Add(new JxbModel(res1, c));
                }
                    
            if (jxbs.Count == 0)
                return new PreQueryCourseResult(false, "找不到教学班");

            return new PreQueryCourseResult(true, c, jxbs);
        }

        public static CommonResult GetChosenJxbs()
        {
            if (!Jac.islogin)
            {
                return new CommonResult(false, "请先登录");
            }
            var headers = Web.PostCommonHeaders();
            headers["Referer"] = BasicInfo.BaseUri + "/xkcx/xkmdcx_cxXkmdcxIndex.html?gnmkdm=N255010&layout=default&su=" + Jac.xh_id;
            var formdic = new Dictionary<string, string>();
            formdic.Add("xnm", BasicInfo.xkxnm);
            formdic.Add("xqm", BasicInfo.xkxqm);
            formdic.Add("kkzt", "1");
            formdic.Add("nd", Common.GetTimeStampMilli());
            formdic.Add("queryModel.showCount", "100");
            formdic.Add("queryModel.currentPage", "1");
            formdic.Add("queryModel.sortOrder", "asc");
            formdic.Add("time", "1");
            var res = Web.Post(BasicInfo.BaseUri + "/xkcx/xkmdcx_cxXkmdcxIndex.html?doType=query&gnmkdm=N255010&su=" + Jac.xh_id, headers, formdic, false);
            if (!res.success)
                return new CommonResult(false, res.result);
            ChosenJxbJsonDto json;
            try
            {
                json = Newtonsoft.Json.JsonConvert.DeserializeObject<ChosenJxbJsonDto>(res.result);
            }
            catch //(Exception ex)
            {
                return new CommonResult(false, "反序列化失败");
            }

            var list = json.items;

            ChosenJxbs = list;
            ChosenJxbsDic = new Dictionary<string, ChosenJxbDto>();
            foreach (var jxb in ChosenJxbs)
                ChosenJxbsDic.Add(jxb.jxb_id, jxb);;

            return new CommonResult(true, "成功");
        }

        public static string CreateCourseQueryString(string querytext, BlockInfo blockInfo)
        {
            var data = "filter_list%5B0%5D=" + System.Web.HttpUtility.UrlEncode(querytext) + "&" +
                "rwlx=" + blockInfo.rwlx + "&" +
                "xkly=" + blockInfo.xkly + "&" +
                "bklx_id=" + blockInfo.bklx_id + "&" +
                "sfkkjyxdxnxq=" + blockInfo.sfkkjyxdxnxq + "&" +
                "xqh_id=" + BasicInfo.xqh_id + "&" +
                "jg_id=" + BasicInfo.jg_id + "&" +
                "njdm_id_1=" + BasicInfo.njdm_id_1 + "&" +
                "zyh_id_1=" + BasicInfo.zyh_id_1 + "&" +
                "zyh_id=" + BasicInfo.zyh_id + "&" +
                "zyfx_id=" + BasicInfo.zyfx_id + "&" +
                "njdm_id=" + BasicInfo.njdm_id + "&" +
                "bh_id=" + BasicInfo.bh_id + "&" +
                "xbm=" + BasicInfo.xbm + "&" +
                "xslbdm=" + BasicInfo.xslbdm + "&" +
                "ccdm=" + BasicInfo.ccdm + "&" +
                "xsbj=" + BasicInfo.xsbj + "&" +
                "sfkknj=" + blockInfo.sfkknj + "&" +
                "sfkkzy=" + blockInfo.sfkkzy + "&" +
                "kzybkxy=" + blockInfo.kzybkxy + "&" +
                "sfznkx=" + blockInfo.sfznkx + "&" +
                "rwzdkxmslx=" + blockInfo.zdkxms + "&" +
                "sfkxq=" + blockInfo.sfkxq + "&" +
                "sfkcfx=" + blockInfo.sfkcfx + "&" +
                "kkbk=" + blockInfo.kkbk + "&" +
                "kkbkdj=" + blockInfo.kkbkdj + "&" +
                "sfkgbcx=" + blockInfo.sfkgbcx + "&" +
                "sfrxtgkcxd=" + blockInfo.sfrxtgkcxd + "&" +
                "tykczgxdcs=" + blockInfo.tykczgxdcs + "&" +
                "xkxnm=" + BasicInfo.xkxnm + "&" +
                "xkxqm=" + BasicInfo.xkxqm + "&" +
                "kklxdm=" + blockInfo.kklxdm + "&" +
                "rlkz=" + blockInfo.rlkz + "&" +
                "xkzgbj=" + blockInfo.xkzgbj + "&" +
                "kspage=1&jspage=100";
            return data;
        }

        public static string CreateJxbQueryString(string querytext, BlockInfo blockInfo, CourseQueryDto courseDto)
        {
            var data = "filter_list%5B0%5D=" + System.Web.HttpUtility.UrlEncode(querytext) + "&" +
                "rwlx=" + blockInfo.rwlx + "&" +
                "xkly=" + blockInfo.xkly + "&" +
                "bklx_id=" + blockInfo.bklx_id + "&" +
                "sfkkjyxdxnxq=" + blockInfo.sfkkjyxdxnxq + "&" +
                "xqh_id=" + BasicInfo.xqh_id + "&" +
                "jg_id=" + BasicInfo.jg_id + "&" +
                "zyh_id=" + BasicInfo.zyh_id + "&" +
                "zyfx_id=" + BasicInfo.zyfx_id + "&" +
                "njdm_id=" + BasicInfo.njdm_id + "&" +
                "bh_id=" + BasicInfo.bh_id + "&" +
                "xbm=" + BasicInfo.xbm + "&" +
                "xslbdm=" + BasicInfo.xslbdm + "&" +
                "ccdm=" + BasicInfo.ccdm + "&" +
                "xsbj=" + BasicInfo.xsbj + "&" +
                "sfkknj=" + blockInfo.sfkknj + "&" +
                "sfkkzy=" + blockInfo.sfkkzy + "&" +
                "kzybkxy=" + blockInfo.kzybkxy + "&" +
                "sfznkx=" + blockInfo.sfznkx + "&" +
                "zdkxms=" + blockInfo.zdkxms + "&" +
                "sfkxq=" + blockInfo.sfkxq + "&" +
                "sfkcfx=" + blockInfo.sfkcfx + "&" +
                "kkbk=" + blockInfo.kkbk + "&" +
                "kkbkdj=" + blockInfo.kkbkdj + "&" +
                "xkxnm=" + BasicInfo.xkxnm + "&" +
                "xkxqm=" + BasicInfo.xkxqm + "&" +
                "xkxskcgskg=" + blockInfo.xkxskcgskg + "&" +
                "rlkz=" + blockInfo.rlkz + "&" +
                "kklxdm=" + blockInfo.kklxdm + "&" +
                "kch_id=" + courseDto.kch_id + "&" +
                "jxbzcxskg=" + blockInfo.xkzgbj + "&" +
                "xkkz_id=" + blockInfo.xkkz_id + "&" +
                "cxbj=" + courseDto.cxbj + "&" +
                "fxbj=" + courseDto.fxbj;
            return data;
        }
    }

    public class XkJsonDto
    {
        public string flag;
        public string msg;
    }

}
