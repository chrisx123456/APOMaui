using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Reflection;

namespace APOMaui;

public partial class ThresholdingTab : ContentPage
{
    private Dictionary<string, int> _imageList = new Dictionary<string, int>();
    private static List<string> _types = new List<string> { "Manual", "Adaptive", "Otsu" };
    private ThreshType? _selected = null;
    private Image<Gray, Byte>? _img = null;
	private int? _imgindex = null;
	private int _slider1Value = 0;
    private int _slider2Value = 0;
    public ThresholdingTab()
	{
		InitializeComponent();
        this.ThreshTypePicker.SelectedIndex = 0;
        _imageList = GetImagesList();
        AddPickersItems();
        ResetSlider();
        SetAll(false);
        WindowFileManager.OnImageClosingEvent += Update;
        WindowFileManager.OnImageOpeningEvent += Update;
        WindowFileManager.RGBToGrayConvertedEvent += Update;
	}
	protected override void OnDisappearing()
	{
		base.OnDisappearing();
        OnButtonThreshCancelClicked(new object(), new EventArgs());
        SetAll(false);

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

    }
    private void Update()
    {
        _imageList.Clear();
        _imageList = GetImagesList();
        AddPickersItems();
        _imgindex = null;
        _img = null;
        _selected = null;
        this.ThreshTypePicker.SelectedIndex = 0;
        this.imgPicker.SelectedIndexChanged -= imgPicker_SelectedIndexChanged;
        this.imgPicker.SelectedIndex = -1;
        this.imgPicker.SelectedIndexChanged += imgPicker_SelectedIndexChanged;
        ResetSlider();

    }
    private void imgPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        OnButtonThreshCancelClicked(sender, new EventArgs());
        this._imgindex = _imageList[imgPicker.SelectedItem.ToString()];
        SetAll(true);
        UpdateInternalImage();
    }

    private Dictionary<string, int> GetImagesList()
    {
        Dictionary<string, int> res = new();
        foreach (WindowImageObject wio in WindowFileManager.OpenedImagesList)
        {
            if(wio.CollectivePage.ImagePage.Type == ImgType.Gray)
            {
                string s = wio.CollectivePage.Title;
                int i = wio.CollectivePage.ImagePage.index;
                res.Add(s, i);
            }
        }
        return res;
    }
    private void AddPickersItems()
    {
        this.ThreshTypePicker.Items.Clear();
        this.imgPicker.Items.Clear();
        foreach (string type in _types)
        {
            this.ThreshTypePicker.Items.Add(type);
        }
        foreach (string s in _imageList.Keys)
        {
            this.imgPicker.Items.Add(s);
        }
    }
    private void UpdateInternalImage()
	{
        this._img = WindowFileManager.OpenedImagesList[(int)this._imgindex].CollectivePage.ImagePage.GrayImage.Clone();
    }


	private void OnSlider1ValueChanged(object sender, ValueChangedEventArgs e)
	{
		int val = (int)e.NewValue;
		_slider1Value = val;
        displayLabel1.Text = String.Format("The T1 value is {0}", (int)val);

    }
    private void OnSlider2ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int val = (int)e.NewValue;
        _slider2Value = val;
        displayLabel2.Text = String.Format("The MaxValue value is {0}", (int)val);

    }

    private  void OnButtonThreshPreviewClicked(object sender, EventArgs e)
	{
        if (this._img != null && this._imgindex != null && this._selected != null)
        {
            switch (_selected)
            {
                case ThreshType.MANUAL:
                    ImageProc.Thresh(this._img, (int)_imgindex, ThreshType.MANUAL, _slider1Value, _slider2Value);
                    break;
                case ThreshType.ADAPTIVE:
                    ImageProc.Thresh(this._img, (int)_imgindex, ThreshType.ADAPTIVE, -1, -1);
                    break;
                case ThreshType.OTSU:
                    ImageProc.Thresh(this._img, (int)_imgindex, ThreshType.OTSU, -1, -1);
                    break;
            }
        }
    }
    private void OnButtonThreshCancelClicked(object sender, EventArgs e)
    {
        if (this._imgindex != null && this._img != null)
        {
            WindowFileManager.OpenedImagesList[(int)_imgindex].CollectivePage.ImagePage.GrayImage = this._img.Clone();
        }
        ResetSlider();
    }
    private void OnButtonThreshAcceptClicked(object sender, EventArgs e)
    {
        if (this._imgindex == null)
        {
            return;
        }
        OnButtonThreshPreviewClicked(sender, e);
        WindowFileManager.OpenedImagesList[(int)this._imgindex].CollectivePage.ImagePage.GrayImage = this._img.Clone();
        UpdateInternalImage();
    }
    private void OnThreshTypePickerIndexChanged(object sender, EventArgs e)
	{
		OnButtonThreshCancelClicked(sender, e); //Sets this interal image in ImagePage, in this case resets the base image
        this.Slider1.IsEnabled = false;
        this.Slider1.IsVisible = false;
        this.displayLabel1.IsVisible = false;
        this.Slider2.IsEnabled = false;
        this.Slider2.IsVisible = false;
        this.displayLabel2.IsVisible = false;
        switch (this.ThreshTypePicker.SelectedItem) 
		{
			case "Manual":
				_selected = ThreshType.MANUAL;
				this.Slider1.IsEnabled = true;
				this.Slider1.IsVisible = true;
				this.displayLabel1.IsVisible = true;
                this.Slider2.IsEnabled = true;
                this.Slider2.IsVisible = true;
                this.displayLabel2.IsVisible = true;
                break;
			case "Adaptive":
				_selected = ThreshType.ADAPTIVE;
				break;
			case "Otsu":
				_selected = ThreshType.OTSU;
                break;
            default:
                return;
               
		}
	}


	private void ResetSlider()
	{
		this.Slider1.ValueChanged -= OnSlider1ValueChanged;
        this.Slider2.ValueChanged -= OnSlider2ValueChanged;

        this.Slider1.Value = 0; //Visual
		this._slider1Value = 0; //In-code value
        displayLabel1.Text = String.Format("The T1 value is {0}", 0);

        this.Slider2.Value = 0; //Visual
        this._slider2Value = 0; //In-code value
        displayLabel2.Text = String.Format("The MaxValue value is {0}", 0);

        this.Slider1.ValueChanged += OnSlider1ValueChanged;
        this.Slider2.ValueChanged += OnSlider2ValueChanged;
    }
	private void SetAll(bool disableOrEnable)
	{
        this.Slider1.IsEnabled = disableOrEnable;
        this.Slider2.IsEnabled = disableOrEnable;
        this.ThreshTypePicker.IsEnabled = disableOrEnable;
        this.ButtonThreshAccept.IsEnabled = disableOrEnable;
        this.ButtonThreshCancel.IsEnabled = disableOrEnable;
		this.ButtonThreshPreview.IsEnabled = disableOrEnable;
        if (!disableOrEnable)
        {
            this._selected = null;
            this._imgindex = null;
            this._img = null;
            this.imgPicker.SelectedIndexChanged -= imgPicker_SelectedIndexChanged;
            this.imgPicker.SelectedIndex = -1;
            this.ThreshTypePicker.SelectedIndex = 0;
            this.imgPicker.SelectedIndexChanged += imgPicker_SelectedIndexChanged;
        }
    }


}