using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public class CarteiraViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly HistoricoViewModel _historiaViewModel;

        private ObservableCollection<HistoricoList> _listItens = new();
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
        private string? _nome;
        private string? _acronimo;
        private string? _escola;
        private string? _email;
        private string? _cardNumber;
        private string? _alerta;
        private string? _image;
        private string? _saldo;
        private string? _tipoUtilizador;

        // Propriedades com notificação
        public string? Nome
        {
            get => _nome;
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Acronimo
        {
            get => _acronimo;
            set
            {
                if (_acronimo != value)
                {
                    _acronimo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? NomeEscola
        {
            get => _escola;
            set
            {
                if (_escola != value)
                {
                    _escola = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? CardNumber
        {
            get => _cardNumber;
            set
            {
                if (_cardNumber != value)
                {
                    _cardNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? TipoUtilizador
        {
            get => _tipoUtilizador;
            set
            {
                if (_tipoUtilizador != value)
                {
                    _tipoUtilizador = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? State
        {
            get => _alerta;
            set
            {
                if (_alerta != value)
                {
                    _alerta = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Saldo
        {
            get => _saldo;
            set
            {
                if (_saldo != value)
                {
                    _saldo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Imagem
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }

        public CarteiraViewModel(HistoricoViewModel historiaViewModel, AuthService authService)
        {
            _authService = authService;
            _historiaViewModel = historiaViewModel;
        }

        public async Task CarregarDados()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== INICIANDO CarregarDados ===");

                // Buscar dados da API
                var utilizador = await GetUtilizador();
                var carteira = await GetCarteira();
                var escola = await GetEscola();

                // LOGS DE DIAGNÓSTICO
                System.Diagnostics.Debug.WriteLine($"DEBUG: utilizador={(utilizador != null ? "OK" : "NULL")}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: carteira={(carteira != null ? "OK" : "NULL")}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: escola={(escola != null ? "OK" : "NULL")}");

                // ✅ CORREÇÃO: Verificar cada um individualmente e usar dados da API quando disponíveis
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
                        // Fallback para Preferences
                        Nome = Preferences.Get("nome", "Danilson da Mata");
                        Email = Preferences.Get("email", "danilsonmata@gmailcom");
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
                System.Diagnostics.Debug.WriteLine($"❌ ERRO em CarregarDados: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private async Task AtualizarHistorico()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Atualizando histórico...");

                // Tentar carregar histórico se estiver vazio
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
                System.Diagnostics.Debug.WriteLine($"❌ ERRO em AtualizarHistorico: {ex.Message}");
            }
        }

        private async Task<CarteiraModel?> GetCarteira()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Buscando carteira...");
                var (carteira, errorMessage) = await _authService.GetCarteira();

                if (errorMessage == "Unauthorized")
                {
                    System.Diagnostics.Debug.WriteLine("Carteira: Unauthorized");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await NotificationToast.ShowToastL("Sessão expirada. Será redirecionado para a tela de login.");
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
                System.Diagnostics.Debug.WriteLine($"❌ ERRO GetCarteira: {ex.Message}");
                return null;
            }
        }

        private async Task<Escola?> GetEscola()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Buscando escola...");
                var (escola, errorMessage) = await _authService.GetEscolaInfo();

                if (errorMessage == "Unauthorized")
                {
                    System.Diagnostics.Debug.WriteLine("Escola: Unauthorized");
                    return null;
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
                System.Diagnostics.Debug.WriteLine($"❌ ERRO GetEscola: {ex.Message}");
                return null;
            }
        }

        private async Task<Utilizador?> GetUtilizador()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Buscando utilizador...");
                var (utilizador, errorMessage) = await _authService.GetUserInfo();

                if (errorMessage == "Unauthorized")
                {
                    System.Diagnostics.Debug.WriteLine("Utilizador: Unauthorized");
                    return null;
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
                System.Diagnostics.Debug.WriteLine($"❌ ERRO GetUtilizador: {ex.Message}");
                return null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}