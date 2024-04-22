
using Emgu.CV.CvEnum;
using Emgu.CV.Ocl;
using System.Diagnostics;

namespace APOMaui;

public partial class Kernels : ContentPage
{ 
    private static readonly List<String> _borderOptions = new List<String> { "Isolated", "Reflect", "Replicate" };
    private static readonly List<String> _kernelSizes = new() { "3", "5", "7" };
    private static readonly List<String> _filtersOptions = new List<String> {"Median", "Blur", "GaussianBlur", "SobelX","SobelY", "LaplacianEdge", "Canny","LaplacianSharpen1",
        "LaplacianSharpen2", "LaplacianSharpen3", "PrewittE", "PrewittSE", "PrewittS", "PrewittSW", "PrewittW", "PrewittNW", "PrewittN", "PrewittNE"};
    private static readonly Dictionary<String, float[,]> _filtersDictionary = new() 
	{
        {"Median",
            new float[,]
            {
                {},
                {},
                {}
            }},
        {"Canny",
            new float[,]
            {
                {},
                {},
                {}
            }},
        {"GaussianBlur",
            new float[,]
            {
                {},
                {},
                {}
            }},
        {"Blur",
            new float[,]
            {
                {},
                {},
                {}
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
                {-1f, 5f,1f},
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
    private static readonly Dictionary<String, Emgu.CV.CvEnum.BorderType> _bordersDictionary = new()
    {
        {"Isolated", Emgu.CV.CvEnum.BorderType.Isolated },
        {"Reflect", Emgu.CV.CvEnum.BorderType.Reflect },
        {"Replicate", Emgu.CV.CvEnum.BorderType.Replicate },
    };

    private static List<Entry> _entries = new List<Entry>();
    private static Emgu.CV.CvEnum.BorderType? _selectedBorder = null;
    private static float[,]? _selectedFilter = null;
    private static string? _selectedBuiltInFilter = null;
    private static int _matrixSize;

    public Kernels()
	{
		InitializeComponent();
        SizePicker.IsEnabled = false;
        SizePicker.IsVisible = false;
        SizePickerLabel.IsEnabled = false;
        SizePickerLabel.IsVisible = false;
		EdgePicker.ItemsSource = _borderOptions;
		FilterPicker.ItemsSource = _filtersOptions;
        SizePicker.ItemsSource = _kernelSizes;
        _selectedBorder = null;
        _selectedFilter = null;
        _matrixSize = 3;
        //SizePicker.SelectedIndex = 0;
        CreateMatrix(_matrixSize, false);

    }
    private void OnFilterPickerSelectedIndexChanged(object sender, EventArgs e)
    {


        if (_filtersDictionary[FilterPicker.SelectedItem.ToString()].GetLength(0) == 3 && _filtersDictionary[FilterPicker.SelectedItem.ToString()].GetLength(1) == 3)
        {
            _selectedFilter = _filtersDictionary[FilterPicker.SelectedItem.ToString()];
            _selectedBuiltInFilter = null;
            FillKernel(_selectedFilter);
            if (FilterPicker.SelectedItem.ToString() == "SobelX" || FilterPicker.SelectedItem.ToString() == "SobelY")
            {
                _selectedBuiltInFilter = FilterPicker.SelectedItem.ToString();
                _selectedFilter = null;
            }
        }
        else
        {
            _selectedFilter = null;
            _selectedBuiltInFilter = FilterPicker.SelectedItem.ToString();
            foreach (Entry entry in _entries) entry.Text = null;
        }
        System.Diagnostics.Debug.WriteLine(_selectedFilter);
        System.Diagnostics.Debug.WriteLine(_selectedBuiltInFilter);

    }
    private void OnEdgePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (EdgePicker.SelectedItem != null && EdgePicker.SelectedIndex != -1)
        {
            _selectedBorder = _bordersDictionary[EdgePicker.SelectedItem.ToString()];
            System.Diagnostics.Debug.WriteLine(_selectedBorder);
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
        if (rows == cols && kernel.GetLength(0) == kernel.GetLength(1) && kernel.GetLength(0) == _matrixSize && rows == _matrixSize)
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
        } else
        {
            foreach (Entry e in _entries) e.Text = null;
        }
    }
    private float[,] ReadKernel()
    {
        float[,] kernel = new float[_matrixSize, _matrixSize];
        int rows = MatrixGrid.RowDefinitions.Count;
        int cols = MatrixGrid.ColumnDefinitions.Count;

        if (rows == cols && rows == _matrixSize && (_matrixSize*_matrixSize) == _entries.Count)
        {
            int size = cols;
            int krow = 0;
            int kcol = 0;
            for (int i = 0; i < _entries.Count; i++)
            {
                if (kcol == size)
                {
                    krow++;
                    kcol = 0;
                }
                //_entries[i].Text = kernel[krow, kcol].ToString();
                if (!float.TryParse(_entries[i].Text, out kernel[krow, kcol])) return null;
                kcol++;
            }
        }
        else return null;

        return kernel;
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
        if (_selectedBorder == null)
        {
            await DisplayAlert("Alert", "Border type not selected", "Ok");
            return;
        }
        switch (IsCustomKernel.IsChecked)
        {
            case true:
                    float[,] customKernel = new float[_matrixSize, _matrixSize];
                    customKernel = ReadKernel();
                    if (customKernel == null)
                    {
                        await DisplayAlert("Alert", "Entered kernel not valid", "Ok");
                        return;
                    }
                    Main.ApplyKernel(index, customKernel, (BorderType)_selectedBorder);
                break;
            case false:
                if (_selectedFilter == null && _selectedBuiltInFilter == null)
                {
                    await DisplayAlert("Alert", "Kernel/Filter not selected", "Ok");
                    return;
                }
                switch (_selectedBuiltInFilter)
                {
                    case "SobelX":
                            Main.EdgeDetectionFilters(index, BuiltInFilters.SobelX, (BorderType)_selectedBorder, -1, -1);
                        break;
                    case "SobelY":
                            Main.EdgeDetectionFilters(index, BuiltInFilters.SobelY, (BorderType)_selectedBorder, -1, -1);
                        break;
                    case "LaplacianEdge":
                            Main.EdgeDetectionFilters(index, BuiltInFilters.LaplacianEdge, (BorderType)_selectedBorder, -1, -1);
                        break;
                    case "Canny":
                            int ths1, ths2;
                            if (!int.TryParse(await DisplayPromptAsync("Threshold 1", "Type Threshold 1 value"), out ths1))
                            {
                                await DisplayAlert("Alert", "Threshold 1 value not valid", "Ok");
                                return;
                            }
                            if (!int.TryParse(await DisplayPromptAsync("Threshold 2", "Type Threshold 2 value"), out ths2))
                            {
                                await DisplayAlert("Alert", "Threshold 2 value not valid", "Ok");
                                return;
                            }
                            Main.EdgeDetectionFilters(index, BuiltInFilters.Canny, (BorderType)_selectedBorder, ths1, ths2);
                        break;
                    case "Median":
                            string[] options = {"3","5","7"};
                            string picked = await DisplayActionSheet("Pick size", "Cancel", null, options);
                            if (!int.TryParse(picked, out int ksize))
                            {
                                await DisplayAlert("Alert", "Error, kernel size not valid", "Ok");
                                return;
                            }
                            Main.MedianFilter(index, ksize, (BorderType)_selectedBorder, (int)(ksize/2));
                        break;
                    case "GaussianBlur":
                            Main.BlurFilrers(index, BuiltInFilters.GaussianBlur, (BorderType)_selectedBorder);
                        break;
                    case "Blur":
                            Main.BlurFilrers(index, BuiltInFilters.Blur, (BorderType)_selectedBorder);
                        break;
                    default:
                            Main.ApplyKernel(index, _selectedFilter, (BorderType)_selectedBorder);
                        break;
                }
                break;
        }
    }

}
