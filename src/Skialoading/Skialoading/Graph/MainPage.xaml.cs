using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PanRepro
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Pos = "Text";
            InitializeComponent();
            var gest = new PanGestureRecognizer();
            grid.GestureRecognizers.Add(gest);
            grid2.GestureRecognizers.Add(gest);
            gest.PanUpdated += Gest_PanUpdated;
        }

        public string Pos { get; set; }

        private void Gest_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Completed)
                return;

            Pos = e.TotalX + " " + e.TotalY;
            foreach(var kid in grid.Children)
            {
                ((Label)kid).Text = Pos;
            }
            foreach (var kid in grid2.Children)
            {
                ((Label)kid).Text = Pos;
            }
        }
    }
}
