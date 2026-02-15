using Meal_Card.Pages;
using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        private readonly InicioViewModel _inicioView;
        private readonly CantinaViewModel _cantinaView;
        private readonly CarteiraViewModel _carteiraView;
        private readonly DetalhesViewModel _detalhesView;
        private readonly CarrinhoViewModel _carrinhoView;

        public App( AuthService authService, InicioViewModel inicioView, CarteiraViewModel carteiraView, CantinaViewModel cantinaView, DetalhesViewModel detalhesView, CarrinhoViewModel carrinhoView )
        {
             InitializeComponent();           Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1NpRGJGfV5ycUVPalhTTnJbUiweQnxTdEBiWX9acH1RRGRYWUN+XEleYg==");
            _authService = authService;
            _inicioView = inicioView;
            _detalhesView = detalhesView;
            _carteiraView = carteiraView;
            _cantinaView = cantinaView;
            _cantinaView = cantinaView;
            _carrinhoView = carrinhoView;
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

         protected override Window CreateWindow( IActivationState? activationState )
         {
             return new Window(new AppShell(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView, _carrinhoView));
         }
    }
}