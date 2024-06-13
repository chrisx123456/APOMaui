using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace APOMaui;

public partial class ProfileLineChart : ContentPage, IDisposable
{
    public ISeries[]? Series { get; set; }
    public ProfileLineChart(int[] values)
	{
		InitializeComponent();
        this.Series = CreateISeries(values);
        BindingContext = this;
        setAxes();
    }
    private void setAxes()
    {
        this.myChart.XAxes = new List<Axis>
        {
            new Axis
            {
            
                MinLimit = -1d,          
                TextSize = 12,
                Padding = new LiveChartsCore.Drawing.Padding(4d)
            }
        };
        this.myChart.YAxes = new List<Axis>
        {
            new Axis
            {
                MinLimit = -1d,
                TextSize = 12,
                Padding = new LiveChartsCore.Drawing.Padding(4d)
                
            }
        };
    }
    private static ISeries[] CreateISeries(int[] values)
    {
        ISeries[] series = new ISeries[] {
            new LineSeries<int>
            {
                Values = values,
                EasingFunction = null,
                IsHoverable = false,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                LineSmoothness = 0.3,
            }
        };
        return series;
    }

    public void Dispose()
    {
        this.Series = null;
        this.ClearLogicalChildren();
    }
}