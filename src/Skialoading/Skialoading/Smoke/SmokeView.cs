using System;
using System.Collections.Generic;
using SkiaLoading.Time;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Smoke
{
    public class SmokeView : SKCanvasView
    {
        private SKPaint fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
        };

        private static Random rnd = new Random();
        private TimeTracker timeTracker = new TimeTracker();
        private List<SmokeParticle> particles = new List<SmokeParticle>();
        public SmokeView()
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

        public int Dx { get; set; } = 5;
        public int Dy { get; set; } = 40;
        public int ParticleAmount { get; set; } = 200;
        public int DRadius { get; set; } = 5;
        public double SpawnChance { get; set; } = 0.3;
        public SKColor Color { get; set; } = SKColors.WhiteSmoke;

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            fillPaint.Color = Color;

            var canvas = e.Surface.Canvas;
            int width = e.Info.Width;
            int height = e.Info.Height;

            canvas.Clear();
            var time = timeTracker.GetTime();
            if (particles.Count < ParticleAmount && rnd.NextDouble() < SpawnChance)
            {
                AddRndParticle(width, height);
            }

            particles.RemoveAll(p => p.Opacity <= 0);
            foreach (var particle in particles)
            {
                fillPaint.Color = Color.WithAlpha((byte)(particle.Opacity / 6));
                particle.Move(time);
                if (particle.Opacity > 0)
                    canvas.DrawCircle(particle.GetPoint, particle.Radius, fillPaint);
            }
        }

        private void AddRndParticle(int width, int height)
        {
            var dx = rnd.Next(-Dx, Dx);

            particles.Add(new SmokeParticle(width / 2 + rnd.Next(-20, 20), height, rnd.Next(1, DRadius * 5), rnd.Next(Dy - Dy / 10, Dy + Dy / 10), rnd.Next(0, DRadius), dx, dx + rnd.Next(1, Dx)));
        }

        public class SmokeParticle
        {
            public SKPoint GetPoint => new SKPoint(X, Y);
            public float X, Y, Radius, Dy, DRadius;
            public int Opacity;
            public int DxMax, DxMin;
            public SmokeParticle(float x, float y, float radius, float dy, float dRadius, int dxMin, int dxMax)
            {
                Opacity = rnd.Next(127, 126 * 12);
                X = x;
                Y = y;
                Radius = radius;
                Dy = dy;
                DRadius = dRadius;
                DxMax = dxMax;
                DxMin = dxMin;
            }

            public void Move(float dt)
            {
                Y -= Dy * dt;
                X += rnd.Next(DxMin, DxMax) * dt;
                Radius += DRadius * dt;
                Opacity -= rnd.Next(0, 5);
            }
        }
    }
}
