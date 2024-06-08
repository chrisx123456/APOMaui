namespace APOMaui;

public partial class CollectivePage : TabbedPage, IDisposable
{
    private int currPage;
    public ImagePage ImagePage { get; private set; }
    public HistogramChart? HistogramChart { get; private set; }
    public AnalysisResultPage? AnalysisResultPage { get; private set; }
    public ProfileLineChart? ProfileLineChart { get; private set; }
    public CollectivePage(ImagePage imagepage)
    {
        InitializeComponent();
        imagepage.Title = "Image"; 
        imagepage.IconImageSource = "imageico.png";
        this.ImagePage = imagepage;
        this.CurrentPageChanged += PageChanged;
        this.Children.Insert(0, imagepage);
    }
    public void PageChanged(object sender, EventArgs e)
    {
        int currentIndex = this.Children.IndexOf(this.CurrentPage);
        this.currPage = currentIndex;
        System.Diagnostics.Debug.WriteLine($"Aktualny indeks strony: {currentIndex}");
    }
    public void SetUpHistogram(HistogramChart histogramChart)
    {
        histogramChart.Title = "Histogram";
        histogramChart.IconImageSource = "chartico.png";
        this.HistogramChart = histogramChart;
        this.Children.Insert(1, histogramChart);

    }
    public void SetUpProfileLine(ProfileLineChart profileLineChart)
    {
        if(ProfileLineChart != null)
        {
            this.ProfileLineChart.Dispose();
            this.Children.Remove(ProfileLineChart);
            this.ProfileLineChart = null;
        }
        profileLineChart.Title = "Profile Line";
        profileLineChart.IconImageSource = "profileico.png";
        this.ProfileLineChart = profileLineChart;
        this.Children.Insert(2, profileLineChart);
    }
    public void SetUpAnalysisResult(AnalysisResultPage analysisResultPage)
    {
        analysisResultPage.Title = "Anaylsis Result";
        analysisResultPage.IconImageSource = "analysisico.png";
        this.AnalysisResultPage = analysisResultPage;
        this.Children.Insert(3, analysisResultPage);
    }
    public void RemoveCurrPage()
    {
        switch (currPage)
        {
            case 0:
                Application.Current?.CloseWindow(WindowFileManager.OpenedImagesList[this.ImagePage.index].CollectivePageWindow);
                break;
            case 1:
                if (this.HistogramChart != null)
                {
                    this.Children.Remove(this.HistogramChart);
                    this.HistogramChart.Dispose();
                    this.HistogramChart = null;
                }
                break;
            case 2:
                if (this.ProfileLineChart != null)
                {
                    this.Children.Remove(this.ProfileLineChart);
                    this.ProfileLineChart.Dispose();
                    this.ProfileLineChart = null;
                }
                break;
            case 3:
                if (this.AnalysisResultPage != null)
                {
                    this.Children.Remove(this.AnalysisResultPage);
                    this.AnalysisResultPage.Dispose();
                    this.AnalysisResultPage = null;
                }
                break;
        }
    }
    public void Dispose()
    {
        this.ImagePage.Dispose();
        if(this.HistogramChart != null) this.HistogramChart.Dispose();
        if(this.ProfileLineChart != null) this.ProfileLineChart.Dispose(); 
        if(this.AnalysisResultPage != null) this.AnalysisResultPage.Dispose();
        this.ClearLogicalChildren();
    }
    private void CollectivePage_SizeChanged(object sender, EventArgs e)
    {
        this.CurrentPage.HeightRequest = this.Height;
        this.CurrentPage.HeightRequest = -1;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WindowFileManager.OnCloseImagePage(this.ImagePage.index);
    }

}