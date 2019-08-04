using System;
using SkiaSharp;

namespace GameEngine.Framework.Components
{
    public interface IView
    {
        void Draw(SKCanvas canvas, float x, float y);
    }
}
