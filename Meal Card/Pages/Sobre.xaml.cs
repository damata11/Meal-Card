namespace Meal_Card.Pages;

public partial class Sobre : ContentPage
{
    public Sobre()
    {
        InitializeComponent();
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
            await Navigation.PopAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

}