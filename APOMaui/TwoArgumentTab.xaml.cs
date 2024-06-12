using Emgu.CV;
using System.Diagnostics;

namespace APOMaui;

public partial class TwoArgumentTab : ContentPage
{
    private readonly List<string> _operations = new List<string>() {"ADD","SUBSTRACT","BLEND","AND","OR","NOT","XOR"};
    private Dictionary<string, int> _imageList = new Dictionary<string, int>();
	private TwoArgsOps? _selectedOperation = null;
	private int? _selectedImage1 = null;
	private int? _selectedImage2 = null;
	public TwoArgumentTab()
	{
		InitializeComponent();
        OperationPicker.ItemsSource = _operations;
		_imageList = GetImagesList();
		AddPickersItems();
        WindowFileManager.OnImageClosingOpeningEvent += this.UpdatePickersItems;
        //Zmienic w kernelach zeby wsm tak samo sie do pickera dodawalo jak tutaj

    }
	private void UpdatePickersItems()
	{
		BindingContext = null;
		_imageList.Clear();
        ResetPickersItems();

        _imageList = GetImagesList();
		AddPickersItems();
		BindingContext = this;
	}
	private void ResetPickersItems()
	{
		Img1Picker.Items.Clear();
        Img2Picker.Items.Clear();
		_selectedImage1 = null;
		_selectedImage2 = null;
		_selectedOperation = null;
    }
	private void AddPickersItems()
	{
        foreach(string s in _imageList.Keys)
        {
			Img1Picker.Items.Add(s);
            Img2Picker.Items.Add(s);
        }
    }
    private Dictionary<string, int> GetImagesList()
	{
		Dictionary<string, int> res = new();
        foreach (WindowImageObject wio in WindowFileManager.OpenedImagesList)
		{
			string s = wio.CollectivePage.Title;
			int i = wio.CollectivePage.ImagePage.index;
			res.Add(s, i);
		}
		return res;
	}
	private void OnImg1PickerSelectedIndexChanged(object sender, EventArgs e)
	{
		if(Img1Picker.SelectedItem != null) _selectedImage1 = _imageList[Img1Picker.SelectedItem.ToString()];
    }
    private void OnImg2PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (Img2Picker.SelectedItem != null) _selectedImage2 = _imageList[Img2Picker.SelectedItem.ToString()];
    }
    private void OnOperationPickerSelectedIndexChanged(object sender, EventArgs e)
    {
		switch(OperationPicker.SelectedItem)
		{
			case "ADD":
				_selectedOperation = TwoArgsOps.ADD;
                Img2Picker.IsEnabled = true;
                break;
			case "SUBSTRACT":
				_selectedOperation = TwoArgsOps.SUBTRACT;
                Img2Picker.IsEnabled = true;
                break;
			case "BLEND":
				_selectedOperation = TwoArgsOps.BLEND;
                Img2Picker.IsEnabled = true;
                break;
			case "AND":
				_selectedOperation = TwoArgsOps.AND;
                Img2Picker.IsEnabled = true;
                break;
			case "OR":
				_selectedOperation = TwoArgsOps.OR;
                Img2Picker.IsEnabled = true;
                break;
			case "NOT":
				_selectedOperation = TwoArgsOps.NOT;
                Img2Picker.IsEnabled = false;
				_selectedImage2 = null;
				Img2Picker.SelectedIndex = -1;
                break;
			case "XOR":
				_selectedOperation = TwoArgsOps.XOR;
                Img2Picker.IsEnabled = true;
                break;
			default:
				_selectedOperation = null;
				break;
		}
    }
    private async void OnTwoArgumentButtonClicked(object sender, EventArgs e)
    {
		if(_selectedImage1 == null || (_selectedImage2 == null && Img2Picker.IsEnabled==true) || _selectedOperation == null)
		{
			await DisplayAlert("Alert", "Image 1/2 or operation not selected", "Ok");
			return;
		}
        if (WindowFileManager.OpenedImagesList[(int)_selectedImage1].CollectivePage.ImagePage.Type != ImgType.Gray 
			|| (_selectedImage2 != null && WindowFileManager.OpenedImagesList[(int)_selectedImage2].CollectivePage.ImagePage.Type != ImgType.Gray))
        {
            await DisplayAlert("Alert", "Selected image 1/2 is not GrayScale", "Ok");
            return;
        }
        if (_selectedOperation == TwoArgsOps.BLEND)
		{
            if (!double.TryParse(await DisplayPromptAsync("Weight 1", "Type Weigt 1 value"), out double w1) && w1 >= 0 && w1 <= 1)
			{
                await DisplayAlert("Alert", "Weight 1 value not valid", "Ok");
                return;
            }
            if (!double.TryParse(await DisplayPromptAsync("Weight 2", "Type Weigt 2 value"), out double w2) && w2 >= 0 && w2 <= 1)
            {
                await DisplayAlert("Alert", "Weight 2 value not valid", "Ok");
                return;
            }
            ImageProc.TwoArgsOperations((int)_selectedImage1, (int)_selectedImage2, (TwoArgsOps)_selectedOperation, w1, w2);
        }
		else if(_selectedOperation == TwoArgsOps.NOT)
		{
            ImageProc.TwoArgsOperations((int)_selectedImage1, (int)_selectedImage1, (TwoArgsOps)_selectedOperation, -1, -1);
        }
        else
		{
			ImageProc.TwoArgsOperations((int)_selectedImage1, (int)_selectedImage2, (TwoArgsOps)_selectedOperation, -1, -1);
		}
		
    }

}