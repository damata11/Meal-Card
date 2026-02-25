using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    public class HistoricoViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<HistoricoList> Historico { get; set; } = new();

        private readonly AuthService _authService;

        public HistoricoViewModel(AuthService authService)
        {
            _authService = authService;

            Historico.Add(new HistoricoList()
            {
                Nome = "Refeiçao consumida",
                Descricao = "Greelhada mista com porco",
                Data = "01/01/2023",
                valor = "",
                status = "Consumido",
                Icon = "consumido"
            });
            Historico.Add(new HistoricoList()
            {
                Nome = "Refeiçao consumida",
                Descricao = "Gerelhada mista",
                Data = "02/01/2023",
                valor = "20,00€",
                status = "Consumido",
                Icon = "consumido"
            });
            Historico.Add(new HistoricoList()
            {
                Nome = "Refeiçao consumida",
                Descricao = "Bacalhau a bras",
                Data = "03/01/2023",
                valor = "20,00€",
                status = "Consumido",
                Icon = "consumido"
            });
            Historico.Add(new HistoricoList()
            {
                Nome = "Refeiçao  não consumida",
                Descricao = "Greelhada mista com porco",
                Data = "01/01/2023",
                valor = "",
                status = "Consumido",
                Icon = "notconsumo"
            });
            Historico.Add(new HistoricoList()
            {
                Nome = "Refeiçao consumida",
                Descricao = "Greelhada mista com porco",
                Data = "01/01/2023",
                valor = "",
                status = "Consumido",
                Icon = "consumido"
            });
        }

        private bool _isRefreshing;
        private bool _isVisible;
        private decimal _valorTotal;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal ValorTotal
        {
            get => _valorTotal;
            set
            {
                if (_valorTotal != value)
                {
                    _valorTotal = value;
                    OnPropertyChanged();
                }
            }
        }


        public async Task Initialize()
        {
            await GetTransacoes();
        }

        public async Task RefreshDataAsync()
        {
            if (IsRefreshing) return;
            try
            {
                IsRefreshing = true;
                await GetTransacoes();
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

        public async Task GetTransacoes()
        {

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
