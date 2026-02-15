using Meal_Card.Models;
using Meal_Card.Services;
using Meal_Card.ViewModels;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Meal_Card.Pages;

public partial class Carrinho : ContentPage
{
    private readonly DetalhesViewModel _detalhesView;
    private readonly AuthService _authService;
    private readonly CarrinhoViewModel _carrinhoView;
    private bool isDataLoaded = false;

    public Carrinho( AuthService authService, CarrinhoViewModel carrinhoView, DetalhesViewModel detalhesView )
    {
        InitializeComponent();
        _detalhesView = detalhesView;
        _authService = authService;
        _carrinhoView = carrinhoView;
        BindingContext = _carrinhoView;
    }

    public int id_produto;
    public string? nome;
    private async void Refreshing( object sender, EventArgs e )
    {
        await _carrinhoView.RefreshDataAsync();
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
        var carrinho = _carrinhoView.Initialize();

        await Task.WhenAll(carrinho);
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

    private async void BtnIncrementar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (sender is Button button && button.BindingContext is Itens_Carrinho item)
            {
                await _carrinhoView.IncrementarQuantidade(item.Id_pedido_itens);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void BtnDecrementar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
                if (sender is Button button && button.BindingContext is Itens_Carrinho item)
                {
                    await _carrinhoView.DecrementarQuantidade(item.Id_pedido_itens);
                }
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async void BtnApagar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {

                if (sender is Button button && button.BindingContext is Itens_Carrinho item)
                {
                    await _carrinhoView.EliminarQuantidade(item.Id_pedido_itens);
                }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void BtnFinalizarCompra_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await Task.Run(async () =>
            {
                await _carrinhoView.FinalizarCompra();
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BtnApagarTudo_Clicked( object sender, EventArgs e )
    {

    }

    private void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (AppShell.Current.Navigation.NavigationStack.Count > 1)
            {
                Navigation.PopAsync();
            }
            else
            {
                //Application.Current?.Quit();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void Btnfavoritos_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await AppShell.Current.GoToAsync($"{nameof(Favoritos)}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void DetalhesTapped_Tapped( object sender, TappedEventArgs e )
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await Task.Run(async () =>
            {
                if (sender is Border border)
                {
                    if (border.BindingContext is Itens_Carrinho produtoSelecionado)
                    {

                        await Navigation.PushAsync(new DetalhesProdutos(produtoSelecionado.Id_produto, produtoSelecionado.Nome, _authService, _detalhesView));

                    }
                }
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao navegar para detalhes: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

}