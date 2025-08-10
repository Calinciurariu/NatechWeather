using NatechCharts.Interfaces;
using NatechCharts.Models;
using NatechCharts.Renderers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace NatechCharts.Controls
{
    public abstract class BaseChart : ContentView
    {
        protected readonly SKGLView SkiaView;
        protected readonly ChartRenderer ChartRenderer;
        private (double X, double Y, string SeriesLabel, ChartDataPoint DataPoint)? _selectedPoint;
        private float _panOffsetX;
        private float _lastTouchX;
        private bool _isMagnifying;
        public SKColor lineColor;
        public BaseChart()
        {
            SkiaView = new SKGLView();
            Content = SkiaView; 
            ChartRenderer = new ChartRenderer(this);
            SkiaView.EnableTouchEvents = true; 
            SkiaView.PaintSurface += OnPaintSurface;
            SkiaView.Touch += OnTouch;
        }

        public void InvalidateSurface()
        {
            SkiaView.InvalidateSurface();
        }
        public static readonly BindableProperty SelectedPointProperty =
           BindableProperty.Create(
               nameof(SelectedPoint),
               typeof(ChartDataPoint),
               typeof(BaseChart),
               defaultValue: null,
               BindingMode.TwoWay,
               propertyChanged: OnSelectedPointChanged);

        public ChartDataPoint SelectedPoint
        {
            get => (ChartDataPoint)GetValue(SelectedPointProperty);
            set => SetValue(SelectedPointProperty, value);
        }
        private static void OnSelectedPointChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not BaseChart chart) return;
            Console.WriteLine($"OnSelectedPointChanged: newValue={(newValue != null ? $"Date={((ChartDataPoint)newValue).Date}, Val={((ChartDataPoint)newValue).Val}" : "null")}");
            chart.InvalidateSurface();
        }

        public static readonly BindableProperty SeriesProperty =
              BindableProperty.Create(
                  nameof(Series),
                  typeof(IList<ISeries>),
                  typeof(BaseChart),
                  defaultValue: null, 
                  propertyChanged: OnSeriesChanged);

        public IList<ISeries> Series
        {
            get => (IList<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        private static void OnSeriesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not BaseChart chart) return;

            chart.UpdateSeriesParent(newValue as IEnumerable<ISeries>);
            if (newValue is IEnumerable<ISeries> series && series.Any(s => s.ItemsSource?.Cast<object>().Any() == true))
            {
                Console.WriteLine("OnSeriesChanged: Invalidating surface");
                chart.InvalidateSurface();
            }
            else
            {
                Console.WriteLine("OnSeriesChanged: Skipping InvalidateSurface (no data)");
            }
        }
        private void UpdateSeriesParent(IEnumerable<ISeries> newItems)
        {
            if (newItems == null) return;
            foreach (var series in newItems)
            {
                series.SetParent(this);
            }
        }
        private void OnTouch(object? sender, SKTouchEventArgs e)
        {
            var touchPoint = e.Location;
            Console.WriteLine($"OnTouch: Action={e.ActionType}, X={touchPoint.X}, Y={touchPoint.Y}");

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    _lastTouchX = touchPoint.X;
                    _isMagnifying = true;
                    _selectedPoint = FindNearestPoint(touchPoint.X, touchPoint.Y);
                    if (_selectedPoint.HasValue)
                    {
                        SelectedPoint = _selectedPoint.Value.DataPoint;
                    }
                    InvalidateSurface();
                    e.Handled = true;
                    break;

                case SKTouchAction.Moved:
                    var deltaX = touchPoint.X - _lastTouchX;
                    _panOffsetX -= deltaX;
                    _lastTouchX = touchPoint.X;
                    _isMagnifying = true;
                    _selectedPoint = FindNearestPoint(touchPoint.X, touchPoint.Y);
                    if (_selectedPoint.HasValue)
                    {
                        SelectedPoint = _selectedPoint.Value.DataPoint;
                    }
                    InvalidateSurface();
                    e.Handled = true;
                    break;

                case SKTouchAction.Released:
                    _isMagnifying = false;
                    _selectedPoint = null;
                    SelectedPoint = null;
                    InvalidateSurface();
                    e.Handled = true;
                    break;
            }
        }
        private (double X, double Y, string SeriesLabel, ChartDataPoint DataPoint)? FindNearestPoint(float touchX, float touchY)
        {
            if (Series == null || !Series.Any()) return null;

            int margin = 50;
            double plotWidth = SkiaView.Width - 2 * margin;
            double plotHeight = SkiaView.Height - 2 * margin;

            var allPoints = new List<(double X, double Y, string SeriesLabel, ChartDataPoint DataPoint)>();
            double xMin = double.MaxValue, xMax = double.MinValue, yMin = double.MaxValue, yMax = double.MinValue;
            foreach (var series in Series.OfType<LineSeries>())
            {
                var points = ChartRenderer.ExtractDataPoints(series);
                var items = series.ItemsSource?.Cast<ChartDataPoint>().ToList() ?? new List<ChartDataPoint>();
                for (int i = 0; i < points.Count && i < items.Count; i++)
                {
                    allPoints.Add((points[i].X, points[i].Y, series.Label, items[i]));
                    xMin = Math.Min(xMin, points[i].X);
                    xMax = Math.Max(xMax, points[i].X);
                    yMin = Math.Min(yMin, points[i].Y);
                    yMax = Math.Max(yMax, points[i].Y);
                }
            }

            if (!allPoints.Any()) return null;

            double normalizedTouchX = (touchX - margin) / plotWidth * (xMax - xMin) + xMin;
            double normalizedTouchY = yMax - (touchY - margin) / plotHeight * (yMax - yMin);

            (double X, double Y, string SeriesLabel, ChartDataPoint DataPoint)? nearest = null;
            double minDistance = double.MaxValue;
            foreach (var point in allPoints)
            {
                double distance = Math.Sqrt(Math.Pow(point.X - normalizedTouchX, 2) + Math.Pow(point.Y - normalizedTouchY, 2));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = point;
                }
            }

            if (minDistance < 1000)
            {
                return nearest;
            }
            return null;
        }

        private void OnPaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs e)
        {
            Console.WriteLine("OnPaintSurface: Rendering chart");
            ChartRenderer.Render(e.Surface, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height, _selectedPoint, _panOffsetX, _isMagnifying);
        }
    }
}