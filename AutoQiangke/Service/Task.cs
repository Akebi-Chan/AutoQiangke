using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoQiangke.Service
{
    public static class Task
    {
        public static BindingList<TaskModel> tasks = new BindingList<TaskModel>();

        public static void Init()
        {
            Application.Current.Resources.Add("Tasks", tasks);
        }
    }
}
