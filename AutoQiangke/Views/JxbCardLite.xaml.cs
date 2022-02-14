using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoQiangke.Views
{
    /// <summary>
    /// JxbCard.xaml 的交互逻辑
    /// </summary>
    public partial class JxbCardLite : UserControl, IBaseJxbCard
    {
        public static readonly int cardheight = 120;
        public static readonly int cardwidth = 140;

        private JxbModel jxb;
        public JxbModel Jxb
        {
            get { return jxb; }
            set { jxb = value; }
        }

        public JxbCardLite()
        {
            InitializeComponent();
            this.MouseDown += JxbCard_MouseDown;
            this.MouseMove += JxbCard_MouseMove;
            this.MouseUp += JxbCard_MouseUp;
            this.MouseLeave += JxbCard_MouseLeave;
        }

        public JxbCardLite(JxbModel jxb)
        {
            InitializeComponent();
            this.DataContext = jxb;
            this.jxb = jxb;
            this.MouseDown += JxbCard_MouseDown;
            this.MouseMove += JxbCard_MouseMove;
            this.MouseUp += JxbCard_MouseUp;
            this.MouseLeave += JxbCard_MouseLeave;
        }

        private void JxbCard_MouseLeave(object sender, MouseEventArgs e)
        {
            p1 = null;
        }

        private void JxbCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            p1 = null;
        }

        Point? p1 = null;

        public event IBaseJxbCard.StartDragDropHandler StartDragDrop;

        private void JxbCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (p1 == null)
                return;
            Point p2 = e.GetPosition(this);
            if (Distance(p1.Value, p2) > 5)
            {
                StartDragDrop.Invoke(this);
                p1 = null;
            }
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        private void JxbCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            p1 = e.GetPosition(this);
        }
    }
}
