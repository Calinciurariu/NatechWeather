using NatechCharts.Controls;
using NatechCharts.Enums;
using NatechCharts.Models;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace NatechCharts.Interfaces
{
    public interface ISeries
    {
        ObservableCollection<ChartDataPoint> ItemsSource { get; set; }
        SKColor Color { get; set; }
        string Label { get; set; }
        ChartType Type { get; }
        void SetParent(BaseChart chart);
    }
}
