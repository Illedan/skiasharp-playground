using System;
using System.Collections.Generic;
using System.Linq;
using SkiaLoading.Extensions;
using SkiaLoading.Time;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Sprite
{
    public class SpriteAnimationView : SKCanvasView
    {
        private int index;
        private List<SKBitmap> bitmaps = new List<SKBitmap>();
        private double sumTime;
        private TimeTracker timeTracker = new TimeTracker();
        public SpriteAnimationView()
        {
            PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                InvalidateSurface();
                return true;
            });
        }

        public List<string> Sprites { get; set; } = new List<string>();
        public double SwapTime { get; set; } = 0.5;

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            if(bitmaps.Count != Sprites.Count)
            {
                bitmaps.Clear();
                bitmaps.AddRange(Sprites.Select(s => BitmapExtensions.LoadBitmapResource(s)));
            }

            var time = timeTracker.GetTime();
            sumTime += time;
            if(sumTime > SwapTime)
            {
                index++;
                if (index >= bitmaps.Count) index = 0;
                sumTime -= SwapTime;
            }

            var canvas = e.Surface.Canvas;
            var width = e.Info.Width;
            var height = e.Info.Height;
            canvas.Clear();
            canvas.Save();
            canvas.Translate(new SKPoint(width / 2.0f, height / 2.0f));

            canvas.DrawBitmap(bitmaps[index], new SKPoint(0, 0));

            canvas.Restore();
        }
    }
}
