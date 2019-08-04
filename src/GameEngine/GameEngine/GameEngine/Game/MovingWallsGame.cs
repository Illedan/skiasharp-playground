using System;
using SkiaSharp;
using GameEngine.Framework;
using System.Collections.Generic;
using GameEngine.Framework.Components;

namespace GameEngine.Game
{
    public class MovingWallsGame
    {
        private readonly float Width, Height;
        private readonly Engine gameEngine;
        public MovingWallsGame(SKCanvas canvas, float width, float height)
        {
            Width = width;
            Height = height;
            gameEngine = new Engine();


            //gameEngine.MainContainer.Entities.Add(GameFactory.CreateBall(canvas, width, height, gameEngine.MainContainer));
            gameEngine.MainContainer.Entities.Add(GameFactory.CreateMountain(canvas, gameEngine.MainContainer, width, height, -10, new SKColor(100,96,107), 0, 10));
            gameEngine.MainContainer.Entities.Add(GameFactory.CreateMountain(canvas, gameEngine.MainContainer, width, height, -9, new SKColor(127, 135, 126), 300, 30));
            gameEngine.MainContainer.Entities.Add(GameFactory.CreateMountain(canvas, gameEngine.MainContainer, width, height, -8, new SKColor(63, 66, 97), 450, 60));
        }

        public void Update()
        {
            gameEngine.Update();
        }
    }
}
