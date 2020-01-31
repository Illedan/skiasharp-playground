using System;
namespace SkiaLoading.Graph
{
    public class ReferenceArea
    {
        public ReferenceArea(float min, float max)
        {
            Max = max;
            Min = min;
        }
        public float Max { get; }
        public float Min { get; }
    }
}
