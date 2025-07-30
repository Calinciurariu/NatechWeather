using NatechCharts.Interfaces;
using NatechCharts.Renderers;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Collections;
using System.Collections.ObjectModel;

namespace NatechCharts.Controls
{
    public abstract class BaseChart : ContentView
    {
        protected readonly SKGLView SkiaView;
        protected readonly ChartRenderer ChartRenderer;

        public BaseChart()
        {
            SkiaView = new SKGLView();
            Content = SkiaView; 

            ChartRenderer = new ChartRenderer(this);
            SkiaView.EnableTouchEvents = true; 
            SkiaView.PaintSurface += OnPaintSurface;
            SkiaView.Touch += OnTouch;
        }

     

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(BaseChart), null,
                propertyChanged: OnItemsSourceChanged);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SeriesProperty =
            BindableProperty.Create(nameof(Series), typeof(ObservableCollection<ISeries>), typeof(BaseChart),
                new ObservableCollection<ISeries>(), propertyChanged: OnSeriesChanged);

        public ObservableCollection<ISeries> Series
        {
            get => (ObservableCollection<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BaseChart chart)
            {
                chart.SkiaView.InvalidateSurface();
            }
        }

        private static void OnSeriesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BaseChart chart)
            {
                // TODO: Handle collectionchanged events for dynamic updates
                chart.SkiaView.InvalidateSurface();

            }
        }
        private void OnTouch(object? sender, SKTouchEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs e)
        {
            ChartRenderer.Render(e.Surface, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
        }
    }
}