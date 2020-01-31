using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SkiaLoading.Time;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkiaLoading.Calendar
{
    public partial class ScrollingCalendar : ContentView
    {
        private const double m_friction = 0.95, m_gravity = 10;
        private double m_lastWidth;
        private List<CalendarBox> m_items = new List<CalendarBox>();
        private double m_prevDx;
        private int m_dragId;
        private readonly object m_mutex = new object();

        public ScrollingCalendar()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Draw();
        }

        private void Draw()
        {
            if (SelectedDate == DateTime.MinValue || SelectedDate == DateTime.MaxValue || m_items.Count > 0)
            {
                return;
            }

            if (Math.Abs(Width - m_lastWidth) > 0.0001 && Width > 0)
            {
                var diff = 9;
                panel.Children.Clear();
                var date = SelectedDate.AddDays(diff/-2);
                for (var i = 0; i < diff; i++)
                {
                    var border = new CalendarBox
                    {
                        BackgroundColor = Color.WhiteSmoke,
                        Padding = 5,
                        Selected = date.Date.Day == SelectedDate.Date.Day,
                        Date = date
                    };

                    AbsoluteLayout.SetLayoutBounds(border, new Rectangle(Width / 5 * (i-2), 0, .2, 1));
                    AbsoluteLayout.SetLayoutFlags(border, AbsoluteLayoutFlags.SizeProportional);
                    panel.Children.Add(border);

                    date = date.AddDays(1);
                    m_items.Add(border);
                }
            }
        }

        private TimeTracker m_tracker;
        private List<(double dist, double t)> m_drags;

        public void OnUpdated(object s, PanUpdatedEventArgs args)
        {
            m_dragId = args.GestureId;
            if(args.StatusType == GestureStatus.Started)
            {
                m_tracker = new TimeTracker();
                m_drags = new List<(double dist, double t)>();
            }
            else if(args.StatusType == GestureStatus.Canceled)
            {

            }
            else if (args.StatusType != GestureStatus.Completed)
            {
                m_drags.Add((args.TotalX, m_tracker.GetTime()));
                MoveItems(args.TotalX - m_prevDx);
            }
            else if(m_drags.Count > 0)
            {
                var i = m_drags.Count - 1;
                var t = 0.0;
                var d = 0.0;
                for(; i >= 0; i--)
                {
                    t += m_drags[i].t;
                    if (t > 0.2) break;
                }

                if (i < 0) i = 0;
                var speed = (m_drags[m_drags.Count - 1].dist - m_drags[i].dist) / (t);
                var timer = new TimeTracker();
                var width = Width;

                Device.StartTimer(TimeSpan.FromMilliseconds(1000.0/60.0), () =>
                {
                    if (m_dragId != args.GestureId || double.IsInfinity(speed) || double.IsNaN(speed)) return false;
                    var time = timer.TotalTime();
                    var theSpeed = speed * Math.Pow(m_friction, time*150);
                    var dist =  theSpeed * time;

                    MoveItems(dist);
                    if (Math.Abs(theSpeed) <= 2)
                    {
                        SelectedChangedCommand?.Execute(SelectedDate);
                        var center = Width / 2 - Width / 10;
                        var closest = m_items.First(item => item.Date.Day == SelectedDate.Day);
                        var diff = center-closest.Bounds.X;
                        MoveItems(diff);
                        return false;
                    }

                    return true;
                });
                m_prevDx = 0;
            }

            m_prevDx = args.TotalX;
        }

        private void MoveItems(double dx)
        {
            lock (m_mutex)
            {
                while (dx < 0 && m_items[8].Bounds.X < Width + Width / 2.5)
                {
                    var last = m_items[8];
                    var first = m_items[0];
                    AbsoluteLayout.SetLayoutBounds(first, new Rectangle(last.Bounds.X + Width/ 5, 0, .2, 1));
                    m_items.RemoveAt(0);
                    m_items.Add(first);
                    first.Date = last.Date.AddDays(1);
                }
                while (dx > 0 && m_items[0].Bounds.X > Width / -2.5)
                {
                    var first = m_items[0];
                    var last = m_items[8];
                    AbsoluteLayout.SetLayoutBounds(last, new Rectangle(first.Bounds.X - Width / 5, 0, .2, 1));
                    m_items.RemoveAt(8);
                    m_items.Insert(0, last);
                    last.Date = first.Date.AddDays(-1);
                }

                for (var z = 0; z < 9; z++)
                {
                    var item = m_items[z];
                    var current = AbsoluteLayout.GetLayoutBounds(item);
                    AbsoluteLayout.SetLayoutBounds(m_items[z], new Rectangle(current.X + dx, 0, .2, 1));
                }

                var prevSelected = m_items.FirstOrDefault(item => item.Selected);
                var center = Width / 2 - Width / 10;
                var closest = m_items.OrderBy(item => Math.Abs(item.Bounds.X - center)).First();
                closest.Selected = true;
                SelectedDate = closest.Date;
                if(prevSelected != null && closest != prevSelected)
                {
                    try
                    {
                        var duration = TimeSpan.FromSeconds(0.01);
                        Vibration.Vibrate(duration);
                    }
                    catch (FeatureNotSupportedException ex)
                    {
                        // Feature not supported on device
                    }
                    catch (Exception ex)
                    {
                        // Other error has occurred.
                    }
                }

                for (var z = 0; z < 9; z++)
                {
                    var item = m_items[z];
                    if (item != closest) item.Selected = false;
                }
            }
        }

        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate),
                typeof(DateTime),
                typeof(ScrollingCalendar),
                DateTime.MinValue,
                defaultBindingMode: BindingMode.TwoWay);

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        //private static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var caller = (ScrollingCalendar)bindable;
        //    caller.Draw();
        //}


        public static readonly BindableProperty SelectedChangedCommandProperty = BindableProperty.Create("SelectedChangedCommand", typeof(ICommand), typeof(ScrollingCalendar), null);

        public ICommand SelectedChangedCommand
        {
            get { return (ICommand)GetValue(SelectedChangedCommandProperty); }
            set { SetValue(SelectedChangedCommandProperty, value); }
        }

        public static Page GetParentPage(VisualElement element)
        {
            if (element != null)
            {
                var parent = element.Parent;
                while (parent != null)
                {
                    if (parent is Page)
                    {
                        return parent as Page;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }
    }
}
