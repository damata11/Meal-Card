using CommunityToolkit.Maui.Alerts;

namespace Meal_Card.Pages;

public partial class Calendario : ContentPage
{
    private const string UrlPrincipal = "https://sige3portal.epamg.pt/Account/Login?ReturnUrl=%2fMeals%2fIndex";
    public Calendario()
    {
        InitializeComponent();
        carregarNavegador();
    }

    public void carregarNavegador()
    {

        BrowserView.Source = UrlPrincipal;

        url.Text = UrlPrincipal;

    }
    private void BtnBack_Clicked( object sender, EventArgs e )
    {
        if (BrowserView.CanGoBack)
        {
            BrowserView.GoBack();
        }
    }

    private void BtnForward_Clicked( object sender, EventArgs e )
    {
        if (BrowserView.CanGoForward)
        {
            BrowserView.GoForward();
        }
    }

    private void BtnClose_Clicked( object sender, EventArgs e )
    {
        AppShell.Current.GoToAsync("..");
    }

    private async void OnSearchCompleted( object sender, EventArgs e )
    {
        var entry = (Entry)sender;

        var url = entry.Text.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult))
            {
                BrowserView.Source = uriResult;

            }
            else
            {
                var notification = Toast.Make("URL inválido. ",
                    CommunityToolkit.Maui.Core.ToastDuration.Short);
                await notification.Show();
            }


        }
        else
        {
            carregarNavegador();
        }
    }
    private void OnBrowserNavigated( object sender, WebNavigatedEventArgs e )
    {
        if (e.Result == WebNavigationResult.Success)
        {
            url.Text = e.Url;
        }

    }


}