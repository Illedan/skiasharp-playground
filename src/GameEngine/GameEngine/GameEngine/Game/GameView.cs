using System;
using GameEngine.Game;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace GameEngine
{
    public class GameView : Grid
    {
        private MovingWallsGame game;
        public GameView()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;

            var canvas = new SKCanvasView();
            canvas.HorizontalOptions = LayoutOptions.FillAndExpand;
            canvas.VerticalOptions = LayoutOptions.FillAndExpand;
            canvas.PaintSurface += Redraw;
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                canvas.InvalidateSurface();
                return true;
            });
            Children.Add(canvas);
        }

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            if (game == null) game = new MovingWallsGame(canvas, e.Info.Width, e.Info.Height);
            canvas.Clear();
            game.Update();
        }
    }
}