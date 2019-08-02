using System;
using System.Diagnostics;
using System.Numerics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkiaTesting
{
    public class MyPage : ContentPage
    {
        private Label l;
        private int circleRadius = 50;
        private Vector2 position = new Vector2(0, 0);
        private Vector2 speed = new Vector2(20, 20);
        private Vector3 accelerometer = new Vector3(0, 0, 0);
        private Vector2 acceleration = new Vector2(1, 0.13f);
        private long lastTime;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        SKPaint backgroundFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.WhiteSmoke
        };

        SKPaint ballFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Orange,
        };

        public MyPage()
        {
            l = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            Accelerometer.Start(SensorSpeed.Game);
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            lastTime = stopwatch.ElapsedMilliseconds;
            var a = new SKCanvasView();
            a.HorizontalOptions = LayoutOptions.FillAndExpand;
            a.VerticalOptions = LayoutOptions.FillAndExpand;
            a.PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {   
                a.InvalidateSurface();
                return true;
            });
            Content = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    a,
                    l
                }
            };
        }

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            l.Text = ($"X: {data.Acceleration.X}\n Y: {data.Acceleration.Y}\n Z: {data.Acceleration.Z}");
            accelerometer = new Vector3(data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
        }

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            var time = stopwatch.ElapsedMilliseconds;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.DrawPaint(backgroundFillPaint);

            int width = e.Info.Width;
            int height = e.Info.Height;
            canvas.Translate(width / 2, height / 2);

            var dt = (time-lastTime) / 1000f;

            var realSpeed = new Vector2(speed.X * dt, speed.Y * dt);

            position = new Vector2(position.X + realSpeed.X, position.Y + realSpeed.Y);

            if(position.X + circleRadius > width / 2)
            {
                speed = new Vector2(Math.Abs(speed.X)*-1, speed.Y);
            }
            else if(position.X - circleRadius < width / -2)
            {
                speed = new Vector2(Math.Abs(speed.X), speed.Y);
            }

            if(position.Y + circleRadius > height / 2)
            {
                speed = new Vector2(speed.X, Math.Abs(speed.Y) * -1);
            }
            else if(position.Y - circleRadius < height / -2)
            {
                speed = new Vector2(speed.X, Math.Abs(speed.Y));
            }

            acceleration = new Vector2(accelerometer.X*-400, accelerometer.Y*400);

            speed = (speed + acceleration * dt) * 0.95f;
            canvas.DrawCircle(position.X, position.Y, circleRadius, ballFillPaint);
            lastTime = time;
        }
    }
}