using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class Cantina : ContentPage
{
    private readonly InicioViewModel _inicioView;
    private readonly CantinaViewModel _cantinaView;
    private readonly CarteiraViewModel _carteiraView;
    private readonly DetalhesViewModel _detalhesViwe;
    private AuthService _authService;

    public Cantina( AuthService authService, InicioViewModel inicioView, CarteiraViewModel carteiraView, CantinaViewModel cantinaView, DetalhesViewModel detalhesViwe )
    {
        InitializeComponent();
        _authService = authService;
        _inicioView = inicioView;
        _carteiraView = carteiraView;
        _cantinaView = cantinaView;
        BindingContext = _cantinaView;
        _detalhesViwe = detalhesViwe;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
    private void Refreshing( object sender, EventArgs e )
    {

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
    /*
    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await Navigation.PopAsync(animated: false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Btnhistorico_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            AppShell.Current.GoToAsync(nameof(Historico));
        }
        finally
        {
            IsBusy = false;
        }
    }



       private async Task GetRefeições()
       {
           // Lógica para obter refeições usando _apiService
       }

       private async Task<IEnumerable<Categoria>> GetListaCategorias()
       {
            try
            {
                var (categorias, errorMessage) = await _authService.GetCategorias("cantina", string.Empty);

                if (errorMessage == "Unauthorized" && !_loginPageDisplay)
                {
                    await DisplayLoginPage();
                    return Enumerable.Empty<Categoria>();
                }

                if(categorias = null)
                {
                    // mostrar mensagem de erro no ecra
                    return Enumerable.Empty<Categoria>();
                }

                Categorias.ItemsSource = categorias;
                return categorias;
            }
            catch (Exception ex)
            {
                // mostrar mensagem de erro no ecra
                return Enumerable.Empty<Categoria>();
            }
       } */


}