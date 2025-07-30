using NatechCharts.Interfaces;
using SkiaSharp;

namespace NatechCharts.Renderers
{
    public class LineSeriesRenderer : ISeriesRenderer
    {
        public void Render(SKCanvas canvas, ISeries series, SKRect chartArea, Func<object, (float X, float Y)> dataTransformer)
        {
            using var path = new SKPath();
            bool isFirstPoint = true;

            foreach (var dataPoint in series.ItemsSource)
            {
                var (x, y) = dataTransformer(dataPoint);

                if (isFirstPoint)
                {
                    path.MoveTo(x, y);
                    isFirstPoint = false;
                }
                else
                {
                    path.LineTo(x, y);
                }
            }

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = series.Color,
                StrokeWidth = 3,
                IsAntialias = true
            };
            canvas.DrawPath(path, paint);
        }
    }
}