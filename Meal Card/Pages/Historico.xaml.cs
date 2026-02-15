using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class Historico : ContentPage
{

    private readonly HistoricoViewModel _viewModel;

    public Historico( HistoricoViewModel viewModel )
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

    }

    private async void Refreshing( object sender, EventArgs e )
    {
        await Task.Delay(1000);
        refreshView.IsRefreshing = false;
    }

    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await Navigation.PopAsync(animated: false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void detalhes_Tapped( object sender, TappedEventArgs e )
    {

    }
}