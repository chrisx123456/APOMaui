namespace APOMaui;

public partial class ConversionTab : ContentPage
{
	public ConversionTab()
	{
		InitializeComponent();
	}
    public async void OnButtonSplitClick(object sender, EventArgs e)
    {
        if (WindowFileManager.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
#if ANDROID
        if(WindowFileManager.OpenedImagesList.Count >= 3)
        {
            await DisplayAlert("Alert", "Cannot open more than 5 photos", "Ok");
            return;
        }
#endif
        int index = (int)WindowFileManager.selectedWindow;
        if (WindowFileManager.OpenedImagesList[index].CollectivePage.ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok");
            return;
        }
        ImageProc.SplitChannels(index);
    }
    public async void OnButtonGrayScaleClick(object sender, EventArgs e)
    {
        if (WindowFileManager.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)WindowFileManager.selectedWindow;
        if (WindowFileManager.OpenedImagesList[index].CollectivePage.ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok");
            return;
        }
        ImageProc.ConvertRgbToGray(index);
    }
    public async void OnButtonHSVClick(object sender, EventArgs e)
    {
        if (WindowFileManager.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
#if ANDROID
        if (WindowFileManager.OpenedImagesList.Count >= 3)
        {
            await DisplayAlert("Alert", "Cannot open more than 5 photos", "Ok");
            return;
        }
#endif
        int index = (int)WindowFileManager.selectedWindow;
        if (WindowFileManager.OpenedImagesList[index].CollectivePage.ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        ImageProc.ConvertRgbToHsv(index);
    }
    public async void OnButtonLabClick(object sender, EventArgs e)
    {
        if (WindowFileManager.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
#if ANDROID
        if (WindowFileManager.OpenedImagesList.Count >= 3)
        {
            await DisplayAlert("Alert", "Cannot open more than 5 photos", "Ok");
            return;
        }
#endif
        int index = (int)WindowFileManager.selectedWindow;
        if (WindowFileManager.OpenedImagesList[index].CollectivePage.ImagePage.Type != ImgType.RGB)
        {
            await DisplayAlert("Alert", "Selected image is not RGB", "Ok"); //Tu musi byc kolejny warunek ze jest juz HSV, albo ze jest szary
            return;
        }
        ImageProc.ConvertRgbToLab(index);

    }
}