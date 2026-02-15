namespace Meal_Card.Pages;

public partial class Notification : ContentPage
{
    public Notification()
    {
        InitializeComponent();
    }

    private void Refreshing( object sender, EventArgs e )
    {
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