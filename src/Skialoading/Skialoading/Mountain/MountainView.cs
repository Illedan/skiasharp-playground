using System;
using System.Collections.Generic;
using System.Linq;
using SkiaLoading.Time;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaDemo.Mountain
{
    public class MountainView : SKCanvasView
    {
        private TimeTracker timeTracker = new TimeTracker();
        private SKPaint fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
        };

        private static readonly Random rnd = new Random();
        private MountainGraph graph;
        private int lastAngle;

        public MountainView()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1.0 / 60.0), () =>
            {
                InvalidateSurface();
                return true;
            });
        }

        public int HeightDiff { get; set; } = 50;
        public float Speed { get; set; } = 10;
        public int UpdateLength { get; set; } = 50;
        public int MinUpdateLength { get; set; } = 5;
        public int MaxAngleChange { get; set; } = 30;
        public SKColor Color { get; set; } = SKColors.Black;

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            fillPaint.Color = Color;
            var canvas = e.Surface.Canvas;
            int width = e.Info.Width;
            int height = e.Info.Height;
            fillPaint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(width/2.0f, 0),
                    new SKPoint(width/2.0f, height),
                    new SKColor[] {Color,
                                                new SKColor(0xC0, 0xC0, 0xC0) },
                    null,
                    SKShaderTileMode.Clamp);
            canvas.Clear();
            if(graph == null || graph.Height != height || graph.Width != width)
            {
                graph = new MountainGraph(width, height);
            }

            MoveGraph();
            UpdateGraph(width);

            using (var path = new SKPath())
            {
                path.MoveTo(new SKPoint(graph.Path.First().X, height));
                foreach(var point in graph.Path)
                {
                    path.LineTo(point);
                }
                path.LineTo(new SKPoint(graph.Path[graph.Path.Count - 1].X, height));
                path.Close();
                canvas.DrawPath(path, fillPaint);
            }
        }

        private void MoveGraph()
        {
            //TODO: Optimize into class.
            var newPoints = new List<SKPoint>();
            var dt = timeTracker.GetTime();
            foreach(var point in graph.Path)
            {
                newPoints.Add(new SKPoint(point.X + Speed * dt, point.Y));
            }

            graph.Path.Clear();
            graph.Path.AddRange(newPoints);
        }

        private void UpdateGraph(int width)
        {
            if (!graph.Path.Any())
            {
                var x = ToFloat(UpdateLength * -1);
                var y = ToFloat(rnd.NextDouble() * HeightDiff);
                graph.Path.Add(new SKPoint(x, y));
            }

            while (graph.Path.Last().X < width + UpdateLength)
                AddRndPoint(false);

            while (graph.Path.Skip(0).First().X > -UpdateLength)
                AddRndPoint(true);

            while (graph.Path.Skip(1).First().X < -UpdateLength)
                graph.Path.RemoveAt(0);
        }

        private void AddRndPoint(bool first)
        {
            var lastPos = graph.Path.Last();
            var nextAngle = Math.Max(-85, Math.Min(85, rnd.Next(-1 * MaxAngleChange, MaxAngleChange + 1) + lastAngle));
            var length = rnd.NextDouble() * UpdateLength + MinUpdateLength;
            var dx = Math.Cos(DegreeToRadian(nextAngle))*length;
            var dy = Math.Sin(DegreeToRadian(nextAngle))*length;
            var nextX = first ? graph.Path.First().X - dx : (lastPos.X + dx);
            var nextY = Math.Max(0, Math.Min(HeightDiff, lastPos.Y + dy));
            if(!first)
                graph.Path.Add(new SKPoint(ToFloat(nextX), ToFloat(nextY)));
            else
                graph.Path.Insert(0, new SKPoint(ToFloat(nextX), ToFloat(nextY)));
            lastAngle = nextAngle;
            if (nextY > HeightDiff - 0.1) lastAngle = -MaxAngleChange;
            else if (nextY < 0.1) lastAngle = MaxAngleChange;
        }

        double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

        float ToFloat(double val) => (float)val;

        private class MountainGraph
        {
            public List<SKPoint> Path { get; } = new List<SKPoint>();
            public int Height { get; }
            public int Width { get; }
            public MountainGraph(int width, int height)
            {
                Height = height;
                Width = width;
            }
        }
    }
}