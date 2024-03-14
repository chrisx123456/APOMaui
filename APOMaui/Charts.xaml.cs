namespace APOMaui;

public partial class Charts : ContentPage
{
	public Charts()
	{
		InitializeComponent();
		BindingContext = this;

	}
	private async void OnButtonChartClicked(object sender, EventArgs e)
	{
		if(Main.selectedWindow == null)
		{
			await DisplayAlert("Alert", "None image is selected!", "Ok");
			return;
		}
		int index = (int)Main.selectedWindow;
		if (Main.OpenedImagesWindowsList[index].winImg.isRGB == true || Main.OpenedImagesWindowsList[index].winImg.GrayImage == null)
		{
			await DisplayAlert("Alert", "Selected image is RGB, Histogram is only supported for grayscale images", "Ok");
			return;
		}
		if (Main.OpenedImagesWindowsList[index].chart != null)
		{
            await DisplayAlert("Alert", "Histogram is already displayed", "Ok");
            return;
        }
		Main.CreateHistogramChart(index);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}