
namespace APOMaui;

public partial class Kernels : ContentPage
{ 
    private static readonly List<String> _edgesOptions = new List<String> { "Isolated", "Reflect", "Replicate" };
    private static readonly List<String> _kernelSizes = new() { "3", "4", "5" };
    private static readonly List<String> _filtersOptions = new List<String> {"Blur", "GaussianBlur", "SobelX","SobelY", "LaplacianEdge", "Canny","LaplacianSharpen1",
        "LaplacianSharpen2", "LaplacianSharpen3", "PrewittE", "PrewittSE", "PrewittS", "PrewittSW", "PrewittW", "PrewittNW", "PrewittN", "PrewittNE"};
    private static readonly Dictionary<String, float[,]> _filtersDictionary = new() 
	{ 
        {"Canny",
            new float[,]
            {
                {2},
                {2},
                {2}
            }},
        {"GaussianBlur",
            new float[,]
            {
                {1},
                {1},
                {1}
            }},
        {"Blur",
            new float[,]
            {
                {0},
                {0},
                {0}
            }},
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

    };//Blur:0, GBlur:1, Canny:2<->In-Built functions so untypical matrix to identify later on
    private static readonly Dictionary<String, Emgu.CV.CvEnum.BorderType> _edgesDictionary = new()
    {
        {"Isolated", Emgu.CV.CvEnum.BorderType.Isolated },
        {"Reflect", Emgu.CV.CvEnum.BorderType.Reflect },
        {"Replicate", Emgu.CV.CvEnum.BorderType.Replicate },
    };

    private static List<Entry> _entries = new List<Entry>();
    private static Emgu.CV.CvEnum.BorderType? _selectedEdge = null;
    private static float[,]? _selectedFilter = null;
    private static int _matrixSize;

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
        _matrixSize = 3;
        //SizePicker.SelectedIndex = 0;
        CreateMatrix(_matrixSize, false);

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
            ClearEntriesValues_ResetCheckedValues();
            foreach(Entry entry in _entries) entry.IsEnabled = true;
          
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
            ClearEntriesValues_ResetCheckedValues();
            _matrixSize = 3; //def val
            CreateMatrix(_matrixSize, false);

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
    private void ClearEntriesValues_ResetCheckedValues()
    {
        foreach(Entry e in _entries) e.Text = null;
        //Disabling events in order to change selected index without calling method
        FilterPicker.SelectedIndexChanged -= OnFilterPickerSelectedIndexChanged;
        SizePicker.SelectedIndexChanged -= OnSizePickerSelectedIndexChanged;
        EdgePicker.SelectedIndexChanged -= OnEdgePickerSelectedIndexChanged;

        FilterPicker.SelectedIndex = -1;
        EdgePicker.SelectedIndex = -1;
        SizePicker.SelectedIndex = -1;
        //Re-enabling events
        FilterPicker.SelectedIndexChanged += OnFilterPickerSelectedIndexChanged;
        SizePicker.SelectedIndexChanged += OnSizePickerSelectedIndexChanged;
        EdgePicker.SelectedIndexChanged += OnEdgePickerSelectedIndexChanged;
    }
    private void OnSizePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if(IsCustomKernel.IsChecked == true && int.TryParse(SizePicker.SelectedItem.ToString(), out int size)){
            _matrixSize = size;
        }
        CreateMatrix(_matrixSize, true);
    }
    private void CreateMatrix(int size, bool editable)
    {
        MatrixGrid.RowDefinitions.Clear();
        MatrixGrid.ColumnDefinitions.Clear();
        MatrixGrid.ClearLogicalChildren();
        MatrixGrid.Clear();
        _entries.Clear();

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
                e.IsEnabled = editable;
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
        if (rows == cols && kernel.GetLength(0) == kernel.GetLength(1) && kernel.GetLength(0) == _matrixSize)
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
    public async void OnKernelButtonClicked(object sender, EventArgs e)
	{
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
        if (_selectedEdge == null)
        {
            await DisplayAlert("Alert", "Border type not selected", "Ok");
            return;
        }
        if (IsCustomKernel.IsChecked == false)
        {
            if (_selectedFilter == null)
            {
                await DisplayAlert("Alert", "Kernel/Filter not selected", "Ok");
                return;
            }
            Main.ApplyKernel(index, _selectedFilter, (Emgu.CV.CvEnum.BorderType)_selectedEdge);
        }
        else
        {
            //Todo
        }

    }

}
