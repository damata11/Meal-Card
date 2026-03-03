using System.Diagnostics;
using Meal_Card.Controls;
using Meal_Card.Services;
using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class Carteira : ContentPage
{
    public readonly CarteiraViewModel _viewModel;
    private AuthService _authService;
    private bool isDataLoaded = false;

    public Carteira(CarteiraViewModel viewModel, AuthService authService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _authService = authService;
        BindingContext = _viewModel;
    }

    private async void Refreshing(object sender, EventArgs e)
    {
        refreshView.IsRefreshing = true;
        await LoadData();
        refreshView.IsRefreshing = false;
    }

    protected override bool OnBackButtonPressed()
    {
        if (AppShell.Current.Navigation.NavigationStack.Count > 1)
        {
            AppShell.Current.GoToAsync("//");
        }
        return true;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (!isDataLoaded)
        {
            await LoadData();
            isDataLoaded = true;
        }
    }

    private async Task LoadData()
    {
        await _viewModel.CarregarPerfil();
    }

    private async void BtnQrCode_Clicked(object sender, EventArgs e)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await AppShell.Current.GoToAsync(nameof(QRcode));
        }
        catch (Exception ex)
        {
            await NotificationToast.MostarToast("Ocorreu um erro!");
            Debug.Write($"Erro: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }

    }

    private async void Tap_Historico_Tapped(object sender, TappedEventArgs e)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await AppShell.Current.GoToAsync(nameof(Historico));
        }
        catch (Exception ex)
        {
            await NotificationToast.MostarToast("Ocorreu um erro!");
            Debug.Write($"Erro: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }

    }

    private async void Card_Payment_NFC_Tapped(object sender, TappedEventArgs e)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await AppShell.Current.GoToAsync(nameof(CarteiraPaymentNFC));
        }
        catch (Exception ex)
        {
            await NotificationToast.MostarToast("Ocorreu um erro!");
            Debug.Write($"Erro: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void VerHistorico_Tapped(object sender, TappedEventArgs e)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await AppShell.Current.GoToAsync(nameof(Historico));
        }
        catch (Exception ex)
        {
            await NotificationToast.MostarToast("Ocorreu um erro!");
            Debug.Write($"Erro: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}