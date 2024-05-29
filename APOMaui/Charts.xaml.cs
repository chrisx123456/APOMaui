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
            byte p1;
            byte p2;
            byte q3;
            byte q4;
            if (!byte.TryParse(await DisplayPromptAsync("p1", "Type p1 value"), out p1))
            {
                await DisplayAlert("Alert", "Q3 Value not Valid", "Ok");
                return;
            }
            if (!byte.TryParse(await DisplayPromptAsync("p2", "Type p2 value"), out p2))
            {
                await DisplayAlert("Alert", "p2 Value not Valid", "Ok");
                return;
            }
            if (!byte.TryParse(await DisplayPromptAsync("q3", "Type q3 value"), out q3))
            {
                await DisplayAlert("Alert", "q3 Value not Valid", "Ok");
                return;
            }
            if (!byte.TryParse(await DisplayPromptAsync("q4", "Type q4 value"), out q4))
            {
                await DisplayAlert("Alert", "q4 Value not Valid", "Ok");
                return;
            }

            Main.HistStretchInRange(index, p1, p2, q3, q4);
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