using CommunityToolkit.Maui;
using Meal_Card.Models;
using Meal_Card.Pages;
using Meal_Card.Services;
using Meal_Card.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using UraniumUI;

namespace Meal_Card
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseMauiCompatibility()
                .UseUraniumUIMaterial()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("GoogleSans_VariableFont.ttf", "GoogleSans");
                    fonts.AddFont("ArchivoBlack-Regular.ttf", "ArchivoRegular");
                    fonts.AddFont("Montserrat_VariableFont_wght.ttf", "MontSerrat");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsBold");
                    fonts.AddFont("SHOOGIE.otf", "Shoogie");
                    fonts.AddMaterialIconFonts();
                });

            // Registering Views and ViewModels for Dependency Injection
            builder.Services.AddTransient<LoginViewModel>()
                 .AddTransient<Login>();

            builder.Services.AddTransient<InicioViewModel>()
                 .AddTransient<Inicio>();

            builder.Services.AddTransient<DetalhesViewModel>()
                 .AddTransient<DetalhesProdutos>();

            builder.Services.AddTransient<CantinaViewModel>()
                 .AddTransient<Refeitorio>();

            builder.Services.AddTransient<CarteiraViewModel>()
                  .AddTransient<Carteira>();

            builder.Services.AddTransient<CarrinhoViewModel>()
                  .AddTransient<Carrinho>();

            builder.Services.AddTransient<HistoricoViewModel>()
                .AddTransient<Historico>();

            builder.Services.AddTransient<ProdutosViewModel>()
                .AddTransient<Reserva>()
                .AddTransient<Refeitorio>()
                .AddTransient<ListaProdutos>();

            builder.Services.AddTransient<AccountViewModel>()
                .AddTransient<ContaInfo>();

            builder.Services.AddTransient<NFCViewModel>()
                .AddTransient<NFC>();

            builder.Services.AddTransient<Escola>();
            builder.Services.AddTransient<Categoria>();
            builder.Services.AddTransient<Utilizador>();
            builder.Services.AddTransient<Produtos_Bar>();
            builder.Services.AddTransient<HistoricoList>();
            builder.Services.AddTransient<CarteiraModel>();
            builder.Services.AddTransient<Itens_Carrinho>();
            builder.Services.AddTransient<Refeicoes>();


#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<AuthViewModel>();
            builder.Services.AddSingleton<FavoritosService>();
            builder.Services.AddTransient<LoadingPage>();

            return builder.Build();
        }

    }

}
