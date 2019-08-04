using System;
using SkiaSharp;

namespace GameEngine.Framework.Components
{
    public class GraphicsComponents : IComponent
    {
        private readonly IEntity entity;
        private readonly SKCanvas canvas;
        private readonly IView view;

        public GraphicsComponents(IEntity entity, SKCanvas canvas, IView view)
        {
            this.entity = entity;
            this.canvas = canvas;
            this.view = view;
        }

        public void Update(float dt)
        {
            var x = entity.GetRelativeX();
            var y = entity.GetRelativeY();
            view.Draw(canvas, x, y);
        }
    }
}
