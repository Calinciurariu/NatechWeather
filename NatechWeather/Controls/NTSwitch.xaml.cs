
using CommunityToolkit.Maui.Extensions;
using NatechWeather.Helpers;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NatechWeather.Controls;
public partial class NTSwitch : ContentView
{
    #region properties
    #region On
    public static readonly BindableProperty OnTextProperty = BindableProperty.Create(nameof(OnText), typeof(string), typeof(NTSwitch), string.Empty);
    public string OnText
    {
        get => (string)GetValue(OnTextProperty);
        set => SetValue(OnTextProperty, value);
    }

    public static readonly BindableProperty OnFATextProperty = BindableProperty.Create(nameof(OnFAText), typeof(string), typeof(NTSwitch), string.Empty);
    public string OnFAText
    {
        get => (string)GetValue(OnFATextProperty);
        set => SetValue(OnFATextProperty, value);
    }


    public static readonly BindableProperty OnFAFontFamilyProperty = BindableProperty.Create(nameof(OnFAFontFamily), typeof(string), typeof(NTSwitch), string.Empty);
    public string OnFAFontFamily
    {
        get => (string)GetValue(OnFAFontFamilyProperty);
        set => SetValue(OnFAFontFamilyProperty, value);
    }

    public static readonly BindableProperty OnCommandProperty =
      BindableProperty.Create(nameof(OnCommand), typeof(ICommand), typeof(NTSwitch), null);

    public ICommand OnCommand
    {
        get { return (ICommand)GetValue(OnCommandProperty); }
        set { SetValue(OnCommandProperty, value); }
    }

    public static readonly BindableProperty OnCommandParameterProperty =
     BindableProperty.Create(nameof(OnCommandParameter), typeof(object), typeof(NTSwitch), null);

    public object OnCommandParameter
    {
        get { return GetValue(OnCommandParameterProperty); }
        set { SetValue(OnCommandParameterProperty, value); }
    }
    #endregion

    #region Off
    public static readonly BindableProperty OffFATextProperty = BindableProperty.Create(nameof(OffFAText), typeof(string), typeof(NTSwitch), string.Empty);
    public string OffFAText
    {
        get => (string)GetValue(OffFATextProperty);
        set => SetValue(OffFATextProperty, value);
    }

    public static readonly BindableProperty OffTextProperty = BindableProperty.Create(nameof(OffText), typeof(string), typeof(NTSwitch), string.Empty);
    public string OffText
    {
        get => (string)GetValue(OffTextProperty);
        set => SetValue(OffTextProperty, value);
    }

    public static readonly BindableProperty OffCommandProperty =
       BindableProperty.Create(nameof(OffCommand), typeof(ICommand), typeof(NTSwitch), null);

    public ICommand OffCommand
    {
        get { return (ICommand)GetValue(OffCommandProperty); }
        set { SetValue(OffCommandProperty, value); }
    }


    public static readonly BindableProperty OffCommandParameterProperty =
     BindableProperty.Create(nameof(OffCommandParameter), typeof(object), typeof(NTSwitch), null);

    public object OffCommandParameter
    {
        get { return GetValue(OffCommandParameterProperty); }
        set { SetValue(OffCommandParameterProperty, value); }
    }

    public static readonly BindableProperty OffFAFontFamilyProperty = BindableProperty.Create(nameof(OffFAFontFamily), typeof(string), typeof(NTSwitch), string.Empty);
    public string OffFAFontFamily
    {
        get => (string)GetValue(OffFAFontFamilyProperty);
        set => SetValue(OffFAFontFamilyProperty, value);
    }
    #endregion

    #region Colors  



    public static readonly BindableProperty SelectedTextColorProperty =
            BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(NTSwitch), Color.FromHex("#FFFFFF"));
    public Color SelectedTextColor
    {
        get { return (Color)GetValue(SelectedTextColorProperty); }
        set { SetValue(SelectedTextColorProperty, value); }
    }



    public static readonly BindableProperty NotSelectedTextColorProperty =
          BindableProperty.Create(nameof(NotSelectedTextColor), typeof(Color), typeof(NTSwitch), Colors.DarkGray);
    public Color NotSelectedTextColor
    {
        get { return (Color)GetValue(NotSelectedTextColorProperty); }
        set { SetValue(NotSelectedTextColorProperty, value); }
    }
    #endregion

    #region IsOn
    public static readonly BindableProperty IsOnProperty = BindableProperty.Create(nameof(IsOn), typeof(bool), typeof(NTSwitch), false, BindingMode.TwoWay, propertyChanged: IsOnPropertyChanged);
    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set
        {
            SetValue(IsOnProperty, value);
        }
    }

    private static void IsOnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is NTSwitch dtSwitch))
        {
            return;
        }
        var newValueBool = (bool)newValue;
        if (newValueBool != dtSwitch.isToggled)
        {
            var stckLayout = newValueBool ? dtSwitch.OnStack : dtSwitch.OffStack;
            dtSwitch.ChangeState(stckLayout, new EventArgs { });
        }

        if (newValueBool)
            dtSwitch.OnCommand?.Execute(dtSwitch.OnCommandParameter);
        else
            dtSwitch.OffCommand?.Execute(dtSwitch.OffCommandParameter);
    }
    #endregion
    #endregion

    public NTSwitch()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsEnabled))
            InternalFrame.BackgroundColorTo(Application.Current.GetThemeColor(IsEnabled ? "primary" : "secondary"));
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        IsOnPropertyChanged(this, isToggled, IsOn);
    }

    internal async Task ChangeState(object sender, EventArgs e)
    {
        if (grdContainer.Width > 0)
        {
            var Control = sender as Element;
            Color selectedColor = Application.Current.GetThemeColor("Black");
            Color unSelectedColor = Application.Current.GetThemeColor("Black");

            switch (Control.AutomationId)
            {
                case "1":
                    if (isToggled && e != null)
                        return;

                    InternalFrame.AbortAnimation("Slide");
                    InternalFrame.Animate("Slide", new Animation((d) =>
                    {
                        InternalFrame.TranslateTo(grdContainer.Width / 2, 0, easing: Easing.CubicIn);

                        lblImgOff.TextColorTo(unSelectedColor);
                        lblTxtOff.TextColorTo(unSelectedColor);

                        lblImgOn.TextColorTo(selectedColor);
                        lblTxtOn.TextColorTo(selectedColor);
                    }), finished: (finalValue, canceled) =>
                    {
                        if (!canceled)
                        {
                            Task.Run(async () =>
                            {
                                await Task.Delay(250);

                                Dispatcher.Dispatch(() =>
                                {
                                    isToggled = true;
                                    IsOn = isToggled;
                                });
                            });
                        }
                    });
                    break;
                case "2":
                    if (!isToggled && e != null)
                        return;

                    InternalFrame.AbortAnimation("Slide");
                    InternalFrame.Animate("Slide", new Animation((d) =>
                    {
                        InternalFrame.TranslateTo(0, 0, easing: Easing.CubicIn);

                        lblImgOff.TextColorTo(selectedColor);
                        lblTxtOff.TextColorTo(selectedColor);

                        lblImgOn.TextColorTo(unSelectedColor);
                        lblTxtOn.TextColorTo(unSelectedColor);
                    }), finished: (finalValue, canceled) =>
                    {
                        if (!canceled)
                        {
                            Task.Run(async () =>
                            {
                                await Task.Delay(250);

                                Dispatcher.Dispatch(() =>
                                {
                                    isToggled = false;
                                    IsOn = isToggled;
                                });
                            });
                        }
                    });

                    break;
                default:
                    break;
            }
        }
    }

    #region On Click
    internal bool isToggled = false;
    public async void Stack_Tapped(object sender, EventArgs e)
    {
        if (this.IsEnabled)
            await ChangeState(sender, e);
    }
    #endregion

    #region On Swipe
    private double valueX, valueY;
    private bool IsTurnX, IsTurnY;
    public async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (!this.IsEnabled)
            return;

        var x = e.TotalX; // TotalX Left/Right
        var y = e.TotalY; // TotalY Up/Down

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                break;
            case GestureStatus.Running:

                if ((x >= 5 || x <= -5) && !IsTurnX && !IsTurnY)
                {
                    IsTurnX = true;
                }

                if ((y >= 5 || y <= -5) && !IsTurnY && !IsTurnX)
                {
                    IsTurnY = true;
                }

                if (IsTurnX && !IsTurnY)
                {
                    if (x <= valueX)
                    {
                        await ChangeState(OffStack, new EventArgs { });
                    }

                    if (x >= valueX)
                    {
                        await ChangeState(OnStack, new EventArgs { });
                    }
                }
                break;

            case GestureStatus.Completed:

                valueX = x;
                valueY = y;

                IsTurnX = false;
                IsTurnY = false;

                break;
            case GestureStatus.Canceled:
                break;
        }
    }
    #endregion
}

