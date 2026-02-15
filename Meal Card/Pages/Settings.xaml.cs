using CommunityToolkit.Maui.Alerts;
using Meal_Card.Controls;
using Meal_Card.Services;
using Meal_Card.ViewModels;
using System.Runtime.CompilerServices;

namespace Meal_Card.Pages;

public partial class Settings : ContentPage
{
    public readonly CarteiraViewModel _carteiraView;
    private readonly AuthService _authService;
    private bool isDataLoaded = false;
    public Settings( AuthService authService, CarteiraViewModel carteiraView )
    {
        InitializeComponent();
        _authService = authService;
        _carteiraView = carteiraView;
        BindingContext = carteiraView;
        //var currentTheme = Application.Current!.RequestedTheme;
        //ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
    }

    private async void Refreshing( object sender, EventArgs e )
    {
        await LoadData();
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
        var horas = DataControllerUser.DataAppUser();
        hora.Text = horas;
        var dados = _carteiraView.CarregarDados();
        await Task.WhenAll(dados);

    }

    private async void Logout_Tapped( object sender, TappedEventArgs e )
    {

        bool confirmacao = await DisplayAlert("Terminar Sessão",
                 "Tens certeza que deseja terminar a sessão ?",
                                                "Sim", "Não");

        if (confirmacao)
        {

            _authService.Logout();
            //Application.Current!.MainPage = new NavigationPage(new Login())
            var notification = Toast.Make("Sessão encerada com sucesso...",
                CommunityToolkit.Maui.Core.ToastDuration.Short);
            await notification.Show();

        }
    }

    /*   public static async Task DisplaySnackbarAsync(string message)
       {
           CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

           var snackbarOptions = new SnackbarOptions
           {
               BackgroundColor = Color.FromArgb("#FF3300"),
               TextColor = Colors.White,
               ActionButtonTextColor = Colors.Yellow,
               CornerRadius = new CornerRadius(0),
               Font = Font.SystemFontOfSize(18),
               ActionButtonFont = Font.SystemFontOfSize(14)
           };

           var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

           await snackbar.Show(cancellationTokenSource.Token);
       }

       public static async Task DisplayToastAsync(string message)
       {
           // Toast is currently not working in MCT on Windows
           if (OperatingSystem.IsWindows())
               return;

           var toast = Toast.Make(message, textSize: 18);

           var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
           await toast.Show(cts.Token);
       }

       private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
       {


           Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
           /* AppTheme selectedTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
            //  Aplica o tema imediatamente
             App.Current.UserAppTheme = selectedTheme;
           //  Salva a preferência do usuário para a próxima vez que o app for aberto
           string themeString = selectedTheme.ToString();
           Preferences.Set("UserAppTheme", themeString);
       }*/

    private async void TapInformações_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(ContaInfo));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void TapAlterSenha_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync(nameof(AlterarSenha));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void TapNotificacao_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {

            await AppShell.Current.GoToAsync(nameof(Notification));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await Navigation.PopAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }
    protected override void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        base.OnPropertyChanged(propertyName);
    }
}