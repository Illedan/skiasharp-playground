using System;
using System.ComponentModel;
using DIPS.Xamarin.UI.Controls.Slidable;
using DIPS.Xamarin.UI.Extensions;

namespace SkiaLoading.Graph
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        private SlidableProperties slidableProperties;

        public GraphViewModel()
        {
            SlidableProperties = new SlidableProperties(-20);
            //AddPoint(new GraphPoint(0, "T1", 5));
            //AddPoint(new GraphPoint(1, "T2", 2));
            //AddPoint(new GraphPoint(2, "T3", 1));
            //AddPoint(new GraphPoint(5, "T4", 7));
            //AddPoint(new GraphPoint(6, "T5", 3));
            //AddPoint(new GraphPoint(8, "T6", 4));
            //AddPoint(new GraphPoint(10, "T7", 2));
            var rnd = new Random();
            for(var i = 0; i < 100; i++)
            {
                if (rnd.NextDouble() < 0.2) continue;
                if (rnd.NextDouble() < 0.7)
                    Graph1.AddPoint(new GraphPoint(i, "T" + i, rnd.Next(3, 7)));
                else
                    Graph1.AddPoint(new GraphPoint(i, "T"+i, rnd.Next(0, 10)));
            }

            for (var i = 0; i < 100; i++)
            {
                if (rnd.NextDouble() < 0.1) continue;
                if (rnd.NextDouble() < 0.3)
                    Graph2.AddPoint(new GraphPoint(i, "T" + i, rnd.Next(3, 7)));
                else
                    Graph2.AddPoint(new GraphPoint(i, "T" + i, rnd.Next(0, 14)));
            }
        }

        public DataRepository Graph1 { get; } = new DataRepository();
        public DataRepository Graph2 { get; } = new DataRepository();

        public ReferenceArea Reference { get; } = new ReferenceArea(3, 7);
        public SliderConfig Config => new SliderConfig(-100, 0);

        public SlidableProperties SlidableProperties { get => slidableProperties; set => PropertyChanged?.RaiseWhenSet(ref slidableProperties, value); }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
