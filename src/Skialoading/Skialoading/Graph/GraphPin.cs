using System;
using SkiaSharp.Views.Forms;

namespace SkiaLoading.Graph
{
    public class GraphPin 
    {
        private readonly SKCanvasView m_canvas;

        public GraphPin(SKCanvasView canvas)
        {
            this.m_canvas = canvas;
            m_canvas.PaintSurface += Redraw;
        }

        private void Redraw(object sender, SKPaintSurfaceEventArgs e)
        {

        }

        public void Update()
        {
            m_canvas.InvalidateSurface();
        }
    }
}
