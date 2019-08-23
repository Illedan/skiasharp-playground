using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using SkiaLoading.Time;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Fireworks
{
    public class FireworksView : SKCanvasView
    {
        private static readonly SKPaint[] colors =
        {
            new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.Red },
            new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.Orange },
            new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.White },
            new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.Yellow },
            new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.Green },
        };

        private static readonly SKPaint rocketColor = new SKPaint { Style = SKPaintStyle.Stroke, Color = SKColors.WhiteSmoke, StrokeWidth = 5 };

        private static Random rnd = new Random();
        private TimeTracker timeTracker = new TimeTracker();
        private readonly List<Rocket> rockets = new List<Rocket>();
        private readonly List<Confetti> confettis = new List<Confetti>();

        public FireworksView()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1.0 / 60.0), () =>
            {
                InvalidateSurface();
                return true;
            });

            FireRocketCommand = new Command(FireRocket);
        }


        public int MaxConfetti { get; set; }
        public int MinConfetti { get; set; }

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            float width = e.Info.Width;
            float height = e.Info.Height;
            var dt = timeTracker.GetTime();
            float GetX(float x) => width / 2 + x;
            float GetY(float y) => height - y;
            canvas.Clear();
            foreach (var rocket in rockets)
            {
                rocket.Move(dt);
                if (rocket.ShouldExplode())
                {
                    confettis.AddRange(rocket.GetConfetti(rnd.Next(MinConfetti, MaxConfetti)));
                }
            }

            rockets.RemoveAll(r => r.ShouldExplode());

            foreach (var confetti in confettis)
            {
                confetti.Move(dt);
            }

            confettis.RemoveAll(c => c.Opacity <= 0);

            foreach (var rocket in rockets)
            {
                var dSpeed = (float)Math.Sqrt(Math.Pow(rocket.Dx, 2) + Math.Pow(rocket.Dy, 2));
                canvas.DrawLine(GetX(rocket.X), GetY(rocket.Y), GetX(rocket.X + rocket.Dx / dSpeed * 10f), GetY(rocket.Y + rocket.Dy / dSpeed * 10f), rocketColor);
            }

            foreach (var confetti in confettis)
            {
                var paint = confetti.Color;
                paint.Color = paint.Color.WithAlpha((byte)confetti.Opacity);
                canvas.DrawCircle(GetX(confetti.X), GetY(confetti.Y), 3, paint);
            }

        }

        public ICommand FireRocketCommand { get; }

        private void FireRocket()
        {
            var x = rnd.Next(-100, 100);
            var y = 0;
            var targetX = rnd.Next(-300, 300);
            var targetY = rnd.Next(800, 1000);
            var time = (float)rnd.Next(2, 6);
            var length = (float)Math.Sqrt(Math.Pow(x - targetX, 2) + Math.Pow(y - targetY, 2));
            rockets.Add(new Rocket(x, y, length, (targetX - x) / time, (targetY - y) / time));
        }

        private class Rocket
        {
            public float X, Y, Dx, Dy;
            private float x, y, explodingDistance;
            public Rocket(float x, float y, float dist, float dx, float dy)
            {
                X = this.x = x;
                Y = this.y = y;
                explodingDistance = dist * dist;
                Dx = dx;
                Dy = dy;
            }

            public void Move(float dt)
            {
                X += Dx * dt;
                Y += Dy * dt;
            }

            public bool ShouldExplode() => Pow(Math.Abs(X - x)) + Pow(Math.Abs(Y - y)) > explodingDistance;

            public IEnumerable<Confetti> GetConfetti(int amount)
            {
                var color = colors[rnd.Next(colors.Length)];
                for (var i = 0; i < amount; i++)
                {
                    yield return new Confetti(X, Y, rnd.Next(-20, 20) / 20f * 80f, rnd.Next(-20, 20) / 20f * 80f, rnd.Next(5, 20), color);
                }
            }

            private static float Pow(float k) => k * k;
        }

        private class Confetti
        {
            private const float Gravity = -30f;
            public float X, Y, Dx, Dy;
            public int Opacity, Radius;
            public SKPaint Color;
            public Confetti(float x, float y, float dx, float dy, int radius, SKPaint color)
            {
                Radius = radius;
                X = x;
                Y = y;
                Dx = dx;
                Dy = dy;
                Opacity = rnd.Next(200, 256);
                //Color = colors[rnd.Next(colors.Length)];
                Color = color;
            }

            public void Move(float dt)
            {
                X += Dx * dt;
                Y += Dy * dt + Gravity * dt;
                Opacity -= rnd.Next(1, 3);
            }
        }
    }
}

