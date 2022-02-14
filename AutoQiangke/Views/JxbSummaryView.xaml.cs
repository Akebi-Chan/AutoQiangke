using AutoQiangke.Converters;
using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// JxbSummaryView.xaml 的交互逻辑
    /// </summary>
    public partial class JxbSummaryView : UserControl
    {
        public BindingList<BindingList<JxbModel>> Jxbs
        {
            get { return (BindingList<BindingList<JxbModel>>)GetValue(JxbsProperty); }
            set { SetValue(JxbsProperty, value); InitCards(value); ArrangeCards(); }
        }

        public static readonly DependencyProperty JxbsProperty =
            DependencyProperty.Register("Jxbs", typeof(BindingList<BindingList<JxbModel>>), typeof(JxbSummaryView), new FrameworkPropertyMetadata(default(object), null, OnJxbsChanged) { BindsTwoWayByDefault = true });

        private static object OnJxbsChanged(DependencyObject d, object baseValue)
        {
            Debug.WriteLine("Summary - OnJxbsChanged");
            JxbSummaryView control = (JxbSummaryView)d;
            control.InitCards((BindingList<BindingList<JxbModel>>)baseValue); control.ArrangeCards();
            return baseValue;
        }

        public List<List<Border>> Cards;

        public JxbSummaryView()
        {
            InitializeComponent();
        }

        public void ArrangeCards()
        {
            int canvasheight = int.MinValue;
            int canvaswidth = int.MinValue;
            MainCanvas.Children.Clear();
            for (int i = 0; i < Cards.Count; i++)
                for (int j = 0; j < Cards[i].Count; j++)
                {
                    canvaswidth = Math.Max(i + 1, canvaswidth);
                    canvasheight = Math.Max(j + 1, canvasheight);
                    var card = Cards[i][j];
                    MainCanvas.Children.Add(card);
                    var p = GetPosition(i, j);
                    Canvas.SetLeft(card, p.X);
                    Canvas.SetTop(card, p.Y);
                }
            MainCanvas.Width = canvaswidth * 16;
            MainCanvas.Height = canvasheight * 16;
        }

        public void InitCards(BindingList<BindingList<JxbModel>> value)
        {
            Cards = new List<List<Border>>();
            if (value == null) return;
            for (int i = 0; i < value.Count; i++)
            {
                List<Border> c = new List<Border>();
                for (int j = 0; j < value[i].Count; j++)
                {
                    var card = new Border() { Width=12,Height=12};
                    card.Background = JxbStateToBrush.Convert(value[i][j].State);
                    c.Add(card);
                }
                Cards.Add(c);
            }
        }
        private Point GetPosition(int x, int y)
        {
            return new Point(x * 16 + 2, y * 16 + 2);
        }

    }
}
