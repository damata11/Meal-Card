using CommunityToolkit.Mvvm.ComponentModel;
using Meal_Card.Controls;
using Meal_Card.Models;
using Meal_Card.Pages;
using Meal_Card.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{
    [QueryProperty(nameof(CropPhotoSource), "new-src")]

    public partial class AccountViewModel : AuthViewModel
    {
        private readonly AuthService _authService;
        public ObservableCollection<Utilizador>? Utilizadores { get; } = new();

        public AccountViewModel(AuthService authService) : base(authService)
        {
            _authService = authService;
        }


        public string? CropPhotoSource { get; set; }
        [ObservableProperty]
        private bool _IsRefreshing;
        [ObservableProperty]
        private string? _Nome;
        [ObservableProperty]
        private string? _Escola;
        [ObservableProperty]
        private string? _Email;
        [ObservableProperty]
        private string? _Imagem;
        [ObservableProperty]
        private string? _Sobrenome;
        [ObservableProperty]
        private string? _CardNumber;


        /* public ImageSource Imagem
         {
             get => _imagem;
             set
             {
                 OnPropertyChanged();
             }
         }*/


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
                    //await NotificationToast.MostarToast("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
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
                    //await NotificationToast.MostarToast("Sessão expirada.Sera redirecionado pars a tela de login, para fazer login novamente.");
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
                        await NotificationToast.MostarToast("Permissão para aceder às fotos negada");
                        return null;
                    }
                }

                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecionar uma foto"
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
                await NotificationToast.MostarToast("Funcionalidade não suportada");
                return null;
            }
            catch (PermissionException)
            {
                await NotificationToast.MostarToast("Permissão negada");
                return null;
            }
            catch (Exception ex)
            {
                await NotificationToast.MostarToast($"Erro: {ex.Message}");
                return null;
            }
        }

        partial void OnCropPhotoSourceChanged(string? oldValue, string? newValue);

        async partial void OnCropPhotoSourceChanged(string? oldValue, string? newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue)) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {

                    if (!File.Exists(newValue))
                    {
                        await NotificationToast.MostarToast("Arquivo de imagem não encontrado");
                        return;
                    }

                    var imageBytes = await File.ReadAllBytesAsync(newValue);

                    var result = await _authService.UploadProfileImage(imageBytes);

                    if (!result.HasError && result.Data)
                    {
                        await NotificationToast.MostarToast("Imagem atualizada com sucesso! 🫡");

                        try { File.Delete(newValue); } catch { }

                        await CarregarDados();
                    }
                    else
                    {
                        await NotificationToast.MostarToast("Erro ao atualizar imagem 😑");
                    }
                }
                catch (Exception ex)
                {
                    await NotificationToast.MostarToast($"Erro: {ex.Message}");
                }
            });
        }

    }

}