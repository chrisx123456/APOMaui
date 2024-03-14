namespace APOMaui;

public partial class Operations : ContentPage
{
	public Operations()
	{
		InitializeComponent();
	}
	public async void  OnButtonGrayScaleClick(object sender, EventArgs e)
	{
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.isRGB == false)
        {
            await DisplayAlert("Alert", "Selected image is already Grayscale", "Ok");
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
        if (Main.OpenedImagesWindowsList[index].winImg.isRGB == false)
        {
            await DisplayAlert("Alert", "Selected image is Grayscale", "Ok");
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
        if (Main.OpenedImagesWindowsList[index].winImg.isRGB == false)
        {
            await DisplayAlert("Alert", "Selected image is already HSV", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        Main.ConvertRgbToHsv(index);
    }
}
