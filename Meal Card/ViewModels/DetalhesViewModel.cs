using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public class DetalhesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Produtos_Bar>? Produtos { get; } = new();
        public ObservableCollection<Favorito>? Favoritos { get; } = new();
        private readonly AuthService _authService;
        private readonly FavoritosService _favoritosService;
        private int id_produto;
        private int _quantidade = 1;

        public DetalhesViewModel( AuthService authService, FavoritosService favoritosService )
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService)); ;
            _favoritosService = favoritosService ?? throw new ArgumentNullException(nameof(favoritosService)); ;
        }

        private string? _btnFavorito = "not_favorito.png";
        private bool _isRefreshing;
        private string? _imagem;
        private string? _nome;
        private bool _isVisible;
        private string? _descricao;
        private decimal _preco;
        private decimal _precototal;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public string? Imagem
        {
            get => _imagem;
            set
            {
                if (_imagem != value)
                {
                    _imagem = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Btn_favorito
        {
            get => _btnFavorito;
            set
            {
                if (_btnFavorito != value)
                {
                    _btnFavorito = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Nome
        {
            get => _nome!;
            set
            {
                if (Nome != value)
                {
                    _nome = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Descricao
        {
            get => _descricao!;
            set
            {
                if (_descricao != value)
                {
                    _descricao = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Preco
        {
            get => _preco;
            set
            {
                if (_preco != value)
                {
                    _preco = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal PrecoTotal
        {
            get => _precototal;
            set
            {
                _precototal = value;
                OnPropertyChanged();
            }
        }

        public int Quantidade
        {
            get => _quantidade;
            set
            {
                _quantidade = value;
                OnPropertyChanged();
                CalcularPrecoTotal();
            }
        }

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
        public async Task Initialize( int Id_produto )
        {
            await Task.Run(async () =>
            {
                id_produto = Id_produto;
                await GetProdutoDetalhes();
                await AtualizarFacoritos();

                int id_utilizador = Preferences.Get("id_utilizador", 0);
                var favorito = await _favoritosService.ReadAsync(id_produto, id_utilizador);
                Btn_favorito = favorito != null ? "favorito.png" : "not_favorito.png";
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
                    await NotificationToast.ShowToastL(" Sessão Expirada. Por favor, faça login novamente...");
                    _authService.Logout();
                    return null!;
                }
                if (produto is null)
                {
                    await NotificationToast.ShowToastL(" Produto não encontrado...");
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
                    await NotificationToast.ShowToastL("Erro ao carregar os detalhes do produto.");
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
        public async Task AdicionarAoCarrinho()
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
                    await NotificationToast.ShowToastL(" Falha ao adicionar o produto ao carrinho.");
                }
                if (response.Data)
                {
                    await NotificationToast.ShowToastS(" Produto adicionado ao carrinho com sucesso 🫡");
                    await _authService.GetCarrinhoAsync();
                }
                else
                {
                    await NotificationToast.ShowToastL(" Falha ao adicionar o produto ao carrinho.");
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
        }

        public void DecrementarQuantidade()
        {
            if (Quantidade > 1)
                Quantidade--;
        }

        private void CalcularPrecoTotal()
        {
            PrecoTotal = Preco * Quantidade;
        }

        public void ReiniciarDados()
        {
            Quantidade = 1;
        }

        // Sessão Favoritos 
        public async Task AddFacorito()
        {
            try
            {
                int id_utilizador = Preferences.Get("id_utilizador", 0);
                var favoritos = await _favoritosService.ReadAsync(id_produto, id_utilizador);

                if (favoritos is not null)
                {
                    await _favoritosService.DeleteAsync(favoritos);
                }
                else
                {
                    var produtoFavorito = new Favorito()
                    {
                        Id_utilizador = id_utilizador,
                        Id_produto = id_produto,
                        Isfavorito = true,
                        Nome = Nome,
                        Descricao = Descricao,
                        Preco = Preco,
                        CaminhoImagem = Imagem
                    };
                    await _favoritosService.CreateAsync(produtoFavorito);
                }
                await AtualizarFacoritos();
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
                    Btn_favorito = "favorito.png";  // 💖 favoritado
                }
                else
                {
                    Btn_favorito = "not_favorito.png"; // 💔 desfavoritado
                }
            });
        }

        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null! )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
