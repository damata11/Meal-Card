using Meal_Card.Controls;
using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class DetalhesProdutos : ContentPage
{
    private readonly DetalhesViewModel _detalhesView;
    private readonly AuthService _authService;
    private int id_produto;
    private string? nome_produto;
    public DetalhesProdutos( int Id_produto, string? Nome_produto, AuthService authService, DetalhesViewModel detalhesView )
    {
        InitializeComponent();
        id_produto = Id_produto;
        nome_produto = Nome_produto;
        _detalhesView = detalhesView;
        BindingContext = _detalhesView;
        _authService = authService ?? throw new ArgumentNullException(nameof(authService)); ;
        SetTitle();
        System.Diagnostics.Debug.WriteLine($"ID do produto: {id_produto}");
        System.Diagnostics.Debug.WriteLine($"Nome do produto: {nome_produto}");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (id_produto > 0)
        {
            try
            {
                await _detalhesView.Initialize(id_produto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar produto: {ex.Message}");
                await NotificationToast.ShowToastL($"Erro ao carregar produto: {ex.Message}");
            }
        }
        else
        {
            await NotificationToast.ShowToastL("Produto não encontrado...");
            return;
        }
    }

    private void SetTitle()
    {
        if (id_produto < 0)
        {
            TitlePage.Text = "Detalhes do Produto";
        }
        else
        {
            TitlePage.Text = nome_produto;
        }
    }

    private async void AdicionarCarrinho_Clicked( object sender, EventArgs e )
    {
        await _detalhesView.AdicionarAoCarrinho();
    }

    private async void SelectFavorito_Clicked( object sender, EventArgs e )
    {
        await _detalhesView.AddFacorito();
    }

    private async void BtnRemove_Clicked( object sender, EventArgs e )
    {
        _detalhesView.DecrementarQuantidade();
    }

    private void BtnAdicionar_Clicked( object sender, EventArgs e )
    {
        _detalhesView.IncrementarQuantidade();
    }

    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            _detalhesView.ReiniciarDados();
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync(animated: false);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }



    private async void Refreshing( object sender, EventArgs e )
    {
        await _detalhesView.RefreshDataAsync();
        refreshView.IsRefreshing = false;
    }
}