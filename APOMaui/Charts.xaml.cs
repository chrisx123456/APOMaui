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
		if (Main.OpenedImagesWindowsList[index].winImg.Type == ImgType.RGB || Main.OpenedImagesWindowsList[index].winImg.GrayImage == null)
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
            if (!byte.TryParse(await DisplayPromptAsync("Q3", "Type Q3 value"), out min))
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
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}