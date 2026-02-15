using Meal_Card.Pages;
using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;
        private readonly CarteiraViewModel _carteiraView;
        private readonly CantinaViewModel _cantinaView;
        private readonly InicioViewModel _inicioView;
        private readonly DetalhesViewModel _detalhesView;
        private readonly CarrinhoViewModel _carrinhoView;

        public AppShell( AuthService apiService, InicioViewModel inicioView, CarteiraViewModel carteiraView, CantinaViewModel cantinaView, DetalhesViewModel detalhesView, CarrinhoViewModel carrinhoView )
        {
            InitializeComponent();
            _authService = apiService;
            _inicioView = inicioView;
            _carteiraView = carteiraView;
            _detalhesView = detalhesView;
            _cantinaView = cantinaView;
            _carrinhoView = carrinhoView;
            ConfigureShell();
            RegisterRoutes();
            _detalhesView = detalhesView;
        }

        private void ConfigureShell()
        {
            var LoadingPage = new LoadingPage(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView, _carrinhoView);
            var Inicio = new Inicio(_authService, _inicioView, _carteiraView, _detalhesView);
            var Carteira = new Carteira(_carteiraView, _authService);
            var Carrinho = new Carrinho(_authService ,_carrinhoView, _detalhesView);
            var Menu = new Menu(_carteiraView, _authService);
            var Cantina = new Cantina(_authService, _inicioView, _carteiraView, _cantinaView, _detalhesView);

            Items.Add(new FlyoutItem
            {
                Items =
                {
                    new ShellContent
                    {
                        Title = "LoadingPage",
                        Route="LoadingPage",
                        Content = LoadingPage
                    }
                }
            });
            Items.Add(new TabBar
            {

                Items = {
                     new ShellContent{
                         Title="Início",
                         Route="Home",
                         Icon="home.png",
                         Content = Inicio
                     },
                     new ShellContent{
                         Title="Refeitorio",
                         Icon="talheres.png",
                         Content = Cantina
                     },
                     new ShellContent{
                         Title="Carteira",
                         Route="Carteira",
                         Icon="cartao.png",
                         Content = Carteira
                     },
                     new ShellContent{
                         Title="Carrrinho",
                         Route="Carrinho",
                         Icon="carrinho.png",
                         Content = Carrinho
                     },
                     new ShellContent{
                         Title="Menu",
                         Route="Menu",
                         Icon="perfil.png",
                         Content = Menu
                     }
                 }
            });
        }
        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(CarteiraPaymentNFC), typeof(CarteiraPaymentNFC));
            Routing.RegisterRoute(nameof(DetalhesProdutos), typeof(DetalhesProdutos));
            Routing.RegisterRoute(nameof(RecuperarSenha), typeof(RecuperarSenha));
            Routing.RegisterRoute(nameof(VerificarEmail), typeof(VerificarEmail));
            Routing.RegisterRoute(nameof(CortarImagem), typeof(CortarImagem));
            Routing.RegisterRoute(nameof(ListaProdutos), typeof(ListaProdutos));
            Routing.RegisterRoute(nameof(AlterarSenha), typeof(AlterarSenha));
            Routing.RegisterRoute(nameof(VerificarOTP), typeof(VerificarOTP));
            Routing.RegisterRoute(nameof(Notification), typeof(Notification));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(Calendario), typeof(Calendario));
            Routing.RegisterRoute(nameof(ContaInfo), typeof(ContaInfo));
            Routing.RegisterRoute(nameof(Historico), typeof(Historico));
            Routing.RegisterRoute(nameof(Favoritos), typeof(Favoritos));
            Routing.RegisterRoute(nameof(Reportar), typeof(Reportar));
            Routing.RegisterRoute(nameof(Settings), typeof(Settings));
            Routing.RegisterRoute(nameof(Contacto), typeof(Contacto));
            Routing.RegisterRoute(nameof(Cantina), typeof(Cantina));
            Routing.RegisterRoute(nameof(QRcode), typeof(QRcode));
            Routing.RegisterRoute(nameof(Sobre), typeof(Sobre));
            Routing.RegisterRoute(nameof(Login), typeof(Login));
        }
    }
}
