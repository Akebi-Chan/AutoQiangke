using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoQiangke.Models
{
    public class BlockInfo : INotifyPropertyChanged
    {
        public string id;
        public BlockType type;

        public string rwlx;
        public string xkly;
        public string bklx_id;
        public string sfkkjyxdxnxq;
        public string sfkknj;
        public string sfkkzy;
        public string kzybkxy;
        public string sfznkx;
        public string zdkxms;
        public string sfkxq;
        public string sfkcfx;
        public string kkbk;
        public string kkbkdj;
        public string sfkgbcx;
        public string sfrxtgkcxd;
        public string tykczgxdcs;
        public string rlkz;
        public string rlzlkz;
        public string xkzgbj;
        public string xkxskcgskg;
        public string jxbzcxskg;
        public string txbsfrl;
        public string xklc;

        public string bbhzxjxb;

        public string name;
        public string predictname;
        public string kklxdm;
        public string xkkz_id;
        public BlockMatchRule rule;

        public bool isdetailed;

        //新建匹配
        public BlockInfo(BlockType type, string predictname, BlockMatchRule rule)
        {
            this.type = type;
            this.predictname = predictname;
            this.rule = rule;
            this.id = Guid.NewGuid().ToString();
        }
        //原生板块
        public BlockInfo(BlockType type, string name, string kklxdm, string xkkz_id)
        {
            this.type = type;
            this.name = name;
            this.kklxdm = kklxdm;
            this.xkkz_id = xkkz_id;
            this.id = Guid.NewGuid().ToString();
        }
        //编辑匹配副本
        public BlockInfo(string predictname, BlockMatchRule rule)
        {
            this.predictname = predictname;
            this.rule = rule;
            this.id = Guid.NewGuid().ToString();
        }
        //读取存储
        public BlockInfo()
        {
        }

        public void RecoverToPredict()
        {
            type = BlockType.Pending;
            rwlx = null;
            xkly = null;
            bklx_id = null;
            sfkkjyxdxnxq = null;
            sfkknj = null;
            sfkkzy = null;
            kzybkxy = null;
            sfznkx = null;
            zdkxms = null;
            sfkxq = null;
            sfkcfx = null;
            kkbk = null;
            kkbkdj = null;
            sfkgbcx = null;
            sfrxtgkcxd = null;
            tykczgxdcs = null;
            rlkz = null;
            xkzgbj = null;
            xkxskcgskg = null;
            jxbzcxskg = null;
            txbsfrl = null;
            name = null;
            kklxdm = null;
            bbhzxjxb = null;
            xkkz_id = null;
            isdetailed = false;
            Refresh();
        }

        public void RecoverToOriginal()
        {
            this.type = BlockType.Original;
            this.Predictname = "";
            this.rule = null;
            Refresh();
        }
        public string ID
        {
            get
            {
                if (type == BlockType.Pending)
                    return "-";
                else
                    return kklxdm;
            }
            set
            {
                this.RaisePropertyChanged("ID");
            }
        }
        public string BlockMatchRuleStr
        {
            get
            {
                if (rule == null)
                    return "-";
                else
                    return rule.ToString();
            }
            set
            {
                this.RaisePropertyChanged("BlockMatchRuleStr");
            }
        }
        public string Predictname
        {
            get
            {
                if (type == BlockType.Original)
                    return name;
                else
                    return predictname;
            }
            set
            {
                predictname = value;
                this.RaisePropertyChanged("Predictname");
            }
        }
        public string StateStr
        {
            get
            {
                switch (type)
                {
                    case BlockType.Pending:
                        return "等待匹配";
                    case BlockType.Matched:
                        return "已匹配：" + name;
                    case BlockType.Original:
                        return "未匹配：" + name;
                }
                return "未知";
            }
        }
        public string DisplayStr
        {
            get
            {
                switch (type)
                {
                    case BlockType.Pending:
                        return predictname;
                    case BlockType.Matched:
                        return predictname + "（已匹配：" + name + "）";
                    case BlockType.Original:
                        return name;
                }
                return "";
            }
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

        public void Refresh()
        {
            RaisePropertyChanged("ID");
            RaisePropertyChanged("BlockMatchRuleStr");
            RaisePropertyChanged("Predictname");
            RaisePropertyChanged("DisplayStr");
            RaisePropertyChanged("StateStr");
        }

        #endregion
    }

    public enum BlockType
    {
        Pending, Matched, Original
    }

    public class BlockMatchRule
    {
        public bool rev1, rev2;

        [JsonConverter(typeof(StringEnumConverter))]
        public VerbType verb1, verb2;
        public string arg1, arg2;
        public int joiner;//0:与；1:或

        public BlockMatchRule()
        {
            verb1 = VerbType.None;
            verb2 = VerbType.None;
            arg1 = "";
            arg2 = "";
        }

        public override string ToString()
        {
            string res;
            var s1 = GetOneString(rev1, verb1, arg1);
            var s2 = GetOneString(rev2, verb2, arg2);
            var s3 = joiner == 1 ? " 或 " : " 与 ";
            if (s1 == "")
                res = s2;
            else if (s2 == "")
                res = s1;
            else
                res = s1 + s3 + s2;
            res = res.Length <= 50 ? res : res.Substring(0, 50);

            return res;
        }
        public bool isMatch(string str)
        {
            var res1 = isOneMatch(rev1, verb1, arg1, str);
            if (verb2==VerbType.None)
                return res1;
            else
            {
                var res2 = isOneMatch(rev2, verb2, arg2, str);
                return joiner == 0 ? res1 && res2 : res1 || res2;
            }
        }

        public bool isOneMatch(bool rev, VerbType verb, string arg, string str)
        {
            bool res = true;
            switch (verb)
            {
                case VerbType.None:
                    return true;
                case VerbType.Regex:
                    res = new Regex(arg).IsMatch(str);
                    break;
                case VerbType.Equal:
                    res = str.Equals(arg);
                    break;
                case VerbType.Contain:
                    res = str.Contains(arg);
                    break;
                case VerbType.StartWith:
                    res = str.StartsWith(arg);
                    break;
                case VerbType.EndWith:
                    res = str.EndsWith(arg);
                    break;
            }
            return rev ? !res : res;
        }
        private static string GetOneString(bool rev, VerbType verb, string arg)
        {
            switch (verb)
            {
                case VerbType.None:
                    return "";
                case VerbType.Regex:
                    return rev? "正则不匹配：" + arg : "正则匹配：" + arg;
                case VerbType.Equal:
                    return rev ? "不等于“" + arg + "”" : "等于“" + arg + "”";
                case VerbType.Contain:
                    return rev ? "不包含“" + arg + "”" : "包含“" + arg + "”";
                case VerbType.StartWith:
                    return rev ? "开头不是“" + arg + "”" : "开头是“" + arg + "”";
                case VerbType.EndWith:
                    return rev ? "结尾不是“" + arg + "”" : "结尾是“" + arg + "”";
            }
            return "";
        }
        public BlockMatchRule Clone()
        {
            BlockMatchRule newrule = new BlockMatchRule();
            newrule.arg1 = this.arg1;
            newrule.arg2 = this.arg2;
            newrule.verb1 = this.verb1;
            newrule.verb2 = this.verb2;
            newrule.rev1 = this.rev1;
            newrule.rev2 = this.rev2;
            newrule.joiner = this.joiner;
            return newrule;
        }
    }

    public enum VerbType
    {
        None, Regex, Equal, Contain, StartWith, EndWith
    }

    public class VerbDisplay
    {
        public VerbType verb;

        public VerbDisplay(VerbType verb, string name)
        {
            this.verb = verb;
            Name = name;
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }

    public class PredictEditResult
    {
        public bool success;
        public BlockInfo result;

        public PredictEditResult(bool success, BlockInfo result)
        {
            this.success = success;
            this.result = result;
        }
    }
}
