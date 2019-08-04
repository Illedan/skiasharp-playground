using System;
using System.Collections.Generic;
using GameEngine.Framework;
using GameEngine.Framework.Components;
using SkiaSharp;

namespace GameEngine.Game
{
    public static class GameFactory
    {
        public static IEntity CreateMountain(SKCanvas canvas, IEntity parent, float width, float height, float z, SKColor color, float baseHeight, float speed)
        {
            return new BaseEntity(new Position(0,0,z), new List<IComponent>
            {
                new RandomMountainCreationComponent(speed, baseHeight+300, baseHeight+100, height, width, color, canvas)
            }, parent);
        }

        public static IEntity CreateBall(SKCanvas canvas, float width, float height, IEntity parent, float z)
        {
            var ball = new BaseEntity(new Position(200, 200, z), new List<IComponent>(), parent);
            var physics = new PhysicsComponent(2, 1, 50, ball);
            ball.Components.Add(physics);
            ball.Components.Add(new WallCollisionComponent(ball, physics, width, height));
            ball.Components.Add(new GraphicsComponents(ball, canvas, new CircleView(physics.Radius, SKColors.Yellow)));
            return ball;
        }
    }
}
