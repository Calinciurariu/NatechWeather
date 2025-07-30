using NatechCharts.Converters;
using NatechCharts.Enums;
using NatechCharts.Interfaces;
using SkiaSharp;
using System.Collections;
using System.ComponentModel;

namespace NatechCharts.Controls
{
    [TypeConverter(typeof(StringToSKColorTypeConverter))]
    public class LineSeries : BindableObject, ISeries
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(LineSeries), null);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ColorProperty =
              BindableProperty.Create(nameof(Color), typeof(SKColor), typeof(LineSeries), SKColors.Black);

        public SKColor Color
        {
            get => (SKColor)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty LabelProperty =
            BindableProperty.Create(nameof(Label), typeof(string), typeof(LineSeries), string.Empty);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public ChartType Type => ChartType.Line;

        public static readonly BindableProperty XValuePathProperty =
            BindableProperty.Create(nameof(XValuePath), typeof(string), typeof(LineSeries), string.Empty);

        public string XValuePath
        {
            get => (string)GetValue(XValuePathProperty);
            set => SetValue(XValuePathProperty, value);
        }

        public static readonly BindableProperty YValuePathProperty =
            BindableProperty.Create(nameof(YValuePath), typeof(string), typeof(LineSeries), string.Empty);

        public string YValuePath
        {
            get => (string)GetValue(YValuePathProperty);
            set => SetValue(YValuePathProperty, value);
        }
    }
}