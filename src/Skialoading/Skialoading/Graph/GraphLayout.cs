using System;
using System.Numerics;
using DIPS.Xamarin.UI.Controls.Slidable;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Graph
{
    public class GraphLayout : SlidableLayout
    {
        private static readonly SKPaint GraphColor = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.DarkCyan, StrokeWidth=5 };
        private double m_lastIndex, m_offset, m_selectedIndex;
        private readonly object m_lock = new object();
        private readonly SKCanvasView m_canvas = new SKCanvasView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        public GraphLayout()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            m_canvas.PaintSurface += Redraw;
            Content = m_canvas;
        }

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            float width = e.Info.Width;
            float height = e.Info.Height;
            if (width < 1 || Repository == null) return;
            lock (m_lock)
            {
                canvas.Clear();
                var max = double.MinValue;
                var min = double.MaxValue;
                var itemWidth = GetItemWidth();
                var totalWidth = (float)(width / itemWidth);
                for (var i = m_selectedIndex - totalWidth ; i <= m_selectedIndex + totalWidth; i++)
                {
                    var pos = (int)Math.Floor(i);
                    var hasValue = Repository.TryGetPoint(Math.Abs(pos), out var point);
                    if (pos < Config.MinValue || pos > Config.MaxValue) continue;
                    if (!hasValue || point.DValue == null) continue;

                    if (point.DValue > max) max = point.DValue.Value;
                    if (point.DValue < min) min = point.DValue.Value;
                }
                var diff = max - min;
                max += diff * 0.2;
                min -= diff * 0.2;
                SKPoint? prev = null;
                for (var i = m_selectedIndex - totalWidth; i <= m_selectedIndex + totalWidth; i++)
                {
                    var pos = (int)Math.Floor(i);
                    var hasValue = Repository.TryGetPoint(Math.Abs(pos), out var point);
                    if (pos < Config.MinValue || pos > Config.MaxValue) continue;
                    if (!hasValue || point.DValue == null) continue;

                    var x = (float)(width/2 + itemWidth * (i - m_lastIndex));
                    var y = GetValue(point.DValue.Value, max, min, height);
                    canvas.DrawCircle(x, y, 10, GraphColor);
                    var currentPos = new SKPoint(x, y);
                    if (prev != null)
                    {
                        canvas.DrawLine(currentPos, prev.Value, GraphColor);
                    }

                    prev = currentPos;
                }
            }
        }

        private float GetValue(double value, double max, double min, double drawableLength)
        {
            var totalLength = max - min;
            var valueLength = value - min;

            var diff = valueLength / (double)totalLength;

            return (float)(drawableLength - drawableLength * diff);
        }

        private void Redraw()
        {
            m_canvas.InvalidateSurface();
        }

        protected override void OnScrolled(double index, double offset, int selectedIndex)
        {
            lock (m_lock)
            {
                m_lastIndex = index;
                m_offset = offset;
                m_selectedIndex = selectedIndex;
            }

            if(Width < 0.1)
            {
                return;
            }

            base.OnScrolled(index, offset, selectedIndex);
            Redraw();
        }

        public static readonly BindableProperty RepositoryProperty = BindableProperty.Create(
            nameof(Repository),
            typeof(IDataRepository),
            typeof(GraphLayout),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (s, e, n) => ((GraphLayout)s).Redraw());

        public IDataRepository Repository
        {
            get => (IDataRepository)GetValue(RepositoryProperty);
            set => SetValue(RepositoryProperty, value);
        }
    }
}
