using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public class NFCViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private ObservableCollection<Escola>? Escola { get; } = new();
        private ObservableCollection<Utilizador>? Utilizadores { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public NFCViewModel(AuthService authService)
        {
            _authService = authService;
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public async Task RefreshDataAsync()
        {
            IsRefreshing = true;
            try
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    // await CarregarDadosCantina();
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

        public async Task CarregarDados()
        {
            await Task.Run(async () =>
            {
                var utilizador = await GetUtilizador();
                var carteira = await GetCarteira();
                var escola = await GetEscola();
            });

            //await Task.WhenAll(utilizador,carteira,escola);

        }

        private async Task<CarteiraModel> GetCarteira()
        {
            try
            {
                var (carteira, ErrorMessage) = await _authService.GetCarteira();

                // Verificar se o utilizador está autorizado caso contrário fazer logout
                if (ErrorMessage == "Unauthorized")
                {
                    // Fazer logout
                    await NotificationToast.MostarToast("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
                    _authService.Logout();
                    return null!;
                }

                if (carteira == null)
                {
                    // await MostarToast("Nenhuma informação encontrada 😑");
                    return null!;
                }
                return carteira;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter informações da carteira: {ex.Message}");
                return null!;
            }
        }

        private async Task<Escola> GetEscola()
        {
            try
            {
                var (escola, ErrorMessage) = await _authService.GetEscolaInfo();

                // Verificar se o utilizador está autorizado caso contrário fazer logout
                if (ErrorMessage == "Unauthorized")
                {
                    // Fazer logout
                    //await NotificationToast.MostarToast("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
                    //_authService.Logout();
                    return null!;
                }

                if (escola == null)
                {
                    // await MostarToast("Nenhuma informação encontrada 😑");
                    return null!;
                }
                Escola?.Add(escola);
                return escola;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter informações da escola: {ex.Message}");
                return null!;
            }
        }

        private async Task<Utilizador> GetUtilizador()
        {
            try
            {
                var (utilizador, ErrorMessage) = await _authService.GetUserInfo();

                if (ErrorMessage == "Unauthorized")
                {
                    //await NotificationToast.MostarToast("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
                    //_authService.Logout();
                    return null!;
                }
                if (utilizador == null)
                {
                    return null!;
                }
                Utilizadores?.Add(utilizador);
                return utilizador;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter informações do utilizador: {ex.Message}");
                return null!;
            }
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

