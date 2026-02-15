using Meal_Card.Controls;
using Meal_Card.Services;
using Meal_Card.ViewModels;
using System.Text.RegularExpressions;

namespace Meal_Card.Pages;

public partial class Login : ContentPage
{
    private readonly AuthService _authService;
    private readonly InicioViewModel _inicioView;
    private readonly CantinaViewModel _cantinaView;
    private readonly CarrinhoViewModel _carrinhoView;
    private readonly CarteiraViewModel _carteiraView;
    private readonly DetalhesViewModel _detalhesView;

    public Login( AuthService authService, InicioViewModel inicioView, CarteiraViewModel carteiraView, CantinaViewModel cantinaView, DetalhesViewModel detalhesView, CarrinhoViewModel carrinhoView )
    {
        InitializeComponent();
        Shell.SetPresentationMode(this, PresentationMode.Modal);
        _authService = authService;
        _inicioView = inicioView;
        _carteiraView = carteiraView;
        _carrinhoView = carrinhoView;
        _cantinaView = cantinaView;
        _detalhesView = detalhesView;
    }

    public bool error = false;
    public string PadraoCard = @"^\d{6}$";
    public string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    protected override bool IsEnabledCore => base.IsEnabledCore;

    private async void BtnLogin_Clicked( object sender, EventArgs e )
    {
        string? card = null;
        string? email = null;
        string? EmailOrCard = txt_utilizador.Text;
        string? senha = txt_senha.Text;
        bool IsEmail = false;
        bool IsCard = false;

        lbl_login.IsInProgress = true;

        if (string.IsNullOrEmpty(EmailOrCard) && string.IsNullOrEmpty(senha))
        {
            await NotificationToast.ShowToastS(" Preencha os campos vazios e tente novamente. ");
            txt_utilizador.BorderColor = Colors.Red;
            txt_senha.BorderColor = Colors.Red;
            error = true;
        }
        if ((IsEmail = Regex.IsMatch(EmailOrCard, emailPattern)))
        {
            email = EmailOrCard;
            error = false;

        }
        else if ((IsCard = Regex.IsMatch(EmailOrCard, PadraoCard)))
        {
            card = EmailOrCard;
            error = false;
        }
        else
        {
            await NotificationToast.ShowToastS("Formato inválido. Use email ou número do cartão.");
            txt_utilizador.BorderColor = Colors.Red;
            lbl_login.IsInProgress = false;
            return;
        }

        // verificar a senha
        if (senha.Length < 4)
        {
            await NotificationToast.ShowToastS("A senha deve conter pelo menos 4 caracteres");
            txt_senha.BorderColor = Colors.Red;
            lbl_login.IsInProgress = false;
            return;
        }
        try
        {

            var response = await _authService.Login(email, card, senha);

            if (!response.HasError && response.Data)
            {
                await NotificationToast.ShowToastL("Login realizado com sucesso");
                txt_utilizador.BorderColor = Colors.Green;
                txt_utilizador.BorderColor = Colors.Green;
                lbl_login.IsInProgress = false;
                //Application.Current?.MainPage = new AppShell(_authService, _inicioView, _carteiraView);
                await AppShell.Current.GoToAsync(state: "//Home");
                //var window = Application.Current?.Windows[0];
                //window!.Page = new AppShell(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView, _carrinhoView);

            }
            else
            {

                //await DisplayAlert($"Sair do Aplicação", response.ErrorMessage, "Sim", "Não");
                await NotificationToast.ShowToastS("Credenciais incorretas ou invalidas");
                Console.WriteLine(response.ErrorMessage);

                lbl_login.IsInProgress = false;
            }
        }
        catch (Exception ex)
        {
            await NotificationToast.ShowToastL($"Algo deu errado... Tente novamente mais tarde! 😑");
            System.Diagnostics.Debug.WriteLine($"LoadingPage Error: {ex.Message}");
        }
        finally
        {
            lbl_login.IsInProgress = false;
        }

        if (error)
        {
            lbl_login.IsInProgress = false;
            return;
        }
    }

    protected override bool OnBackButtonPressed()
    {

        Dispatcher.Dispatch(async () =>
        {
            bool Comfirmation = await DisplayAlert("Sair do Aplicação", "Tens certeza queres sair ?", "Sim", "Não");
            if (Comfirmation)
            {
                Application.Current?.Quit();
            }
        });
        return true;
    }


    private void Txt_utilizador_TextChanged( object sender, TextChangedEventArgs e )
    {
        string user = txt_utilizador.Text;
        bool IsEmail = Regex.IsMatch(user, emailPattern);
        bool IsCard = Regex.IsMatch(user, PadraoCard);

        if (string.IsNullOrEmpty(user))
        {
            txt_utilizador.BorderColor = Colors.Red;
            error = true;
        }
        else if (IsEmail || IsCard)
        {
            txt_utilizador.BorderColor = Colors.Green;
            error = false;
        }
        else
        {
            txt_utilizador.BorderColor = Colors.Red;
            error = true;
        }

    }
    private void Txt_senha_TextChanged( object sender, TextChangedEventArgs e )
    {
        string senha = txt_senha.Text;

        if (string.IsNullOrEmpty(senha))
        {
            txt_senha.BorderColor = Colors.Red;
            error = true;

        }
        else if (senha.Length < 4)
        {
            txt_senha.BorderColor = Colors.Red;
            error = true;
        }
        else
        {
            txt_senha.BorderColor = Colors.Green;
            error = false;
        }
    }
    private async void TapRecuperar_Tapped( object sender, TappedEventArgs e )
    {
        await AppShell.Current.GoToAsync(nameof(VerificarEmail));
    }

}