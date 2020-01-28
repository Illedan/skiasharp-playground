using System;
using System.Collections.Generic;

namespace SkiaLoading.Graph
{
    public class DataRepository : IDataRepository
    {
        private readonly Dictionary<int, GraphPoint> m_points = new Dictionary<int, GraphPoint>();
        private readonly object m_lock = new object();

        public void AddMany(IEnumerable<GraphPoint> graphPoints)
        {
            lock(m_lock)
            {
                foreach(var point in graphPoints)
                {
                    m_points.Add(point.Id, point);
                }

                RaiseChanged();
            }
        }

        public void AddPoint(GraphPoint graphPoint)
        {
            lock (m_lock)
            {
                m_points.Add(graphPoint.Id, graphPoint);
                RaiseChanged();
            }
        }

        public void Reset()
        {
            lock (m_lock)
            {
                m_points.Clear();
                RaiseChanged();
            }
        }

        private void RaiseChanged()
        {
            OnDataChanged?.Invoke(this, EventArgs.Empty);
        }
         
        public event EventHandler OnDataChanged;

        public bool TryGetPoint(int id, out GraphPoint point)
        {
            lock (m_lock)
            {
                return m_points.TryGetValue(id, out point);
            }
        }
    }

    public interface IDataRepository
    {
        bool TryGetPoint(int id, out GraphPoint point);
        event EventHandler OnDataChanged;
    }
}
