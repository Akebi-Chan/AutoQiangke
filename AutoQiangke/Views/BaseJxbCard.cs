using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutoQiangke.Views
{
    public interface IBaseJxbCard 
    {
        public delegate void StartDragDropHandler(UserControl card);

        public event StartDragDropHandler StartDragDrop;

        JxbModel Jxb { get; set; }
    }
}
