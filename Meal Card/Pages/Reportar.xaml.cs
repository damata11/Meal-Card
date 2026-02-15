namespace Meal_Card.Pages;

public partial class Reportar : ContentPage
{
    public Reportar()
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
        await Navigation.PopAsync();
    }

}