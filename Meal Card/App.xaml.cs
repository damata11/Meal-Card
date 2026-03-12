using Meal_Card.Services;
using Meal_Card.ViewModels;
using System.Timers;

namespace Meal_Card
{
    public partial class App : Application
    {
        private System.Timers.Timer? _sessionTimer;
        private readonly SemaphoreSlim _checkLock = new(1, 1);
        private bool _isSessionValid = true;
        private readonly AuthViewModel _authView;
        private readonly AuthService _authService;
        private readonly InicioViewModel _inicioView;
        private readonly CantinaViewModel _cantinaView;
        private readonly CarteiraViewModel _carteiraView;
        private readonly DetalhesViewModel _detalhesView;
        private readonly ProdutosViewModel _produtosView;
        private readonly CarrinhoViewModel _carrinhoView;

        public App(AuthService authService, InicioViewModel inicioView, CarteiraViewModel carteiraView, CantinaViewModel cantinaView, DetalhesViewModel detalhesView, CarrinhoViewModel carrinhoView, AuthViewModel authView, ProdutosViewModel produtosView)
        {
            InitializeComponent(); Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1NpRGJGfV5ycUVPalhTTnJbUiweQnxTdEBiWX9acH1RRGRYWUN+XEleYg==");
            _authService = authService;
            _inicioView = inicioView;
            _detalhesView = detalhesView;
            _carteiraView = carteiraView;
            _cantinaView = cantinaView;
            _carrinhoView = carrinhoView;
            _authView = authView;
            SetupSessionMonitoring();
            _produtosView = produtosView;
        }

        private void SetupSessionMonitoring()
        {
            // Verificar a cada 30 segundos
            _sessionTimer = new System.Timers.Timer(30000);
            _sessionTimer.Elapsed += async (s, e) => await CheckSessionValidity();
            _sessionTimer.AutoReset = true;
            _sessionTimer.Start();

            // Verificar quando app entra em foreground
            if (Current != null)
            {
                Current.PageAppearing += OnPageAppearing;
            }
        }

        private async Task CheckSessionValidity()
        {
            if (!_checkLock.Wait(0)) return;

            try
            {

                if (!_isSessionValid) return;

                var data_expiracao = Preferences.Get("data_expiracao", DateTime.MinValue);

                if (DateTime.UtcNow >= data_expiracao)
                {
                    _isSessionValid = false;
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await _authView.TratarUnauthorized();
                    });
                }

            }
            finally
            {
                _checkLock.Release();
            }
        }

        private async void OnPageAppearing(object? sender, Page e)
        {
            // Verificar sempre que uma página aparece
            await CheckSessionValidity();
        }

        public void UpdateSessionValidity(bool isValid)
        {
            _isSessionValid = isValid;
        }

        protected override void OnSleep()
        {
            // Pausar timer quando app está em background
            _sessionTimer?.Stop();
            base.OnSleep();
        }

        protected override void OnResume()
        {
            // Retomar timer quando app volta ao foreground
            _sessionTimer?.Start();

            // Verificar imediatamente ao retornar
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await CheckSessionValidity();
            });

            base.OnResume();
        }

        protected override void CleanUp()
        {
            // Limpar recursos
            _sessionTimer?.Dispose();
            if (Current != null)
            {
                Current.PageAppearing -= OnPageAppearing;
            }
            base.CleanUp();
        }

        /* protected override Window CreateWindow( IActivationState? activationState )
         {
             return new Window(new LoadingPage(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView));
         }*/

        private async void RequisitarPermissão()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView, _carrinhoView,  _produtosView));
        }
    }
}