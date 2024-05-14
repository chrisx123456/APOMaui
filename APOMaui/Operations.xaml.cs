namespace APOMaui;

public partial class Operations : ContentPage
{
	public Operations()
	{
		InitializeComponent();
	}
	public async void OnButtonGrayScaleClick(object sender, EventArgs e)
	{
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok");
            return;
        }
        Main.ConvertRgbToGray(index);
    }
    public async void OnButtonSplitClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok");
            return;
        }
        Main.SplitChannels(index);
    }
    public async void OnButtonHSVClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        Main.ConvertRgbToHsv(index);
    }
    public async void OnButtonLabClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        Main.ConvertRgbToLab(index);

    }
    public async void OnButtonNegativeClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
        Main.ImageNegative(index);
    }

    public async void OnButtonPosterizeClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
        byte levels;
        if (!byte.TryParse(await DisplayPromptAsync("Grayscale levels", "Type number of gray levels"), out levels))
        {
            await DisplayAlert("Alert", "Value not Valid", "Ok");
            return;
        }
        if(levels < 2)
        {
            await DisplayAlert("Alert", "Value not Valid", "Ok");
            return;
        }
        Main.Posterize(index, levels);
    }

    private async void OnButtonHoughClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
        Main.HoughLines(index);
    }
    private async void OnButtonPyrClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
        var result = await DisplayActionSheet("Choose", "Cancel", null, new string[] { "Up", "Down" });
        if(result == "Up") 
        {
            Main.PyrUp(index);
        }
        else
        {
            Main.PyrDown(index);
        }
    }
    private async void OnButtonInpaintClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        Main.Inpainting(index);
    }
    private async void OnButtonAnalizeClick(object sender, EventArgs e)
    {
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        Main.AnalizeImage(index);
    }
}
