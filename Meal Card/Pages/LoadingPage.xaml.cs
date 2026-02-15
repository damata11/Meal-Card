using Meal_Card.Services;
using Meal_Card.ViewModels;
using Syncfusion.Maui.Toolkit.Charts;

namespace Meal_Card.Pages;

public partial class LoadingPage : ContentPage
{
    private readonly InicioViewModel _inicioView;
    private readonly CarteiraViewModel _carteiraView;
    private readonly CantinaViewModel _cantinaView;
    private readonly CarrinhoViewModel _carrinhoView;
    private readonly DetalhesViewModel _detalhesView;
    private readonly AuthService _authService;
    public LoadingPage( AuthService authService, InicioViewModel inicioView, CarteiraViewModel carteiraView, CantinaViewModel cantinaView, DetalhesViewModel detalhesView, CarrinhoViewModel carrinhoView )
    {
        InitializeComponent();
        _authService = authService;
        _inicioView = inicioView;
        _carteiraView = carteiraView;
        _cantinaView = cantinaView;
        _carrinhoView = carrinhoView;
        _detalhesView = detalhesView;
        app_version.Text = AppInfo.VersionString;
        this.BackgroundColor = Application.Current?.UserAppTheme == AppTheme.Dark
          ? Colors.Black
          : Colors.White;
    }


    protected async override void OnNavigatedTo( NavigatedToEventArgs args )
    {
        base.OnNavigatedTo(args);

        await Task.Delay(3000);

        try
        {
            // Verificar token armazenado
            var accessToken = await SecureStorage.Default.GetAsync("accesstoken");

            if (string.IsNullOrEmpty(accessToken))
            {
                // se não existir token - navegar para Login
                await AppShell.Current.GoToAsync($"{nameof(Login)}");
            }
            else
            {
                // Com token - manter na página atual (navegação já configurada)
                // A LoadingPage deve desaparecer e mostrar o conteúdo principal
                await AppShell.Current.GoToAsync(state: "//Home");
                //var window = Application.Current?.Windows[0];
                //window!.Page = new AppShell(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView, _carrinhoView);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert($"Sair do Aplicação", ex.Message, "Sim", "Não");
            System.Diagnostics.Debug.WriteLine($"LoadingPage Error: {ex.Message}");
            await AppShell.Current.GoToAsync($"{nameof(Login)}");
        }
    }

    /*   protected async override void OnAppearing()
      {
          base.OnAppearing();

          try
          {
              // Verificar token armazenado
              var accessToken = await SecureStorage.Default.GetAsync("accesstoken");

              if (string.IsNullOrEmpty(accessToken))
              {
                  // se não existir token - navegar para Login
                  await AppShell.Current.GoToAsync($"{nameof(Login)}");
              }
              else
              {
                  // Com token - manter na página atual (navegação já configurada)
                  // A LoadingPage deve desaparecer e mostrar o conteúdo principal
                  // await AppShell.Current.GoToAsync(state: "//Home");
                  Application.Current.MainPage = new AppShell(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView);
                  //App.Current.Go
                  //await AppShell.Current.GoToAsync($"{nameof(Inicio)}");
              }
          }
          catch (Exception ex)
          {
              await DisplayAlert($"Sair do Aplicação", ex.Message, "Sim", "Não");
              System.Diagnostics.Debug.WriteLine($"LoadingPage Error: {ex.Message}");
              await AppShell.Current.GoToAsync($"{nameof(Login)}");
          }

      }*/
}

