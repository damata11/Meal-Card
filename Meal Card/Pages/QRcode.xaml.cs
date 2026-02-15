

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Meal_Card.Pages;

public partial class QRcode : ContentPage
{

    public QRcode()
    {
        InitializeComponent();

    }

    private async void Refreshing( object sender, EventArgs e )
    {
        await SetQRCodeAsync();
        refreshView.IsRefreshing = false;
    }

    private string? Nome;
    private string? CardNumber;
    protected override async void OnAppearing()
    {
        loadIndicator.IsRunning = true;
        loadIndicator.IsVisible = true;
        base.OnAppearing();
        await SetQRCodeAsync();
        loadIndicator.IsRunning = false;
        loadIndicator.IsVisible = false;

    }

    private async Task SetQRCodeAsync()
    {
        try
        {

            Nome = Preferences.Get("nome", "José") + " " + Preferences.Get("sobrenome", "José");
            CardNumber = Preferences.Get("card", "122380");
            DateTime dataAtual = DateTime.UtcNow;



            if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(Nome))
            {
                var notification = Toast.Make("Erro inesperado 😑\n Tente novamente mais tarde!",
                    ToastDuration.Long);
                await notification.Show();
                QRCodeImage.Source = null;

            }

            string cardInfo = $@"{{
                ""nome"": ""{Nome}"",
                ""cardNumber"": ""{CardNumber}"",
                ""timestamp"": ""{dataAtual:o}""
              }}";


            // Gera a imagem do QR Code a partir dos dados.
            var qrCodeImageSource = GenerateQRCode(cardInfo);

            // Atribui a imagem gerada ao controlo no XAML.
            QRCodeImage.Source = qrCodeImageSource;
        }
        catch (Exception ex)
        {

            var notification = Toast.Make($"Error: {ex.Message}", ToastDuration.Short);
            await notification.Show();
        }
    }
    private ImageSource GenerateQRCode( string text )
    {
        using var qrGenerator = new QRCoder.QRCodeGenerator(); // Qualificar QRCodeGenerator
        var qrCodeData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);

        // Usar PngByteQRCode para obter diretamente os bytes PNG
        var pngByteQrCode = new QRCoder.PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = pngByteQrCode.GetGraphic(20); // '20' é o tamanho do módulo

        // Retornar um novo MemoryStream a partir dos bytes para o ImageSource
        return ImageSource.FromStream(() => new System.IO.MemoryStream(qrCodeBytes));
    }

    private async void BtnVoltar_Clicked( object sender, EventArgs e )
    {
        await Navigation.PopAsync();
    }
}