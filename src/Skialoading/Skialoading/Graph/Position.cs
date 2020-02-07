using System;
namespace SkiaLoading.Graph
{
    public class Position
    {
        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }

        public int Index { get; set; }
        public float X { get; }
        public float Y { get; }

        private float Pow(float x) => x * x;

        public float Distance2(Position p2) => Pow(p2.X - X) + Pow(p2.Y - Y);
    }
}
