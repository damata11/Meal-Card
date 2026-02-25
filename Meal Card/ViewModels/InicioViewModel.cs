using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Meal_Card.ViewModels
{
    public partial class InicioViewModel : ObservableObject, IDisposable
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private ObservableCollection<InicioCarosselList> _carosselItems = new();

        [ObservableProperty]
        private ObservableCollection<Categoria> _categorias = new();

        [ObservableProperty]
        private ObservableCollection<Produtos_Bar> _produtosPlus = new();

        [ObservableProperty]
        private ObservableCollection<Produtos_Bar> _produtosPopulares = new();

        [ObservableProperty]
        private int _currentPosition;

        [ObservableProperty]
        private bool _isAutoSlideEnabled = true;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isVisible;

        private string? _currentUserId;
        private bool _hasLoadedOnce = false;
        private IDispatcherTimer? _timer;

        public InicioViewModel(AuthService authService)
        {
            _authService = authService;
            InicializarCarrossel();
        }

        private void InicializarCarrossel()
        {
            var items = new List<InicioCarosselList>
            {
                new() { Titulo = "Bem-vindo", Descrição = "Descubra as novidades", Imagem = "img_slide1" },
                new() { Titulo = "Promoções", Descrição = "Os melhores preços", Imagem = "img_slide2" },
                new() { Titulo = "Rápido", Descrição = "Pedido em minutos", Imagem = "img_slide3" },
                new() { Titulo = "Fácil", Descrição = "Pague com seu cartão", Imagem = "img_slide4" }
            };

            CarosselItems = new ObservableCollection<InicioCarosselList>(items);
        }

        [RelayCommand]
        private async Task VerificarECarregarDadosAsync()
        {
            var userId = Preferences.Get("user_id", string.Empty);
            var isNewUser = userId != _currentUserId;

            Debug.WriteLine($"Verificando: UserID atual={userId}, anterior={_currentUserId}, isNewUser={isNewUser}, hasLoaded={_hasLoadedOnce}");

            if (!_hasLoadedOnce || isNewUser)
            {
                Debug.WriteLine("Carregando dados da API...");
                await CarregarDadosApiAsync();

                _currentUserId = userId;
                _hasLoadedOnce = true;
            }
            else
            {
                Debug.WriteLine("Dados já carregados para este usuário, pulando...");
            }

            IniciarTimer();
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            Debug.WriteLine("Pull-to-refresh: Forçando atualização...");
            await CarregarDadosApiAsync();
        }

        public void ResetarParaNovoLogin()
        {
            Debug.WriteLine("Resetando ViewModel para novo login");

            Categorias.Clear();
            ProdutosPlus.Clear();
            ProdutosPopulares.Clear();

            _hasLoadedOnce = false;
            _currentUserId = null;

            PararTimer();
        }

        private async Task CarregarDadosApiAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            IsRefreshing = true;

            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Categorias.Clear();
                    ProdutosPlus.Clear();
                    ProdutosPopulares.Clear();
                });

                await Task.WhenAll(
                    CarregarCategoriasAsync(),
                    CarregarProdutosPlusAsync(),
                    CarregarProdutosPopularesAsync()
                );

                Debug.WriteLine("Dados carregados com sucesso");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar: {ex.Message}");
                await NotificationToast.ShowToastL("Erro ao carregar dados");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        private async Task CarregarCategoriasAsync()
        {
            var (dados, erro) = await _authService.GetCategorias("bar");

            if (erro == "Unauthorized")
            {
                await HandleUnauthorizedAsync();
                return;
            }

            if (dados?.Any() == true)
            {
                var lista = new ObservableCollection<Categoria>(dados);
                MainThread.BeginInvokeOnMainThread(() => Categorias = lista);
            }
        }

        private async Task CarregarProdutosPlusAsync()
        {
            var (dados, erro) = await _authService.GetProdutosBar("plus", string.Empty);

            if (erro == "Unauthorized") return;

            if (dados?.Any() == true)
            {
                var lista = new ObservableCollection<Produtos_Bar>(dados);
                MainThread.BeginInvokeOnMainThread(() => ProdutosPlus = lista);
            }
        }

        private async Task CarregarProdutosPopularesAsync()
        {
            var (dados, erro) = await _authService.GetProdutosBar("popular", string.Empty);

            if (erro == "Unauthorized") return;

            if (dados?.Any() == true)
            {
                var lista = new ObservableCollection<Produtos_Bar>(dados);
                MainThread.BeginInvokeOnMainThread(() => ProdutosPopulares = lista);
            }
        }

        private async Task HandleUnauthorizedAsync()
        {
            await NotificationToast.ShowToastL("Sessão expirada. Faça login novamente.");
            _authService.Logout();
        }

        public void OnPageAppearing()
        {
            IniciarTimer();
        }

        public void OnPageDisappearing()
        {
            PararTimer();
        }

        private void IniciarTimer()
        {
            if (CarosselItems.Count <= 1) return;
            if (_timer?.IsRunning == true) return;

            if (_timer == null && Application.Current?.Dispatcher != null)
            {
                _timer = Application.Current.Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromSeconds(5);
                _timer.Tick += OnTimerTick;
            }

            _timer?.Start();
            Debug.WriteLine("Timer iniciado");
        }

        private void PararTimer()
        {
            _timer?.Stop();
            Debug.WriteLine("Timer parado");
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!IsAutoSlideEnabled || CarosselItems.Count <= 1) return;
                CurrentPosition = (CurrentPosition + 1) % CarosselItems.Count;
            });
        }

        public void Dispose()
        {
            PararTimer();
            if (_timer != null)
            {
                _timer.Tick -= OnTimerTick;
                _timer = null;
            }
        }
    }
}