using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQiangke.Helpers
{
    public class Logger
    {
        public List<Log> list;

        public Logger()
        {
            list = new List<Log>();
        }

        public void log(string message)
        {
            list.Add(new Log(message));
        }
    }

    public class Log
    {
        public string message;
        public DateTime time;

        public Log(string message)
        {
            this.message = message;
            this.time = DateTime.Now;
        }

        public override string ToString()
        {
            return time.ToString("G") + "  " + message;
        }
    }
}
