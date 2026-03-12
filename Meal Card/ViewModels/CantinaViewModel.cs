using CommunityToolkit.Mvvm.ComponentModel;
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public partial class CantinaViewModel : AuthViewModel
    {
        private readonly AuthService _authService;
        public Command CarregarCalendarioCommand { get; set; }
        public Command ProximoMesCommand { get; set; }
        public Command MesAnteriorCommand { get; set; }
        public List<Refeicoes>? RefeicoesCarne { get; set; }
        public List<Refeicoes>? RefeicoesPeixe { get; set; }
        public List<Refeicoes>? RefeicoesVegetariano { get; set; }
        public Command<RefeitorioDia> SelecionarDiaCommand { get; set; }
        public Command<Refeicoes> MarcarRefeicaoCommand { get; set; }

        private ObservableCollection<RefeitorioDia>? _diasCalendario = new();


        public CantinaViewModel(AuthService authService) : base(authService)
        {
            _authService = authService;
            _MesAtual = DateTime.UtcNow.Month;
            _AnoAtual = DateTime.UtcNow.Year;
            _diasCalendario = new ObservableCollection<RefeitorioDia>();

            CarregarCalendarioCommand = new Command(async () => await CarregarCalendario());
            ProximoMesCommand = new Command(async () => await ProximoMes());
            MesAnteriorCommand = new Command(async () => await MesAnterior());
            SelecionarDiaCommand = new Command<RefeitorioDia>(async (dia) => await SelecionarDia(dia));
            MarcarRefeicaoCommand = new Command<Refeicoes>(async (refeicao) => await MarcarRefeicao(refeicao));
        }

        [ObservableProperty]
        private int? _MesAtual;
        [ObservableProperty]
        private int? _AnoAtual;
        [ObservableProperty]
        private RefeitorioDia? _diaSelecionado;
        [ObservableProperty]
        private Calendario? _CalendarioAtual;
        [ObservableProperty]
        private bool _IsRefreshing;


        public ObservableCollection<RefeitorioDia>? DiasCalendario
        {
            get => _diasCalendario;
            set => SetProperty(ref _diasCalendario, value);
        }

        public RefeitorioDia? Dia
        {
            get => _diaSelecionado;
            set => SetProperty(ref _diaSelecionado, value);
        }

        public string MesAnoExibicao => CalendarioAtual?.Nome_Mes ?? "Carregando...";

        public async Task CarregarCalendario()
        {
            await MakeApiCall(async () =>
            {
                try
                {
                    IsRefreshing = true;

                    var (calendario, erro) = await _authService.GetCalendarioAsync(AnoAtual, MesAtual);

                    if (erro == "Unauthorized")
                    {
                        throw new UnauthorizedAccessException("Unauthorized");
                    }

                    if (calendario != null)
                    {
                        CalendarioAtual = calendario;

                        AtualizarDiasCalendario();

                        OnPropertyChanged(nameof(MesAnoExibicao));
                    }
                    else
                    {
                        await NotificationToast.MostarToast($"Erro ao carregar calendário: {erro}");
                    }
                }
                catch (Exception ex)
                {
                    await NotificationToast.MostarToast($"Erro ao carregar calendário: {ex.Message}");
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }

        private async Task CarregarRefeicoes()
        {
            throw new NotImplementedException();
        }

        public async Task ProximoMes()
        {
            if (MesAtual == 12)
            {
                MesAtual = 1;
                AnoAtual++;
            }
            else
            {
                MesAtual++;
            }

            await CarregarCalendario();
        }

        public async Task MesAnterior()
        {
            if (MesAtual == 1)
            {
                MesAtual = 12;
                AnoAtual--;
            }
            else
            {
                MesAtual--;
            }

            await CarregarCalendario();
        }

        public async Task SelecionarDia(RefeitorioDia dia)
        {
            DiaSelecionado = dia;
            // Aqui você pode navegar para uma página de detalhes do dia
            await Shell.Current.GoToAsync($"detalhes-dia?data={dia.Data}");
        }

        public async Task MarcarRefeicao(Refeicoes refeicao)
        {
            await MakeApiCall(async () =>
            {
                try
                {
                    if (refeicao == null)
                    {
                        await NotificationToast.MostarToast("Refeição não encontrada");
                        return;
                    }

                    if (DiaSelecionado == null || string.IsNullOrEmpty(DiaSelecionado.Data)) {
                        await NotificationToast.MostarToast("Selecione um dia primeiro"); 
                        return;
                    }

                    var dataMarcacao = DateOnly.Parse(DiaSelecionado.Data).ToDateTime(TimeOnly.MinValue);

                    var reserva = new CriarReserva
                    {
                        Data_marcacao = dataMarcacao,
                        Tipo_refeicao = refeicao.Tipo_Refeicao
                    };
                    var response = await _authService.MarcarRefeicao(reserva);

                    if (response.Data)
                    {
                        await CarregarCalendario();
                        await Shell.Current.DisplayAlert("Sucesso", "Refeição marcada!", "OK");
                    }
                    else
                    {
                        await NotificationToast.MostarToast("Erro ao marcar refeição");
                        await Shell.Current.DisplayAlert("Erro", $"{response.ErrorMessage}", "OK");

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro ao marcar refeição: {ex.Message}");
                }

            });
        }

        private void AtualizarDiasCalendario()
        {
            DiasCalendario?.Clear();

            if (CalendarioAtual != null)
            {
                foreach (var dia in CalendarioAtual.Dias!)
                {
                    DiasCalendario?.Add(dia);
                }
            }
        }

        public void OnAppearing()
        {
            MainThread.BeginInvokeOnMainThread(async () => await CarregarCalendario());
        }

        public async Task RefreshDataAsync()
        {
            IsRefreshing = true;
            try
            {
                await Task.Delay(2000);
                await CarregarCalendario();
                //await CarregarRefeicoes();
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

    }
}
