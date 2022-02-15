using AutoQiangke.Helpers;
using AutoQiangke.Models;
using AutoQiangke.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoQiangke.Service
{
    public static class Storage
    {
        public static string BasePath = Environment.GetEnvironmentVariable("LocalAppData") + "\\qiangke\\";
        public static Dictionary<string, BlockInfo> BlockInfoMapper;
        public static Dictionary<string, CourseModel> CourseMapper;

        public static CommonResult ReadData()
        {
            try
            {
                ReadBlockInfos(); ReadCourses(); ReadTasks();
                return new CommonResult(true, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取数据失败\n" + ex.ToString());
                return new CommonResult(false, ex.ToString());
            }
        }
        public static CommonResult SaveData()
        {
            try
            {
                SaveBlockInfos(); SaveCourses(); SaveTasks();
                return new CommonResult(true, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("存储数据失败\n" + ex.ToString());
                return new CommonResult(false, ex.ToString());
            }
        }
        public static CommonResult SaveBlockInfos()
        {
            IniHelper.SetKeyValue("main","SaveFullBlockInfos",GlobalSettings.saveFullBlockInfos.ToString(),IniHelper.inipath);
            if (GlobalSettings.saveFullBlockInfos)
            {
                List<BlockInfoStorageFull> list = new List<BlockInfoStorageFull>();
                foreach (var blockInfo in Block.blockInfos)
                {
                    list.Add(new BlockInfoStorageFull(blockInfo));
                }
                string json = JsonConvert.SerializeObject(list);
                File.WriteAllText(BasePath + "BlockInfos.json", json, Encoding.Default);
            }
            else
            {
                List<BlockInfoStorageLite> list = new List<BlockInfoStorageLite>();
                foreach (var blockInfo in Block.blockInfos)
                    if (blockInfo.type != BlockType.Original)
                        list.Add(new BlockInfoStorageLite(blockInfo));
                string json = JsonConvert.SerializeObject(list);
                File.WriteAllText(BasePath + "BlockInfos.json", json, Encoding.Default);
            }
            return new CommonResult(true, "");
        }
        public static CommonResult ReadBlockInfos()
        {
            Block.blockInfos.Clear();
            if (!File.Exists(BasePath + "BlockInfos.json"))
                return new CommonResult(false, "文件不存在");
            if (GlobalSettings.saveFullBlockInfos)
            {
                var json = File.ReadAllText(BasePath + "BlockInfos.json");
                List<BlockInfoStorageFull> list = JsonConvert.DeserializeObject<List<BlockInfoStorageFull>>(json);
                foreach (var b in list)
                    Block.blockInfos.Add(b.ToBlockInfo());
            }
            else
            {
                var json = File.ReadAllText(BasePath + "BlockInfos.json");
                List<BlockInfoStorageLite> list = JsonConvert.DeserializeObject<List<BlockInfoStorageLite>>(json);
                foreach (var b in list)
                    Block.blockInfos.Add(b.ToBlockInfo());
            }
            BlockInfoMapper = new Dictionary<string, BlockInfo>();
            foreach (var b in Block.blockInfos)
                BlockInfoMapper.Add(b.id, b);
            return new CommonResult(true, "");
        }
        public static CommonResult SaveTasks()
        {
            List<TaskModelStorage> list = new List<TaskModelStorage>();
            foreach (var task in Task.tasks)
                list.Add(new TaskModelStorage(task));
            string json = JsonConvert.SerializeObject(list);
            File.WriteAllText(BasePath + "Tasks.json", json, Encoding.Default);
            return new CommonResult(true, "");
        }
        public static CommonResult ReadTasks()
        {
            if (!File.Exists(BasePath + "Tasks.json"))
                return new CommonResult(false, "文件不存在");
            var json = File.ReadAllText(BasePath + "Tasks.json");
            List<TaskModelStorage> list = JsonConvert.DeserializeObject<List<TaskModelStorage>>(json);
            foreach (var task in list)
                Task.tasks.Add(task.ToTaskModel());
            return new CommonResult(true, "");
        }
        public static CommonResult ReadCourses()
        {
            if (!File.Exists(BasePath + "Courses.json"))
                return new CommonResult(false, "文件不存在");
            var json = File.ReadAllText(BasePath + "Courses.json");
            CourseMapper = new Dictionary<string, CourseModel>();
            List<CourseModelStorage> list = JsonConvert.DeserializeObject<List<CourseModelStorage>>(json);
            foreach (var c in list)
                CourseMapper.Add(c.id, c.ToCourseModel());
            return new CommonResult(true, "");
        }
        public static CommonResult SaveCourses()
        {
            List<CourseModelStorage> list = new List<CourseModelStorage>();
            List<string> idlist = new List<string>();
            foreach (var task in Task.tasks)
                if (task.jxbs != null)
                    foreach (var column in task.jxbs)
                        foreach (var jxb in column)
                            if (jxb.course != null)
                                if (!idlist.Contains(jxb.course.id))
                                {
                                    list.Add(new CourseModelStorage(jxb.course));
                                    idlist.Add(jxb.course.id);
                                }
            string json = JsonConvert.SerializeObject(list);
            File.WriteAllText(BasePath + "Courses.json", json, Encoding.Default);
            return new CommonResult(true, "");
        }
    }

    public class BlockInfoStorageFull
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public BlockType type;
        public string id;

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

        public string name;
        public string predictname;
        public string kklxdm;
        public string xkkz_id;
        public BlockMatchRule rule;

        public bool isdetailed;

        public BlockInfoStorageFull()
        {
        }
        public BlockInfoStorageFull(BlockInfo b)
        {
            this.id = b.id;
            this.type = b.type;
            this.rwlx = b.rwlx;
            this.xkly = b.xkly;
            this.bklx_id = b.bklx_id;
            this.sfkkjyxdxnxq = b.sfkkjyxdxnxq;
            this.sfkknj = b.sfkknj;
            this.sfkkzy = b.sfkkzy;
            this.kzybkxy = b.kzybkxy;
            this.sfznkx = b.sfznkx;
            this.zdkxms = b.zdkxms;
            this.sfkxq = b.sfkxq;
            this.sfkcfx = b.sfkcfx;
            this.kkbk = b.kkbk;
            this.kkbkdj = b.kkbkdj;
            this.sfkgbcx = b.sfkgbcx;
            this.sfrxtgkcxd = b.sfrxtgkcxd;
            this.tykczgxdcs = b.tykczgxdcs;
            this.rlkz = b.rlkz;
            this.rlzlkz = b.rlzlkz;
            this.xkzgbj = b.xkzgbj;
            this.xkxskcgskg = b.xkxskcgskg;
            this.jxbzcxskg = b.jxbzcxskg;
            this.txbsfrl = b.txbsfrl;
            this.name = b.name;
            this.predictname = b.predictname;
            this.kklxdm = b.kklxdm;
            this.xkkz_id = b.xkkz_id;
            this.rule = b.rule;
            this.isdetailed = b.isdetailed;
            this.xklc = b.xklc;
        }
        internal BlockInfo ToBlockInfo()
        {
            BlockInfo b = new BlockInfo();
            b.id = this.id;
            b.type = this.type;
            b.rwlx = this.rwlx;
            b.xkly = this.xkly;
            b.bklx_id = this.bklx_id;
            b.sfkkjyxdxnxq = this.sfkkjyxdxnxq;
            b.sfkknj = this.sfkknj;
            b.sfkkzy = this.sfkkzy;
            b.kzybkxy = this.kzybkxy;
            b.sfznkx = this.sfznkx;
            b.zdkxms = this.zdkxms;
            b.sfkxq = this.sfkxq;
            b.sfkcfx = this.sfkcfx;
            b.kkbk = this.kkbk;
            b.kkbkdj = this.kkbkdj;
            b.sfkgbcx = this.sfkgbcx;
            b.sfrxtgkcxd = this.sfrxtgkcxd;
            b.tykczgxdcs = this.tykczgxdcs;
            b.rlkz = this.rlkz;
            b.rlzlkz = this.rlzlkz;
            b.xkzgbj = this.xkzgbj;
            b.xkxskcgskg = this.xkxskcgskg;
            b.jxbzcxskg = this.jxbzcxskg;
            b.txbsfrl = this.txbsfrl;
            b.name = this.name;
            b.predictname = this.predictname;
            b.kklxdm = this.kklxdm;
            b.xkkz_id = this.xkkz_id;
            b.rule = this.rule;
            b.isdetailed = this.isdetailed;
            b.xklc = this.xklc;
            return b;
        }
    }

    public class BlockInfoStorageLite
    {
        public string id;
        public string predictname;
        public BlockMatchRule rule;

        public BlockInfoStorageLite()
        {
        }
        public BlockInfoStorageLite(BlockInfo b)
        {
            this.id = b.id;
            this.predictname = b.predictname;
            this.rule = b.rule;
        }
        public BlockInfo ToBlockInfo()
        {
            var blockInfo = new BlockInfo();
            blockInfo.id = this.id;
            blockInfo.type = BlockType.Pending;
            blockInfo.predictname = this.predictname;
            blockInfo.rule = this.rule;
            return blockInfo;
        }
    }

    public class TaskModelStorage
    {
        public string courseModelId;//存储用ID
        public string blockInfoId;//存储用ID

        public List<List<JxbModelStorage>> jxbs;
        public string title;
        public string courseId;
        public bool isCourseIdValid;
        public string courseName;
        public string courseCountText;
        public int interval;
        public int times;
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskState state;
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskType type;
        public string message;
        public string id;

        public TaskModelStorage()
        {
        }

        public TaskModelStorage(TaskModel task)
        {
            if (task.BlockInfo != null)
                this.blockInfoId = task.BlockInfo.id;
            if (task.course != null)
                this.courseModelId = task.course.id;
            if (task.jxbs != null)
            {
                this.jxbs = new List<List<JxbModelStorage>>();
                foreach (var column in task.jxbs)
                {
                    List<JxbModelStorage> newcolumn = new List<JxbModelStorage>();
                    foreach (var jxb in column)
                    {
                        newcolumn.Add(new JxbModelStorage(jxb));
                    }
                    this.jxbs.Add(newcolumn);
                }
            }
            
            this.title = task.Title;
            this.courseId = task.CourseId;
            this.isCourseIdValid = task.IsCourseIdValid;
            this.courseName = task.CourseName;
            this.courseCountText = task.CourseCountText;
            this.interval = task.Interval;
            this.times = task.Times;
            this.state = task.State == TaskState.Started ? TaskState.Wait : task.State;
            this.type = task.type;
            this.message = task.Message;
            this.id = task.id;
        }

        public TaskModel ToTaskModel()
        {
            TaskModel task = new TaskModel();
            if (this.blockInfoId != null)
                if (Storage.BlockInfoMapper.ContainsKey(this.blockInfoId))
                    task.BlockInfo = Storage.BlockInfoMapper[this.blockInfoId];
            if (courseModelId != null)
                if (Storage.CourseMapper.ContainsKey(this.courseModelId))
                    task.course = Storage.CourseMapper[this.courseModelId];
            if (this.jxbs != null)
            {
                task.Jxbs = new BindingList<BindingList<JxbModel>>();
                foreach (var column in this.jxbs)
                {
                    BindingList<JxbModel> newcolumn = new BindingList<JxbModel>();
                    foreach (var jxb in column)
                    {
                        newcolumn.Add(jxb.ToJxbModel());
                    }
                    task.Jxbs.Add(newcolumn);
                }
            }
            task.Title = this.title;
            task.CourseId = this.courseId;
            task.IsCourseIdValid = this.isCourseIdValid;
            task.CourseName = this.courseName;
            task.CourseCountText = this.courseCountText;
            task.Interval = this.interval;
            task.Times = this.times;
            task.State = this.state;
            task.type = this.type;
            task.Message = this.message;
            task.id = this.id;
            return task;
        }
    }

    public class CourseModelStorage
    {
        public string blockInfoId;//存储用ID
        public string id;
        public string kcmc;
        public string kch_id;
        public string kch;
        public string fxbj;
        public string cxbj;
        public string xxkbj;
        public int jxbcount;

        public CourseModelStorage()
        {
        }

        public CourseModelStorage(CourseModel course)
        {
            if (course.blockInfo != null)
                this.blockInfoId = course.blockInfo.id;
            this.id = course.id;
            this.kcmc = course.kcmc;
            this.kch_id = course.kch_id;
            this.kch = course.kch;
            this.jxbcount = course.jxbcount;
            this.fxbj = course.fxbj;
            this.cxbj = course.cxbj;
            this.xxkbj = course.xxkbj;
        }

        public CourseModel ToCourseModel()
        {
            CourseModel c = new CourseModel();
            c.id = this.id;
            c.kcmc = this.kcmc;
            c.kch_id = this.kch_id;
            c.kch = this.kch;
            c.fxbj = this.fxbj;
            c.cxbj = this.cxbj;
            c.xxkbj = this.xxkbj;
            c.jxbcount = this.jxbcount;
            if (this.blockInfoId != null)
                if (Storage.BlockInfoMapper.ContainsKey(this.blockInfoId))
                    c.blockInfo = Storage.BlockInfoMapper[this.blockInfoId];
            return c;
        }
    }

    public class JxbModelStorage
    {
        public string courseModelId;//存储用ID
        public string do_jxb_id;
        public string jsxx;
        public string jxdd;
        public string sksj;
        public string jxbrl;
        public string jxb_id;
        public string jxbmc;
        public string yxzrs;

        public string jxb_shortid;
        [JsonConverter(typeof(StringEnumConverter))]
        public JxbStateEnum state;
        public string stateStr;
        public JxbModelStorage()
        {
        }
        public JxbModelStorage(JxbModel jxb)
        {
            if (jxb.course != null)
                this.courseModelId = jxb.course.id;
            this.do_jxb_id = jxb.do_jxb_id;
            this.jsxx = jxb.jsxx;
            this.jxdd = jxb.jxdd;
            this.sksj = jxb.sksj;
            this.jxbrl = jxb.jxbrl;
            this.jxb_id = jxb.jxb_id;
            this.jxbmc = jxb.jxbmc;
            this.yxzrs = jxb.yxzrs;
            this.jxb_shortid = jxb.Jxb_shortid;
            this.state = jxb.State == JxbStateEnum.Chosen || jxb.State == JxbStateEnum.Known ? JxbStateEnum.Ready : jxb.State;
            this.stateStr = jxb.StateStr;
        }
        public JxbModel ToJxbModel()
        {
            JxbModel jxb=new JxbModel();
            if (courseModelId != null)
                if (Storage.CourseMapper.ContainsKey(this.courseModelId))
                    jxb.course = Storage.CourseMapper[courseModelId];
            jxb.do_jxb_id = this.do_jxb_id;
            jxb.jsxx = this.jsxx;
            jxb.jxdd = this.jxdd;
            jxb.sksj = this.sksj;
            jxb.jxbrl = this.jxbrl;
            jxb.jxb_id = this.jxb_id;
            jxb.jxbmc = this.jxbmc;
            jxb.yxzrs = this.yxzrs;
            jxb.Jxb_shortid = this.jxb_shortid;
            jxb.State = this.state;
            jxb.StateStr = this.stateStr;
            return jxb;
        }
    }
}
