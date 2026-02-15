using Meal_Card.Controls;
using Meal_Card.Services;
using Meal_Card.ViewModels;
using System.Threading.Tasks;

namespace Meal_Card.Pages;

public partial class ContaInfo : ContentPage
{
    private readonly AuthService _authService;
    private readonly AccountViewModel _accountModel;
    private bool isDataLoaded = false;

    public ContaInfo( AuthService authService, AccountViewModel accountView )
    {
        InitializeComponent();
        _authService = authService;
        _accountModel = accountView;
        BindingContext = _accountModel;
    }
    private async void Refreshing( object sender, EventArgs e )
    {
        await _accountModel.RefreshDataAsync();
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
        var dados = _accountModel.CarregarDados();

        await Task.WhenAll(dados);
    }

    private async void TapSelectPhoto_Tapped( object sender, TappedEventArgs e )
    {
        await   _accountModel.uploadFoto();
    }


    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        await Navigation.PopAsync();
    }

}