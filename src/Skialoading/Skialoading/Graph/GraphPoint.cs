using System;
namespace SkiaLoading.Graph
{
    public struct GraphPoint
    {
        public GraphPoint(int id, string sValue, double? dValue = null)
        {
            Id = id;
            SValue = sValue;
            DValue = dValue;
        }

        public int Id { get; }
        public string SValue { get; }
        public double? DValue { get; }
    }
}
