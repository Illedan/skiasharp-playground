using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SkiaLoading.Calendar
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage()
        {
            InitializeComponent();
            BindingContext = new CalendarViewModel(DateTime.Now);
        }
    }
}
