using NatechCharts.Enums;
using SkiaSharp;
using System.Collections;

namespace NatechCharts.Interfaces
{
    public interface ISeries
    {
        IEnumerable ItemsSource { get; set; }
        SKColor Color { get; set; }
        string Label { get; set; }
        ChartType Type { get; }
    }
}
