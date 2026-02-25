using Meal_Card.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Meal_Card.ViewModels
{
    public class CantinaViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;

        public CantinaViewModel(AuthService authService)
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

                await Task.Delay(2000);
                // await CarregarDadosCantina();
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
