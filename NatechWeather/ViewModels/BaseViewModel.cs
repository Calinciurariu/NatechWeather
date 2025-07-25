using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NatechWeather.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, IViewModel
    {
        protected INavigationPageService Navigation;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand GoBackCommand { get; }

        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        private bool _isRefreshing = false;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public BaseViewModel(INavigationPageService navigation)
        {
            Navigation = navigation;
            GoBackCommand = new AsyncRelayCommand(BackHandler);

        }
        public virtual async Task BackHandler()
        {
            await Navigation.PopAsync();
        }
        public virtual void SetParameters(IDictionary<string, object> parameters) { }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
