namespace APOMaui;

public partial class ConversionTab : ContentPage
{
	public ConversionTab()
	{
		InitializeComponent();
	}
    public async void OnButtonGrayScaleClick(object sender, EventArgs e)
    {
        if (ImageProc.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)ImageProc.selectedWindow;
        if (ImageProc.OpenedImagesList[index].ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok");
            return;
        }
        ImageProc.ConvertRgbToGray(index);
    }
    public async void OnButtonHSVClick(object sender, EventArgs e)
    {
        if (ImageProc.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)ImageProc.selectedWindow;
        if (ImageProc.OpenedImagesList[index].ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        ImageProc.ConvertRgbToHsv(index);
    }
    public async void OnButtonLabClick(object sender, EventArgs e)
    {
        if (ImageProc.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)ImageProc.selectedWindow;
        if (ImageProc.OpenedImagesList[index].ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        ImageProc.ConvertRgbToLab(index);

    }
}