using System.Windows.Input;

namespace Meal_Card.Controls;

public partial class ButtonControl : Border
{
    public ButtonControl()
    {
        InitializeComponent();
    }

    public event EventHandler<EventArgs>? Clicked, Tapped;

    public static readonly BindableProperty CommandProperty = BindableProperty.Create
    (propertyName: nameof(Command),
    returnType: typeof(ICommand),
    declaringType: typeof(ButtonControl),
    defaultBindingMode: BindingMode.TwoWay);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set { SetValue(CommandProperty, value); }
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create
    (propertyName: nameof(Text),
    returnType: typeof(string),
    declaringType: typeof(ButtonControl),
    defaultValue: "",
    defaultBindingMode: BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set { SetValue(TextProperty, value); }
    }

    public static readonly BindableProperty LoadingTextProperty = BindableProperty.Create
        (propertyName: nameof(LoadingText),
        returnType: typeof(string),
        declaringType: typeof(ButtonControl),
        defaultValue: "",
        defaultBindingMode: BindingMode.TwoWay);

    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set { SetValue(LoadingTextProperty, value); }
    }

    public static readonly BindableProperty IsInProgressProperty = BindableProperty.Create
    (propertyName: nameof(IsInProgress),
    returnType: typeof(bool),
    declaringType: typeof(ButtonControl),
    defaultValue: false,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: IsInProgressPropertyChanged);

    private static void IsInProgressPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ButtonControl controls)
        {
            bool isInProgress = (bool)newValue;
            if (isInProgress)
            {
                controls.lblButtonText.Text = controls.LoadingText;
            }
            else
            {
                controls.lblButtonText.Text = controls.Text;
            }
        }
    }
    public bool IsInProgress
    {
        get => (bool)GetValue(IsInProgressProperty);
        set { SetValue(IsInProgressProperty, value); }
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        propertyName: nameof(TextColor),
        returnType: typeof(Color),
        declaringType: typeof(ButtonControl),
        defaultValue: Colors.Black,
        defaultBindingMode: BindingMode.OneWay);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        propertyName: nameof(FontSize),
        returnType: typeof(double),
        declaringType: typeof(ButtonControl),
        defaultValue: 16.0d,
        defaultBindingMode: BindingMode.OneWay);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
nameof(CharacterSpacing), typeof(double), typeof(ButtonControl), defaultValue: 0.0d, defaultBindingMode: BindingMode.OneWay);

    public double CharacterSpacing
    {
        get => (double)GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
nameof(FontFamily), typeof(string), typeof(ButtonControl), defaultValue: "", defaultBindingMode: BindingMode.TwoWay);

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
nameof(FontAttributes), typeof(FontAttributes), typeof(ButtonControl), defaultValue: FontAttributes.None, defaultBindingMode: BindingMode.TwoWay);

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (this.IsInProgress)
        {
            return;
        }

        Clicked?.Invoke(sender, e);
    }
    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        Tapped?.Invoke(sender, e);
    }

    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

}
