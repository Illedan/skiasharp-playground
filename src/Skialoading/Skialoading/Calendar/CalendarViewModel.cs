using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SkiaLoading.Calendar
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private DateTime selectedDate;
        private string dateUrl;

        public CalendarViewModel(DateTime selectedDate)
        {
            SelectedDate = selectedDate;
            SelectedDateCommand = new Command(() => { DateUrl += "YOLO"; });
        }

        public ICommand SelectedDateCommand { get; }

        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedDate)));
                DateUrl = selectedDate.ToString("dd.MM.yyyy");
            }
        }

        public string DateUrl
        {
            get => dateUrl;
            set
            {
                dateUrl = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(DateUrl)));
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
