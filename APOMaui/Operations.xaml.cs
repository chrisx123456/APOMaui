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
    public async void OnButtonEqualizationClick(object sender, EventArgs e)
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
        Main.HistEqualization(index);
    }
    public async void OnButtonStretchClick(object sender, EventArgs e)
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
        Main.HistStretch(index);
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
    public async void OnButtonQ3Q4Click(object sender, EventArgs e)
    {
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
            byte min;
            byte max;
            if(!byte.TryParse(await DisplayPromptAsync("Q3", "Type Q3 value"), out min))
            {
                await DisplayAlert("Alert", "Q3 Value not Valid", "Ok");
                return;
            }
            if (!byte.TryParse(await DisplayPromptAsync("Q4", "Type Q4 value"), out max))
            {
                await DisplayAlert("Alert", "Q4 Value not Valid", "Ok");
                return;
            }

            Main.HistStretchInRange(index, min, max);
        }
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
}
