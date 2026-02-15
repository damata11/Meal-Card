using CommunityToolkit.Maui.Alerts;

namespace Meal_Card.Pages;

public partial class RecuperarSenha : ContentPage
{
    public RecuperarSenha()
    {
        InitializeComponent();
        Shell.SetPresentationMode(this, PresentationMode.Modal);
    }

    public bool error;

    public async void BtnRedifinir_Clicked( object sender, TappedEventArgs e )
    {

    }

    private async void txt_ConfirmarSenha_TextChanged( object sender, TextChangedEventArgs e )
    {
        if (txt_NovaSenha.Text != txt_ConfirmarSenha.Text)
        {
            var notification = Toast.Make("As senhas não coincidem",
                CommunityToolkit.Maui.Core.ToastDuration.Short);
            await notification.Show();
            txt_ConfirmarSenha.BorderColor = Colors.Red;
            txt_NovaSenha.BorderColor = Colors.Red;
            error = true;
        }
        if (txt_ConfirmarSenha.Text.Length < 4)
        {
            var notification = Toast.Make("A senha deve conter entre 4 a 20 caracteres, incluindo letras e numeros",
            CommunityToolkit.Maui.Core.ToastDuration.Short);
            await notification.Show();
            txt_NovaSenha.BorderColor = Colors.Red;
            error = true;
        }
        else
        {
            txt_ConfirmarSenha.BorderColor = Colors.Black;
            txt_NovaSenha.BorderColor = Colors.Black;
            error = false;
        }

    }

    private async void txt_NovaSenha_TextChanged( object sender, TextChangedEventArgs e )
    {
        if (txt_NovaSenha.Text.Length < 4)
        {
            var notification = Toast.Make("A senha deve conter entre 4 a 20 caracteres, incluindo letras e numeros",
            CommunityToolkit.Maui.Core.ToastDuration.Short);
            await notification.Show();
            txt_ConfirmarSenha.BorderColor = Colors.Red;
            txt_NovaSenha.BorderColor = Colors.Red;
            error = true;
        }
        else
        {
            txt_NovaSenha.BorderColor = Colors.Black;
            error = false;
        }
    }
}