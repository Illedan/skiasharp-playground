using System;
using SkiaSharp;

namespace GameEngine.Framework.Components
{
    public class CircleView : IView
    {
        private readonly float radius;
        private readonly SKPaint ballFillPaint;

        public CircleView(float radius, SKColor color)
        {
            this.radius = radius;
            ballFillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color,
            };
        }
        public void Draw(SKCanvas canvas, float x, float y)
        {
            canvas.DrawCircle(x, y, radius, ballFillPaint);
        }
    }
}
