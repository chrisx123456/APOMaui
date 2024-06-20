using Emgu.CV;
using Emgu.CV.Structure;
using System.Reflection;

namespace APOMaui;

public partial class ThresholdingTab : ContentPage
{
    private static List<string> _types = new List<string> { "Manual", "Adaptive", "Otsu" };
    private ThreshType? _selected = null;
    private Image<Gray, Byte>? _img = null;
	//private int? _imgindex = null;
	private int _slider1Value = 0;
    private int _slider2Value = 0;
    public ThresholdingTab()
	{
		InitializeComponent();
        this.ThreshTypePicker.SelectedIndex = 0;
        AddPickersItems();
        UpdateInternalImage();
        ResetSlider();
        WindowFileManager.BeforeClosingEvent += this.UpdateInternalImage; //Cancel preview etc.
        //WindowFileManager.OnImageClosingOpeningEvent += this.DisableAll;
        WindowFileManager.OnImageClosingEvent += this.DisableAll; //disable all when img closed
        WindowFileManager.OnImageOpeningEvent += this.UpdateInternalImage; //when new image updare, but it's doubled i think cuz' when new img is opened, index changes so selection event fires
        WindowFileManager.OnImageSelectionChanged += this.UpdateInternalImage; //New img selected
	}
	protected override void OnDisappearing()
	{
		base.OnDisappearing(); 
        if(WindowFileManager.OpenedImagesList.Count != 0) OnButtonThreshCancelClicked(new object(), new EventArgs());
    }
    private void UpdateInternalImage()
	{
		System.Diagnostics.Debug.WriteLine("Thresh Updating img");
        ResetSlider();
        OnButtonThreshCancelClicked(new object(), new EventArgs()); //Sets this interal image in ImagePage, in this case resets the base image
        if (WindowFileManager.selectedWindow != null && WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.Type == ImgType.Gray)
		{
            this._img = WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.GrayImage.Clone();
			this.ThreshTypePicker.IsEnabled = true;
            this.Slider1.IsEnabled = true;
            this.Slider2.IsEnabled = true;
            this.ButtonThreshAccept.IsEnabled = true;
			this.ButtonThreshCancel.IsEnabled = true;
			this.ButtonThreshPreview.IsEnabled = true;
        }
        else DisableAll();
    }
    private void AcceptImage()
    {
        System.Diagnostics.Debug.WriteLine("Thresh Updating MAIN img");
        ResetSlider();
        if (WindowFileManager.selectedWindow != null && WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.Type == ImgType.Gray)
        {
            this._img = WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.GrayImage.Clone();
            this.ThreshTypePicker.IsEnabled = true;
            this.Slider1.IsEnabled = true;
            this.Slider2.IsEnabled = true;
            this.ButtonThreshAccept.IsEnabled = true;
            this.ButtonThreshCancel.IsEnabled = true;
            this.ButtonThreshPreview.IsEnabled = true;
            OnButtonThreshCancelClicked(new object(), new EventArgs()); //Sets this interal image in ImagePage
        }
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

    private  void OnButtonThreshPreviewClicked(object sender, EventArgs e)
	{
        if (WindowFileManager.selectedWindow == null || WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.Type == ImgType.RGB)
        {
            return;
        }
        if (this._img != null && WindowFileManager.selectedWindow != null && this._selected != null)
        {
            switch (_selected)
            {
                case ThreshType.MANUAL:
                    ImageProc.Thresh(this._img, (int)WindowFileManager.selectedWindow, ThreshType.MANUAL, _slider1Value, _slider2Value);
                    break;
                case ThreshType.ADAPTIVE:
                    ImageProc.Thresh(this._img, (int)WindowFileManager.selectedWindow, ThreshType.ADAPTIVE, -1, -1);
                    break;
                case ThreshType.OTSU:
                    ImageProc.Thresh(this._img, (int)WindowFileManager.selectedWindow, ThreshType.OTSU, -1, -1);
                    break;
            }
        }
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
		}
	}
	private void OnButtonThreshCancelClicked(object sender, EventArgs e)
	{
        if(WindowFileManager.selectedWindow == null || WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.Type == ImgType.RGB)
        {
            return;
        }
        if (this._img != null && WindowFileManager.selectedWindow != null)
		{
            WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.GrayImage = this._img.Clone();
		}
		ResetSlider();
    }
    private void OnButtonThreshAcceptClicked(object sender, EventArgs e)
    {
        if (WindowFileManager.selectedWindow == null || WindowFileManager.OpenedImagesList[(int)WindowFileManager.selectedWindow].CollectivePage.ImagePage.Type == ImgType.RGB)
        {
            return;
        }
        OnButtonThreshPreviewClicked(sender, e);
        AcceptImage();
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
        this._img = null;
    }
}