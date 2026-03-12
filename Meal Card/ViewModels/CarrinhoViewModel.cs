using CommunityToolkit.Mvvm.ComponentModel;
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public partial class CarrinhoViewModel : AuthViewModel
    {
        private ObservableCollection<Itens_Carrinho>? _itensCarrinho = new();
        private readonly AuthService _authService;

        public CarrinhoViewModel(AuthService authService) : base(authService)
        {
            _authService = authService;
            ItensCarrinho = new ObservableCollection<Itens_Carrinho>();
        }

        [ObservableProperty]
        private bool _isRefreshing;
        [ObservableProperty]
        private bool _isVisible = true;
        [ObservableProperty]
        private decimal _valorTotal;

        public ObservableCollection<Itens_Carrinho>? ItensCarrinho
        {
            get => _itensCarrinho;
            set => SetProperty(ref _itensCarrinho, value);
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

                if (ItensCarrinho != null)
                {
                    ItensCarrinho.Clear();

                    foreach (var item in pedidos)
                    {
                        ItensCarrinho.Add(item);
                    }
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
                await NotificationToast.MostarToast("Falha após retries: " + ex.Message);
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
                var total = ItensCarrinho?.Sum(p => p.Preco * p.Quantidade) ?? 0m;

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
                if (ItensCarrinho!.Any())
                {
                    var response = await _authService.CriarPedido();

                    if (!response.HasError && response.Data)
                    {

                        ItensCarrinho?.Clear();
                        ValorTotal = 0;
                        IsVisible = true;

                        await NotificationToast.MostarToast($" Pedido criado com sucesso 🫡 ");
                    }
                    else
                    {
                        await NotificationToast.MostarToast($" Não foi possivel processar o pedido 😑 ");
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

        public async Task IncrementarQuantidade(int id_item)
        {
            try
            {
                var item = ItensCarrinho?.FirstOrDefault(p => p.Id_pedido_itens == id_item);
                if (item != null)
                {
                    item.Quantidade++;
                    AtualizarTotal();
                }

                await _authService.GerenciarCarrinho(id_item, "aumentar");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao incrementar quantidade {ex.Message}");
            }

        }

        public async Task DecrementarQuantidade(int id_item)
        {
            try
            {
                var item = ItensCarrinho?.FirstOrDefault(p => p.Id_pedido_itens == id_item);
                if (item == null) return;

                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                    await _authService.GerenciarCarrinho(id_item, "diminuir");
                }
                else
                {
                    ItensCarrinho!.Remove(item);
                    await _authService.GerenciarCarrinho(id_item, "eliminar");
                }

                AtualizarTotal();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao decrementar quantidade {ex.Message}");
            }

        }

        public async Task EliminarQuantidade(int id_item)
        {
            try
            {
                await _authService.GerenciarCarrinho(id_item, "eliminar");
                AtualizarTotal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao eliminar produto {ex.Message}");
            }

        }

        public async Task EsvaziarCarrinhoAsync()
        {
            try
            {
                await _authService.EsvaziarCarrinhoAsync();
                AtualizarTotal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esvaziar carrinho {ex.Message}");
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

    }
}
