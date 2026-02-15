namespace Meal_Card;

public partial class Contacto : ContentPage
{
    public Contacto()
    {
        InitializeComponent();
    }


    private async void Refreshing( object sender, EventArgs e )
    {
        await Task.Delay(1000);
        refreshView.IsRefreshing = false;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
    }
    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        await Navigation.PopAsync();
    }
}