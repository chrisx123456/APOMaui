using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
namespace APOMaui;

public partial class Chart : ContentPage
{
    public Window? window;
    public int indexOfImg;
    public ISeries[] Series { get; set; }


    public Chart(int[] values, int indexOfImg)
	{
		InitializeComponent();
        myChart.AutoUpdateEnabled = true;
        this.Series = CreateChart_HistGrayscale(values);
        this.indexOfImg = indexOfImg;
        AddElementsToTableChart(values);
        BindingContext = this;
	}
    public void UpdateChart(int[] values)
    {
        BindingContext = null;
        this.Series = CreateChart_HistGrayscale(values);
        TableHistogram.Children.Clear();
        AddElementsToTableChart(values);
        System.Diagnostics.Debug.WriteLine($"Chart of {Main.OpenedImagesWindowsList[indexOfImg].window.Title} Updated");
        BindingContext = this;
    }
    private static ISeries[] CreateChart_HistGrayscale(int[] values)
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
    public void AddElementsToTableChart(int[] tab)
    {
        int elements = tab.Length;
        TableHistogram.Children.Add(new HorizontalStackLayout{
            Children = {new Label{Text ="Value", Margin=2, Padding=2, FontSize=12}, new Label{Text ="Qt.", Margin=2, Padding=2, FontSize=12}}
        });     
        for (int i = 0; i < elements; i++)
        {
            TableHistogram.Children.Add(new HorizontalStackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text =  i.ToString() + " :",
                        Margin=2,
                        Padding=2,
                        FontSize=12
                    },
                    new Label
                    {
                        Text = tab[i].ToString(),
                        Margin=2,
                        Padding=new Thickness(15,2,2,2),
                        FontSize=12,
                        HorizontalOptions=LayoutOptions.End,
                    },
                }
            });

        }
    }
}
