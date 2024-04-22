using Emgu.CV;
using System.Diagnostics;

namespace APOMaui;

public partial class TwoArgument : ContentPage
{
    private readonly List<string> _operations = new List<string>() {"ADD","SUBSTRACT","BLEND","AND","OR","NOT","XOR"};
    private readonly Dictionary<string, int> _imageList = new Dictionary<string, int>();
	private TwoArgsOps? _selectedOperation = null;
	private int? _selectedImage1 = null;
	private int? _selectedImage2 = null;
	public TwoArgument()
	{
		InitializeComponent();
        OperationPicker.ItemsSource = _operations;
		_imageList = GetImagesList();
		AddPickersItems();
		Main.OnWinIMGClosingEvent += this.UpdatePickersItems;
        //Dodac event zeby to sie aktualizowalo
        //Zmienic w kernelach zeby wsm tak samo sie do pickera dodawalo jak tutaj

    }
	private void UpdatePickersItems()
	{
		ResetPickersItems();
		AddPickersItems();
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
        foreach (WindowImageObject wio in Main.OpenedImagesWindowsList)
		{
			string s = wio.winImg.GetTitle;
			int i = wio.winImg.index;
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
				break;
			case "SUBSTRACT":
				_selectedOperation = TwoArgsOps.SUBTRACT;
				break;
			case "BLEND":
				_selectedOperation = TwoArgsOps.BLEND;
				break;
			case "AND":
				_selectedOperation = TwoArgsOps.AND;
				break;
			case "OR":
				_selectedOperation = TwoArgsOps.OR;
				break;
			case "NOT":
				_selectedOperation = TwoArgsOps.NOT;
				break;
			case "XOR":
				_selectedOperation = TwoArgsOps.XOR;
				break;
			default:
				_selectedOperation = null;
				break;
		}
    }
    private async void OnTwoArgumentButtonClicked(object sender, EventArgs e)
    {
		if(_selectedImage1 == null || _selectedImage2 == null || _selectedOperation == null)
		{
			await DisplayAlert("Alert", "Image 1/2 or operation not selected", "Ok");
			return;
		}
		//TODO; Blend weight
        Main.TwoArgsOperations((int)_selectedImage1, (int)_selectedImage1, (TwoArgsOps)_selectedOperation, -1, -1);
    }

}