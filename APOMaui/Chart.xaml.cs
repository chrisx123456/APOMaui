using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
namespace APOMaui;

public partial class Chart : ContentPage
{
    public Window window;
    public int indexOfImg;
    private ISeries[] series;
    public ISeries[] Series
    {
        get { return series; }
        set 
        {
            BindingContext = null;
            series = value;
            System.Diagnostics.Debug.WriteLine("Chart Set");
            BindingContext = this;

        }
    }

    public Chart(int[] values, int indexOfImg)
	{
		InitializeComponent();
        myChart.AutoUpdateEnabled = true;
        this.Series = CreateChart_HistGrayscale(values);
        this.indexOfImg = indexOfImg; 
        BindingContext = this;
	}
    private ISeries[] CreateChart_HistGrayscale(int[] values)
    {
        ISeries[] series = new ISeries[] {
            new ColumnSeries<int>
            {
                Values = values,
                Fill = new SolidColorPaint(SKColors.Gray),
                MaxBarWidth=10,
                Padding=0,
            }
        };
        return series;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Main.OnCloseEventChart(indexOfImg);
    }

}