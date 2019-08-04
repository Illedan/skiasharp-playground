using System;
using System.Collections.Generic;
using SkiaSharp;

namespace GameEngine.Framework.Components
{
    public class RandomMountainCreationComponent : IComponent
    {
        private List<Position> points = new List<Position>();
        private readonly IEntity entity;
        private readonly float speed;
        private readonly float max;
        private readonly float min;
        private readonly float height;
        private readonly float width;
        private readonly SKCanvas canvas;
        private float lastX;
        private const float minDist = 10, maxDist = 100;
        private readonly Random rnd = new Random();
        SKPaint fillPaint;

        public RandomMountainCreationComponent(float speed, float max, float min, float height, float width, SKColor color, SKCanvas canvas)
        {
            fillPaint  = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color
            };
            this.speed = speed;
            this.max = max;
            this.min = min;
            this.height = height;
            this.width = width;
            this.canvas = canvas;
            lastX = maxDist * -1;
            var lastHeight = (float)(rnd.NextDouble() * (max - min) + min);
            while (lastX < width)
            {
                float next = (float)(rnd.NextDouble() * (maxDist - minDist) + minDist);
                var nextHeight = (float)(rnd.NextDouble() * (max - min) / 2 + rnd.NextDouble() * (max - min) / 2 + min);
                points.Add(new Position(lastX+next, nextHeight, 0));
                lastX += next;
            }
        }

        public void Update(float dt)
        {
            foreach(var p in points)
            {
                p.X -= dt * speed;
            }

            if(points[1].X < 0)
            {
                points.RemoveAt(0);
            }

            while(points[points.Count-1].X < width)
            {
                float next = (float)(rnd.NextDouble() * (maxDist - minDist) + minDist);
                var nextHeight = (float)(rnd.NextDouble() * (max - min) / 2 + rnd.NextDouble() * (max - min) / 2 + min);
                points.Add(new Position(points[points.Count - 1].X+next, nextHeight, 0));
                lastX += next;
            }

            SKPath path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };

            path.MoveTo(new SKPoint(points[0].X, height));
            foreach(var p in points)
            {
                path.LineTo(new SKPoint(p.X, p.Y));
            }
            path.LineTo(new SKPoint(points[points.Count - 1].X, height));
            path.Close();
            canvas.DrawPath(path, fillPaint);
        }
    }
}
