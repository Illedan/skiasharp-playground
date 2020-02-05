using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PanRepro
{
    public partial class MyPage : ContentPage
    {
        public MyPage()
        {
            Pos = "Text";
            InitializeComponent();
            var gest = new PanGestureRecognizer();
            grid.GestureRecognizers.Add(gest);
            gest.PanUpdated += Gest_PanUpdated;
        }

        public string Pos { get; set; }

        private void Gest_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Pos = e.TotalX + " " + e.TotalY;
            foreach (var kid in grid.Children)
            {
                ((Label)kid).Text = Pos;
            }
        }
    }
}
