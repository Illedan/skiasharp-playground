using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SkiaLoading.Fireworks
{
    public partial class FireworksPage : ContentPage
    {
        public FireworksPage()
        {
            InitializeComponent();
            button.BindingContext = firework;
        }
    }
}
