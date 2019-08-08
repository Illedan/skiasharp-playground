using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
namespace SkiaLoading.Stars
{
    public class StarView : SKCanvasView
    {
        private SKPaint fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
        };
        private int lastHeight;
        private static readonly Random rnd = new Random();
        private static List<Star> stars = new List<Star>();
        public StarView()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                InvalidateSurface();
                return true;
            });
        }

        public int NumStars { get; set; } = 60;
        public int MinDist { get; set; } = 30;
        public int MaxSize { get; set; } = 5;
        public int MinSize { get; set; } = 1;
        public float Speed { get; set; } = 1;
        public SKColor Color { get; set; } = SKColors.Yellow;

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            fillPaint.Color = Color;
            var canvas = e.Surface.Canvas;
            var width = e.Info.Width;
            var height = e.Info.Height;
            canvas.Clear();

            if (stars.Count > NumStars || lastHeight != height)
                stars.Clear();
            lastHeight = height;

            MoveStars();
            RemoveOutsideStars(width);
            SpawnNewStarts(width, height);
            RenderStars(canvas);
        }

        private void RenderStars(SKCanvas canvas)
        {
            foreach(var star in stars)
            {
                canvas.DrawCircle(star.GetPoint, star.Radius, fillPaint);
            }
        }

        private void SpawnNewStarts(int width, int height)
        {
            if (!stars.Any())
            {
                for(var i = 0; i < NumStars*2 && stars.Count < NumStars; i++)
                {
                    var star = CreateRandomStar(width, height);
                    if (stars.Any(s => s.Distance(star) < MinDist)) continue;
                    stars.Add(star);
                }
            }

            for(var i = 0; i < NumStars && stars.Count < NumStars; i++)
            {
                var star = CreateRandomStar(width, height);
                star.X = Speed > 0 ? rnd.Next(-MinDist, 0) : rnd.Next(width, width + MinDist);
                if (stars.Any(s => s.Distance(star) < MinDist)) continue;
                stars.Add(star);
            }
        }

        private Star CreateRandomStar(int width, int height)
        {
            return new Star(rnd.Next(width), rnd.Next(height), ToFloat(rnd.NextDouble() * (MaxSize - MinSize) + MinSize));
        }

        private void RemoveOutsideStars(int width)
        {
            stars.RemoveAll(s => s.X < -MinDist || s.X > width + MinDist);
        }

        private void MoveStars()
        {
            foreach(var star in stars)
            {
                star.X += Speed;
            }
        }

        float ToFloat(double val) => (float)val;

        private class Star
        {
            public SKPoint GetPoint => new SKPoint(X, Y);
            public Star(float x, float y, float radius)
            {
                X = x;
                Y = y;
                Radius = radius;
            }
            public float X, Y, Radius;
            public double Distance(Star p2) => Math.Sqrt(Math.Pow(Math.Abs(X - p2.X), 2) + Math.Pow(Math.Abs(Y - p2.Y), 2));
        }
    }
}
