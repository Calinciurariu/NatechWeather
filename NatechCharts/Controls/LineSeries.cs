using NatechCharts.Enums;
using NatechCharts.Interfaces;
using NatechCharts.Models;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NatechCharts.Controls
{
    public class LineSeries : BindableObject, ISeries
    {
        private BaseChart _parentChart;
        private bool _isFullyInitialized;

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(ObservableCollection<ChartDataPoint>),
                typeof(LineSeries),
                defaultValueCreator: bindable => new ObservableCollection<ChartDataPoint>(),
                propertyChanged: OnItemsSourceChanged);

        public ObservableCollection<ChartDataPoint> ItemsSource
        {
            get => (ObservableCollection<ChartDataPoint>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(
                nameof(Color),
                typeof(SKColor),
                typeof(LineSeries),
                SKColors.Black,
                propertyChanged: OnColorChanged);

        public SKColor Color
        {
            get => (SKColor)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty LabelProperty =
            BindableProperty.Create(
                nameof(Label),
                typeof(string),
                typeof(LineSeries),
                string.Empty,
                propertyChanged: OnPropertyChanged);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public ChartType Type => ChartType.Line;

        public static readonly BindableProperty XValuePathProperty =
            BindableProperty.Create(
                nameof(XValuePath),
                typeof(string),
                typeof(LineSeries),
                string.Empty,
                propertyChanged: OnPropertyChanged);

        public string XValuePath
        {
            get => (string)GetValue(XValuePathProperty);
            set => SetValue(XValuePathProperty, value);
        }

        public static readonly BindableProperty YValuePathProperty =
            BindableProperty.Create(
                nameof(YValuePath),
                typeof(string),
                typeof(LineSeries),
                string.Empty,
                propertyChanged: OnPropertyChanged);

        public string YValuePath
        {
            get => (string)GetValue(YValuePathProperty);
            set => SetValue(YValuePathProperty, value);
        }

        public LineSeries() : base()
        {
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not LineSeries series)
            {
                Console.WriteLine("OnItemsSourceChanged: bindable is not LineSeries");
                return;
            }

            Console.WriteLine($"OnItemsSourceChanged: oldValue={oldValue?.GetType()?.Name}, newValue={newValue?.GetType()?.Name}");

            if (oldValue is ObservableCollection<ChartDataPoint> oldCollection)
            {
                oldCollection.CollectionChanged -= series.OnItemsSourceCollectionChanged;
            }

            if (newValue is ObservableCollection<ChartDataPoint> newCollection)
            {
                newCollection.CollectionChanged += series.OnItemsSourceCollectionChanged;
            }

            series.TryInvalidateSurface();
        }

        private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LineSeries series && newValue is SKColor skColor)
            {
                Console.WriteLine($"OnColorChanged: newValue={newValue}");
                series.ChangeColor(skColor);
            }
        }

        private void ChangeColor(SKColor skColor)
        {
            if (_parentChart != null && _isFullyInitialized && ItemsSource?.Any() == true)
            {
                _parentChart.lineColor = skColor;
                Console.WriteLine("ChangeColor: Invalidating surface");
                _parentChart.InvalidateSurface();
            }
            else
            {
                Console.WriteLine($"TryInvalidateSurface: Skipping (parent={_parentChart != null}, initialized={_isFullyInitialized}, hasData={ItemsSource?.Any() == true})");
            }
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LineSeries series)
            {
                Console.WriteLine($"OnPropertyChanged: Property={bindable.GetType().GetProperty("Name")?.GetValue(bindable)}, newValue={newValue}");
                series._isFullyInitialized = !string.IsNullOrEmpty(series.XValuePath) && !string.IsNullOrEmpty(series.YValuePath) && !string.IsNullOrEmpty(series.Label);
                series.TryInvalidateSurface();
            }
        }

        private void TryInvalidateSurface()
        {
            if (_parentChart != null && _isFullyInitialized && ItemsSource?.Any() == true)
            {
                Console.WriteLine("TryInvalidateSurface: Invalidating surface");
                _parentChart.InvalidateSurface();
            }
            else
            {
                Console.WriteLine($"TryInvalidateSurface: Skipping (parent={_parentChart != null}, initialized={_isFullyInitialized}, hasData={ItemsSource?.Any() == true})");
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine($"OnItemsSourceCollectionChanged: Action={e.Action}, NewItems={e.NewItems?.Count ?? 0}");
            TryInvalidateSurface();
        }

        void ISeries.SetParent(BaseChart chart)
        {
            _parentChart = chart;
            Console.WriteLine("SetParent: Parent set");
            TryInvalidateSurface();
        }
    }
}