using System;
using System.Collections.Generic;
using SkiaLoading.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Sprite
{
    public class SpriteAnimationView : SKCanvasView
    {
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

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var width = e.Info.Width;
            var height = e.Info.Height;
            canvas.Clear();
            canvas.Save();
            canvas.Translate(new SKPoint(width / 2.0f, height / 2.0f));

            var bitmap = BitmapExtensions.LoadBitmapResource("Bird1.png");
            canvas.DrawBitmap(bitmap, new SKPoint(0, 0));

            canvas.Restore();
        }
    }
}
