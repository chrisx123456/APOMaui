using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Diagnostics;

namespace APOMaui;

public partial class HistogramChart : ContentPage, IDisposable
{
    public int indexOfImg;
    public ISeries[]? Series { get; set; }
    public HistogramChart(int[] values, int indexOfImg)
	{
		InitializeComponent();
        myChart.AutoUpdateEnabled = true;
        this.Series = CreateISeries(values);
        this.indexOfImg = indexOfImg;
        AddElementsToTableChart(values);
        setAxes();

        BindingContext = this;
	}
    private void setAxes()
    {
        this.myChart.XAxes = new List<Axis>
        {
            new Axis
            {
                MinLimit = 0,
                TextSize = 12,
                Padding = new LiveChartsCore.Drawing.Padding(2d)
            }
        };
        this.myChart.YAxes = new List<Axis>
        {
            new Axis
            {
                MinLimit = 0,
                TextSize = 12,
                Padding = new LiveChartsCore.Drawing.Padding(2d)
            }
        };
    }

    public void Dispose()
    {
        BindingContext = null;  
        this.TableHistogram.Clear();
        this.Series = null;
        System.Diagnostics.Debug.WriteLine($"Chart of {indexOfImg} Disposed");
    }
    public async void UpdateChart(int[] values)
    {
        //Img sets first but photo tends to lag sometimes
        BindingContext = null;
        await this.Dispatcher.DispatchAsync(() =>  //Invokes UI thread
        {  
            this.Dispose();
            AddElementsToTableChart(values);
        });
        this.Series = CreateISeries(values);
        //System.Diagnostics.Debug.WriteLine($"Chart of {WindowFileManager.OpenedImagesList[indexOfImg].ImagePageWindow.Title} Updated");
        BindingContext = this;

        //this.Dispatcher.Dispatch(() => {  //Img sets while hist is done
        //    BindingContext = null;
        //    this.Dispose();
        //    this.Series = CreateISeries(values);
        //    AddElementsToTableChart(values);
        //    System.Diagnostics.Debug.WriteLine($"Chart of {Main.OpenedImagesWindowsList[indexOfImg].window.Title} Updated");
        //    BindingContext = this;
        //});

    }
    private static ISeries[] CreateISeries(int[] values)
    {
        ISeries[] series = new ISeries[] {
            new ColumnSeries<int>
            {
                Values = values,
                Fill = new SolidColorPaint(SKColors.Gray),
                MaxBarWidth=10,
                Padding=0,
                EasingFunction = null,
                IsHoverable = false,   
            }
            
        };
        return series;
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
