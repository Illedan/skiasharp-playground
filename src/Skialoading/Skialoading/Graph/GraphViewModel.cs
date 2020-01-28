﻿using System;
using System.ComponentModel;
using DIPS.Xamarin.UI.Controls.Slidable;
using DIPS.Xamarin.UI.Extensions;

namespace SkiaLoading.Graph
{
    public class GraphViewModel : DataRepository, INotifyPropertyChanged
    {
        private SlidableProperties slidableProperties;

        public GraphViewModel()
        {
            AddPoint(new GraphPoint(0, "T1", 5));
            AddPoint(new GraphPoint(1, "T2", 2));
            AddPoint(new GraphPoint(2, "T3", 1));
            AddPoint(new GraphPoint(5, "T4", 7));
            AddPoint(new GraphPoint(6, "T5", 3));
            AddPoint(new GraphPoint(8, "T6", 4));
            AddPoint(new GraphPoint(10, "T7", 2));
        }

        public SliderConfig Config => new SliderConfig(-11, 0);

        public SlidableProperties SlidableProperties { get => slidableProperties; set => PropertyChanged?.RaiseWhenSet(ref slidableProperties, value); }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
