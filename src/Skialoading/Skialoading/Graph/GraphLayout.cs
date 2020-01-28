using System;
using System.Collections.Generic;
using System.Numerics;
using DIPS.Xamarin.UI.Controls.Slidable;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Graph
{
    public class GraphLayout : SlidableLayout
    {
        private static readonly SKPaint GraphColor = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.MediumPurple, StrokeWidth=7 };
        private static readonly SKPaint VerticalLineColor = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.LightGray, StrokeWidth = 2 };

        private static readonly SKPaint ReferenceColor = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.DarkCyan,
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Square,
            PathEffect = SKPathEffect.CreateDash(new []{10f, 20f}, 25)
        };

        private double m_lastIndex, m_offset, m_selectedIndex;
        private readonly object m_lock = new object();
        private readonly SKCanvasView m_canvas = new SKCanvasView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand,
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
                
                var points = new List<SKPoint>();

                canvas.Clear();
                var max = double.MinValue;
                var min = double.MaxValue;
                if(Reference != null)
                {
                    max = Reference.Max;
                    min = Reference.Min;
                }
                var itemWidth = width*ElementWidth;
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
                if (Reference != null)
                {
                    var x0 = (float)(-width*2 + itemWidth * (m_selectedIndex - m_lastIndex));
                    var x1 = (float)(width * 1.5);
                    var y0 = GetValue(Reference.Max, max, min, height);
                    var y1 = GetValue(Reference.Min, max, min, height);
                    canvas.DrawLine(new SKPoint(x0, y0), new SKPoint(x1, y0), ReferenceColor);
                    canvas.DrawLine(new SKPoint(x0, y1), new SKPoint(x1, y1), ReferenceColor);
                }
                SKPoint? prev = null;
                float? firstX = null;
                float lastX = 0;
                for (var i = m_selectedIndex - totalWidth; i <= m_selectedIndex + totalWidth; i++)
                {
                    var pos = (int)Math.Floor(i);
                    var hasValue = Repository.TryGetPoint(Math.Abs(pos), out var point);
                    if (pos < Config.MinValue || pos > Config.MaxValue) continue;
                    var x = (float)(width/2 + itemWidth * (i - m_lastIndex+0.5));
                    canvas.DrawLine(x, height, x, 0, VerticalLineColor);

                    if (!hasValue || point.DValue == null) continue;
                    lastX = x;
                    if(firstX == null)
                    {
                        firstX = x;
                    }

                    var y = GetValue(point.DValue.Value, max, min, height);
                    canvas.DrawCircle(x, y, 15, GraphColor);
                    var currentPos = new SKPoint(x, y);
                    if (prev != null)
                    {
                        canvas.DrawLine(currentPos, prev.Value, GraphColor);
                    }

                    prev = currentPos;
                    points.Add(currentPos);
                }
                
                if(firstX != null)
                {
                    var color = GraphColor.Color.WithAlpha((byte)20);

                    var fillPaint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = color
                    };

                    points.Add(new SKPoint(lastX, height));
                    points.Add(new SKPoint(firstX.Value, height));

                    canvas.DrawPoints(SKPointMode.Polygon, points.ToArray(), fillPaint);
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

        public static readonly BindableProperty ReferenceProperty = BindableProperty.Create(
            nameof(Reference),
            typeof(ReferenceArea),
            typeof(GraphLayout),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (s, e, n) => ((GraphLayout)s).Redraw());

        public ReferenceArea Reference
        {
            get => (ReferenceArea)GetValue(ReferenceProperty);
            set => SetValue(ReferenceProperty, value);
        }
    }
}
