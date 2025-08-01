using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace NatechWeather.Controls
{
    public partial class LocationInputView : ContentView
    {
        public LocationInputView()
        {
            InitializeComponent();
        }
        
        public static readonly BindableProperty IsEnabledLocationProperty =
        BindableProperty.Create(nameof(IsEnabledLocation), typeof(bool), typeof(LocationInputView), default(bool), BindingMode.TwoWay);

        public bool IsEnabledLocation
        {
            get => (bool)GetValue(IsEnabledLocationProperty);
            set => SetValue(IsEnabledLocationProperty, value);
        }
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(LocationInputView), default(string), BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LocationInputView), default(string));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty GetLocationCommandProperty =
            BindableProperty.Create(nameof(GetLocationCommand), typeof(ICommand), typeof(LocationInputView), null);

        public ICommand GetLocationCommand
        {
            get => (ICommand)GetValue(GetLocationCommandProperty);
            set => SetValue(GetLocationCommandProperty, value);
        }
    }
}
