namespace APOMaui;

public partial class Thresholding : ContentPage
{
	private static List<string> _types = new List<string> { "Manual", "Adaptive", "Otsu" };
	private ThreshType? _selected = null;
	public Thresholding()
	{
		InitializeComponent();
		AddPickersItems();


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
        displayLabel.Text = String.Format("The Slider value is {0}", (int)val);
    }

    private void OnThreshTypePickerIndexChanged(object sender, EventArgs e)
	{
		switch (this.ThreshTypePicker.SelectedItem) 
		{
			case "Manual":
				_selected = ThreshType.MANUAL;
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
		
	}
    private void OnButtonThreshAcceptClicked(object sender, EventArgs e)
    {

    }
}