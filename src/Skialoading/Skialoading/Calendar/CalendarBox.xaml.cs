using System;
using Xamarin.Forms;

namespace SkiaLoading.Calendar
{
    public partial class CalendarBox : ContentView
    {
        private static Color m_selectedColor = Color.DarkGreen, m_normalColor = Color.Transparent;
        private static Color m_selectedForeground = Color.WhiteSmoke, m_normalForeground= Color.Black;
        public CalendarBox()
        {
            InitializeComponent();
            Selected = false;
        }

        public string Day => Date.ToString("dd").ToUpper();
        public string DayOfWeek => Date.ToString("ddd");
        public string Month => Date.ToString("MMM").ToUpper();

        public Button Btn => btn;

        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create(nameof(Selected),
                typeof(bool),
                typeof(CalendarBox),
                false,
                BindingMode.TwoWay,
                propertyChanged: OnChanged);


        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set
            {
                SetValue(SelectedProperty, value);

            }
        }

        public static readonly BindableProperty DateProperty =
            BindableProperty.Create(nameof(Date),
                typeof(DateTime),
                typeof(CalendarBox),
                DateTime.MinValue,
                BindingMode.TwoWay,
                propertyChanged: OnChanged);

        private static void OnChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var cb = (CalendarBox)bindable;
            if (cb.Selected)
            {
                cb.monthlabel.IsVisible = true;
                cb.frame.BackgroundColor = m_selectedColor;
                cb.daylabel.TextColor = m_selectedForeground;
                cb.monthlabel.TextColor = m_selectedForeground;
                cb.datelabel.TextColor = m_selectedForeground;
                cb.frame.FadeTo(1.0, 500);
            }
            else
            {
                cb.monthlabel.IsVisible = false;
                cb.frame.BackgroundColor = m_normalColor;
                cb.daylabel.TextColor = m_normalForeground;
                cb.datelabel.TextColor = m_normalForeground;
                cb.frame.FadeTo(0.5, 50);
            }
        }

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set
            {
                SetValue(DateProperty, value);
                OnPropertyChanged(nameof(Day));
                OnPropertyChanged(nameof(DayOfWeek));
                OnPropertyChanged(nameof(Month));
            }
        }

    }
}
