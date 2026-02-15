namespace Meal_Card.Pages;

public partial class HistoricoDetails : ContentPage
{
    public HistoricoDetails()
    {
        InitializeComponent();
    }

    private void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Navigation.PopAsync(animated: false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Refreshing( object sender, EventArgs e )
    {
        Task.Delay(1000);
        refreshView.IsRefreshing = false;
    }
}