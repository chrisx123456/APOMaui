
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace APOMaui;

public partial class WinIMG : ContentPage
{
    private readonly double realWidth;
    private readonly double realHeight;

    private Image<Bgr, Byte>? colorImage;
    private Image<Gray, Byte>? grayImage;



    public int index;
    public ImageSource ImageSource { get; set; }
    public double DesiredWidth { get; set; }
    public double ImgScale { get; set; }

    public ImgType? Type;

    public Image<Bgr, Byte> ColorImage
    {
        get 
        { 
            if(colorImage!=null) return colorImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get color image while image is GrayScale");
                return new Image<Bgr, Byte>(0,0);
            }
        }
        set 
        {
            BindingContext = null;
            if(grayImage != null) grayImage.Dispose();
            colorImage = value;
            this.ImageSource = EmguImgToImageSource(colorImage);
            BindingContext = this;
            Type = ImgType.RGB;
        }
    }   // Setter&Getter to refresh content after RGB->Grayscale, Dispose other img, Set new ImageSource
    public Image<Gray, Byte> GrayImage
    {
        get 
        {
            if(grayImage!=null) return grayImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get grayscale image while image is Color");
                return new Image<Gray, Byte>(0, 0);
            }
        }
        set
        {
            BindingContext = null;
            if (colorImage != null) colorImage.Dispose();
            grayImage = value;
            this.ImageSource = EmguImgToImageSource(grayImage);
            BindingContext = this;
            Type = ImgType.Gray;
            if (Main.OpenedImagesWindowsList[index].chart != null)
            {
                byte[] bytes = grayImage.Bytes;
                int[] hist = new int[256];
                foreach (byte b in bytes) hist[b]++;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Main.OpenedImagesWindowsList[index].chart.UpdateChart(hist);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            
        }
    }   // Setter&Getter to refresh content after RGB->Grayscale, Dispose other img, Set new ImageSource

    public WinIMG(Image<Bgr, Byte> img, int index, int realWidth, int realHeight)
    {
        InitializeComponent();
        this.Type = ImgType.RGB;
        this.colorImage = img;
        this.realHeight = realHeight;
        this.realWidth = realWidth;
        this.index = index;
        this.DesiredWidth = Main.InitWidth;
        this.ImageSource = EmguImgToImageSource(colorImage);
        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");
        BindingContext = this;
        this.winImgBox.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Main.ChangeSelectedtWinIMG(this.index))
        });
    }
    public WinIMG(Image<Gray, Byte> img, int index, int realWidth, int realHeight)
    {
        InitializeComponent();
        this.Type = ImgType.Gray;
        this.grayImage = img;
        this.realHeight = realHeight;
        this.realWidth = realWidth;
        this.index = index;
        this.DesiredWidth = Main.InitWidth;
        this.ImageSource = EmguImgToImageSource(grayImage);
        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");

        BindingContext = this;
        this.winImgBox.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Main.ChangeSelectedtWinIMG(index))
        });
    }
    public static ImageSource EmguImgToImageSource(Image<Bgr, Byte> img)
    {
        Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    public static ImageSource EmguImgToImageSource(Image<Gray, Byte> img)
    {
        Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    private double CalcScale()
    {
        return Math.Round((((double)this.DesiredWidth / (double)this.realWidth) * 100), 2);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Main.OnCloseEventWinIMG(index);
    }
    public void ZoomIn(object sender, EventArgs e)
    {
        this.DesiredWidth += this.DesiredWidth * 0.1D;
        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");
        Main.ResizeWindow(index, DesiredWidth, realWidth, realHeight);
    }
    public void ZoomOut(object sender, EventArgs e)
    {
        this.DesiredWidth -= this.DesiredWidth * 0.1D;
        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");
        Main.ResizeWindow(index, DesiredWidth, realWidth, realHeight);
    }


}