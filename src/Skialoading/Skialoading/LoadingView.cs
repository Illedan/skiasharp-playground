using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Skialoading
{
    public class LoadingView : SKCanvasView
    {
        private readonly SKPaint strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 10,
            IsStroke = true,
            Color = SKColors.Black
        };

        private int index;
        private Graph graph;
        public LoadingView()
        {
            PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                InvalidateSurface();
                return true;
            });
        }

        public int Length { get; set; } = 60;
        public int Speed { get; set; } = 3;
        public int StrokeWidth { get; set; } = 5;

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            strokePaint.StrokeWidth = StrokeWidth;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            int width = e.Info.Width;
            int height = e.Info.Height;
            canvas.Clear();
            canvas.Save();
            canvas.Translate(width / 2, height / 2);

            if(graph == null || graph.Width != width || graph.Height != height)
            {
                graph = CreateGraph(width, height);
            }

            for(var i = 0; i < Speed; i++)
            {
                index = GetNext(index, graph.Path.Count);
            }

            using(var path = new SKPath())
            {
                var pos = index;
                var point = graph.Path[pos];
                path.MoveTo(point);
                for(var i = 0; i < Length; i++)
                {
                    pos = GetNext(pos, graph.Path.Count);
                    point = graph.Path[pos];
                    path.LineTo(point);
                }
                canvas.DrawPath(path, strokePaint);
            }

            canvas.Restore();
        }

        private int GetNext(int i, int max)
        {
            i++;
            if (i >= max) i = 0;
            return i;
        }

        private static Graph CreateGraph(int width, int height)
        {
            var path = new List<SKPoint>();

            var size = width / 8.0;
            double DegreeToRadian(double angle)
            {
                return Math.PI * angle / 180.0;
            }

            float ToFloat(double val) => (float)val;

            for(var i = 180; i > -180; i--)
            {
                var rad = DegreeToRadian(i);
                var x = ToFloat(Math.Cos(rad) * size + size);
                var y = ToFloat(Math.Sin(rad) * size);
                path.Add(new SKPoint(x, y));
            }

            for (var i = 0; i < 360; i++)
            {
                var rad = DegreeToRadian(i);
                var x = ToFloat(Math.Cos(rad) * size - size);
                var y = ToFloat(Math.Sin(rad) * size);
                path.Add(new SKPoint(x, y));
            }

            return new Graph(width, height, path);
        }
    }

    public class Graph
    {
        public int Width { get; }
        public int Height { get; }
        public List<SKPoint> Path { get; }
        public Graph(int width, int height, List<SKPoint> path)
        {
            Width = width;
            Height = height;
            Path = path;
        }
    }
}
