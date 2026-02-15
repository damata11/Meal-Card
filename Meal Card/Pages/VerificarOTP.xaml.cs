namespace Meal_Card.Pages;

public partial class VerificarOTP : ContentPage
{
    public VerificarOTP()
    {
        InitializeComponent();
        Shell.SetPresentationMode(this, PresentationMode.Modal);
    }

    public bool error;
    private async void TapOTP_Tapped( object sender, TextChangedEventArgs e )
    {
        await AppShell.Current.GoToAsync(state: "//Login");
    }

    private void OTP_TextChanged( object sender, TextChangedEventArgs e )
    {
        string OTPcode = txt_OTP.Text;

        if (string.IsNullOrEmpty(OTPcode))
        {
            txt_OTP.BorderColor = Colors.Red;
            error = true;

        }
        else
        {
            txt_OTP.BorderColor = Colors.Green;
            error = false;

        }

        if (error)
        {
            return;

        }
    }

    private void TapEnviar_Tapped( object sender, TappedEventArgs e )
    {

    }
}