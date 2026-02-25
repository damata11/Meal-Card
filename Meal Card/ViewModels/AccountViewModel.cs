using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Pages;
using Meal_Card.Services;
using ServiceStack;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    [QueryProperty(nameof(CropPhotoSource), "new-src")]

    public partial class AccountViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        public ObservableCollection<Utilizador>? Utilizadores { get; } = new();

        public AccountViewModel( AuthService authService )
        {
            _authService = authService;
        }

        public string? CropPhotoSource { get; set; }
        private bool _isRefreshing;
        private string? _nome;
        private string? _escola;
        private string? _email;
        private string? _imagem;
        private string? _sobrenome;
        private string? _cardNumber;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }
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

        public string? Sobrenome
        {
            get => _sobrenome;
            set
            {
                if (_sobrenome != value)
                {
                    _sobrenome = value;
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

        public string? Escola
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

        /* public ImageSource Imagem
         {
             get => _imagem;
             set
             {
                 OnPropertyChanged();
             }
         }*/

        public string? Imagem
        {
            get => _imagem;
            set
            {
                if (_imagem != value)
                {
                    _imagem = value;
                    OnPropertyChanged();
                }
            }
        }

        public Image? _imageCrup;
        public Image? ImagemCrup
        {
            get => _imageCrup;
            set
            {
                if (_imageCrup != value)
                {
                    _imageCrup = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task RefreshDataAsync()
        {
            IsRefreshing = true;
            try
            {
                await Task.Run(async () =>
                {
                    await CarregarDados();
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
                var escola = await GetSchoolInfo();

            if (utilizador == null)
            {

            Nome = Preferences.Get("nome", "Danilson");
            Sobrenome = Preferences.Get("sobrenome", "da Mata");
            Email = Preferences.Get("email", "danilsonmata@gmail.com");
            CardNumber = Preferences.Get("card", "123456");
            Imagem = "perfil.png";
        }
            if (utilizador != null)
            {
                Preferences.Set("nome", utilizador.Nome);
                Preferences.Set("sobrenome", utilizador.Sobrenome);
                Preferences.Set("email", utilizador.Email);
                Preferences.Set("card", utilizador.Card);
                Nome = utilizador.Nome;
                Escola = escola.Nome;
                Sobrenome = utilizador.Sobrenome;
                Email = utilizador.Email;
                CardNumber = utilizador.Card;
                Imagem = utilizador.CaminhoImagem;
            }


            });
        }
        private async Task<Utilizador> GetUtilizador()
        {
            try
            {
                var (utilizador, ErrorMessage) = await _authService.GetUserInfo();

                if (ErrorMessage == "Unauthorized")
                {
                    //await NotificationToast.ShowToastL("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
                    _authService.Logout();
                    return null!;
                }
                if (utilizador == null)
                {
                    return null!;
                }
                return utilizador;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter informações do utilizador: {ex.Message}");
                return null!;
            }
        }

        private async Task<Escola> GetSchoolInfo()
        {
            try
            {
                var (escola, ErrorMessage) = await _authService.GetSchoolInfo();

                if (ErrorMessage == "Unauthorized")
                {
                    //await NotificationToast.ShowToastL("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
                    _authService.Logout();
                    return null!;
                }
                if (escola == null)
                {

                    return null!;
                }
                return escola;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter informações da escola: {ex.Message}");
                return null!;
            }
        }

        public async Task uploadFoto()
        {
            var photo = await carregarFoto();
            if (!string.IsNullOrWhiteSpace(photo))
                {
                    var param = new Dictionary<string, object>
                    {
                        [nameof(CortarImagem.PhotoSource)] = photo
                    };

                    await AppShell.Current.GoToAsync(nameof(CortarImagem), param);
                }

            }

        private async Task<string?> carregarFoto()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Photos>();
                    if (status != PermissionStatus.Granted)
                    {
                        await NotificationToast.ShowToastL("Permissão para aceder às fotos negada");
                        return null;
                    }
                }

                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecionar uma Foto"
                });

                if (photo == null) return null;

                var extension = Path.GetExtension(photo.FileName)?.ToLower() ?? ".jpg";
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    extension = ".jpg";

                var tempFile = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}{extension}");

                using (var sourceStream = await photo.OpenReadAsync())
                using (var fileStream = File.Create(tempFile))
                {
                    await sourceStream.CopyToAsync(fileStream);
                }

                return tempFile;
            }
            catch (FeatureNotSupportedException)
            {
                await NotificationToast.ShowToastL("Funcionalidade não suportada");
                return null;
            }
            catch (PermissionException)
            {
                await NotificationToast.ShowToastL("Permissão negada");
                return null;
            }
            catch (Exception ex)
            {
                await NotificationToast.ShowToastL($"Erro: {ex.Message}");
                return null;
            }
        }

        partial void OnCropPhotoSourceChanged( string? oldValue, string? newValue );

        async partial void OnCropPhotoSourceChanged(string? oldValue, string? newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue)) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {

                    if (!File.Exists(newValue))
                    {
                        await NotificationToast.ShowToastL("Arquivo de imagem não encontrado");
                        return;
                    }

                    var imageBytes = await File.ReadAllBytesAsync(newValue);

                    var result = await _authService.UploadProfileImage(imageBytes);

                    if (!result.HasError && result.Data)
                    {
                        await NotificationToast.ShowToastS("Imagem atualizada com sucesso! 🫡");

                        try { File.Delete(newValue); } catch { }

                        await CarregarDados();
                    }
                    else
                    {
                        await NotificationToast.ShowToastL("Erro ao atualizar imagem 😑");
                    }
                }
                catch (Exception ex)
                {
                    await NotificationToast.ShowToastL($"Erro: {ex.Message}");
                }
            });
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged( [CallerMemberName] string? name = null ) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }

}