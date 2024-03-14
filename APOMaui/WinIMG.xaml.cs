
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
    private Image<Hsv, UInt16>? hsvImage;
    private Image<Lab, SByte>? labImage;

    public int index;
    public ImageSource ImageSource { get; set; }
    public double DesiredWidth { get; set; }
    public double ImgScale { get; set; }

    public bool isRGB;
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
        }
    }   // Setter&Getter to refresh content after RGB->Grayscale, Dispose other img, Set new ImageSource
    public Image<Hsv, UInt16> HsvImage
    {
        get
        {
            if (hsvImage != null) return hsvImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get hsv image while image type is different");
                return new Image<Hsv, UInt16>(0, 0);
            }
        }
        set
        {
            BindingContext = null;
            if(colorImage!= null) colorImage.Dispose();
            hsvImage = value;
            this.ImageSource = EmguImgToImageSource(hsvImage);
            BindingContext = this;
            Type = ImgType.HSV;

        }
            
    }
    public Image<Lab, SByte> LabImage
    {
        get
        {
            if(labImage != null) return labImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get lab image while image type is different");
                return new Image<Lab, SByte>(0, 0);
            }
        }
        set
        {
            BindingContext = null;
            if(colorImage != null) colorImage.Dispose();
            labImage = value;
            this.ImageSource = EmguImgToImageSource(labImage);
            BindingContext = this;
            Type = ImgType.Lab;
        }
    }

    public WinIMG(Image<Bgr, Byte> img, int index, int realWidth, int realHeight)
    {
        InitializeComponent();
        this.ColorImage = img;
        this.realHeight = realHeight;
        this.realWidth = realWidth;
        this.index = index;
        this.DesiredWidth = Main.InitWidth;
        this.isRGB = true;

        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");

        BindingContext = this;
        this.winImgBox.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Main.ChangeSelectedtWinIMG(index))
        });
    }
    public WinIMG(Image<Gray, Byte> img, int index, int realWidth, int realHeight)
    {
        InitializeComponent();
        this.GrayImage = img;
        this.realHeight = realHeight;
        this.realWidth = realWidth;
        this.index = index;
        this.DesiredWidth = Main.InitWidth;
        this.isRGB = false;

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
        MemoryStream stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    public static ImageSource EmguImgToImageSource(Image<Gray, Byte> img)
    {
        Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    public static ImageSource EmguImgToImageSource(Image<Hsv, UInt16> img)
    {
        Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    public static ImageSource EmguImgToImageSource(Image<Lab, SByte> img)
    {
        Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new MemoryStream();
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
        System.Diagnostics.Debug.WriteLine("CloseEvent");
        Main.selectedWindow = null;
        Main.OnCloseEventWINImg(index);
    }
    public void ZoomIn(object sender, EventArgs e)
    {
        this.DesiredWidth += this.DesiredWidth * 0.1D;
        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");
        Main.ResizeWindow(index, DesiredWidth, realWidth, realHeight);
        System.Diagnostics.Debug.WriteLine(isRGB);
    }
    public void ZoomOut(object sender, EventArgs e)
    {
        this.DesiredWidth -= this.DesiredWidth * 0.1D;
        this.ImgScale = CalcScale();
        this.scaleLabel.Text = new string(ImgScale.ToString() + "%");
        Main.ResizeWindow(index, DesiredWidth, realWidth, realHeight);
    }


}