using Emgu.CV;
using Emgu.CV.Structure;

namespace APOMaui;

public partial class Thresholding : ContentPage
{
	private static List<string> _types = new List<string> { "Manual", "Adaptive", "Otsu" };
	private ThreshType? _selected = null;
	private Image<Gray, Byte>? _img = null;
	private int? _imgindex = null;
	private int _sliderValue = 0;
	public Thresholding()
	{
		InitializeComponent();
        AddPickersItems();
        this.ThreshTypePicker.SelectedIndex = 0;
        UpdateImg();
        ResetSlider();
        Main.OnWinIMGClosingOpeningEvent += this.UpdateImg;
		Main.OnWinIMGSelectionChanged += this.UpdateImg;
	}
	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		OnButtonThreshCancelClicked(new object(), new EventArgs());
	}

    private void UpdateImg()
	{
		System.Diagnostics.Debug.WriteLine("Thresh Updating img");
		if(Main.selectedWindow != null)
		{
			if (Main.selectedWindow!= null && Main.OpenedImagesWindowsList[(int)Main.selectedWindow].winImg.Type == ImgType.Gray)
			{
                this._imgindex = Main.selectedWindow;
                this._img = Main.OpenedImagesWindowsList[(int)_imgindex].winImg.GrayImage.Clone();
				this.ThreshTypePicker.IsEnabled = true;
                this.Slider.IsEnabled = true;
				this.ButtonThreshAccept.IsEnabled = true;
				this.ButtonThreshCancel.IsEnabled = true;
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
	private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
	{
		int val = (int)e.NewValue;
		_sliderValue = val;
        displayLabel.Text = String.Format("The Slider value is {0}", (int)val);

    }

	private void OnButtonThreshPreviewClicked(object sender, EventArgs e)
	{
        if (this._img != null && this._imgindex != null && this._selected != null)
        {
            switch (_selected)
            {
                case ThreshType.MANUAL:
                    Main.Thresh(this._img, (int)this._imgindex, ThreshType.MANUAL, _sliderValue, ActionType.PREVIEW);
                    break;
                case ThreshType.ADAPTIVE:
                    Main.Thresh(this._img, (int)this._imgindex, ThreshType.ADAPTIVE, -1, ActionType.PREVIEW);
                    break;
                case ThreshType.OTSU:
                    Main.Thresh(this._img, (int)this._imgindex, ThreshType.OTSU, -1, ActionType.PREVIEW);
                    break;
            }
        }
    }
    private void OnThreshTypePickerIndexChanged(object sender, EventArgs e)
	{
		OnButtonThreshCancelClicked(sender, e);
        this.Slider.IsEnabled = false;
        this.Slider.IsVisible = false;
        this.displayLabel.IsVisible = false;
        switch (this.ThreshTypePicker.SelectedItem) 
		{
			case "Manual":
				_selected = ThreshType.MANUAL;
				this.Slider.IsEnabled = true;
				this.Slider.IsVisible = true;
				this.displayLabel.IsVisible = true;
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
			Main.OpenedImagesWindowsList[(int)_imgindex].winImg.GrayImage = this._img.Clone();
		}
		ResetSlider();
    }
    private void OnButtonThreshAcceptClicked(object sender, EventArgs e)
    {
		ResetSlider();
        UpdateImg();
    }
	private void ResetSlider()
	{
		this.Slider.ValueChanged -= OnSliderValueChanged;
        this.Slider.Value = 0; //Visual
		this._sliderValue = 0; //In-code value
        displayLabel.Text = String.Format("The Slider value is {0}", 0);
        this.Slider.ValueChanged += OnSliderValueChanged;
    }
	private void DisableAll()
	{
        System.Diagnostics.Debug.WriteLine("Thresh nulling img");
        this.Slider.IsEnabled = false;
        this.ThreshTypePicker.IsEnabled = false;
        this.ButtonThreshAccept.IsEnabled = false;
        this.ButtonThreshCancel.IsEnabled = false;
        this._imgindex = null;
        this._img = null;
    }
}