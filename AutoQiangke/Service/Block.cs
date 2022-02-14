using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AutoQiangke.Service
{
    public static class Block
    {
        public static BindingList<BlockInfo> blockInfos = new BindingList<BlockInfo>();

        public static void Init()
        {
            Application.Current.Resources.Add("BlockInfos", blockInfos);
        }

        public static CommonResult GetBlockDetailedInfo(BlockInfo blockInfo)
        {
            if (!Jac.islogin)
            {
                return new CommonResult(false, "请先登录");
            }
            Dictionary<string, string> h = new Dictionary<string, string>();
            h.Add("Connection", "keep-alive");
            h.Add("Content-Length", "67");
            h.Add("Pragma", "no-cache");
            h.Add("Cache-Control", "no-cache");
            h.Add("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"96\", \"Microsoft Edge\";v=\"96\"");
            h.Add("Accept", "text/html, */*; q=0.01");
            h.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            h.Add("X-Requested-With", "XMLHttpRequest");
            h.Add("sec-ch-ua-mobile", "?0");
            h.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.9 Safari/537.36");
            h.Add("sec-ch-ua-platform", "\"Windows\"");
            h.Add("Origin", BasicInfo.BaseUri);
            h.Add("Sec-Fetch-Site", "same-origin");
            h.Add("Sec-Fetch-Mode", "cors");
            h.Add("Sec-Fetch-Dest", "empty");
            h.Add("Referer", BasicInfo.BaseUri + "/xsxk/zzxkyzb_cxZzxkYzbIndex.html?gnmkdm=N253512&layout=default&su=" + Jac.xh_id);
            h.Add("Accept-Encoding", "gzip, deflate, br");
            h.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            h.Add("Cookie", Jac.mycookie);

            Dictionary<string, string> f = new Dictionary<string, string>();
            f.Add("xkkz_id", blockInfo.xkkz_id);
            f.Add("xszxzt", BasicInfo.xszxzt);
            f.Add("kspage", "0");
            f.Add("jspage", "0");

            var postres = Web.Post(BasicInfo.BaseUri + "/xsxk/zzxkyzb_cxZzxkYzbDisplay.html?gnmkdm=N253512&su=" + Jac.xh_id, h, f, false);

            if (!postres.success)
            {
                return new CommonResult(false, "获取板块信息失败");
            }

            var reg = new Regex(@"(?<=<input type="".*"" name="")(.*)"" id=""(.*)"" value=""(.*)(?=""/>)");
            var regres = reg.Matches(postres.result);
            foreach (Match i in regres)
            {
                if (i.Groups[1].Value == "rwlx") blockInfo.rwlx = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkly") blockInfo.xkly = i.Groups[3].Value;
                if (i.Groups[1].Value == "bklx_id") blockInfo.bklx_id = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfkkjyxdxnxq") blockInfo.sfkkjyxdxnxq = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfkknj") blockInfo.sfkknj = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfkkzy") blockInfo.sfkkzy = i.Groups[3].Value;
                if (i.Groups[1].Value == "kzybkxy") blockInfo.kzybkxy = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfznkx") blockInfo.sfznkx = i.Groups[3].Value;
                if (i.Groups[1].Value == "zdkxms") blockInfo.zdkxms = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfkxq") blockInfo.sfkxq = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfkcfx") blockInfo.sfkcfx = i.Groups[3].Value;
                if (i.Groups[1].Value == "kkbk") blockInfo.kkbk = i.Groups[3].Value;
                if (i.Groups[1].Value == "kkbkdj") blockInfo.kkbkdj = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfkgbcx") blockInfo.sfkgbcx = i.Groups[3].Value;
                if (i.Groups[1].Value == "sfrxtgkcxd") blockInfo.sfrxtgkcxd = i.Groups[3].Value;
                if (i.Groups[1].Value == "tykczgxdcs") blockInfo.tykczgxdcs = i.Groups[3].Value;
                if (i.Groups[1].Value == "rlkz") blockInfo.rlkz = i.Groups[3].Value;
                if (i.Groups[1].Value == "rlzlkz") blockInfo.rlzlkz = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkzgbj") blockInfo.xkzgbj = i.Groups[3].Value;
                if (i.Groups[1].Value == "xkxskcgskg") blockInfo.xkxskcgskg = i.Groups[3].Value;
                if (i.Groups[1].Value == "jxbzcxskg") blockInfo.jxbzcxskg = i.Groups[3].Value;
                if (i.Groups[1].Value == "txbsfrl") blockInfo.txbsfrl = i.Groups[3].Value;

                if (i.Groups[1].Value == "xklcmc") Application.Current.Resources["xklcmc"] = i.Groups[3].Value;
                if (i.Groups[1].Value == "xklc") blockInfo.xklc = i.Groups[3].Value;
            }

            blockInfo.isdetailed = true;
            return new CommonResult(true, "");
        }

        public static void ClearBlocks()
        {
            for (int i = blockInfos.Count - 1; i >= 0; i--)
            {
                var t = blockInfos[i];
                if (t.type == BlockType.Original) continue;
                if (t.type == BlockType.Pending)
                    blockInfos.RemoveAt(i);
                else if (t.type == BlockType.Matched)
                {
                    t.RecoverToOriginal();
                }
            }
        }

        public static void AddBlock(BlockInfo blockinfo)//Predict Block
        {
            for (int i = 0; i < blockInfos.Count; i++)
            {
                var t = blockInfos[i];
                if (t.type == BlockType.Original)
                {
                    if (blockinfo.rule.isMatch(t.name))
                    {
                        t.type = BlockType.Matched;
                        t.Predictname = blockinfo.Predictname;
                        t.rule = blockinfo.rule;
                        t.Refresh();
                        return;
                    }
                }
            }
            blockInfos.Add(blockinfo);
        }

        public static void DeleteBlock(BlockInfo blockinfo)
        {
            if (blockinfo.type == BlockType.Original)
            {
                Common.SnackbarManager.Publish(new MySnackBarMessage("本身存在的板块不可删除！", TimeSpan.FromSeconds(3)));
                return;
            }
            if (blockinfo.type == BlockType.Pending)
            {
                blockInfos.Remove(blockinfo);
                return;
            }
            if (blockinfo.type == BlockType.Matched)
            {
                blockinfo.type = BlockType.Original;
                blockinfo.Predictname = "";
                blockinfo.rule = null;
                blockinfo.Refresh();
                return;
            }
        }

        public static void EditBlock(BlockInfo oldblock, BlockInfo newblock)
        {
            if (oldblock.type == BlockType.Original)
                return;
            if (oldblock.type == BlockType.Pending)
            {
                oldblock.Predictname = newblock.Predictname;
                oldblock.rule = newblock.rule;
                SearchForMatch(oldblock);
            }
            else if (oldblock.type == BlockType.Matched)
            {
                if (!newblock.rule.isMatch(oldblock.name))
                {
                    var splitblock = new BlockInfo(newblock.Predictname, newblock.rule);
                    blockInfos.Add(splitblock);
                    oldblock.RecoverToOriginal();
                    SearchForMatch(oldblock);
                    if (splitblock.type != BlockType.Matched)
                        SearchForMatch(splitblock);
                }
            }
        }

        public static void SearchForMatch(BlockInfo block0)
        {
            foreach (var block in blockInfos)
            {
                if (block.type == BlockType.Pending && block0.type == BlockType.Original)
                    if (block.rule.isMatch(block0.name))
                    {
                        MatchTwoBlock(block, block0);
                        break;
                    }
                if (block.type == BlockType.Original && block0.type == BlockType.Pending)
                    if (block0.rule.isMatch(block.name))
                    {
                        MatchTwoBlock(block0, block);
                        break;
                    }
            }
        }

        private static void MatchTwoBlock(BlockInfo block_predict, BlockInfo block_original)
        {
            block_predict.type = BlockType.Matched;
            block_predict.xkly = block_original.xkly;
            block_predict.bklx_id = block_original.bklx_id;
            block_predict.sfkkjyxdxnxq = block_original.sfkkjyxdxnxq;
            block_predict.sfkknj = block_original.sfkknj;
            block_predict.sfkkzy = block_original.sfkkzy;
            block_predict.kzybkxy = block_original.kzybkxy;
            block_predict.sfznkx = block_original.sfznkx;
            block_predict.zdkxms = block_original.zdkxms;
            block_predict.sfkxq = block_original.sfkxq;
            block_predict.sfkcfx = block_original.sfkcfx;
            block_predict.kkbk = block_original.kkbk;
            block_predict.kkbkdj = block_original.kkbkdj;
            block_predict.sfkgbcx = block_original.sfkgbcx;
            block_predict.sfrxtgkcxd = block_original.sfrxtgkcxd;
            block_predict.tykczgxdcs = block_original.tykczgxdcs;
            block_predict.rlkz = block_original.rlkz;
            block_predict.xkzgbj = block_original.xkzgbj;
            block_predict.xkxskcgskg = block_original.xkxskcgskg;
            block_predict.jxbzcxskg = block_original.jxbzcxskg;
            block_predict.txbsfrl = block_original.txbsfrl;
            block_predict.name = block_original.name;
            block_predict.kklxdm = block_original.kklxdm;
            block_predict.xkkz_id = block_original.xkkz_id;
            block_predict.isdetailed = block_original.isdetailed;
            blockInfos.Remove(block_original);
            block_predict.Refresh();
        }

        public static void MergeBlocks(List<BlockInfo> list)
        {
            //Clear Old Result
            for (int i = blockInfos.Count - 1; i >= 0; i--)
            {
                var t = blockInfos[i];
                if (t.type == BlockType.Pending || list.Contain(t)) continue;
                if (t.type == BlockType.Original)
                    blockInfos.RemoveAt(i);
                else if (t.type == BlockType.Matched)
                {
                    t.RecoverToPredict();
                }
            }

            foreach (var i in list)
            {
                if (blockInfos.Contain(i))
                {
                    i.type = BlockType.Matched;
                    continue;
                }
                for (int j = 0; j < blockInfos.Count; j++)
                    if (blockInfos[j].type==BlockType.Pending)
                        if(blockInfos[j].rule.isMatch(i.name))
                        {
                            blockInfos[j].type = BlockType.Matched;
                            blockInfos[j].name = i.name;
                            blockInfos[j].kklxdm = i.kklxdm;
                            blockInfos[j].xkkz_id = i.xkkz_id;
                            i.type = BlockType.Matched;
                            blockInfos[j].Refresh();
                            break;
                        }
                if (i.type != BlockType.Matched)
                    blockInfos.Add(i);
            }
        }

        public static bool Contain(this IEnumerable<BlockInfo> list, BlockInfo item)
        {
            foreach (var i in list)
                if (i.xkkz_id == item.xkkz_id)
                    return true;
            return false;
        }
    }
}
