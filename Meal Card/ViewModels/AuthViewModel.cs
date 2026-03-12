using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Meal_Card.Controls;
using Meal_Card.Services;

namespace Meal_Card.ViewModels
{


    public partial class AuthViewModel : ObservableObject
    {
        private readonly AuthService _authservice;
        private bool _isHandlingUnauthorized = false;

        [ObservableProperty]
        private bool isBusy;

        public AuthViewModel(AuthService authservice)
        {
            _authservice = authservice;
        }

        protected async Task MakeApiCall(Func<Task> apiCall)
        {
            //if (IsBusy) return;
            IsBusy = true;
            try
            {
                await apiCall.Invoke();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await TratarUnauthorized();
                }
                else
                {
                    await NotificationToast.MostarToast("Ocorreu um erro");
                    Debug.WriteLine($"Ocorreu um erro: {ex.Message}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                await TratarUnauthorized();
            }
            catch (Exception ex)
            {
                await NotificationToast.MostarToast("Ocorreu um erro");
                Debug.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task<T?> MakeListApiCall<T>(Func<Task<T>> apiCall)
        {
            //if (IsBusy) return;
            IsBusy = true;
            try
            {
                T? result = await apiCall.Invoke();
                return result;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await TratarUnauthorized();
                }
                else
                {
                    await NotificationToast.MostarToast("Ocorreu um erro");
                    Debug.WriteLine($"Ocorreu um erro: {ex.Message}");
                }
                return default;
            }
            catch (UnauthorizedAccessException)
            {
                await TratarUnauthorized();
                return default;
            }
            catch (Exception ex)
            {
                await NotificationToast.MostarToast("Ocorreu um erro");
                Debug.WriteLine($"Ocorreu um erro: {ex.Message}");
                return default;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task TratarUnauthorized()
        {
            if (_isHandlingUnauthorized)
                return;

            _isHandlingUnauthorized = true;
            try
            {
                if (Shell.Current.CurrentPage.GetType().Name != "Login")
                {
                    var logoutTask = Task.Delay(5000).ContinueWith(_ =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            _authservice.Logout();
                        });
                    });

                    await AppShell.Current.DisplayAlert("Sessão Expirada",
                        "A sua sessão expirou. Será redirecionado para a página de login.",
                        "OK");

                    _authservice.Logout();
                }
            }
            catch (System.Exception)
            {
                // Se falhar, faz logout mesmo assim
                _authservice.Logout();
            }
            finally
            {
                _isHandlingUnauthorized = false;
            }

        }
    }
}