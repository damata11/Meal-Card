using Meal_Card.Models;
using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class Favoritos : ContentPage
{
    private readonly DetalhesViewModel _detalhesView;
    private readonly AuthService _authService;
    private readonly FavoritosService _favoritosService;
    private bool isDataLoaded = false;

    public Favoritos( DetalhesViewModel detalhesView, AuthService authService, FavoritosService favoritosService )
    {
        InitializeComponent();
        _detalhesView = detalhesView;
        BindingContext = _detalhesView;
        _authService = authService;
        _favoritosService = favoritosService;
    }

    private async void Refreshing( object sender, EventArgs e )
    {
        await _detalhesView.CarregarFavoritos();
        refreshView.IsRefreshing = false;
    }
    protected override async void OnAppearing()
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
        var dados = _detalhesView.CarregarFavoritos();

        await Task.WhenAll(dados);
    }

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

    private async void Cv_Favorito_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {

            var currentSelection = e.CurrentSelection.FirstOrDefault() as Favorito;
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar  favoritos: {e.CurrentSelection.FirstOrDefault() as Produtos_Bar}");

            if (currentSelection is null) return;

            await Navigation.PushAsync(new DetalhesProdutos(currentSelection.Id_produto, currentSelection.Nome, _authService, _detalhesView));

            ((CollectionView)sender).SelectedItem = null;

        }
        finally
        {
            IsBusy = false;
        }
    }
}