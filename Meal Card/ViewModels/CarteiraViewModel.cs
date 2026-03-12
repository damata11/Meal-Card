using CommunityToolkit.Mvvm.ComponentModel;
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using SQLitePCL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public partial class CarteiraViewModel : AuthViewModel
    {
        private readonly AuthService _authService;
        private readonly HistoricoViewModel _historiaViewModel;

        private ObservableCollection<HistoricoList> _listItens = new();

        public CarteiraViewModel(HistoricoViewModel historiaViewModel, AuthService authService) : base(authService)
        {
            _authService = authService;
            _historiaViewModel = historiaViewModel;
        }

        public ObservableCollection<HistoricoList> ListItens
        {
            get => _listItens;
            private set
            {
                _listItens = value;
                OnPropertyChanged();
            }
        }

        // Campos privados
        [ObservableProperty]
        private string? _Nome;
        [ObservableProperty]
        private string? _Acronimo;
        [ObservableProperty]
        private string? _NomeEscola;
        [ObservableProperty]
        private string? _Email;
        [ObservableProperty]
        private string? _CardNumber;
        [ObservableProperty]
        private string? _State;
        [ObservableProperty]
        private string? _Imagem;
        [ObservableProperty]
        private string? _saldo;
        [ObservableProperty]
        private string? _tipoUtilizador;

        public async Task CarregarMenu()
        {
            try
            {
                var utilizador = await GetUtilizador();

                if (utilizador != null)
                {
                    Nome = $"{utilizador.Nome} {utilizador.Sobrenome}";
                    Imagem = utilizador.CaminhoImagem;
                    Email = utilizador.Email;
                }
                else
                {
                    Nome = Preferences.Get("nome", null);
                    Email = Preferences.Get("email", null);
                    Imagem = "perfil.png";
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        public async Task CarregarPerfil()
        {
            try
            {

                // Buscar dados da API
                var utilizador = await GetUtilizador();
                var carteira = await GetCarteira();
                var escola = await GetEscola();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (utilizador != null)
                    {
                        // Dados do utilizador da API
                        TipoUtilizador = utilizador.Tipo?.ToUpper() switch
                        {
                            "PROFESSOR" => "PROFESSOR",
                            "FUNCIONARIO" => "FUNCIONARIO",
                            "ADMIN" => "ADMINISTRADOR",
                            _ => "ESTUDANTE"
                        };
                        Nome = $"{utilizador.Nome} {utilizador.Sobrenome}";
                        Email = utilizador.Email;
                        CardNumber = utilizador.Card;
                        Imagem = utilizador.CaminhoImagem;
                    }
                    else
                    {
                        Nome = Preferences.Get("nome", null);
                        Email = Preferences.Get("email", null);
                        CardNumber = Preferences.Get("card", "12345");
                        Imagem = "perfil.png";
                        TipoUtilizador = Preferences.Get("tipo", "Estudante").ToUpper();
                    }

                    if (escola != null)
                    {
                        NomeEscola = escola.Nome;
                        Acronimo = escola.Acronomo;
                    }
                    else
                    {
                        // Fallback para Preferences ou defaults
                        NomeEscola = Preferences.Get("escola_nome", "Escola Padrão");
                        Acronimo = Preferences.Get("escola_acronimo", "EP");
                    }

                    if (carteira != null)
                    {
                        Saldo = $"{carteira.Saldo_disponivel:F2} €";
                    }
                    else
                    {
                        Saldo = $"{Preferences.Get("Saldo", "18.00")}€";
                    }

                    // State sempre atualiza
                    State = " 👆🏼 Clique aqui para pagar 👆🏼 ";
                });

                await AtualizarHistorico();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private async Task AtualizarHistorico()
        {
            try
            {

                if (_historiaViewModel.Historico == null || !_historiaViewModel.Historico.Any())
                {
                    System.Diagnostics.Debug.WriteLine("Histórico vazio, tentando carregar...");
                    // Descomente se quiser forçar recarregar:
                    // await _historiaViewModel.CarregarHistorico();
                }

                var novaLista = new ObservableCollection<HistoricoList>();

                if (_historiaViewModel.Historico != null && _historiaViewModel.Historico.Any())
                {
                    var ultimosItens = _historiaViewModel.Historico.TakeLast(3).ToList();
                    foreach (var item in ultimosItens)
                    {
                        novaLista.Add(item);
                    }
                    System.Diagnostics.Debug.WriteLine($"Adicionados {novaLista.Count} itens ao histórico");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Nenhum item no histórico");
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ListItens = novaLista;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" ERRO em AtualizarHistorico: {ex.Message}");
            }
        }

        private async Task<CarteiraModel?> GetCarteira()
        {
           return await MakeListApiCall(async () =>
            {

                try
                {
                    var (carteira, errorMessage) = await _authService.GetCarteira();

                    if (errorMessage == "Unauthorized")
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await NotificationToast.MostarToast("Sessão expirada. Será redirecionado para a tela de login.");
                            _authService.Logout();
                        });
                        return null;
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        System.Diagnostics.Debug.WriteLine($"Carteira erro: {errorMessage}");
                    }

                    System.Diagnostics.Debug.WriteLine($"Carteira: {(carteira != null ? "OK" : "NULL")}");
                    return carteira;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($" ERRO GetCarteira: {ex.Message}");
                    return null;
                }
            });
        }

        private async Task<Escola?> GetEscola()
        {
           return await MakeListApiCall(async () =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Buscando escola...");
                    var (escola, errorMessage) = await _authService.GetEscolaInfo();

                    if (errorMessage == "Unauthorized")
                    {
                        throw new UnauthorizedAccessException("Unauthorized");
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        System.Diagnostics.Debug.WriteLine($"Escola erro: {errorMessage}");
                    }

                    System.Diagnostics.Debug.WriteLine($"Escola: {(escola != null ? "OK" : "NULL")}");
                    return escola;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($" ERRO GetEscola: {ex.Message}");
                    return null;
                }
            });
        }

        private async Task<Utilizador?> GetUtilizador()
        {
           return await MakeListApiCall(async () =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Buscando utilizador...");
                    var (utilizador, errorMessage) = await _authService.GetUserInfo();

                    if (errorMessage == "Unauthorized")
                    {
                       throw new UnauthorizedAccessException("Unauthorized");
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        System.Diagnostics.Debug.WriteLine($"Utilizador erro: {errorMessage}");
                    }

                    System.Diagnostics.Debug.WriteLine($"Utilizador: {(utilizador != null ? "OK" : "NULL")}");
                    return utilizador;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($" ERRO GetUtilizador: {ex.Message}");
                    return null;
                }
            });
        }
    }
}