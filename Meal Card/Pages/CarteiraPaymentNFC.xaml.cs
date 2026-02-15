using Meal_Card.ViewModels;

namespace Meal_Card.Pages;

public partial class CarteiraPaymentNFC : ContentPage
{
    private readonly NFCViewModel _nfcView;
    public CarteiraPaymentNFC( NFCViewModel NfcView )
    {
        InitializeComponent();
        _nfcView = NfcView;
    }

    private void Refreshing( object sender, EventArgs e )
    {

    }

    private void BtnQRCode_Clicked( object sender, EventArgs e )
    {

    }
}