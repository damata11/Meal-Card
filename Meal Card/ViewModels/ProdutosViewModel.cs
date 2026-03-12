
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;

namespace Meal_Card.ViewModels
{
    public partial class ProdutosViewModel : AuthViewModel
    {
        private readonly AuthService _authService;
        public static List<Produtos_Bar> SearchBar { get; set; } = new();
        public ObservableCollection<Produtos_Bar> Produtos { get; set; } = new();

        public ProdutosViewModel(AuthService authService) : base(authService)
        {
            _authService = authService;
        }

        public async Task GetListaProdutos()
        {
           await MakeApiCall(async () => { 
            try
            {
                var (produtos, ErrorMessage) = await _authService.GetAllProdutosBar();

                if (ErrorMessage == "Unauthorized")
                {
                       throw new UnauthorizedAccessException("Unauthorized");
                }

                if (produtos is null || !produtos.Any())
                {
                    return;
                }

                   if (Produtos.Count > 0)
                       Produtos.Clear();

                   if (SearchBar.Count > 0)
                       SearchBar.Clear();

                   if(produtos != null) {
                      var lista = new ObservableCollection<Produtos_Bar>(produtos);
                      MainThread.BeginInvokeOnMainThread(() => Produtos = lista);
                       SearchBar.AddRange(produtos);
                       
                   }
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter produtos mais vendidos: {ex.Message}");
                return ;
            }
          });
        }

        public async Task GetListaProdutosCategoria(int? id_categoria)
        {
            await MakeApiCall(async () => {
                try
                {
                   var (produtos, ErrorMessage) = await _authService.GetProdutosBar("categoria", id_categoria!.Value);

                if (ErrorMessage == "Unauthorized")
                {
                        throw new UnauthorizedAccessException("Unauthorized");
                    }

                if (produtos is null || !produtos.Any())
                {
                    return;
                }

                var lista = new ObservableCollection<Produtos_Bar>(produtos);
                MainThread.BeginInvokeOnMainThread(() => Produtos = lista);
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter produtos mais vendidos: {ex.Message}");
                return;
            }
              });
        }
    }

}