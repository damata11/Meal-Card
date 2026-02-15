using CommunityToolkit.Maui.Alerts;
using Meal_Card.Controls;
using Syncfusion.Maui.ImageEditor;
using System.Threading.Tasks;

namespace Meal_Card.Pages;

[QueryProperty(nameof(PhotoSource),nameof(PhotoSource))]
public partial class CortarImagem : ContentPage, IQueryAttributable
{
	public CortarImagem()
    {
		InitializeComponent();
	}

    public string? PhotoSource { get; set; }

    public async void ApplyQueryAttributes( IDictionary<string, object> query )
    {
        if (query.TryGetValue(nameof(PhotoSource), out var photoSourceObject) && photoSourceObject is string photoSource)
        {
            if (string.IsNullOrWhiteSpace(photoSource))
            {
                await Toast.Make("Nenhuma foto selecionada").Show();
                await Shell.Current.GoToAsync("..");
                return;
            }
            PhotoSource = photoSource;
        }

        ImageEditor.Source = PhotoSource;
        ImageEditor.ImageLoaded += ImageEdito_Loaded;
    }

    private void ImageEdito_Loaded( object? sender, EventArgs e )
    {
        ImageEditor.Crop(Syncfusion.Maui.ImageEditor.ImageCropType.Circle);
        ImageEditor.ImageLoaded -= ImageEdito_Loaded;
    }

    private async void Salvar_click( object sender, EventArgs e )
    {
        if (ImageEditor.HasUnsavedEdits)
        {

            await Shell.Current.DisplayAlert("Cancelar Corte", "As alterações não foram guardadas !", "Sim", "Não");

        }

        ImageEditor.SaveEdits();
        var newPothoStream = await ImageEditor.GetImageStream();

         var extension = Path.GetExtension(PhotoSource);

        var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}{extension}");

        using (var fileStream = File.OpenWrite(tempFilePath))
            {
               await newPothoStream.CopyToAsync(fileStream);
            }


        await newPothoStream.DisposeAsync();

        await Shell.Current.GoToAsync($"..?new-src" + tempFilePath);
    }

    private async void Cancelar_click( object sender, EventArgs e )
    {
        if (ImageEditor.HasUnsavedEdits)
        {
            if (await Shell.Current.DisplayAlert("Cancelar Corte", "Voce tem certeza que deseja cancelar o corte da imagem ?", "Sim", "Não"))
            {
                ImageEditor.CancelEdits();
                await AppShell.Current.GoToAsync("..");
            }
        }
        else if (await Shell.Current.DisplayAlert("Cancelar Corte", "Voce tem certeza ?", "Sim", "Não"))
        {
            await AppShell.Current.GoToAsync("..");
        }

    }

}