using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace APOMaui;

public partial class ProfileLineChart : ContentPage
{
    public ISeries[]? Series { get; set; }
    public ProfileLineChart(int[] values)
	{
		InitializeComponent();
        this.Series = CreateISeries(values);
        BindingContext = this;

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
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this.Series = null;
    }
}