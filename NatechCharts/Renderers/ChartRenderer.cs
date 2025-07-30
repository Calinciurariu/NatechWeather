using NatechCharts.Controls;
using SkiaSharp;

namespace NatechCharts.Renderers
{
    public class ChartRenderer
    {
        private readonly BaseChart _chart;

        private readonly SKPaint _gridPaint;
        private readonly SKPaint _axisLabelPaint;
        private readonly SKFont _font;

        public ChartRenderer(BaseChart chart)
        {
            _chart = chart;

            _gridPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.LightGray,
                StrokeWidth = 1
            };

            _axisLabelPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Black,
                
            };
            _font = new SKFont(SKTypeface.Default, 24);
            
        }

        public void Render(SKSurface surface, int width, int height)
        {
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            if (_chart.ItemsSource == null || !_chart.Series.Any())
            {
                return;
            }

            var chartArea = CalculateLayout(width, height);
            DrawGridAndAxes(canvas, chartArea);
            DrawDataSeries(canvas, chartArea);
            DrawInteractiveElements(canvas, chartArea);
        }

        private SKRect CalculateLayout(int viewWidth, int viewHeight)
        {
            return new SKRect(50, 20, viewWidth - 20, viewHeight - 40);
        }

        private void DrawGridAndAxes(SKCanvas canvas, SKRect chartArea)
        {
        }

        private void DrawDataSeries(SKCanvas canvas, SKRect chartArea)
        {
        }

        private void DrawInteractiveElements(SKCanvas canvas, SKRect chartArea)
        {
        }
    }
}