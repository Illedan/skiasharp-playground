using System;
using System.Diagnostics;

namespace SkiaLoading.Time
{
    public class TimeTracker
    {
        private Stopwatch stopwatch = Stopwatch.StartNew();
        private long lastTime;
        private bool hasLastTime;
        public float GetTime()
        {
            var time = stopwatch.ElapsedMilliseconds;
            if (!hasLastTime)
            {
                lastTime = time;
                hasLastTime = true;
            }

            var dt = (time - lastTime) / 1000.0f;
            lastTime = time;
            return dt;
        }
    }
}
