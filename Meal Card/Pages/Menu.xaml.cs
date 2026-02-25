using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class Menu : ContentPage
{
    public readonly AuthService _authService;
    public readonly CarteiraViewModel _viewModel;
    private bool isDataLoaded = false;
    //private bool _loginPadeDesabled = false;
    public Menu( CarteiraViewModel viewModel, AuthService authService )
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        BindingContext = _viewModel;
    }

    private async void Refreshing( object sender, EventArgs e )
    {
        refreshView.IsRefreshing = true;
        await _viewModel.CarregarMenu();
        refreshView.IsRefreshing = false;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        if (!isDataLoaded)
        {
            await LoadData();
            isDataLoaded = true;
        }
    }
    private async Task LoadData()
    {
        var dados = _viewModel.CarregarPerfil();

        await Task.WhenAll(dados);
    }

    protected override bool OnBackButtonPressed()
    {
        if (AppShell.Current.Navigation.NavigationStack.Count > 1)
        {
            AppShell.Current.GoToAsync("//");
        }
        else
        {
            //Application.Current?.Quit();
        }
        return true;
    }

    /*   private async Task CarregarDadosUsuario()
       {
           try
           {

               string? nome =  Preferences.Get("nome", "Danilson");
               string? sobrenome = Preferences.Get("sobrenome", "da Mata");
               string? email = Preferences.Get("email", "danilsonmata@gmail.com");
               lbl_Nome.Text = nome + " " + sobrenome;
               lbl_Email.Text = email;

           }
           catch (Exception ex)
           {
               var message = ex.Message;
           }
       }*/


    private async void TapReportarBug_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (AppShell.Current != null)
            {
                await AppShell.Current.GoToAsync(nameof(Reportar));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void TapRefeicao_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;
            await AppShell.Current.GoToAsync(nameof(Calendario));

        }
        finally
        {

            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
            IsBusy = false;
        }

    }

    private async void Activict_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {

            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;
            await AppShell.Current.GoToAsync(nameof(Historico));
        }
        finally
        {
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
            IsBusy = false;
        }
    }

    private async void Settings_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(Settings));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void TapSobre_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(Sobre));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void GoProfile_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(Settings));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void TapContacto_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(Contacto));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void Favoritos_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(Favoritos));
        }
        finally
        {
            IsBusy = false;
        }
    }
}