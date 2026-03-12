using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public partial class DetalhesViewModel : AuthViewModel
    {
        public ObservableCollection<Produtos_Bar>? Produtos { get; } = new();
        public ObservableCollection<Favorito>? Favoritos { get; } = new();
        private readonly AuthService _authService;
        private readonly FavoritosService _favoritosService;
        private int id_produto;


        public DetalhesViewModel(AuthService authService, FavoritosService favoritosService) : base(authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService)); ;
            _favoritosService = favoritosService ?? throw new ArgumentNullException(nameof(favoritosService)); ;
        }

        [ObservableProperty]
        private string? _Btn_Favorito = "not_favorito.png";
        [ObservableProperty]
        private bool _IsRefreshing;
        [ObservableProperty]
        private string? _Imagem;
        [ObservableProperty]
        private string? _Nome;
        [ObservableProperty]
        private bool _IsVisible;
        [ObservableProperty]
        private string? _Descricao;
        [ObservableProperty]
        private decimal _Preco;
        [ObservableProperty]
        private decimal _PrecoTotal;
        [ObservableProperty]
        private int _Quantidade = 1;

        // Sessão atualizar
        public async Task RefreshDataAsync()
        {
            IsRefreshing = true;
            try
            {
                await Task.Run(async () =>
                {
                    await Initialize(id_produto);
                    await GetProdutoDetalhes();
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        // Sessão inicializar
        public async Task Initialize(int Id_produto)
        {
            await Task.Run(async () =>
            {
                id_produto = Id_produto;
                await GetProdutoDetalhes();
                await AtualizarFacoritos();

                int id_utilizador = Preferences.Get("id_utilizador", 0);
                var (favorito, error) = await _authService.GetFavoritoAsync(id_produto);
                if (error != null)
                {
                    await NotificationToast.MostarToast("Ocorreu um erro");
                }
                Btn_Favorito = favorito != null ? "favorito.png" : "not_favorito.png";
            });
        }

        // Sessão obter detalhes do produtos
        private async Task<Produtos_Bar> GetProdutoDetalhes()
        {

            try
            {
                var (produto, ErrorMessage) = await _authService.GetProdutosDetalhes(id_produto);

                if (ErrorMessage == "Unauthorized")
                {
                    await NotificationToast.MostarToast("Sessão Expirada. Por favor, faça login novamente...");
                    _authService.Logout();
                    return null!;
                }
                if (produto is null)
                {
                    await NotificationToast.MostarToast("Produto não encontrado...");
                    return null!;
                }

                if (produto != null)
                {
                    Nome = produto.Nome!;
                    Preco = produto.Preco!.Value;
                    Descricao = produto.Descricao!;
                    Imagem = produto.CaminhoImagem!;
                    PrecoTotal = produto.Preco!.Value;
                }
                else
                {
                    await NotificationToast.MostarToast("Erro ao carregar os detalhes do produto.");
                    return null!;
                }
                return produto;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar produtos: {id_produto} - Message: {ex.Message} ");
                return null!;
            }
        }

        // Sessão obter adicinar ao carrinho
        public async Task AdicionarAoCarrinho(int id_produto)
        {
            try
            {
                var carrinhoItem = new Incluir_Carrinho()
                {
                    Id_produto = id_produto,
                    Quantidade = Quantidade,

                };

                var response = await _authService.AdicionarItemCarinho(carrinhoItem);

                if (response.ErrorMessage != null)
                {
                    await NotificationToast.MostarToast("Falha ao adicionar o produto ao carrinho.");
                }
                if (response.Data)
                {
                    await NotificationToast.MostarToast("Produto adicionado ao carrinho com sucesso 🫡");
                    await _authService.GetCarrinhoAsync();
                }
                else
                {
                    await NotificationToast.MostarToast("Falha ao adicionar o produto ao carrinho.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar ao carrinho: {ex.Message}");
            }
        }

        // Sessão incrementar e calcular
        public void IncrementarQuantidade()
        {
            Quantidade++;
            CalcularPrecoTotal();
        }

        public void DecrementarQuantidade()
        {
            if (Quantidade > 1)
                Quantidade--;
            CalcularPrecoTotal();
        }

        private void CalcularPrecoTotal()
        {
            PrecoTotal = Preco * Quantidade;
        }

        public void ReiniciarDados()
        {
            Favoritos?.Clear();
            id_produto = 0;
            Nome = null;
            Quantidade = 1;
        }

        // Sessão Favoritos 
        public async Task AddFacorito(int id_produto)
        {
            try
            {
                var Id = Preferences.Get("id", string.Empty);
                var Id_utilizador = int.Parse(Id);
                var (favoritos, error) = await _authService.GetFavoritoAsync(id_produto);
                if (error != null)
                {
   
                if (favoritos is not null)
                {
                    await _authService.RemoverFavorito(id_produto);
                }
                else
                {
                        /*  var produtoFavorito = new Favorito()
                          {
                              Id_utilizador = Id_utilizador,
                              Id_produto = id_produto,
                              Isfavorito = true,
                              Nome = Nome,
                              Descricao = Descricao,
                              Preco = Preco,
                              CaminhoImagem = Imagem
                          };*/

                        var New_Favorito = new Favorito()
                        {
                            Id_produto = id_produto
                        };

                    await _authService.AdicionarFavorito(New_Favorito);
                 }
                await AtualizarFacoritos();
                }
                else
                {
                   await NotificationToast.MostarToast("Esse produto ja se encontra na lista de favoritos");
                }

            }
            catch (Exception ex)

            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar produto aos favoritos: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Favorito>> CarregarFavoritos()
        {
            try
            {
                Favoritos?.Clear();

                int id_utilizador = Preferences.Get("id_utilizador", 0);
                var favoritos = await _favoritosService.ReadAllAsync(id_utilizador);


                if (favoritos is null || favoritos.Count == 0)
                {
                    IsVisible = true;
                    return Enumerable.Empty<Favorito>();
                }
                else
                {
                    IsVisible = false;

                    foreach (var favorito in favoritos)
                    {
                        Favoritos?.Add(favorito);
                    }

                    return favoritos;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar  favoritos: {ex.Message}");
                return Enumerable.Empty<Favorito>();
            }
        }

        private async Task AtualizarFacoritos()
        {
            await Task.Run(async () =>
            {
                int id_utilizador = Preferences.Get("id_utilizador", 0);
                var favorito = await _favoritosService.ReadAsync(id_produto, id_utilizador);

                if (favorito is not null)
                {
                    Btn_Favorito = "favorito.png";  // 💖 favoritado
                }
                else
                {
                    Btn_Favorito = "not_favorito.png"; // 💔 desfavoritado
                }
            });
        }

    }
}
