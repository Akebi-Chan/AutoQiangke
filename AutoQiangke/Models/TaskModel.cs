using AutoQiangke.Helpers;
using AutoQiangke.Service;
using AutoQiangke.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace AutoQiangke.Models
{
    public class TaskModel : INotifyPropertyChanged
    {
        public BindingList<BindingList<JxbModel>> jxbs;
        public BindingList<BindingList<JxbModel>> Jxbs
        {
            get { return jxbs; }
            set
            {
                jxbs = value;
                this.RaisePropertyChanged("Jxbs");
            }
        }

        public void CheckIsChosenJxb(JxbModel jxb)
        {
            if (Course.ChosenJxbsDic == null) return;
            if (jxb.jxb_id != null)
                if (Course.ChosenJxbsDic.ContainsKey(jxb.jxb_id))
                {
                    var jxb1 = Course.ChosenJxbsDic[jxb.jxb_id];
                    jxb.State = JxbStateEnum.Chosen;
                    jxb.StateStr = "已选";
                    if (jxb1.do_jxb_id != null)
                        jxb.do_jxb_id = jxb1.do_jxb_id;
                    jxb.jsxx = jxb1.jsxx;
                    jxb.jxdd = jxb1.jxdd;
                    jxb.sksj = jxb1.sksj;
                    jxb.jxb_id = jxb1.jxb_id;
                    if (jxb.course == null)
                        jxb.course = new CourseModel() { kcmc = jxb1.kcmc, kch = jxb1.kch, kch_id = jxb1.kch_id };
                    if (jxb.course.kch_id == null)
                        jxb.course.kch_id = jxb1.kch_id;
                }
        }
        public void CheckIsChosenJxbs()
        {
            if (Course.ChosenJxbsDic == null) return;
            bool flag = false;
            foreach (var column in Jxbs)
                foreach (JxbModel jxb in column)
                    if (jxb.jxb_id != null)
                        if (Course.ChosenJxbsDic.ContainsKey(jxb.jxb_id))
                        {
                            var jxb1 = Course.ChosenJxbsDic[jxb.jxb_id];
                            jxb.State = JxbStateEnum.Chosen;
                            jxb.StateStr = "已选";
                            if (jxb1.do_jxb_id != null)
                                jxb.do_jxb_id = jxb1.do_jxb_id;
                            jxb.jsxx = jxb1.jsxx;
                            jxb.jxdd = jxb1.jxdd;
                            jxb.sksj = jxb1.sksj;
                            jxb.jxb_id = jxb1.jxb_id;
                            if (jxb.course == null)
                                jxb.course = new CourseModel() { kcmc = jxb1.kcmc, kch = jxb1.kch, kch_id = jxb1.kch_id };
                            if (jxb.course.kch_id == null)
                                jxb.course.kch_id = jxb1.kch_id;
                            flag = true;
                        }
            if (flag) RaiseJxbsChanged();
        }
        public void UpdateJxbsAfterPreQuery()//Only For Lite
        {
            if (Jxbs == null) return;
            Dictionary<string, JxbModel> jxbdic = new Dictionary<string, JxbModel>();
            bool flag = false;
            foreach (JxbModel jxb in queryresult)
            {
                if (jxbdic.ContainsKey(GetShortId(jxb.jxbmc)))
                {
                    flag = true;
                    continue;
                }
                jxbdic.Add(GetShortId(jxb.jxbmc), jxb);
            }
            if (flag) MessageBox.Show("课程号重复，请检查学期学年设置");
            foreach (var column in Jxbs)
                foreach (JxbModel jxb in column)
                {
                    jxb.State = JxbStateEnum.Unknow;
                    jxb.course = this.course;
                    jxb.StateStr = "未知";
                    jxb.Teacher = "";
                    jxb.CourseLocation = "";
                    jxb.CourseTime = "";
                }
            foreach (var column in Jxbs)
                foreach (JxbModel jxb in column)
                {
                    if (jxb.State == JxbStateEnum.Unknow)
                        if (jxbdic.ContainsKey(jxb.Jxb_shortid))
                        {
                            var newjxb = jxbdic[jxb.Jxb_shortid];
                            jxb.Teacher = newjxb.jsxx;
                            jxb.CourseTime = newjxb.sksj;
                            jxb.CourseLocation = newjxb.jxdd;
                            jxb.jxb_id = newjxb.jxb_id;
                            jxb.jxbmc = newjxb.jxbmc;
                            jxb.State = JxbStateEnum.Ready;
                            jxb.StateStr = "预备";
                        }
                }
            this.RaisePropertyChanged("Jxbs");
        }
        public void UpdateJxbsAfterQuery()//Only For Lite
        {
            Dictionary<string, JxbModel> jxbdic = new Dictionary<string, JxbModel>();
            bool flag = false;
            foreach (JxbModel jxb in queryresult)
            {
                if (jxbdic.ContainsKey(GetShortId(jxb.jxbmc)))
                {
                    flag = true;
                    continue;
                }
                jxbdic.Add(GetShortId(jxb.jxbmc), jxb);
            }
            //if (flag) MessageBox.Show("课程号重复，请检查学期学年设置");

            //foreach (var column in Jxbs)
            //    foreach (JxbModel jxb in column)
            //        if (jxb.State == JxbStateEnum.Known)
            //        {
            //            jxb.State = JxbStateEnum.Unknow;
            //            jxb.course = this.course;
            //            jxb.StateStr = "未知";
            //            jxb.Teacher = "";
            //            jxb.CourseLocation = "";
            //            jxb.CourseTime = "";
            //        }
            foreach (var column in Jxbs)
                foreach (JxbModel jxb in column)
                {
                    if (jxbdic.ContainsKey(jxb.Jxb_shortid))
                    {
                        var resjxb = jxbdic[jxb.Jxb_shortid];
                        jxb.Teacher = resjxb.jsxx;
                        jxb.do_jxb_id = resjxb.do_jxb_id;
                        jxb.CourseTime = resjxb.sksj;
                        jxb.CourseLocation = resjxb.jxdd;
                        jxb.jxbrl = resjxb.jxbrl;
                        jxb.jxb_id = resjxb.jxb_id;
                        jxb.jxbmc = resjxb.jxbmc;
                        jxb.yxzrs = resjxb.yxzrs;
                        jxb.State = JxbStateEnum.Known;
                        jxb.StateStr = "已知";
                    }
                }
            RaiseJxbsChanged();
        }
        public void UpdateJxbByAdd(JxbModel jxb0)//Only For Lite
        {
            Dictionary<string, JxbModel> jxbdic = new Dictionary<string, JxbModel>();
            foreach (JxbModel jxb in queryresult)
            {
                jxbdic.Add(GetShortId(jxb.jxbmc), jxb);
            }
            if (jxb0.State == JxbStateEnum.Unknow)
                if (jxbdic.ContainsKey(jxb0.Jxb_shortid))
                {
                    var newjxb = jxbdic[jxb0.Jxb_shortid];
                    jxb0.Teacher = newjxb.jsxx;
                    jxb0.CourseTime = newjxb.sksj;
                    jxb0.CourseLocation = newjxb.jxdd;
                    jxb0.jxb_id = newjxb.jxb_id;
                    jxb0.jxbmc = newjxb.jxbmc;
                    jxb0.State = JxbStateEnum.Ready;
                    jxb0.StateStr = "预备";
                }
            RaiseJxbsChanged();
        }
        public void UpdateJxbAfterPreQueryFull(JxbModel jxb, JxbModel resjxb)//Only For Full
        {
            jxb.course = resjxb.course;
            jxb.Kcmc = jxb.course.kcmc;
            //jxb.course.blockInfo = this.BlockInfo;
            jxb.Teacher = resjxb.jsxx;
            jxb.CourseTime = resjxb.sksj;
            jxb.CourseLocation = resjxb.jxdd;
            jxb.jxb_id = resjxb.jxb_id;
            if (jxb.State != JxbStateEnum.Chosen)
            {
                jxb.State = JxbStateEnum.Ready;
                jxb.StateStr = "预备";
            }
        }
        public void UpdateJxbAfterQueryFull(JxbModel jxb, JxbQueryResult res)//Only For Full
        {
            foreach (var resjxb in res.jxbs)
                if (jxb.Jxbmc == resjxb.Jxbmc)
                {
                    jxb.course = res.course;
                    jxb.Teacher = resjxb.jsxx;
                    jxb.do_jxb_id = resjxb.do_jxb_id;
                    jxb.CourseTime = resjxb.sksj;
                    jxb.CourseLocation = resjxb.jxdd;
                    jxb.jxbrl = resjxb.jxbrl;
                    jxb.jxb_id = resjxb.jxb_id;
                    jxb.jxbmc = resjxb.jxbmc;
                    jxb.yxzrs = resjxb.yxzrs;
                    jxb.State = JxbStateEnum.Known;
                    jxb.StateStr = "已知";
                }
        }
        public void RaiseJxbsChanged()
        {
            this.RaisePropertyChanged("Jxbs");
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                this.RaisePropertyChanged("Title");
            }
        }

        public CourseModel course;//Only For Lite
        public List<JxbModel> queryresult;//Only For Lite

        private BlockInfo blockInfo;
        public BlockInfo BlockInfo
        {
            get { return blockInfo; }
            set
            {
                blockInfo = value;
                this.RaisePropertyChanged("BlockInfo");
            }
        }//Only For Lite
        private string courseId;
        public string CourseId
        {
            get { return courseId; }
            set
            {
                courseId = value;
                this.RaisePropertyChanged("CourseId");
            }
        }//Only For Lite
        private bool isCourseIdValid;
        public bool IsCourseIdValid
        {
            get { return isCourseIdValid; }
            set
            {
                isCourseIdValid = value;
                this.RaisePropertyChanged("IsCourseIdValid");
            }
        }//Only For Lite
        private string courseName;
        public string CourseName
        {
            get { return courseName; }
            set
            {
                courseName = value;
                this.RaisePropertyChanged("CourseName");
            }
        }//Only For Lite
        private string courseCountText;
        public string CourseCountText
        {
            get { return courseCountText; }
            set
            {
                courseCountText = value;
                this.RaisePropertyChanged("CourseCountText");
            }
        }//Only For Lite

        private int interval;
        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                this.RaisePropertyChanged("Interval");
            }
        }
        private int times;
        public int Times
        {
            get { return times; }
            set
            {
                times = value;
                this.RaisePropertyChanged("Times");
            }
        }
        private TaskState state;
        public TaskState State
        {
            get { return state; }
            set
            {
                state = value;
                this.RaisePropertyChanged("State");
            }
        }

        private string message = "等待开始";
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        public Logger logger;
        public UserControl view;
        public TaskType type;

        public string id;

        //private Timer ti;
        private BackgroundWorker bgw;

        public delegate void OnGoAnimationHandler();

        public event OnGoAnimationHandler OnGoAnimation;

        #region Task Work
        private bool Go()
        {
            Times++;
            //----Check Condition----//
            if (course == null)
            {
                Message = "课程不能为空";
                return false;
            }
            if (blockInfo == null)
            {
                Message = "板块不能为空";
                return false;
            }
            //if (Interval == 0)
            //{
            //    Message = "Interval不能为0";
            //    return false;
            //}
            if (!BlockInfo.isdetailed)
            {
                var res0 = Block.GetBlockDetailedInfo(BlockInfo);
                if (!res0.success)
                {
                    Message = res0.result;
                    return false;
                }
            }

            var res = Course.Query(course.kch, BlockInfo);
            if (!res.success)
            {
                Message = res.message;
                Common.logger.log("Query失败！ " + course.kch + " " + BlockInfo.DisplayStr);
                return false;
            }
            //----Update Course----//
            this.course = res.course;
            foreach (var column in Jxbs)
                foreach (JxbModel jxb in column)
                    jxb.course = this.course;

            //----Merge Infos----//
            queryresult = res.jxbs;
            UpdateJxbsAfterQuery();
            CheckIsChosenJxbs();

            //----Check----//
            for (int i = 0; i < Jxbs.Count; i++)
                for (int j = 0; j < Jxbs[i].Count; j++)
                {
                    var jxb = Jxbs[i][j];
                    if (jxb.State == JxbStateEnum.Known)
                    {
                        if (int.Parse(jxb.yxzrs) < int.Parse(jxb.jxbrl))
                        {
                            bool chosenchanged = TryXuanke(i, j);
                            if (chosenchanged)
                            {
                                Thread t = new Thread(() =>
                                {
                                    Course.GetChosenJxbs();
                                    CheckIsChosenJxbs();
                                });
                                t.Start();
                            }
                            return true;
                        }
                    }
                    else if (jxb.State == JxbStateEnum.Chosen)
                    {
                        if (i == 0)
                        {
                            Message = "任务完成";
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }//For
            Message = "容量已满";
            return true;
        }

        private bool GoFull()
        {
            Times++;
            //----Check Condition----//
            foreach (var column in Jxbs)
                foreach (JxbModel jxb in column)
                    if (true)//jxb.State != JxbStateEnum.Chosen
                    {
                        if (jxb.course == null) continue;
                        if (jxb.course.blockInfo == null) continue;
                        if (!jxb.course.blockInfo.isdetailed)
                        {
                            var res0 = Block.GetBlockDetailedInfo(jxb.course.blockInfo);
                            if (!res0.success)
                            {
                                Message = res0.result;
                                return false;
                            }
                        }

                        var res = Course.Query(jxb.Jxbmc, jxb.course.blockInfo);
                        if (!res.success)
                        {
                            Common.logger.log("Query失败！ " + jxb.Jxbmc + " " + jxb.course.blockInfo.DisplayStr);
                            jxb.State = JxbStateEnum.Error;
                            jxb.StateStr = "错误";
                            continue;
                        }

                        UpdateJxbAfterQueryFull(jxb, res);
                    }

            //----Merge Infos----//
            CheckIsChosenJxbs();
            RaiseJxbsChanged();

            ////----Check----//
            for (int i = 0; i < Jxbs.Count; i++)
                for (int j = 0; j < Jxbs[i].Count; j++)
                {
                    var jxb = Jxbs[i][j];
                    if (jxb.State == JxbStateEnum.Known)
                    {
                        if (int.Parse(jxb.yxzrs) < int.Parse(jxb.jxbrl))
                        {
                            bool chosenchanged = TryXuanke(i, j);
                            if (chosenchanged)
                            {
                                Thread t = new Thread(() =>
                                {
                                    Course.GetChosenJxbs();
                                    CheckIsChosenJxbs();
                                });
                                t.Start();
                            }
                            return true;
                        }
                    }
                    else if (jxb.State == JxbStateEnum.Chosen)
                    {
                        if (i == 0)
                        {
                            Message = "任务完成";
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            Message = "容量已满";
            return true;
        }

        private bool TryXuanke(int i, int j)
        {
            bool chosenchanged = false;
            //----如果同级有已选，退出----//
            var jxb0 = Jxbs[i][j];
            foreach(var jxb in Jxbs[i])
                if (jxb.State == JxbStateEnum.Chosen)
                {
                    Message = "容量已满";
                    return false;
                }
                    
            //----退掉所有后面的课----//
            for (int p = i + 1; p < Jxbs.Count; p++)
                for (int q = 0; q < Jxbs[p].Count; q++)
                {
                    var jxb = Jxbs[p][q];
                    if (jxb.State == JxbStateEnum.Chosen)
                    {
                        var res = Course.Go退课(jxb);
                        if (!res.success)
                        {
                            Message = res.result;
                            Common.logger.log("退课失败：" + jxb.course.kcmc + " " + jxb.jxbmc);
                            Common.logger.log(res.result);
                            return chosenchanged;
                        }
                        else
                        {
                            chosenchanged = true;
                            Common.logger.log("退课成功：" + jxb.course.kcmc + " " + jxb.jxbmc);
                        }
                            
                    }
                }
            //----选课----//
            var res1 = Course.Go选课(jxb0);
            if (!res1.success)
            {
                Message = res1.result;
                Common.logger.log("选课失败：" + jxb0.course.kcmc + " " + jxb0.jxbmc);
                Common.logger.log(res1.result);
                return chosenchanged;
            }
            else
            {
                chosenchanged = true;
                Common.logger.log("选课成功：" + jxb0.course.kcmc + " " + jxb0.jxbmc);
            }
            Message = "抢课成功";
            return chosenchanged;
        }

        public void StartRun()
        {
            if (State == TaskState.Started) return;
            if (!CanRun())
            {
                //
                return;
            }
            if (BasicInfo.IsXKSJ)
            {
                State = TaskState.Started;
                initbgw();
                bgw.RunWorkerAsync();
                OnGoAnimation.Invoke();
            }
            else
            {
                State = TaskState.Wait;
                Message = "等待选课开始";
            }
        }

        public void StopRun()
        {
            //bgw = null;
            State = TaskState.None;
            Message = "等待开始";
        }

        public bool CanRun()
        {
            return type == TaskType.Full ? true : IsCourseIdValid && (BlockInfo != null);
        }

        public void ToggleRun()
        {
            if (State == TaskState.None)
            {
                StartRun();
            }
            else
            {
                StopRun();
            }
        }

        private void initbgw()
        {
            if (bgw == null)
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += Bgw_DoWork;
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            }
        }

        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                bgw.RunWorkerAsync();
            }
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var res = type == TaskType.Lite ? Go() : GoFull();//true继续
            e.Result = res;
            if (!res)
            {
                State = TaskState.None;
                return;
            }
            if (State != TaskState.Started)
            {
                StopRun();
                e.Result = false;
                return;
            }
            OnGoAnimation.Invoke();
            System.Threading.Thread.Sleep(Interval);
            if (State != TaskState.Started)
            {
                StopRun();
                e.Result = false;
                return;
            }
        }
        #endregion

        #region Constructors
        //Recover From Storage
        public TaskModel()
        {
            this.logger = new Logger();
        }
        //Add New
        public TaskModel(string title, int interval, TaskType type)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title;
            this.type = type;
            this.interval = interval;
            this.Jxbs = new BindingList<BindingList<JxbModel>>();
            this.logger = new Logger();
        }
        #endregion

        #region Other
        private string GetShortId(string s)
        {
            return s.Substring(s.LastIndexOf('-') + 1);
        }
        #endregion

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

        #endregion
    }

    public enum TaskState
    {
        None, Wait, Started
    }

    public enum TaskType
    {
        Lite, Full
    }
}
