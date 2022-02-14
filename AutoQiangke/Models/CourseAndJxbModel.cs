using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AutoQiangke.Models
{
    public class CourseQueryJsonDto
    {
        public List<CourseQueryDto> tmpList;
    }

    public class PreQueryCourseJsonDto
    {
        public List<PreQueryCourseDto> items;
        public int totalCount;
    }

    public class PreQueryCourseDto
    {
        public string jxb_id;//教学班id
        public string jxbmc;//教学班名称
        public string kcmc;//课程名称
        public string kcdm;//课程号（）
        public string jsxx;//老师
        public string js;//地点
        public string kcap;//时间
    }

    public class JxbQueryResult
    {
        public List<JxbModel> jxbs;
        public CourseModel course;
        public bool success;
        public string message;

        public JxbQueryResult()
        {
        }

        public JxbQueryResult(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }

        public JxbQueryResult(List<JxbModel> jxb, CourseModel c, string message)
        {
            this.jxbs = jxb;
            this.course = c;
            this.message = message;
            this.success = true;
        }
    }

    public class PreQueryCourseResult
    {
        public CourseModel course;
        public List<JxbModel> jxbs;
        public bool success;
        public string message;

        public PreQueryCourseResult()
        {
        }

        public PreQueryCourseResult(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }

        public PreQueryCourseResult(bool success, CourseModel course)
        {
            this.course = course;
            this.success = success;
        }

        public PreQueryCourseResult(bool success, CourseModel course, List<JxbModel> jxbs)
        {
            this.course = course;
            this.success = success;
            this.jxbs = jxbs;
        }
    }

    //public class PreQueryJxbFullDto
    //{
    //    public string jxb_id;//教学班id
    //    public string jxbmc;//教学班名称
    //    public string kcmc;//课程名称
    //    public string kcdm;//课程号（）
    //    public string jsxx;//老师
    //    public string js;//地点
    //    public string kcap;//时间
    //}

    //public class PreQueryJxbFullJsonDto
    //{
    //    public List<PreQueryJxbFullDto> items;
    //    public int totalCount;
    //}

    public class PreQueryJxbFullResult
    {
        public JxbModel jxb;
        public bool success;
        public string message;

        public PreQueryJxbFullResult()
        {
        }

        public PreQueryJxbFullResult(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }

        public PreQueryJxbFullResult(bool success, JxbModel jxb)
        {
            this.jxb = jxb;
            this.success = success;
        }
    }

    public class CourseQueryDto
    {
        public string jxb_id;//教学班id
        public string jxbmc;//教学班名称
        public string kcmc;//课程名称
        public string yxzrs;//已选人数
        public string kch_id;//课程代码
        public string kch;//课程号
        public string cxbj;//重修标记
        public string fxbj;//辅修标记
        public string xxkbj;//？？标记
    }

    public class JxbQueryDto
    {
        public string jxb_id;//教学班id
        public string do_jxb_id;
        public string jsxx;//老师
        public string jxdd;//地点
        public string sksj;//时间
        public string kch_id;//课程号
        public string jxbrl;//容量
    }

    public class CourseModel
    {
        public string id;
        public BlockInfo blockInfo;
        public string kcmc;
        public string kch_id;
        public string kch;
        public string cxbj;
        public string fxbj;
        public string xxkbj;
        public int jxbcount;
        //PreQuery
        public CourseModel()
        {
            this.id = Guid.NewGuid().ToString();
        }
        //Query
        public CourseModel(BlockInfo blockInfo, CourseQueryDto courseQueryDto)
        {
            this.blockInfo = blockInfo;
            this.kcmc = courseQueryDto.kcmc;
            this.kch_id = courseQueryDto.kch_id;
            this.kch = courseQueryDto.kch;
            this.cxbj = courseQueryDto.cxbj;
            this.fxbj = courseQueryDto.fxbj;
            this.xxkbj = courseQueryDto.xxkbj;
            this.id = Guid.NewGuid().ToString();
        }
    }

    public class JxbModel : INotifyPropertyChanged
    {
        public CourseModel course;
        public string do_jxb_id;
        public string jsxx;
        public string jxdd;
        public string sksj;
        public string jxbrl;
        public string jxb_id;
        public string jxbmc;
        private string jxb_shortid;
        public string yxzrs;

        public string Kcmc
        {
            get { return course.kcmc; }
            set
            {
                course.kcmc = value;
            }
        }//Only For Full
        public string Jxbmc
        {
            get { return jxbmc; }
            set
            {
                jxbmc = value;
                this.RaisePropertyChanged("Jxbmc");
            }
        }//Only For Full
        public string Jxb_shortid
        {
            get { return jxb_shortid; }
            set
            {
                jxb_shortid = value;
                this.RaisePropertyChanged("Jxb_shortid");
            }
        }//Only For Lite
        public string Teacher
        {
            get { return jsxx; }
            set
            {
                jsxx = value;
                this.RaisePropertyChanged("Teacher");
            }
        }
        public string CourseTime
        {
            get { return sksj; }
            set
            {
                sksj = value;
                this.RaisePropertyChanged("CourseTime");
            }
        }
        public string CourseLocation
        {
            get { return jxdd; }
            set
            {
                jxdd = value;
                this.RaisePropertyChanged("CourseLocation");
            }
        }

        private JxbStateEnum state;
        public JxbStateEnum State
        {
            get { return state; }
            set
            {
                state = value;
                this.RaisePropertyChanged("State");
            }
        }

        private string stateStr;
        public string StateStr
        {
            get { return stateStr; }
            set
            {
                stateStr = value;
                this.RaisePropertyChanged("StateStr");
            }
        }
        //New Jxb(Lite)
        public JxbModel(CourseModel c,string jxb_shortid)
        {
            this.course = c;
            this.Jxb_shortid = jxb_shortid;
            State = JxbStateEnum.Unknow;
            StateStr = "未知";
        }
        //New Jxb(Full)
        public JxbModel(string jxbmc)
        {
            this.jxbmc = jxbmc;
            State = JxbStateEnum.Unknow;
            StateStr = "未知";
        }
        //Storage
        public JxbModel()
        {
            State = JxbStateEnum.Unknow;
        }
        //Query，Query中间有UpdateJxbs
        public JxbModel(JxbQueryDto a, CourseQueryDto b, CourseModel c)
        {
            course = c;
            do_jxb_id = a.do_jxb_id;
            jsxx = a.jsxx;
            sksj = a.sksj;
            jxbrl = a.jxbrl;
            jxdd = a.jxdd;
            jxb_id = b.jxb_id;
            jxbmc = b.jxbmc;
            yxzrs = b.yxzrs;
            State = JxbStateEnum.Unknow;
        }
        //PreQuery，成功后有UpdateJxbsAfterPreQuery
        public JxbModel(PreQueryCourseDto a, CourseModel c)
        {
            course = c;
            jxb_id = a.jxb_id;
            jxbmc = a.jxbmc;
            jsxx = a.jsxx;
            jxdd = a.js;
            sksj = a.kcap;
            State = JxbStateEnum.Unknow;
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

        #endregion
    }

    public class ChosenJxbDto
    {
        public string do_jxb_id;
        public string jsxx;
        public string jxdd;
        public string sksj;
        public string jxbrl;
        public string jxb_id;
        public string jxbmc;
        public string yxzrs;

        public string kcmc;
        public string kch_id;
        public string kch;

        public string kklxmc;
        public string kklxdm;
    }

    public class ChosenJxbJsonDto
    {
        public List<ChosenJxbDto> items;
        public int totalCount;
    }

    public enum JxbStateEnum
    {
        Unknow, Ready, Known, Chosen, Error, NotFound
    }
}
