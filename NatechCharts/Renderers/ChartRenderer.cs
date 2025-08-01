using NatechCharts.Controls;
using NatechCharts.Models;
using SkiaSharp;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Maui.Devices;

namespace NatechCharts.Renderers
{
    public class ChartRenderer
    {
        private readonly BaseChart _chart;
        private readonly Dictionary<string, PropertyInfo> _propertyCache = new Dictionary<string, PropertyInfo>();
        private readonly SKFont _font = new SKFont
        {
            Size = 20,
            Typeface = SKTypeface.FromFamilyName("Helvetica", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        private readonly SKPaint _axisLabelPaint;
        private readonly SKPaint _gridPaint;
        private readonly SKPaint _linePaint;
        private readonly SKPaint _markerPaint;
        private readonly SKPaint _tooltipPaint;
        private readonly SKPaint _tooltipTextPaint;
        private readonly SKPaint _magnifierPaint;
        private readonly Dictionary<string, List<(double X, double Y)>> _previousPoints = new Dictionary<string, List<(double X, double Y)>>();
        private float _animationProgress = 0f;
        private readonly TimeSpan _animationDuration = TimeSpan.FromMilliseconds(500);
        private DateTime _animationStartTime;
        private readonly object _collectionLock = new object();

        public ChartRenderer(BaseChart chart)
        {
            _chart = chart;
            DeviceDisplay.MainDisplayInfoChanged += (s, e) => _chart.InvalidateSurface();

            // Initialize paints with theme-based colors
            _axisLabelPaint = new SKPaint { IsAntialias = true };
            _gridPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                IsAntialias = true
            };
            _linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 4,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round
            };
            _markerPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            _tooltipPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            _tooltipTextPaint = new SKPaint
            {
                IsAntialias = true
            };
            _magnifierPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 2,
                IsAntialias = true
            };
        }

        private SKColor GetThemeColor(SKColor lightColor, SKColor darkColor)
        {
            return Application.Current.UserAppTheme == AppTheme.Dark ? darkColor : lightColor;
        }
        private SKPaint GetThemeColor(SKPaint lightColor, SKPaint darkColor)
        {
            return Application.Current.UserAppTheme == AppTheme.Dark ? darkColor : lightColor;
        }

        private SKShader GetMagnifierShader()
        {
            var lightColors = new[] { SKColors.White, SKColors.WhiteSmoke };
            var darkColors = new[] { SKColors.DarkGray, SKColors.Gray };
            var colors = Application.Current.UserAppTheme == AppTheme.Dark ? darkColors : lightColors;
            return SKShader.CreateRadialGradient(
                new SKPoint(0, 0),
                50,
                colors,
                new[] { 0f, 1f },
                SKShaderTileMode.Clamp);
        }

        public void Render(SKSurface surface, int width, int height, (double X, double Y, string SeriesLabel, ChartDataPoint DataPoint)? selectedPoint, float panOffsetX, bool isMagnifying)
        {
            var canvas = surface.Canvas;
            canvas.Clear(GetThemeColor(SKColors.White, SKColors.DarkGray));

            // Set theme-based colors
            _axisLabelPaint.Color = GetThemeColor(SKColors.Gray, SKColors.WhiteSmoke);
            _gridPaint.Color = GetThemeColor(SKColors.LightGray.WithAlpha(100), SKColors.Gray.WithAlpha(100));
            _markerPaint.Color = GetThemeColor(SKColors.Red, SKColors.Crimson);
            _tooltipPaint.Color = GetThemeColor(SKColors.Black.WithAlpha(220), SKColors.White.WithAlpha(220));
            _tooltipTextPaint.Color = GetThemeColor(SKColors.White, SKColors.Black);
            _magnifierPaint.Shader = GetMagnifierShader();

            if (_chart.Series == null || !_chart.Series.Any() || _chart.Series.All(s => s.ItemsSource == null || !s.ItemsSource.Cast<object>().Any()))
            {
                canvas.DrawText("No data to display", width / 2f, height / 2f, _font, _axisLabelPaint);
                return;
            }

            if (_animationProgress < 1f)
            {
                var elapsed = DateTime.Now - _animationStartTime;
                _animationProgress = (float)(elapsed.TotalMilliseconds / _animationDuration.TotalMilliseconds);
                if (_animationProgress > 1f) _animationProgress = 1f;
                _chart.InvalidateSurface();
            }

            float margin = 60;
            float plotWidth = width - 2 * margin;
            float plotHeight = height - 2 * margin;

            var allPoints = new List<(double X, double Y)>();
            var seriesPoints = new Dictionary<string, List<(double X, double Y)>>();

            lock (_collectionLock)
            {
                foreach (var series in _chart.Series.OfType<LineSeries>())
                {
                    var points = ExtractDataPoints(series);
                    seriesPoints[series.Label] = points;
                    allPoints.AddRange(points);
                }
            }

            if (!allPoints.Any())
            {
                canvas.DrawText("No data to display", width / 2f, height / 2f, _font, _axisLabelPaint);
                return;
            }

            var xMin = allPoints.Min(p => p.X);
            var xMax = allPoints.Max(p => p.X);
            var yMin = allPoints.Min(p => p.Y);
            var yMax = allPoints.Max(p => p.Y);

            if (xMax == xMin || yMax == yMin)
            {
                canvas.DrawText("Invalid data range", width / 2f, height / 2f, _font, _axisLabelPaint);
                return;
            }

            var xRange = xMax - xMin;
            var panShift = panOffsetX / plotWidth * xRange;
            xMin += panShift;
            xMax += panShift;

            var yTicks = 5;
            for (int i = 0; i <= yTicks; i++)
            {
                var yPos = margin + plotHeight - (i / (float)yTicks) * plotHeight;
                canvas.DrawLine(margin, yPos, margin + plotWidth, yPos, _gridPaint);
            }

            canvas.DrawLine(margin, margin + plotHeight, margin + plotWidth, margin + plotHeight, _axisLabelPaint);
            canvas.DrawLine(margin, margin, margin, margin + plotHeight, _axisLabelPaint);

            var totalTimeSpan = TimeSpan.FromSeconds(xMax - xMin);
            bool useDayFormat = totalTimeSpan.TotalHours > 24;
            string labelFormat = useDayFormat ? "ddd HH:mm" : "HH:mm";
            int xTicks = useDayFormat ? 4 : 5;

            for (int i = 0; i <= xTicks; i++)
            {
                var xValue = xMin + (xMax - xMin) * i / xTicks;
                var date = DateTimeOffset.FromUnixTimeSeconds((long)xValue).LocalDateTime;
                var xPos = margin + (i / (float)xTicks) * plotWidth;

                if (useDayFormat)
                {
                    canvas.Save();
                    canvas.Translate(xPos, margin + plotHeight + 10);
                    canvas.RotateDegrees(45);
                    canvas.DrawText(date.ToString(labelFormat), 0, 0, _font, _axisLabelPaint);
                    canvas.Restore();
                }
                else
                {
                    canvas.DrawText(date.ToString(labelFormat), xPos, margin + plotHeight + 30, _font, _axisLabelPaint);
                }
            }

            for (int i = 0; i <= yTicks; i++)
            {
                var yValue = yMin + (yMax - yMin) * i / yTicks;
                var yPos = margin + plotHeight - (i / (float)yTicks) * plotHeight;
                canvas.DrawText($"{yValue:F1}°C", margin - 50, yPos + 5, _font, _axisLabelPaint);
            }

            SKBitmap magnifierBitmap = null;
            SKCanvas magnifierCanvas = null;
            if (isMagnifying && selectedPoint.HasValue)
            {
                magnifierBitmap = new SKBitmap(100, 100);
                magnifierCanvas = new SKCanvas(magnifierBitmap);
                magnifierCanvas.Clear(SKColors.Transparent);
                magnifierCanvas.Scale(2f);
                magnifierCanvas.Translate((float)(-selectedPoint.Value.X / 2f + 50 / 2f), (float)(-selectedPoint.Value.Y / 2f + 50 / 2f));
            }

            foreach (var series in _chart.Series.OfType<LineSeries>())
            {
                var points = seriesPoints[series.Label];
                if (!points.Any()) continue;

                _linePaint.Color = series.Color.WithAlpha((byte)(255 * _animationProgress));
                _linePaint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(margin, margin),
                    new SKPoint(margin, margin + plotHeight),
                    new[] { series.Color, series.Color.WithAlpha(100) },
                    new[] { 0f, 1f },
                    SKShaderTileMode.Clamp);

                var path = new SKPath();
                bool firstPoint = true;

                var previousPoints = _previousPoints.ContainsKey(series.Label) ? _previousPoints[series.Label] : new List<(double X, double Y)>();
                for (int i = 0; i < points.Count; i++)
                {
                    var current = points[i];
                    var prev = previousPoints.ElementAtOrDefault(i);
                    var x = margin + (float)((current.X - xMin) / (xMax - xMin) * plotWidth);
                    var y = margin + plotHeight - (float)((current.Y - yMin) / (yMax - yMin) * plotHeight);

                    if (_animationProgress < 1f && previousPoints.Any())
                    {
                        var prevX = margin + (float)((prev.X - xMin) / (xMax - xMin) * plotWidth);
                        var prevY = margin + plotHeight - (float)((prev.Y - yMin) / (yMax - yMin) * plotHeight);
                        x = prevX + (x - prevX) * _animationProgress;
                        y = prevY + (y - prevY) * _animationProgress;
                    }

                    if (firstPoint)
                    {
                        path.MoveTo(x, y);
                        firstPoint = false;
                    }
                    else
                    {
                        path.LineTo(x, y);
                    }

                    if (magnifierCanvas != null)
                    {
                        var magnifierPath = new SKPath();
                        magnifierPath.MoveTo(x, y);
                        magnifierPath.LineTo(x, y);
                        magnifierCanvas.DrawPath(magnifierPath, _linePaint);
                    }
                }

                canvas.DrawPath(path, _linePaint);
                canvas.DrawText(series.Label, margin + 10, margin - 20, _font, GetThemeColor(_markerPaint, _axisLabelPaint));
            }

            if (isMagnifying && selectedPoint.HasValue && magnifierBitmap != null)
            {
                var x = margin + (float)((selectedPoint.Value.X - xMin) / (xMax - xMin) * plotWidth);
                var y = margin + plotHeight - (float)((selectedPoint.Value.Y - yMin) / (yMax - yMin) * plotHeight);

                magnifierCanvas?.DrawLine(x - 5, y, x + 5, y, _markerPaint);
                magnifierCanvas?.DrawLine(x, y - 5, x, y + 5, _markerPaint);

                canvas.DrawBitmap(magnifierBitmap, x - 50, y - 70, _magnifierPaint);
                magnifierBitmap.Dispose();
                magnifierCanvas?.Dispose();
            }

            if (selectedPoint.HasValue && !isMagnifying)
            {
                var x = margin + (float)((selectedPoint.Value.X - xMin) / (xMax - xMin) * plotWidth);
                var y = margin + plotHeight - (float)((selectedPoint.Value.Y - yMin) / (yMax - yMin) * plotHeight);

                canvas.DrawCircle(x, y, 6, _markerPaint);

                var date = DateTimeOffset.FromUnixTimeSeconds((long)selectedPoint.Value.X).LocalDateTime;
                var tooltipText = $"{selectedPoint.Value.SeriesLabel}: {selectedPoint.Value.Y:F1}°C\n{date:HH:mm}";
                _font.MeasureText(tooltipText, out var bounds);
                var tooltipRect = new SKRect(x + 10, y - bounds.Height - 10, x + bounds.Width + 20, y + 10);
                canvas.DrawRoundRect(tooltipRect, 5, 5, _tooltipPaint);
                canvas.DrawText(tooltipText, x + 15, y - 5, _font, _tooltipTextPaint);
            }

            if (_animationProgress >= 1f)
            {
                foreach (var series in _chart.Series.OfType<LineSeries>())
                {
                    _previousPoints[series.Label] = seriesPoints[series.Label];
                }
            }
        }

        public List<(double X, double Y)> ExtractDataPoints(LineSeries series)
        {
            var dataPoints = new List<(double X, double Y)>();
            if (series.ItemsSource == null)
            {
                return dataPoints;
            }

            if (!_previousPoints.ContainsKey(series.Label))
            {
                _previousPoints[series.Label] = new List<(double X, double Y)>();
                _animationStartTime = DateTime.Now;
                _animationProgress = 0f;
            }

            List<object> itemsToProcess;
            lock (_collectionLock)
            {
                itemsToProcess = series.ItemsSource.Cast<object>().ToList();
            }

            foreach (var item in itemsToProcess)
            {
                var xValue = GetPropertyValue(item, series.XValuePath);
                var yValue = GetPropertyValue(item, series.YValuePath);
                if (xValue.HasValue && yValue.HasValue)
                {
                    dataPoints.Add((xValue.Value, yValue.Value));
                }
            }
            return dataPoints;
        }

        private double? GetPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            var pathParts = propertyPath.Split('.');
            object currentObject = obj;
            foreach (var part in pathParts)
            {
                if (currentObject == null)
                {
                    return null;
                }
                var type = currentObject.GetType();
                if (!_propertyCache.TryGetValue(type.FullName + "." + part, out var propInfo))
                {
                    propInfo = type.GetProperty(part);
                    if (propInfo == null)
                    {
                        return null;
                    }
                    _propertyCache[type.FullName + "." + part] = propInfo;
                }
                currentObject = propInfo.GetValue(currentObject);
            }

            if (currentObject == null)
            {
                return null;
            }

            try
            {
                if (currentObject is DateTime dateTime)
                {
                    if (dateTime == DateTime.MinValue)
                    {
                        return null;
                    }
                    var timestamp = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
                    return timestamp;
                }
                var result = Convert.ToDouble(currentObject);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}