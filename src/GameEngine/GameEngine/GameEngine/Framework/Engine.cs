using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace GameEngine.Framework
{
    public class Engine
    {
        private float SECOND = 1000.0f;
        private long lastTime;
        private Stopwatch stopwatch = Stopwatch.StartNew();
        public Engine()
        {
            MainContainer = new EntityContainer(null);
        }

        public EntityContainer MainContainer { get; }

        public void Update()
        {
            var currentTime = stopwatch.ElapsedMilliseconds;
            if (lastTime == 0) lastTime = currentTime;
            var dt = (currentTime-lastTime) / SECOND;
            MainContainer.Update(dt);
            lastTime = currentTime;
        }
    }
}