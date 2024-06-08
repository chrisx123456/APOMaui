using System.Collections.ObjectModel;

namespace APOMaui;

public partial class AnalysisResultPage : ContentPage, IDisposable
{
    public ObservableCollection<AnalysisResult> AnalysisCollection { get; set; }

    public AnalysisResultPage(List<AnalysisResult> res)
	{
		InitializeComponent();
        AnalysisCollection = [..res];
        analysisResultsListView.ItemsSource = AnalysisCollection;
        BindingContext = this;
    }

    public void Dispose()
    {
        this.ClearLogicalChildren();
    }
}
