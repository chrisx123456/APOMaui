
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace APOMaui;

public partial class KernelTab : ContentPage
{
    private static readonly Dictionary<string, float[,]> _filtersDictionary = new() 
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
                { 1f, 2f, 1f },
                { 2f, 4f, 2f },
                { 1f, 2f, 1f }
            }},
        {"Blur",
            new float[,]
            {
                { 1f, 1f, 1f },
                { 1f, 1f, 1f },
                { 1f, 1f, 1f }
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
                {0f,-1f,0f},
                {-1f, 5f,-1f},
                {0f,-1f,0f}
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
                { 0f, 0f, 0f},
                {-1f,-1f,-1f}
            }},
        {"PrewittNE",
            new float[,]
            {
                { 0f, 1f,1f},
                {-1f, 0f,1f},
                {-1f,-1f,0f}
            }},

        {"PrewittE",
            new float[,]
            {
                {-1f,0f,1f},
                {-1f,0f,1f},
                {-1f,0f,1f}
            }},

        {"PrewittSE",
            new float[,]
            {
                {-1f,-1f,0f},
                {-1f, 0f,1f},
                { 0f, 1f,1f}
            }},

        {"PrewittS",
            new float[,]
            {
                {-1f,-1f,-1f},
                { 0f, 0f, 0f},
                { 1f, 1f, 1f}
            }},

        {"PrewittSW",
            new float[,]
            {
                {0f,-1f,-1f},
                {1f, 0f,-1f},
                {1f, 1f, 0f}
            }},
        {"PrewittW",
            new float[,]
            {
                {1f, 0f, -1f},
                {1f, 0f, -1f},
                {1f, 0f, -1f}
            }},

        {"PrewittNW",
            new float[,]
            {
                {1f, 1f, 0f},
                {1f, 0f,-1f},
                {0f,-1f,-1f}
            }},

    };
    private static readonly Dictionary<string, Emgu.CV.CvEnum.BorderType> _bordersDictionary = new()
    {
        {"Isolated", Emgu.CV.CvEnum.BorderType.Isolated },
        {"Reflect", Emgu.CV.CvEnum.BorderType.Reflect },
        {"Replicate", Emgu.CV.CvEnum.BorderType.Replicate },
    };

    private static List<Entry> _entries = new List<Entry>();
    private static List<Entry> _entries2 = new List<Entry>();
    private static List<Entry> _entries3 = new List<Entry>();


    private static Emgu.CV.CvEnum.BorderType? _selectedBorder = null;
    private static float[,]? _selectedFilter = null;
    private static string? _selectedBuiltInFilter = null;

    private static string? _stage1SelectedFilter = null;
    private static string? _stage2SelectedFilter = null;

    private static int _matrixSize;

    public KernelTab()
	{
		InitializeComponent();
        ViewInit();
    }
    private void ViewInit()
    {
        SizePicker.IsVisible = false;
        SizePickerLabel.IsVisible = false;
        MatrixGrid2Layout.IsVisible = false;
        MatrixGrid3.IsVisible = false;
        Stage2Label.IsVisible = false;
        _selectedBorder = null;
        _selectedFilter = null;
        _matrixSize = 3;

        EdgePicker.ItemsSource = new List<string> { "Isolated", "Reflect", "Replicate" };
        SizePicker.ItemsSource = new List<string> { "3", "5", "7" };
        Stage1Picker.ItemsSource = new List<string> { "Blur", "GaussianBlur" };
        Stage2Picker.ItemsSource = new List<string> { "LaplacianSharpen1", "LaplacianSharpen2", "LaplacianSharpen3" };
        foreach(KeyValuePair<string, float[,]> el in _filtersDictionary)
        {
            FilterPicker.Items.Add(el.Key);
        }
        CreateMatrix(_matrixSize, false, ref this.MatrixGrid, ref _entries);
        CreateMatrix(3, false, ref this.MatrixGrid2, ref _entries2);
        CreateMatrix(5, false, ref this.MatrixGrid3, ref _entries3);
    }

    private void IsTwoStageCheckedChanged(object sender, EventArgs e)
    {
        if(IsTwoStage.IsChecked == true)
        {
            ClearEntriesValues_ResetCheckedValues();

            IsCustomKernel.IsChecked = false;

            FilterPicker.IsVisible = false;
            FilterPickerLabel.IsVisible = false;

            Stage1Picker.IsVisible = true;
            Stage2Picker.IsVisible = true;
            Stage1PickerLabel.IsVisible = true;
            Stage2PickerLabel.IsVisible = true;

            Stage2Label.IsVisible = true;
            MatrixGrid2Layout.IsVisible = true;
            MatrixGrid3.IsVisible = true;
        } 
        else
        {
            ClearEntriesValues_ResetCheckedValues();

            FilterPicker.IsVisible = true;
            FilterPickerLabel.IsVisible = true;
            MatrixGrid.IsVisible = true;

            Stage1Picker.IsVisible = false;
            Stage2Picker.IsVisible = false;
            Stage1PickerLabel.IsVisible = false;
            Stage2PickerLabel.IsVisible = false;
            Stage2Label.IsVisible = false; //kernel label

            MatrixGrid2Layout.IsVisible = false;
            MatrixGrid3.IsVisible = false;
        }
    }
    private void IsCustomKernelCheckedChanged(object sender, EventArgs e)
    {
        if (IsCustomKernel.IsChecked == true)
        {
            SizePicker.SelectedIndex = 0;
            
            IsTwoStage.IsChecked = false;
            ClearEntriesValues_ResetCheckedValues();
            foreach (Entry entry in _entries) entry.IsEnabled = true;

            FilterPicker.IsVisible = false;
            FilterPickerLabel.IsVisible = false;

            SizePicker.IsVisible = true;
            SizePickerLabel.IsVisible = true;

            Stage2Label.IsVisible = false;
        }
        else
        {
            ClearEntriesValues_ResetCheckedValues();
            _matrixSize = 3; //def val
            CreateMatrix(_matrixSize, false, ref MatrixGrid, ref _entries);

            FilterPicker.IsVisible = true;
            FilterPickerLabel.IsVisible = true;

            SizePicker.IsVisible = false;
            SizePickerLabel.IsVisible = false;
            Stage2Label.IsVisible = false;
        }
    }

    private void OnFilterPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (FilterPicker.SelectedIndex != -1 && FilterPicker.SelectedItem != null && 
            _filtersDictionary[FilterPicker.SelectedItem.ToString()].GetLength(0) == 3 && 
            _filtersDictionary[FilterPicker.SelectedItem.ToString()].GetLength(1) == 3) //Check if filter is EmguCv separate method
        {
            _selectedFilter = _filtersDictionary[FilterPicker.SelectedItem.ToString()];
            _selectedBuiltInFilter = null;
            FillKernel(_selectedFilter, ref _entries, ref this.MatrixGrid);
        }
        else
        {
            _selectedFilter = null;
            _selectedBuiltInFilter = FilterPicker.SelectedItem?.ToString();
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
    private void OnStage1PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if(Stage1Picker.SelectedItem != null && Stage1Picker.SelectedIndex != -1) 
        {
            _stage1SelectedFilter = Stage1Picker.SelectedItem.ToString();
            FillKernel(_filtersDictionary[_stage1SelectedFilter], ref _entries2, ref MatrixGrid2);
            Get5x5Kernel(true);
        }
    }
    private void OnStage2PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (Stage2Picker.SelectedItem != null && Stage2Picker.SelectedIndex != -1)
        {
            _stage2SelectedFilter = Stage2Picker.SelectedItem.ToString();
            FillKernel(_filtersDictionary[_stage2SelectedFilter], ref _entries, ref MatrixGrid);
            Get5x5Kernel(true);
        }
    }
    private void OnSizePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if(IsCustomKernel.IsChecked == true && int.TryParse(SizePicker.SelectedItem.ToString(), out int size)){
            _matrixSize = size;
        }
        CreateMatrix(_matrixSize, true, ref this.MatrixGrid, ref _entries);
    }

    private void ClearEntriesValues_ResetCheckedValues()
    {
        foreach (Entry e in _entries) e.Text = null;
        foreach (Entry e in _entries2) e.Text = null;
        foreach (Entry e in _entries3) e.Text = null;

        //Disabling events in order to change selected index without calling method
        FilterPicker.SelectedIndexChanged -= OnFilterPickerSelectedIndexChanged;
        SizePicker.SelectedIndexChanged -= OnSizePickerSelectedIndexChanged;
        EdgePicker.SelectedIndexChanged -= OnEdgePickerSelectedIndexChanged;
        Stage1Picker.SelectedIndexChanged -= OnStage1PickerSelectedIndexChanged;
        Stage2Picker.SelectedIndexChanged -= OnStage2PickerSelectedIndexChanged;

        FilterPicker.SelectedIndex = -1;
        EdgePicker.SelectedIndex = -1;
        SizePicker.SelectedIndex = -1;
        Stage1Picker.SelectedIndex = -1;
        Stage2Picker.SelectedIndex = -1;
        _selectedBorder = null;
         _selectedFilter = null;
        _selectedBuiltInFilter = null;
        _stage1SelectedFilter = null;
        _stage2SelectedFilter = null;
    //Re-enabling events
        FilterPicker.SelectedIndexChanged += OnFilterPickerSelectedIndexChanged;
        SizePicker.SelectedIndexChanged += OnSizePickerSelectedIndexChanged;
        EdgePicker.SelectedIndexChanged += OnEdgePickerSelectedIndexChanged;
        Stage1Picker.SelectedIndexChanged += OnStage1PickerSelectedIndexChanged;
        Stage2Picker.SelectedIndexChanged += OnStage2PickerSelectedIndexChanged;
    }
    private void CreateMatrix(int size, bool editable, ref Grid grid, ref List<Entry> listentry)
    {
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        grid.ClearLogicalChildren();
        grid.Clear();
        listentry.Clear();

        for (int i = 0; i < size; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
        for(int row = 0; row < size; row++)
        {
            for(int col = 0; col < size; col++)
            {
                Entry e = new Entry();
                e.Keyboard = Keyboard.Numeric;
                e.IsEnabled = editable;
                e.SetDynamicResource(Entry.StyleProperty, "EntryKernel");
                grid.Add(e, col, row);
                listentry.Add(e);
            }
        }
    }
    private void FillKernel(float[,] kernel, ref List<Entry> list, ref Grid grid)
    {
        int rows = grid.RowDefinitions.Count;
        int cols = grid.ColumnDefinitions.Count;
        if (rows == cols && kernel.GetLength(0) == rows && kernel.GetLength(0) == cols)
        {
            int size = cols;
            int krow = 0;
            int kcol = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if(kcol == size)
                {
                    krow++;
                    kcol = 0;
                }
                list[i].Text = kernel[krow, kcol].ToString();
                kcol++;
            }
        } else
        {
            foreach (Entry e in list) e.Text = null;
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
    private float[,] Get5x5Kernel(bool fill)
    {
        if (_stage1SelectedFilter == null || _stage2SelectedFilter == null) return null;
        foreach (Entry e in _entries3) e.Text = null;
        Matrix<float> stage1 = new Matrix<float>(_filtersDictionary[_stage1SelectedFilter]);
        Matrix<float> stage2 = new Matrix<float>(_filtersDictionary[_stage2SelectedFilter]);
        CvInvoke.CopyMakeBorder(stage1, stage1, 1, 1, 1, 1, BorderType.Constant, new Emgu.CV.Structure.MCvScalar(0, 0, 0, 0));
        CvInvoke.CopyMakeBorder(stage2, stage2, 1, 1, 1, 1, BorderType.Constant, new Emgu.CV.Structure.MCvScalar(0, 0, 0, 0));
        Matrix<float> res5x5 = new Matrix<float>(new float[5, 5]);
        CvInvoke.Filter2D(stage1, res5x5, stage2, new System.Drawing.Point(-1, -1), 0 , BorderType.Constant);
        if(fill) FillKernel(res5x5.Data, ref _entries3, ref MatrixGrid3);
        return res5x5.Data;
    }


    public async void OnKernelButtonClicked(object sender, EventArgs e)
    {
        if (WindowFileManager.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)WindowFileManager.selectedWindow;
        if (WindowFileManager.OpenedImagesList[index].CollectivePage.ImagePage.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
        if (_selectedBorder == null)
        {
            await DisplayAlert("Alert", "Border type not selected", "Ok");
            return;
        }

        BorderType border = (BorderType)_selectedBorder;

        if (IsCustomKernel.IsChecked == true && IsTwoStage.IsChecked == true)
        {
            await DisplayAlert("Error", "CustomKernel && TwoStage are True", "Ok");
            return;
        }
        
        if (IsCustomKernel.IsChecked == true)
        {
            ApplyCustomKernel(index, border);
        }

        if(IsTwoStage.IsChecked == true)
        {
            TwoStageFilter(index, border);
        }

        if (IsCustomKernel.IsChecked == false && IsTwoStage.IsChecked == false)
        {
            ApplyFilter(index, border);
        }

    }
    private async void TwoStageFilter(int index, BorderType border)
    {
        if (_stage1SelectedFilter == null || _stage2SelectedFilter == null)
        {
            await DisplayAlert("Alert", "Stage 1/2 not selected", "Ok");
            return;
        }
        float[,] stage1kernel;
        float[,] stage2kernel;
        stage1kernel = _filtersDictionary[_stage1SelectedFilter];
        stage2kernel = _filtersDictionary[_stage2SelectedFilter];
        string result = await DisplayActionSheet("Option", "Cancel", null, "2*3x3 Kernel", "5x5 Kernel");
        switch(result)
        {
            case "Cancel":
                return;
            case "2*3x3 Kernel":
                ImageProc.TwoStage233(index, border, new Matrix<float>(stage1kernel), new Matrix<float>(stage2kernel));
                break;
            case "5x5 Kernel":
                ImageProc.TwoStage55(index, border, new Matrix<float>(Get5x5Kernel(false)));
                break;
        }
    }
    private async void ApplyCustomKernel(int index, BorderType border)
    {
        float[,] customKernel = new float[_matrixSize, _matrixSize];
        customKernel = ReadKernel();
        if (customKernel == null)
        {
            await DisplayAlert("Alert", "Entered kernel not valid", "Ok");
            return;
        }
        ImageProc.ApplyKernel(index, customKernel, border);
    }
    private async void ApplyFilter(int index, BorderType border)
    {
        if (_selectedFilter == null && _selectedBuiltInFilter == null)
        {
            await DisplayAlert("Alert", "Kernel/Filter not selected", "Ok");
            return;
        }
        switch (_selectedBuiltInFilter)
        {
            case "Canny":
                int ths1, ths2;
                if (!int.TryParse(await DisplayPromptAsync("Threshold 1", "Type Threshold 1 value"), out ths1) && ths1>=0 && ths1<=255)
                {
                    await DisplayAlert("Alert", "Threshold 1 value not valid", "Ok");
                    return;
                }
                if (!int.TryParse(await DisplayPromptAsync("Threshold 2", "Type Threshold 2 value"), out ths2) && ths2 >= 0 && ths2 <= 255)
                {
                    await DisplayAlert("Alert", "Threshold 2 value not valid", "Ok");
                    return;
                }
                ImageProc.Canny(index, border, ths1, ths2);
                break;
            case "Median":
                string[] options = { "3", "5", "7" };
                string picked = await DisplayActionSheet("Pick size", "Cancel", null, options);
                if (!int.TryParse(picked, out int ksize))
                {
                    await DisplayAlert("Alert", "Error, kernel size not valid", "Ok");
                    return;
                }
                ImageProc.MedianFilter(index, ksize, border, (int)(ksize / 2));
                break;
            default:
                ImageProc.ApplyKernel(index, _selectedFilter, border);
                break;
        }
    }
}
