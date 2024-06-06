using Emgu.CV;
using Emgu.CV.Structure;

namespace APOMaui;

public partial class ThresholdingTab : ContentPage
{
	private static List<string> _types = new List<string> { "Manual", "Adaptive", "Otsu" };
	private ThreshType? _selected = null;
	private Image<Gray, Byte>? _img = null;
	private int? _imgindex = null;
	private int _slider1Value = 0;
    private int _slider2Value = 0;
    public ThresholdingTab()
	{
		InitializeComponent();
        AddPickersItems();
        this.ThreshTypePicker.SelectedIndex = 0;
        UpdateImg();
        ResetSlider();
        ImageProc.OnImageClosingOpeningEvent += this.UpdateImg;
		ImageProc.OnImageSelectionChanged += this.UpdateImg;
	}
	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		OnButtonThreshCancelClicked(new object(), new EventArgs());
	}

    private void UpdateImg()
	{
		System.Diagnostics.Debug.WriteLine("Thresh Updating img");
        ResetSlider();
        OnButtonThreshCancelClicked(new object(), new EventArgs());
        //this.ThreshTypePicker.SelectedIndex = 0;
        if (ImageProc.selectedWindow != null)
		{
			if (ImageProc.selectedWindow!= null && ImageProc.OpenedImagesList[(int)ImageProc.selectedWindow].ImagePage.Type == ImgType.Gray)
			{
                this._imgindex = ImageProc.selectedWindow;
                this._img = ImageProc.OpenedImagesList[(int)_imgindex].ImagePage.GrayImage.Clone();
				this.ThreshTypePicker.IsEnabled = true;
                this.Slider1.IsEnabled = true;
                this.Slider2.IsEnabled = true;
                this.ButtonThreshAccept.IsEnabled = true;
				this.ButtonThreshCancel.IsEnabled = true;
				this.ButtonThreshPreview.IsEnabled = true;
            }
			else DisableAll();
        } 
		else DisableAll();
    }
	private void AddPickersItems()
	{
		foreach (string type in _types)
		{
			this.ThreshTypePicker.Items.Add(type);
		}
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

    private void OnButtonThreshPreviewClicked(object sender, EventArgs e)
	{
        if (this._img != null && this._imgindex != null && this._selected != null)
        {
            switch (_selected)
            {
                case ThreshType.MANUAL:
                    ImageProc.Thresh(this._img, (int)this._imgindex, ThreshType.MANUAL, _slider1Value, _slider2Value);
                    break;
                case ThreshType.ADAPTIVE:
                    ImageProc.Thresh(this._img, (int)this._imgindex, ThreshType.ADAPTIVE, -1, -1);
                    break;
                case ThreshType.OTSU:
                    ImageProc.Thresh(this._img, (int)this._imgindex, ThreshType.OTSU, -1, -1);
                    break;
            }
        }
    }
    private void OnThreshTypePickerIndexChanged(object sender, EventArgs e)
	{
		OnButtonThreshCancelClicked(sender, e);
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
		}
	}
	private void OnButtonThreshCancelClicked(object sender, EventArgs e)
	{
        if (this._img != null && this._imgindex != null)
		{
			ImageProc.OpenedImagesList[(int)_imgindex].ImagePage.GrayImage = this._img.Clone();
		}
		ResetSlider();
    }
    private void OnButtonThreshAcceptClicked(object sender, EventArgs e)
    {
		OnButtonThreshPreviewClicked(sender, e);
        UpdateImg();
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
	private void DisableAll()
	{
        System.Diagnostics.Debug.WriteLine("Thresh nulling img");
        this.Slider1.IsEnabled = false;
        this.Slider2.IsEnabled = false;
        this.ThreshTypePicker.IsEnabled = false;
        this.ButtonThreshAccept.IsEnabled = false;
        this.ButtonThreshCancel.IsEnabled = false;
		this.ButtonThreshPreview.IsEnabled = false;
        this._imgindex = null;
        this._img = null;
    }
}