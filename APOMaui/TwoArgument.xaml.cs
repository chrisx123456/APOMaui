namespace APOMaui;

public partial class TwoArgument : ContentPage
{
	private readonly List<string> _operations = new List<string>() {"ADD","SUBSTRACT","BLEND","AND","OR","NOT","XOR"};
	private readonly Dictionary<string, int> _imageList = new Dictionary<string, int>();
	public TwoArgument()
	{
		InitializeComponent();
        OperationPicker.ItemsSource = _operations;
		_imageList = GetImagesList();
		AddPickersItems();
		//Dodac event zeby to sie aktualizowalo
		//Zmienic w kernelach zeby wsm tak samo sie do pickera dodawalo jak tutaj
		
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
			string s = wio.winImg.Title;
			int i = wio.winImg.index;
			res.Add(s, i);
		}
		return res;
	}
	private void OnTwoArgumentButtonClicked(object sender, EventArgs e)
	{

	}
	private void OnImg1PickerSelectedIndexChanged(object sender, EventArgs e)
	{

	}
    private void OnImg2PickerSelectedIndexChanged(object sender, EventArgs e)
    {

    }
    private void OnOperationPickerSelectedIndexChanged(object sender, EventArgs e)
    {

    }

}