

using System.Diagnostics;

namespace APOMaui;

public partial class AndroidTabbedPage : TabbedPage
{
	public AndroidTabbedPage()
	{
		InitializeComponent();
        WindowFileManager.OnImageSelectionChanged += Refresh;
		//WindowFileManager.OnImageClosingOpeningEvent += Refresh;
		WindowFileManager.OnImageClosingEvent += Refresh;
		WindowFileManager.OnImageOpeningEvent += Refresh;
		this.CurrentPageChanged += PageChanged;
	}
	public void Refresh()
	{
		TabbedPage_SizeChanged(new object(), new EventArgs());
	}
	public void AddPage(CollectivePage page)
	{
		this.Children.Add(page);
	}
	public void RemovePage(CollectivePage page)
	{
		this.Children.Remove(page);
	}

    private void TabbedPage_SizeChanged(object sender, EventArgs e)
    {
		if(this.CurrentPage != null)
		{
            this.CurrentPage.HeightRequest = this.Height;
            this.CurrentPage.HeightRequest = -1;
			Debug.WriteLine("Resizing fix test!!!!!!!!!!!!!!!!!!"); //dodac te picker 40px w reszcie miejsc
        }
    }
    public void PageChanged(object sender, EventArgs e)
	{
		if(this.CurrentPage != null)
		{
            WindowFileManager.ChangeSelectedImagePage(((CollectivePage)this.CurrentPage).ImagePage.index);
        }
    }
   
}