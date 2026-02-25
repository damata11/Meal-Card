using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public class CarrinhoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Itens_Carrinho>? _itensCarrinho = new();
        private readonly AuthService _authService;

        public CarrinhoViewModel(AuthService authService)
        {
            _authService = authService;
        }

        private bool _isRefreshing;
        private bool _isVisible = true;
        private decimal _valorTotal;

        public ObservableCollection<Itens_Carrinho>? Carrinho
        {
            get => _itensCarrinho;
            set
            {
                if (_itensCarrinho != value)
                {
                    _itensCarrinho = value;
                    OnPropertyChanged(nameof(Carrinho));
                }
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
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

        public decimal ValorTotal
        {
            get => _valorTotal;
            set
            {
                if (_valorTotal != value)
                {
                    _valorTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task Initialize()
        {
            try
            {
                await GetCarrinho();
            }
            catch (Exception)
            {

                System.Diagnostics.Debug.WriteLine(" Erro ao carregar carrinho ");
            }
        }

        public async Task RefreshDataAsync()
        {
            IsRefreshing = true;
            try
            {
                await GetCarrinho();
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

        private const int MaxRetries = 3;
        public async Task<IEnumerable<Itens_Carrinho>> GetCarrinho(int retryCount = 0)
        {
            try
            {

                var (pedidos, ErrorMessage) = await CallServiceWithTimeout(() => _authService.GetCarrinhoAsync());

                if (ErrorMessage != null || pedidos is null || !pedidos.Any())
                {
                    IsVisible = true;
                    return Enumerable.Empty<Itens_Carrinho>();
                }

                IsVisible = false;
                Carrinho?.Clear();

                foreach (var item in pedidos)
                {
                    Carrinho?.Add(item);
                }

                AtualizarTotal();
                return pedidos;

            }
            catch (TimeoutException)
            {

                return await RetryGetCarrinho(3);
            }
            catch (Exception ex)
            {
                if (retryCount < MaxRetries)
                {
                    await Task.Delay(1000);
                    return await GetCarrinho(retryCount + 1);
                }
                await NotificationToast.ShowToastL("Falha após retries: " + ex.Message);
                return Enumerable.Empty<Itens_Carrinho>();
            }
        }

        private async Task<IEnumerable<Itens_Carrinho>> RetryGetCarrinho(int maxRetries)
        {
            for (int i = 1; i <= maxRetries; i++)
            {
                try { return await GetCarrinho(); }
                catch { await Task.Delay(1000 * i); }
            }
            System.Diagnostics.Debug.WriteLine("Falha após retries. Verifique conexão.");
            IsVisible = true;
            return Enumerable.Empty<Itens_Carrinho>();
        }


        public void AtualizarTotal()
        {
            try
            {
                var total = Carrinho?.Sum(p => p.Preco * p.Quantidade) ?? 0m;

                if (ValorTotal != total)
                {
                    ValorTotal = total;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERRO] Ao calcular total: {ex.Message}");

            }
        }

        public async Task FinalizarCompra()
        {
            try
            {
                if (Carrinho!.Any())
                {
                    var response = await _authService.CriarPedido();

                    if (!response.HasError && response.Data)
                    {
                        await Initialize();
                        await NotificationToast.ShowToastL($" Pedido criado com sucesso 🫡 ");
                        IsVisible = true;
                    }
                    else
                    {
                        await NotificationToast.ShowToastL($" Não foi possivel processar o pedido 😑 ");
                    }
                }
                else
                {
                    await AppShell.Current.DisplayAlert("Carrinho Vazio", "O carrinho esta vazio volte experimente adicionar algo...", "OK");
                    await AppShell.Current.GoToAsync("//home");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao finalizar o pedido: {ex.Message}");
            }
        }

        public async Task IncrementarQuantidade(int id_CarrinhoItem)
        {
            try
            {
                var item = Carrinho?.FirstOrDefault(p => p.Id_pedido_itens == id_CarrinhoItem);
                if (item != null)
                {
                    item.Quantidade++;
                    AtualizarTotal();
                }

                await _authService.GerenciarCarrinho(id_CarrinhoItem, "aumentar");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao incrementar quantidade {ex.Message}");
            }

        }

        public async Task DecrementarQuantidade(int id_CarrinhoItem)
        {
            try
            {
                var item = Carrinho?.FirstOrDefault(p => p.Id_pedido_itens == id_CarrinhoItem);
                if (item == null) return;

                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                    await _authService.GerenciarCarrinho(id_CarrinhoItem, "diminuir");
                }
                else
                {
                    Carrinho!.Remove(item);
                    await _authService.GerenciarCarrinho(id_CarrinhoItem, "eliminar");
                }

                AtualizarTotal();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao decrementar quantidade {ex.Message}");
            }

        }

        public async Task EliminarQuantidade(int id_CarrinhoItem)
        {
            try
            {
                await _authService.GerenciarCarrinho(id_CarrinhoItem, "eliminar");
                AtualizarTotal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao eliminar produto {ex.Message}");
            }

        }

        public async Task<(T, string?)> CallServiceWithTimeout<T>(Func<Task<(T, string?)>> serviceCall, int timeoutMs = 5000, int maxRetries = 3)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var timeoutTask = Task.Delay(timeoutMs);
                    var resultTask = serviceCall();
                    var completed = await Task.WhenAny(resultTask, timeoutTask);
                    if (completed == timeoutTask)
                    {
                        throw new TimeoutException("Tempo esgotado.");
                    }
                    return await resultTask;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Tentativa {retry + 1} falhou: {ex.Message}");
                    if (retry == maxRetries - 1) throw;
                    await Task.Delay(1000 * (retry + 1));
                }
            }
            return (default!, null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
