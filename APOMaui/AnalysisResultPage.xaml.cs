using System.Collections.ObjectModel;

namespace APOMaui;

public partial class AnalysisResultPage : ContentPage
{
    public ObservableCollection<AnalysisResult> AnalysisCollection { get; set; }

    public AnalysisResultPage(List<AnalysisResult> res)
	{
		InitializeComponent();
        AnalysisCollection = [..res];
        analysisResultsListView.ItemsSource = AnalysisCollection;
        BindingContext = this;

    }

}
