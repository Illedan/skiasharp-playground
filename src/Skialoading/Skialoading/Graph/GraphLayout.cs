using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DIPS.Xamarin.UI.Controls.Slidable;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaLoading.Graph
{
    public class GraphLayout : SlidableLayout
    {

        private readonly static SKPaint FramePaint = new SKPaint
        {
            Color = SKColors.LightGray,
            Style = SKPaintStyle.Stroke,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        private readonly static SKPaint PinFillPaint = new SKPaint
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.StrokeAndFill,
            IsAntialias = true
        };

        private readonly static SKPaint SelectedPaint = new SKPaint
        {
            Color = new SKColor(31, 75, 81),
            Style = SKPaintStyle.StrokeAndFill,
            IsAntialias = true
        };

        SKPaint TextPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextAlign = SKTextAlign.Left,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.ExtraBold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright)
        };
        private static float PinScale = 3.0f;
        private static float TriangleWidth => 4f * PinScale;
        private static float ValueLabelPadding => 5f * PinScale;
        private static float ValueLabelFontSize => 14f * PinScale;
        private static float ToolTipCornerRadius => 4f * PinScale;
        private static float BorderThickness => 1f * PinScale;

        private static readonly SKPaint GraphColor = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.MediumPurple, StrokeWidth = 7 };
        private static readonly SKPaint VerticalLineColor = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.LightGray, StrokeWidth = 2 };
        private static readonly SKPaint FillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = GraphColor.Color.WithAlpha(30),
            StrokeCap = SKStrokeCap.Butt,
            StrokeWidth = 5
        };

        private static readonly SKPaint ReferenceColor = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.DarkCyan,
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Square,
            PathEffect = SKPathEffect.CreateDash(new[] { 10f, 20f }, 25)
        };

        private double m_index;
        private readonly object m_lock = new object();
        private readonly SKCanvasView m_tooltipCanvas = new SKCanvasView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand,
        };

        private readonly SKCanvasView m_canvas = new SKCanvasView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand,
        };
        private Grid m_grid;
        public GraphLayout()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            m_canvas.PaintSurface += Redraw;
            m_grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            m_grid.Children.Add(m_canvas);
            m_grid.Children.Add(m_tooltipCanvas);
            Content = m_grid;
        }

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            if (Width < 1 || Repository == null) return;
            var height = e.Info.Height;
            var width = e.Info.Width;
            var center = width/2;
            var itemWidth = width / 5;
            var selectedIndex = GetIndexFromValue(m_index);
            var itemCount = (center * 2) / itemWidth;

            lock (m_lock)
            {
                var posses = new List<Position>();
                canvas.Clear();
                var max = double.MinValue;
                var min = double.MaxValue;
                if (Reference != null)
                {
                    max = Reference.Max;
                    min = Reference.Min;
                }
                var first = GetIndexFromValue(m_index - itemCount);
                var last = GetIndexFromValue(m_index + itemCount);

                for (var i = first+1; i <= last-1; i++)
                {
                    var iIndex = GetIndexFromValue(i);
                    var hasValue = Repository.TryGetPoint(Math.Abs(iIndex), out var point);
                    if (iIndex < Config.MinValue || iIndex > Config.MaxValue) continue;
                    if (!hasValue || point.DValue == null) continue;

                    max = Math.Max(max, point.DValue.Value);
                    min = Math.Min(min, point.DValue.Value);
                }

                var c = (max - min) / 2.0 + min;

                if (Repository.TryGetPoint(Math.Abs(first), out var firstValue) && first >= Config.MinValue && first <= Config.MaxValue)
                {
                    var value = firstValue.DValue.Value;
                    var dist = Math.Abs(first + 0.5 - (m_index - itemCount));
                    if (value > max)
                    {
                        max = Math.Max(max, (value - c) * dist + c);
                    }
                    else if (value < min)
                    {
                        min = Math.Min(min, (value - c) * dist + c);
                    }
                }

                if (Repository.TryGetPoint(Math.Abs(last), out var lastValue) && last >= Config.MinValue && last <= Config.MaxValue)
                {
                    var value = lastValue.DValue.Value;
                    var dist = Math.Abs(last - 0.5 - (m_index + itemCount));
                    if (value > max)
                    {
                        max = Math.Max(max, (value - c) * dist + c);
                    }
                    else if (value < min)
                    {
                        min = Math.Min(min, (value - c) * dist + c);
                    }
                }

                var diff = max - min;
                max += diff * 0.2;
                min -= diff * 0.2;
                if (Reference != null)
                {
                    var x0 = (float)(-width * 2 + itemWidth * (selectedIndex - m_index));
                    var x1 = (float)(width * 1.5);
                    var y0 = GetValue(Reference.Max, max, min, height);
                    var y1 = GetValue(Reference.Min, max, min, height);
                    canvas.DrawLine(new SKPoint(x0, y0), new SKPoint(x1, y0), ReferenceColor);
                    canvas.DrawLine(new SKPoint(x0, y1), new SKPoint(x1, y1), ReferenceColor);
                }

                SKPoint? prev = null;
                float? firstX = null;
                float lastX = 0;

                using (var path = new SKPath())
                {
                    for (var i = m_index - itemCount; i <= m_index + itemCount; i++)
                    {
                        var iIndex = GetIndexFromValue(i);
                        if (iIndex < Config.MinValue || iIndex > Config.MaxValue) continue;
                        var hasValue = Repository.TryGetPoint(Math.Abs(iIndex), out var point);
                        if (!hasValue) continue;
                        var x = (float)(width/2 + itemWidth * (GetIndexFromValue(i) - m_index));
                        canvas.DrawLine(x, (float)height, x, 0, VerticalLineColor);

                        if (!hasValue || point.DValue == null) continue;
                        lastX = x;
                        var y = GetValue(point.DValue.Value, max, min, height);
                        var currentPos = new SKPoint(x, y);
                        if (firstX == null)
                        {
                            firstX = x;
                            path.MoveTo(currentPos);
                        }

                        canvas.DrawCircle(x, y, 15, GraphColor);
                        if (prev != null)
                        {
                            canvas.DrawLine(currentPos, prev.Value, GraphColor);
                        }

                        posses.Add(new Position(currentPos.X, currentPos.Y) { Index = iIndex });
                        prev = currentPos;

                        path.LineTo(currentPos);
                    }

                    if (firstX != null)
                    {
                        path.LineTo(new SKPoint(lastX, (float)height));
                        path.LineTo(new SKPoint(firstX.Value, (float)height));
                        path.Close();
                        canvas.DrawPath(path, FillPaint);
                    }
                }

                //Pin drawing
                var closest = posses.OrderBy(p => Math.Abs(center-p.X)).FirstOrDefault();
                if (closest != null && closest.Index >= Config.MinValue && closest.Index <= Config.MaxValue)
                {
                    var nextVal = closest;
                    var dir = closest.Index < m_index ? 1 : -1;
                    if(dir == 1)
                    {
                        nextVal = posses.Where(p => p.X > closest.X).OrderBy(p => Math.Abs(p.X - closest.X)).FirstOrDefault();
                    }
                    else
                    {
                        nextVal = posses.Where(p => p.X < closest.X).OrderBy(p => Math.Abs(p.X - closest.X)).FirstOrDefault();
                    }

                    Position drawPos;

                    if (!Repository.TryGetPoint(Math.Abs(closest.Index), out var value))
                    {
                        return;
                    }

                    if (nextVal == null)
                    {
                        drawPos = new Position(closest.X, closest.Y);
                    }
                    else
                    {
                        var dx = nextVal.X - closest.X;
                        var dy = nextVal.Y - closest.Y;
                        var dist = Math.Abs(center - closest.X) / Math.Abs(nextVal.X - closest.X);

                        drawPos = new Position(center, closest.Y + dy * dist);
                    }
                    
                    var txt = value.DValue.Value + "";
                    TextPaint.TextSize = ValueLabelFontSize;
                    FramePaint.StrokeWidth = BorderThickness;
                    canvas.DrawCircle(drawPos.X, drawPos.Y, 20, SelectedPaint);
                    // Move the tip little away from the dots
                    var tipX = drawPos.X + 8.0f;
                    var tipY = drawPos.Y;

                    SKRect frameRect = new SKRect();
                    TextPaint.MeasureText(txt, ref frameRect);
                    var textHeight = frameRect.Top;
                    // Inflate for padding
                    frameRect.Inflate(ValueLabelPadding, ValueLabelPadding);

                    SKPoint GetPoint(float x, float y) => new SKPoint(tipX + x + TriangleWidth, tipY + y);

                    using (var path = new SKPath())
                    {
                        var corners = new SKPoint[] {
                            GetPoint(0, -frameRect.Height  / 2),
                            GetPoint(frameRect.Width*1.5f, -frameRect.Height*1 / 2),
                            GetPoint(frameRect.Width*1.5f, frameRect.Height*1 / 2),
                            GetPoint(0, frameRect.Height*1  / 2)
                        };

                        path.MoveTo(GetPoint(-TriangleWidth, 0));
                        path.LineTo(GetPoint(0, -TriangleWidth));
                        path.ArcTo(corners[0], corners[1], ToolTipCornerRadius);
                        path.ArcTo(corners[1], corners[2], ToolTipCornerRadius);
                        path.ArcTo(corners[2], corners[3], ToolTipCornerRadius);
                        path.ArcTo(corners[3], corners[0], ToolTipCornerRadius);
                        path.LineTo(GetPoint(0, TriangleWidth));
                        path.Close();
                        canvas.DrawPath(path, PinFillPaint);
                        canvas.DrawPath(path, FramePaint);
                    }

                    var xText = tipX + TriangleWidth + ValueLabelPadding;
                    var yText = tipY - frameRect.Height / 2 + ValueLabelPadding - textHeight;

                    canvas.DrawText(txt, xText, yText, TextPaint);
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

        protected override void OnScrolled(double index)
        {
            lock (m_lock)
            {
                m_index = index;
            }

            if (Width < 0.1)
            {
                return;
            }

            base.OnScrolled(index);
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
