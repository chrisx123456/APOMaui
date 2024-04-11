using Emgu.CV;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
namespace APOMaui;

public partial class Kernels : ContentPage
{

    private static readonly List<String> _edgesOptions = new List<String> { "Isolated", "Reflect", "Replicate" };
    private static readonly List<String> _filtersOptions = new List<String> {"Blur", "GaussianBlur", "SobelX","SobelY", "LaplacianEdge", "Canny","LaplacianSharpen1",
        "LaplacianSharpen2", "LaplacianSharpen3", "PrewittE", "PrewittSE", "PrewittS", "PrewittSW", "PrewittW", "PrewittNW", "PrewittN", "PrewittNE"};
    private static readonly Dictionary<String, float[,]> _filtersDictionary = new() // No Blur/GausianBlur/Canny
	{
        {"SobelX",
            new float[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            }},
        {"SobelY",
            new float[,]
            {
                {-1,-2,-1},
                { 0, 0, 0},
                { 1, 2, 1}
            }},
        {"LaplacianEdge",
            new float[,]
            {
                {0, 1,0},
                {1,-4,1},
                {0, 1,0}
            }},
        {"LaplacianSharpen1",
            new float[,]
            {
                { 0,-1,0},
                {-1, 5,1},
                { 0,-1,0}
            }},
        {"LaplacianSharpen2",
            new float[,]
            {
                {-1,-1,-1},
                {-1, 9,-1},
                {-1,-1,-1}
            }},
        {"LaplacianSharpen3",
            new float[,]
            {
                { 1,-2, 1},
                {-2, 5,-2},
                { 1,-2, 1}
            }},
        {"PrewittN",
            new float[,]
            {
                { 1, 1, 1},
                { 0, 1, 0},
                {-1,-1,-1}
            }},
        {"PrewittNE",
            new float[,]
            {
                { 0, 1,1},
                {-1, 1,1},
                {-1,-1,0}
            }},

        {"PrewittE",
            new float[,]
            {
                {-1,0,1},
                {-1,1,1},
                {-1,0,1}
            }},

        {"PrewittSE",
            new float[,]
            {
                {-1,-1,0},
                {-1, 1,1},
                { 0, 1,1}
            }},

        {"PrewittS",
            new float[,]
            {
                {-1,-1,-1},
                { 0, 1, 0},
                { 1, 1, 1}
            }},

        {"PrewittSW",
            new float[,]
            {
                {0,-1,-1},
                {1, 1,-1},
                {1, 1, 0}
            }},
        {"PrewittW",
            new float[,]
            {
                {1, 0, -1},
                {1, 1, -1},
                {1, 0, -1}
            }},

        {"PrewittNW",
            new float[,]
            {
                {1, 1, 0},
                {1, 1,-1},
                {0,-1,-1}
            }},

    };
    private static readonly Dictionary<String, Emgu.CV.CvEnum.BorderType> _edgesDictionary = new()
    {
        {"Isolated", Emgu.CV.CvEnum.BorderType.Isolated },
        {"Reflect", Emgu.CV.CvEnum.BorderType.Reflect },
        {"Replicate", Emgu.CV.CvEnum.BorderType.Replicate },
    };
    private static Emgu.CV.CvEnum.BorderType? _selectedEdge = null;
    private static float[,]? _selectedFilter = null;
    public Kernels()
	{
		InitializeComponent();
		EdgePicker.ItemsSource = _edgesOptions;
		FilterPicker.ItemsSource = _filtersOptions;
        _selectedEdge = null;
        _selectedFilter = null;

    }
    private void OnFilterPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        _selectedFilter = _filtersDictionary[FilterPicker.SelectedItem.ToString()];
        System.Diagnostics.Debug.WriteLine(_selectedFilter);
    }
    private void OnEdgePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        _selectedEdge = _edgesDictionary[EdgePicker.SelectedItem.ToString()];
        System.Diagnostics.Debug.WriteLine(_selectedEdge);
    }
    private void FillKernel(float[,] kernel)
    {

    }
    public static void OnKernelButtonClicked(object sender, EventArgs e)
	{

	}
}
