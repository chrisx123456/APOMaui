
namespace APOMaui;

public partial class Kernels : ContentPage
{
    
    private static List<Entry> _entries = new List<Entry>();
    private static readonly List<String> _edgesOptions = new List<String> { "Isolated", "Reflect", "Replicate" };
    private static readonly List<String> _filtersOptions = new List<String> {"Blur", "GaussianBlur", "SobelX","SobelY", "LaplacianEdge", "Canny","LaplacianSharpen1",
        "LaplacianSharpen2", "LaplacianSharpen3", "PrewittE", "PrewittSE", "PrewittS", "PrewittSW", "PrewittW", "PrewittNW", "PrewittN", "PrewittNE"};
    private static readonly Dictionary<String, float[,]> _filtersDictionary = new() // No Blur/GausianBlur/Canny
	{
        {"SobelX",
            new float[,]
            {
                {-1f, 0f, 1f},
                {-2f, 0f, 2f},
                {-1f, 0f, 1f}
            }},
        {"SobelY",
            new float[,]
            {
                {-1f,-2f,-1f},
                { 0f, 0f, 0f},
                { 1f, 2f, 1f}
            }},
        {"LaplacianEdge",
            new float[,]
            {
                {0f, 1f,0f},
                {1f,-4f,1f},
                {0f, 1f,0f}
            }},
        {"LaplacianSharpen1",
            new float[,]
            {
                { 0f,-1f,0f},
                {-1f, 5,1f},
                { 0f,-1f,0f}
            }},
        {"LaplacianSharpen2",
            new float[,]
            {
                {-1f,-1f,-1f},
                {-1f, 9f,-1f},
                {-1f,-1f,-1f}
            }},
        {"LaplacianSharpen3",
            new float[,]
            {
                { 1f,-2f, 1f},
                {-2f, 5f,-2f},
                { 1f,-2f, 1f}
            }},
        {"PrewittN",
            new float[,]
            {
                { 1f, 1f, 1f},
                { 0f, 1f, 0f},
                {-1f,-1f,-1f}
            }},
        {"PrewittNE",
            new float[,]
            {
                { 0f, 1f,1f},
                {-1f, 1f,1f},
                {-1f,-1f,0f}
            }},

        {"PrewittE",
            new float[,]
            {
                {-1f,0f,1f},
                {-1f,1f,1f},
                {-1f,0f,1f}
            }},

        {"PrewittSE",
            new float[,]
            {
                {-1f,-1f,0f},
                {-1f, 1f,1f},
                { 0f, 1f,1f}
            }},

        {"PrewittS",
            new float[,]
            {
                {-1f,-1f,-1f},
                { 0f, 1f, 0f},
                { 1f, 1f, 1f}
            }},

        {"PrewittSW",
            new float[,]
            {
                {0f,-1f,-1f},
                {1f, 1f,-1f},
                {1f, 1f, 0f}
            }},
        {"PrewittW",
            new float[,]
            {
                {1f, 0f, -1f},
                {1f, 1f, -1f},
                {1f, 0f, -1f}
            }},

        {"PrewittNW",
            new float[,]
            {
                {1f, 1f, 0f},
                {1f, 1f,-1f},
                {0f,-1f,-1f}
            }},

    };
    private static readonly Dictionary<String, Emgu.CV.CvEnum.BorderType> _edgesDictionary = new()
    {
        {"Isolated", Emgu.CV.CvEnum.BorderType.Isolated },
        {"Reflect", Emgu.CV.CvEnum.BorderType.Reflect },
        {"Replicate", Emgu.CV.CvEnum.BorderType.Replicate },
    };
    private static readonly List<String> _kernelSizes = new() { "3", "4", "5" };
    private static Emgu.CV.CvEnum.BorderType? _selectedEdge = null;
    private static float[,]? _selectedFilter = null;
    public Kernels()
	{
		InitializeComponent();
        SizePicker.IsEnabled = false;
        SizePicker.IsVisible = false;
        SizePickerLabel.IsEnabled = false;
        SizePickerLabel.IsVisible = false;
		EdgePicker.ItemsSource = _edgesOptions;
		FilterPicker.ItemsSource = _filtersOptions;
        SizePicker.ItemsSource = _kernelSizes;
        _selectedEdge = null;
        _selectedFilter = null;
        CreateMatrix(3);

    }
    private void OnFilterPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        _selectedFilter = _filtersDictionary[FilterPicker.SelectedItem.ToString()];
        FillKernel(_selectedFilter);
        System.Diagnostics.Debug.WriteLine(_selectedFilter);

    }
    private void OnEdgePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (EdgePicker.SelectedItem != null && EdgePicker.SelectedIndex != -1)
        {
            _selectedEdge = _edgesDictionary[EdgePicker.SelectedItem.ToString()];
            System.Diagnostics.Debug.WriteLine(_selectedEdge);
        }

    }
    private void IsCustomKernelCheckedChanged(object sender, EventArgs e)
    {
        if (IsCustomKernel.IsChecked == true)
        {
            EdgePicker.IsEnabled = false;
            EdgePicker.IsVisible = false;
            EdgePickerLabel.IsEnabled = false;
            EdgePickerLabel.IsVisible = false;
            FilterPicker.IsEnabled = false;
            FilterPicker.IsVisible = false;
            FilterPickerLabel.IsVisible = false;
            FilterPickerLabel.IsEnabled = false;

            SizePicker.IsEnabled = true;
            SizePicker.IsVisible = true;
            SizePickerLabel.IsEnabled = true;
            SizePickerLabel.IsVisible = true;
        }
        else
        {
            EdgePicker.IsEnabled = true;
            EdgePicker.IsVisible = true;
            EdgePickerLabel.IsEnabled = true;
            EdgePickerLabel.IsVisible = true;
            FilterPicker.IsEnabled = true;
            FilterPicker.IsVisible = true;
            FilterPickerLabel.IsVisible = true;
            FilterPickerLabel.IsEnabled = true;

            SizePicker.IsEnabled = false;
            SizePicker.IsVisible = false;
            SizePickerLabel.IsEnabled = false;
            SizePickerLabel.IsVisible = false;
        }
    }
    private void OnSizePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    private void CreateMatrix(int size)
    {
        // Usunac stary matrix jesli zmieniamy na np 4x4
        Style s = new Style(typeof(Entry))
        {
            Setters =
            {
                new Setter{Property = Entry.MinimumHeightRequestProperty, Value = 35},
                new Setter{Property = Entry.MinimumWidthRequestProperty, Value = 35 },
                new Setter{Property = Entry.MaximumHeightRequestProperty, Value = 35},
                new Setter{Property = Entry.MaximumWidthRequestProperty, Value = 35},
                new Setter{Property = Entry.MarginProperty, Value = 4},
                new Setter{Property = Entry.FontSizeProperty, Value=12},
                new Setter{Property = Entry.BackgroundColorProperty, Value = Color.FromArgb("D3D3D3")},

            }
        };

        for (int i = 0; i < size; i++)
        {
            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
        for(int row = 0; row < size; row++)
        {
            for(int col = 0; col < size; col++)
            {
                Entry e = new Entry();
                e.SetDynamicResource(Entry.StyleProperty, "EntryKernel");
                MatrixGrid.Add(e, col, row);
                _entries.Add(e);
            }
        }

    }
    private void FillKernel(float[,] kernel)
    {
        int rows = MatrixGrid.RowDefinitions.Count;
        int cols = MatrixGrid.ColumnDefinitions.Count;
        if (rows == cols)
        {
            int size = cols;
            int krow = 0;
            int kcol = 0;
            for (int i = 0; i < _entries.Count; i++)
            {
                if(kcol == size)
                {
                    krow++;
                    kcol = 0;
                }
                _entries[i].Text = kernel[krow, kcol].ToString();
                kcol++;
            }
        }
    }
    public static void OnKernelButtonClicked(object sender, EventArgs e)
	{

	}

    private void isCustomKernel_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {

    }
}
