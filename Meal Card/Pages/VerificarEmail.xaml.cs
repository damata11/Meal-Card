using System.Text.RegularExpressions;

namespace Meal_Card.Pages;

public partial class VerificarEmail : ContentPage
{
    public VerificarEmail()
    {
        InitializeComponent();
        Shell.SetPresentationMode(this, PresentationMode.Modal);
    }

    public bool error = false;
    public string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public void BtnVerificar_Clicked( object sender, EventArgs e )
    {

    }

    private void txt_email_TextChanged( object sender, TextChangedEventArgs e )
    {
        string email = txt_email.Text;
        if (string.IsNullOrEmpty(email))
        {

            txt_email.BorderColor = Colors.Red;
            error = true;

        }
        else if (!Regex.IsMatch(email, emailPattern))
        {

            txt_email.BorderColor = Colors.Red;
            error = true;

        }
        else
        {

            txt_email.BorderColor = Colors.Green;
            error = false;

        }

        if (error)
        {
            return;

        }
    }
}