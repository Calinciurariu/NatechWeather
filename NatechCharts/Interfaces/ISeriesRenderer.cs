using SkiaSharp;

namespace NatechCharts.Interfaces
{
    public interface ISeriesRenderer
    {
        void Render(SKCanvas canvas, ISeries series, SKRect chartArea, Func<object, (float X, float Y)> dataTransformer);
    }
}
