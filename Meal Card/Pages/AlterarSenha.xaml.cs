using Meal_Card.Controls;
using Meal_Card.Services;

namespace Meal_Card.Pages;

public partial class AlterarSenha : ContentPage
{
    private readonly AuthService _authService;
    public AlterarSenha( AuthService authService )
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void Refreshing( object sender, EventArgs e )
    {
        refreshView.IsRefreshing = true;
        await Task.Delay(1000);
        refreshView.IsRefreshing = false;
    }

    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        await Navigation.PopAsync();
    }

    public bool Error = false;
    private void Cancelar_Clicked( object sender, EventArgs e )
    {
        txt_SenhaAtual.Text = "";
        txt_NovaSenha.Text = "";
        txt_ConfirmarSenha.Text = "";
        Error = false;

    }

    private async void Alterar_Clicked( object sender, EventArgs e )
    {
        string? Senha_Atual = txt_SenhaAtual.Text;
        string? Nova_Senha = txt_NovaSenha.Text;
        string? Confirmar_Senha = txt_ConfirmarSenha.Text;

        if (string.IsNullOrEmpty(Senha_Atual) && string.IsNullOrEmpty(Nova_Senha) && string.IsNullOrEmpty(Confirmar_Senha))
        {

            await NotificationToast.ShowToastL("Campos vazios. Preencha os campos e tente novamente");
            txt_SenhaAtual.BorderColor = Colors.Red;
            txt_NovaSenha.BorderColor = Colors.Red;
            txt_ConfirmarSenha.BorderColor = Colors.Red;
            Error = true;

        }
        else if (Senha_Atual.Length < 4 || Nova_Senha.Length < 4 || Confirmar_Senha.Length < 4)
        {

            await NotificationToast.ShowToastL("A senha deve conter pelo menos 4 caracteres");
            txt_SenhaAtual.BorderColor = Colors.Red;
            txt_NovaSenha.BorderColor = Colors.Red;
            txt_ConfirmarSenha.BorderColor = Colors.Red;
            Error = true;

        }
        if (Nova_Senha != Confirmar_Senha)
        {
            await NotificationToast.ShowToastL("As senhas não correspondem");
            txt_NovaSenha.BorderColor = Colors.Red;
            txt_ConfirmarSenha.BorderColor = Colors.Red;
            Error = true;

        }
        try
        {
            var response = await _authService.PostNovaSenha(Senha_Atual, Confirmar_Senha);

            if (!response.HasError && response.Data)
            {
                await NotificationToast.ShowToastL("Senha alterada com sucesso...");
                txt_SenhaAtual.BorderColor = Colors.Green;
                txt_NovaSenha.BorderColor = Colors.Green;
                txt_ConfirmarSenha.BorderColor = Colors.Green;
                Error = false;

            }
            else
            {
                await NotificationToast.ShowToastL(" Não foi possivel alterar a senha. " +
                                                   " Por favor, tente novamente ... ");
            }

        }
        catch (Exception ex)
        {
            await NotificationToast.ShowToastL(" Não foi possivel alterar a senha. " +
                                               " Por favor, tente novamente ... ");
            System.Diagnostics.Debug.WriteLine($"LoadingPage Error: {ex.Message}");
            txt_SenhaAtual.BorderColor = Colors.Red;
            txt_NovaSenha.BorderColor = Colors.Red;
            txt_ConfirmarSenha.BorderColor = Colors.Red;
            Error = true;
        }

    }

}
