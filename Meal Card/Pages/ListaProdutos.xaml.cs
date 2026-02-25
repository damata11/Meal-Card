using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using Meal_Card.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Meal_Card.Pages;

public partial class ListaProdutos : ContentPage
{
    private readonly DetalhesViewModel _detalhesView;
    private ObservableCollection<Produtos_Bar> listaProdutos;
    private readonly AuthService _authService;
    private int? id_categoria;
    private string? nome;

    public ListaProdutos( int? Id_categoria, string? Nome, AuthService authService, DetalhesViewModel detalhesView )
    {
        InitializeComponent();
        listaProdutos = new ObservableCollection<Produtos_Bar>();
        _detalhesView = detalhesView;
        _authService = authService;
        id_categoria = Id_categoria;
        nome = Nome ?? "Produtos";
        SetTitle();
    }

    private async void Refreshing( object sender, EventArgs e )
    {
        refreshView.IsRefreshing = true;
        SetTitle();
        if (!id_categoria.HasValue)
        {
            await GetListaProdutos();
        }
        else
        {
            await GetListaProdutos(id_categoria.Value);
        }
        refreshView.IsRefreshing = false;
    }

    private void SetTitle()
    {
        if (!id_categoria.HasValue)
        {
            TitlePage.Text = "Todos Produtos";
        }
        else
        {
            TitlePage.Text = "Categoria " + nome;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (!id_categoria.HasValue)
            {
                await GetListaProdutos();
            }
            else
            {
                await GetListaProdutos(id_categoria.Value);
            }
        }
        catch(Exception ex)
        {
          await  NotificationToast.ShowToastS($"Erro inesperado {ex.Message}");
        }

    }

    private async Task<IEnumerable<Produtos_Bar>> GetListaProdutos()
    {

        try
        {
            var (produtos, ErrorMessage) = await _authService.GetAllProdutosBar();

            if (ErrorMessage == "Unauthorized")
            {
                await NotificationToast.ShowToastL("Sessão Expirada. Por favor, faça login novamente...");
                _authService.Logout();
                return Enumerable.Empty<Produtos_Bar>();
            }

            if (produtos is null || !produtos.Any())
            {
                return Enumerable.Empty<Produtos_Bar>();
            }

            Cv_Produtos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao obter produtos mais vendidos: {ex.Message}");
            return Enumerable.Empty<Produtos_Bar>();
        }

    }

    private async Task<IEnumerable<Produtos_Bar>> GetListaProdutos( int id_categoria )
    {


        try
        {
            var (produtos, ErrorMessage) = await _authService.GetProdutosBar("categoria", id_categoria.ToString());

            if (ErrorMessage == "Unauthorized")
            {
                await NotificationToast.ShowToastL("Sessão Expirada. Por favor, faça login novamente...");
                _authService.Logout();
                return Enumerable.Empty<Produtos_Bar>();
            }

            if (produtos is null || !produtos.Any())
            {
                return Enumerable.Empty<Produtos_Bar>();
            }

            Cv_Produtos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao obter produtos mais vendidos: {ex.Message}");
            return Enumerable.Empty<Produtos_Bar>();
        }

    }

    private async void Cv_Produto_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {

            var currentSelection = e.CurrentSelection.FirstOrDefault() as Produtos_Bar;

            if (currentSelection is null) return;

            await Navigation.PushAsync(new DetalhesProdutos(currentSelection.Id_produto, currentSelection.Nome, _authService, _detalhesView));

            ((CollectionView)sender).SelectedItem = null;

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
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }


    private async void Sb_nome_TextChanged( object sender, TextChangedEventArgs e )
    {

        string nome = Sb_nome.Text;
        await Task.Delay(300); // Debounce para evitar muitas consultas
        if (IsBusy) return;
        IsBusy = true;
        try
        {

            var produtosFiltrados = await BuscarProduto(nome, id_categoria);
            Cv_Produtos.ItemsSource = produtosFiltrados;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<List<Produtos_Bar>> BuscarProduto( string nome, int? id_categoria = null )
    {
        try
        {

            if (listaProdutos == null || !listaProdutos.Any())
            {
                Debug.WriteLine("⚠️ listaProdutos está vazia! Carregue os produtos primeiro.");
                return new List<Produtos_Bar>(); // Retorna lista vazia
            }

            IEnumerable<Produtos_Bar> query = listaProdutos;

            if (id_categoria.HasValue && id_categoria.Value > 0)
            {
                query = query.Where(p => p.Id_categoria == id_categoria.Value);
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(p => p.Nome != null &&
                                        p.Nome.ToLower().Contains(nome.ToLower()));
            }

            var produtosFiltrados = query.OrderBy(p => p.Nome).ToList();
            return produtosFiltrados;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao buscar produtos: {ex.Message}");
            return new List<Produtos_Bar>();
        }

    }

    private void BtnAdicionar_Clicked(object sender, EventArgs e)
    {
    }
}