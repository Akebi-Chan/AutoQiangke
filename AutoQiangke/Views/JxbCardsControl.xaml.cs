using AutoQiangke.Models;
using AutoQiangke.Views;
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
    /// JxbCardsControl.xaml 的交互逻辑
    /// </summary>
    public partial class JxbCardsControl : UserControl
    {
        public BindingList<BindingList<JxbModel>> Jxbs
        {
            get { return (BindingList<BindingList<JxbModel>>)GetValue(JxbsProperty); }
            set { SetValue(JxbsProperty, value); InitCards(value); ArrangeCards(); }
        }

        public static readonly DependencyProperty JxbsProperty =
            DependencyProperty.Register("Jxbs", typeof(BindingList<BindingList<JxbModel>>), typeof(JxbCardsControl), new FrameworkPropertyMetadata(default(object), propertyChangedCallback, OnJxbsChanged) { BindsTwoWayByDefault = true });

        private static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("PropertyChangedCallback - OnJxbsChanged");
        }

        private static object OnJxbsChanged(DependencyObject d, object baseValue)
        {
            Debug.WriteLine("CoerceValueCallback - OnJxbsChanged");
            if (baseValue == null) return baseValue;
            if (baseValue is BindingList<BindingList<JxbModel>> jxbs)
            {
                JxbCardsControl control = (JxbCardsControl)d;
                if (jxbs == control.newjxbs)
                {
                    control.newjxbs = null;
                    return baseValue;
                }
                control.InitCards(jxbs); control.ArrangeCards();
            }
            return baseValue;
        }

        public BindingList<BindingList<JxbModel>> newjxbs;
        public List<List<UserControl>> Cards;
        private int CardWidth;
        private int CardHeight;
        public intPoint dragcardpos;
        public UserControl dragcard;
        public bool newcolumn;
        private TaskType type;
        public TaskType Type
        {
            get { return type; }
            set { type = value; }
        }

        public JxbCardsControl()
        {
            InitializeComponent();
        }

        public void InitCardSize()
        {
            if (Type == TaskType.Lite)
            {
                CardWidth = JxbCardLite.cardwidth;
                CardHeight = JxbCardLite.cardheight;
            }
            else
            {
                CardWidth = JxbCardFull.cardwidth;
                CardHeight = JxbCardFull.cardheight;
            }
        }
        //public void AddJxb(JxbModel jxb)
        //{
        //    var c2 = new List<UserControl>();
        //    var card = new UserControl(jxb);
        //    card.StartDragDrop += Card_StartDragDrop;
        //    c2.Add(card);
        //    Cards.Insert(0, c2);
        //    ArrangeCards();
        //}

        private void InitCards(BindingList<BindingList<JxbModel>> jxbs)
        {
            ClearCardsEvent();
            Cards = new List<List<UserControl>>();
            for (int i = 0; i < jxbs.Count; i++)
            {
                List<UserControl> c = new List<UserControl>();
                for (int j = 0; j < jxbs[i].Count; j++)
                {
                    UserControl card = (Type == TaskType.Lite) ? new JxbCardLite(jxbs[i][j]) : new JxbCardFull(jxbs[i][j]);
                    c.Add(card);
                    ((IBaseJxbCard)card).StartDragDrop += Card_StartDragDrop;
                }
                Cards.Add(c);
            }
        }

        private void ClearCardsEvent()
        {
            if (Cards == null) return;
            for (int i = 0; i < Cards.Count; i++)
                for (int j = 0; j < Cards[i].Count; j++)
                    ((IBaseJxbCard)Cards[i][j]).StartDragDrop -= Card_StartDragDrop;
        }

        private void ArrangeCards()
        {
            int canvasheight = 0;
            int canvaswidth = 0;
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
            MainCanvas.Width = canvaswidth * CardWidth + CardWidth / 2;
            MainCanvas.Height = canvasheight * CardHeight + CardHeight / 2;
        }

        private void ArrangeCards1(List<JxbCardArrange> cardArranges)
        {
            int canvasheight = 0;
            int canvaswidth = 0;
            for (int i = 0; i < cardArranges.Count; i++)
            {
                var t = cardArranges[i];
                canvaswidth = Math.Max(t.pos.x + 1, canvaswidth);
                canvasheight = Math.Max(t.pos.y + 1, canvasheight);
                var p = GetPosition(t.pos.x, t.pos.y);
                Canvas.SetLeft(t.card, p.X);
                Canvas.SetTop(t.card, p.Y);
            }
            dragcard.Opacity = 0.5;
            var p1 = GetPosition(dragcardpos.x, dragcardpos.y);
            Canvas.SetLeft(dragcard, p1.X);
            Canvas.SetTop(dragcard, p1.Y);

            MainCanvas.Width = canvaswidth * CardWidth + CardWidth / 2;
            MainCanvas.Height = canvasheight * CardHeight + CardHeight / 2;
        }

        private void Card_StartDragDrop(UserControl card)
        {
            //card.Background = new SolidColorBrush(Colors.Red);
            dragcard = card;
            DeleteCard(card);
            for (int i = 0; i < Cards.Count; i++)
                for (int j = 0; j < Cards[i].Count; j++)
                    Cards[i][j].IsHitTestVisible = false;
            dragcard.IsHitTestVisible = false;
            DragDrop.DoDragDrop(card, card, DragDropEffects.Move);
        }

        private void DeleteCard(UserControl card)
        {
            foreach (var column in Cards)
                if (column.Contains(card))
                {
                    column.Remove(card);
                    if (column.Count == 0)
                        Cards.Remove(column);
                    break;
                }
        }

        private void MainCanvas_Drop(object sender, DragEventArgs e)
        {
            if (GetJxbCardData(e) == null) return;
            for (int i = 0; i < Cards.Count; i++)
                for (int j = 0; j < Cards[i].Count; j++)
                    Cards[i][j].IsHitTestVisible = true;
            dragcard.IsHitTestVisible = true;
            dragcard.Opacity = 1;
            if (newcolumn)
            {
                var c = new List<UserControl>();
                c.Add(dragcard);
                Cards.Insert(dragcardpos.x, c);
            }
            else if (dragcardpos.x > Cards.Count - 1)
            {
                var c = new List<UserControl>();
                c.Add(dragcard);
                Cards.Add(c);
            }
            else
            {
                Cards[dragcardpos.x].Insert(dragcardpos.y, dragcard);
            }

            UpdateJxbsAccordingToCards();
        }

        private void UpdateJxbsAccordingToCards()
        {
            this.Jxbs.Clear();
            newjxbs = this.Jxbs;//new BindingList<BindingList<JxbModel>>();
            for (int i = 0; i < Cards.Count; i++)
            {
                BindingList<JxbModel> c = new BindingList<JxbModel>();
                for (int j = 0; j < Cards[i].Count; j++)
                {
                    c.Add(((IBaseJxbCard)Cards[i][j]).Jxb);
                }
                newjxbs.Add(c);
            }
            this.Jxbs = newjxbs;
        }

        private void MainCanvas_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (GetJxbCardData(e) == null) return;
            Point p = e.GetPosition(MainCanvas);
            intPoint pos = intPoint.GetPos(p, new intPoint(CardWidth, CardHeight));
            bool flag = false;//是否已有card
            if (pos.x <= Cards.Count - 1)
                if (pos.y <= Cards[pos.x].Count - 1)
                    flag = true;
            newcolumn = false;
            int delta = 0;
            if (flag)
                if (pos.y == 0)
                    delta = GetEdge(p, pos);
            var newarrange = new List<JxbCardArrange>();
            if (flag)
            {
                if (delta != 0)
                {
                    for (int i = 0; i < Cards.Count; i++)
                        for (int j = 0; j < Cards[i].Count; j++)
                        {
                            var t = Cards[i][j];
                            intPoint pos1 = new intPoint(i, j);
                            if (i >= pos.x + (delta == 1 ? 1 : 0))
                                pos1.x++;
                            newarrange.Add(new JxbCardArrange(t, pos1));
                        }
                    dragcardpos = new intPoint(pos.x + (delta == 1 ? 1 : 0), 0);
                    newcolumn = true;
                }
                else
                {
                    for (int i = 0; i < Cards.Count; i++)
                        for (int j = 0; j < Cards[i].Count; j++)
                        {
                            var t = Cards[i][j];
                            intPoint pos1 = new intPoint(i, j);
                            if (i == pos.x && j >= pos.y)
                                pos1.y++;
                            newarrange.Add(new JxbCardArrange(t, pos1));
                        }
                    dragcardpos = pos;
                }
            }
            else
            {
                for (int i = 0; i < Cards.Count; i++)
                    for (int j = 0; j < Cards[i].Count; j++)
                    {
                        var t = Cards[i][j];
                        intPoint pos1 = new intPoint(i, j);
                        newarrange.Add(new JxbCardArrange(t, pos1));
                    }
                if (pos.x >= Cards.Count)
                    dragcardpos = new intPoint(Cards.Count, 0);
                else
                    dragcardpos = new intPoint(pos.x, Cards[pos.x].Count);
            }
            ArrangeCards1(newarrange);
            e.Handled = true;
        }

        private UserControl GetJxbCardData(DragEventArgs e)
        {
            var res1 = (JxbCardLite)e.Data.GetData(typeof(JxbCardLite));
            if (res1 != null)
                return res1;
            var res2 = (JxbCardFull)e.Data.GetData(typeof(JxbCardFull));
            if (res2 != null)
                return res2;
            return null;
        }
        private int GetEdge(Point p, intPoint pos)
        {
            double left = pos.x * CardWidth;
            double right = (pos.x + 1) * CardWidth;
            if ((p.X - left) / CardWidth < 0.1)
                return -1;
            if ((right - p.X) / CardWidth < 0.1)
                return 1;
            return 0;
        }

        private Point GetPosition(int x,int y)
        {
            return new Point(x * CardWidth, y * CardHeight);
        }

        private void MainCanvas_PreviewDragLeave(object sender, DragEventArgs e)
        {
            //Debug.WriteLine("MainCanvas_PreviewDragLeave");
            if (GetJxbCardData(e) == null) return;
            for (int i = 0; i < Cards.Count; i++)
                for (int j = 0; j < Cards[i].Count; j++)
                {
                    var card = Cards[i][j];
                    card.IsHitTestVisible = true;
                    var p = GetPosition(i, j);
                    Canvas.SetLeft(card, p.X);
                    Canvas.SetTop(card, p.Y);
                }
            MainCanvas.Children.Remove(dragcard);
            ((IBaseJxbCard)dragcard).StartDragDrop -= Card_StartDragDrop;
            dragcard = null;
            UpdateJxbsAccordingToCards();
            e.Handled = true;
        }

        private void MainCanvas_PreviewDragEnter(object sender, DragEventArgs e)
        {
            //Debug.WriteLine("MainCanvas_PreviewDragEnter");
            dragcard = GetJxbCardData(e);
            if (dragcard == null) return;
            ((IBaseJxbCard)dragcard).StartDragDrop += Card_StartDragDrop;
            for (int i = 0; i < Cards.Count; i++)
                for (int j = 0; j < Cards[i].Count; j++)
                    Cards[i][j].IsHitTestVisible = false;
            if (!MainCanvas.Children.Contains(dragcard))
                MainCanvas.Children.Add(dragcard);
            e.Handled = true;
        }
    }

    public class JxbCardArrange
    {
        public UserControl card;
        public intPoint pos;

        public JxbCardArrange(UserControl card, intPoint pos)
        {
            this.card = card;
            this.pos = pos;
        }
    }

    public class intPoint
    {
        public int x;
        public int y;
        public intPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        internal static intPoint GetPos(Point p, intPoint CardSize)
        {
            return new intPoint((int)(p.X / CardSize.x), (int)(p.Y / CardSize.y));
        }
    }
}
