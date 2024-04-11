using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace APOMaui;

public partial class Chart : ContentPage, IDisposable
{
    public Window? window;
    public int indexOfImg;
    public ISeries[]? Series { get; set; }
    public Chart(int[] values, int indexOfImg)
	{
		InitializeComponent();
        myChart.AutoUpdateEnabled = true;
        this.Series = CreateISeries(values);
        this.indexOfImg = indexOfImg;
        AddElementsToTableChart(values);
        BindingContext = this;
	}
    public void Dispose()
    {
        this.myChart.Legend = null;
        foreach (var series in myChart.Series) series.Values = null;
        this.Series = null;
        this.TableHistogram.ClearLogicalChildren();
        this.TableHistogram.Children.Clear();
        this.TableHistogram.Clear();
        GC.Collect(2, GCCollectionMode.Forced);
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
        System.Diagnostics.Debug.WriteLine($"Chart of {Main.OpenedImagesWindowsList[indexOfImg].window.Title} Updated");
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
            }
        };
        return series;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this.Series = null;
        this.BindingContext = null;
        myChart.BindingContext = null;
        myChart.Legend = null;
        myChart.ClearLogicalChildren();
        myChart.Series = null;
        TableHistogram.Children.Clear();
        Main.OpenedImagesWindowsList[indexOfImg].chart = null;
        System.Diagnostics.Debug.WriteLine($"Chart of {indexOfImg} Disposed");
        window.ClearLogicalChildren();
        Application.Current.CloseWindow(window);
        this.window = null;
        GC.Collect();
        GC.ReRegisterForFinalize(this);
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
