using System.Diagnostics;
using Meal_Card.Models;
using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class Inicio : ContentPage
{
    private readonly DetalhesViewModel _detalhesView;
    private readonly InicioViewModel _inicioView;
    private readonly CarteiraViewModel _carteiraView;
    private AuthService _authService;

    public Inicio(AuthService authService, InicioViewModel inicioView, CarteiraViewModel carteiraView, DetalhesViewModel detalhesView)
    {
        InitializeComponent();
        _authService = authService;
        _inicioView = inicioView;
        _detalhesView = detalhesView;
        _carteiraView = carteiraView;
        BindingContext = _inicioView;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Debug.WriteLine("Inicio.OnAppearing");

        await _inicioView.VerificarECarregarDadosCommand.ExecuteAsync(null);
        _inicioView.OnPageAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _inicioView.OnPageDisappearing();
    }

    private async void Refreshing(object sender, EventArgs e)
    {
        try
        {
            await _inicioView.RefreshDataCommand.ExecuteAsync(null);
        }
        finally
        {
            refreshView.IsRefreshing = false;
        }
    }

    private async void Cv_Categorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsBusy) return;

        var categoria = e.CurrentSelection.FirstOrDefault() as Categoria;
        if (categoria == null) return;

        IsBusy = true;
        try
        {
            await Navigation.PushAsync(new ListaProdutos(
                categoria.Id_categoria,
                categoria.Nome,
                _authService,
                _detalhesView));
        }
        finally
        {
            ((CollectionView)sender).SelectedItem = null;
            IsBusy = false;
        }
    }

    private async void ListarTodos_Tapped(object sender, TappedEventArgs e)
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            await Navigation.PushAsync(new ListaProdutos(
                null,
                string.Empty,
                _authService,
                _detalhesView));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void Cv_Produto_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsBusy) return;

        var produto = e.CurrentSelection.FirstOrDefault() as Produtos_Bar;
        if (produto == null) return;

        IsBusy = true;
        try
        {
            await Navigation.PushAsync(new DetalhesProdutos(
                produto.Id_produto,
                produto.Nome,
                _authService,
                _detalhesView));
        }
        finally
        {
            ((CollectionView)sender).SelectedItem = null;
            IsBusy = false;
        }
    }

    private async void BtnProfile_Clicked(object sender, EventArgs e)
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

    protected override bool OnBackButtonPressed()
    {
        if (AppShell.Current.Navigation.NavigationStack.Count > 1)
        {
            _ = AppShell.Current.GoToAsync("//");
        }
        return true;
    }
}