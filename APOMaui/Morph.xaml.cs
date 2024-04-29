using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace APOMaui;

public partial class Morph : ContentPage
{
	private Emgu.CV.CvEnum.ElementShape? _structElement;
	private Emgu.CV.CvEnum.MorphOp? _morphOp;
	private Emgu.CV.CvEnum.BorderType? _borderType;
	private bool isSkeletonize = false;
    public Morph()
	{
		InitializeComponent();
		setPickers();
	}
	private void setPickers()
	{
		foreach(Emgu.CV.CvEnum.ElementShape e in Enum.GetValues(typeof(Emgu.CV.CvEnum.ElementShape)))
		{
			if(e != Emgu.CV.CvEnum.ElementShape.Custom)
				StructPicker.Items.Add(e.ToString());
		}
        foreach (Emgu.CV.CvEnum.MorphOp m in Enum.GetValues(typeof(Emgu.CV.CvEnum.MorphOp)))
        {
            MorphPicker.Items.Add(m.ToString());
        }
		foreach(Emgu.CV.CvEnum.BorderType b in Enum.GetValues(typeof(Emgu.CV.CvEnum.BorderType)))
		{
			if(b != BorderType.NegativeOne && b != BorderType.Default)
			{
				EdgePicker.Items.Add(b.ToString());
			}
		}
		MorphPicker.Items.Add("Skeletonize");
    }
	private void OnEdgePickerSelectedIndexChanged(object sender, EventArgs e)
	{
		switch (EdgePicker.SelectedItem.ToString()) 
		{
			case "Constant":
				_borderType = BorderType.Constant;
				break;
			case "Replicate":
				_borderType = BorderType.Replicate;
				break;
			case "Reflect":
				_borderType = BorderType.Reflect;
				break;
			case "Wrap":
				_borderType = BorderType.Wrap;
				break;
			case "Reflect101":
				_borderType = BorderType.Reflect101;
				break;
			case "Transparent":
				_borderType = BorderType.Transparent;
				break;
			case "Isolated":
				_borderType = BorderType.Isolated;
				break;
			default:
				_borderType = null;
				break;

        }
	}
    private void OnMorphPickerSelectedIndexChanged(object sender, EventArgs e)
	{
        isSkeletonize = false;
        switch (MorphPicker.SelectedItem.ToString())
		{
			case "Erode":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Erode;
				break;
			case "Dilate":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Dilate;
				break;
			case "Open":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Open;
                break;
			case "Close":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Close;
				break;
			case "Gradient":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Gradient;
				break;
			case "Tophat":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Tophat;
				break;
			case "Blackhat":
				_morphOp = Emgu.CV.CvEnum.MorphOp.Blackhat;
				break;
			case "HitMiss":
				_morphOp = Emgu.CV.CvEnum.MorphOp.HitMiss;
				break;
			case "Skeletonize":
				_morphOp = null;
                isSkeletonize = true;
                break;
			default:
				_morphOp = null;
				break;
		}
	}
    private void OnStructPickerSelectedIndexChanged(object sender, EventArgs e)
    {
		switch(StructPicker.SelectedItem.ToString())
		{
			case "Rectangle":
				_structElement = Emgu.CV.CvEnum.ElementShape.Rectangle;
				break;
			case "Cross":
				_structElement = Emgu.CV.CvEnum.ElementShape.Cross;
				break;
			case "Ellipse":
				_structElement = Emgu.CV.CvEnum.ElementShape.Ellipse;
				break;
			default:
				_structElement = null;
				break;
        }
    }
	private async void OnMorphButtonClicked(object sender, EventArgs e)
	{
        if (Main.selectedWindow == null)
        {
            await DisplayAlert("Alert", "None image is selected!", "Ok");
            return;
        }
        int index = (int)Main.selectedWindow;
        if (Main.OpenedImagesWindowsList[index].winImg.Type != ImgType.Gray)
        {
            await DisplayAlert("Alert", "Selected image is not GrayScale", "Ok");
            return;
        }
		if (_borderType == null || _structElement == null || (_morphOp == null && isSkeletonize==false))
        {
            await DisplayAlert("Alert", "Border / Struct. element / Morhp.op not selected", "Ok");
            return;
        }
		int constB = 0;
        if (_borderType == BorderType.Constant)
        {
            if (!int.TryParse(await DisplayPromptAsync("Constant Border", "Type Constant border value"), out constB))
            {
                await DisplayAlert("Alert", "Const. border value not valid", "Ok");
                return;
            }
        }
		if (isSkeletonize)
		{
			Main.Skeletonize(index, (ElementShape)_structElement, (BorderType)_borderType, new Emgu.CV.Structure.MCvScalar(constB, constB, constB, constB));
		}
        else Main.MathMorph(index, (MorphOp)_morphOp, (ElementShape)_structElement, (BorderType)_borderType, new Emgu.CV.Structure.MCvScalar(constB, constB, constB, constB));


    }

}