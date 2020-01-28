using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SkiaLoading.Graph
{
    public partial class GraphPage : ContentPage
    {
        public GraphPage()
        {
            BindingContext = new GraphViewModel();
            InitializeComponent();
        }
    }
}
