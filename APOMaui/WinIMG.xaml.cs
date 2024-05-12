
using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Drawing;
namespace APOMaui;

public partial class WinIMG : ContentPage
{
    //Add backup 4 color images!
    private Image<Gray, Byte>? backupGray;
    private Image<Bgr, Byte>? colorImage;
    private Image<Gray, Byte>? grayImage;
    public ImageSource ImageSource { get; set; }


    string title;
    public string GetTitle
    {
        get => title;
    }
    public int index;
    double imgScale;
    public ImgType? Type;

    public Image<Bgr, Byte> ColorImage
    {
        get
        {
            if (colorImage != null) return colorImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get color image while image is GrayScale");
                return new Image<Bgr, Byte>(0, 0);
            }
        }
        set
        {
            BindingContext = null;
            if (grayImage != null) grayImage.Dispose();
            if (colorImage != null) colorImage.Dispose();
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
            if (grayImage != null) return grayImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get grayscale image while image is Color");
                return new Image<Gray, Byte>(0, 0);
            }
        }
        set
        {
            OnSetGrayImage(value);
            System.Diagnostics.Debug.WriteLine("Setter Done");
        }
    }   // Setter&Getter to refresh content after RGB->Grayscale, Dispose other img, Set new ImageSource
    public WinIMG(Image<Bgr, Byte> img, int index, string title)
    {
        InitializeComponent();
        this.title = title;
        this.Type = ImgType.RGB;
        this.colorImage = img;
        this.index = index;
        this.ImageSource = EmguImgToImageSource(colorImage);
        this.imgScale = (double)img.Height / (double)img.Width;
        BindingContext = this;
    }
    public WinIMG(Image<Gray, Byte> img, int index, string title)
    {
        InitializeComponent();
        this.title = title;
        this.Type = ImgType.Gray;
        this.grayImage = img;
        this.index = index;
        this.ImageSource = EmguImgToImageSource(grayImage);
        this.imgScale = (double)img.Height / (double)img.Width;
        BindingContext = this;
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
    public void OnSetGrayImage(Image<Gray, Byte> value)
    {
        BindingContext = null;
        if (colorImage != null) colorImage.Dispose();
        if (grayImage != null)
        {
            backupGray = grayImage.Clone();
            grayImage.Dispose();
        }
        grayImage = value;
        this.ImageSource = EmguImgToImageSource(grayImage);
        BindingContext = this;
        Type = ImgType.Gray;
        _ = Task.Run(() =>
        {
            if (Main.OpenedImagesWindowsList[index].chart != null)
            {

                byte[] bytes = grayImage.Bytes;
                int[] hist = Main.CalcHistValues(bytes);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Main.OpenedImagesWindowsList[index].chart.UpdateChart(hist);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                System.Diagnostics.Debug.WriteLine("Task chart updating done");
            }
        });

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Main.OnCloseEventWinIMG(index);
    }
    private void ZoomIn(object sender, EventArgs e)
    {
        Main.OpenedImagesWindowsList[index].window.Height += (int)((20 * imgScale) + 0.5); // Approx. Height
        Main.OpenedImagesWindowsList[index].window.Width += 20;
        winImgBox.WidthRequest = winImgBox.Width + 20;

    }
    private void ZoomOut(object sender, EventArgs e)
    {
        Main.OpenedImagesWindowsList[index].window.Height -= (int)((20 * imgScale) + 0.5); // Approx. Height
        Main.OpenedImagesWindowsList[index].window.Width -= 20;
        winImgBox.WidthRequest = winImgBox.Width - 20;
    }
    private void Undo(object sender, EventArgs e)
    {
        if (backupGray != null && grayImage != null && colorImage == null)
        {
            this.GrayImage = backupGray;
            backupGray.Dispose();
            backupGray = null;
        }
    }
    private void ProfileLine(object sender, EventArgs e)
    {
        if (grayImage == null) return;
        int c = 0;
        TapGestureRecognizer tgr = new TapGestureRecognizer();
        winImgBox.GestureRecognizers.Add(tgr);
        System.Drawing.Point p1 = new System.Drawing.Point();
        System.Drawing.Point p2 = new System.Drawing.Point();
        tgr.Tapped += (s, e) =>
        {

            Main.ChangeSelectedtWinIMG(this.index);
            var img = s as Microsoft.Maui.Controls.Image;
            var tappedEvent = e as TappedEventArgs;
            Microsoft.Maui.Graphics.Point? tapPos = e.GetPosition(img);
            if (img != null && tapPos != null && grayImage != null)
            {
                // Rozmiar obrazka
                var imageSize = new System.Drawing.Size((int)img.Width, (int)img.Height);

                // Rozmiar obrazu źródłowego
                var sourceSize = new System.Drawing.Size(grayImage.Width, grayImage.Height);

                // Współrzędne piksela w obrazie źródłowym
                var pixelX = (int)(tapPos?.X * (sourceSize.Width / (double)imageSize.Width));

                var pixelY = (int)(tapPos?.Y * (sourceSize.Height / (double)imageSize.Height));

                //Wyświetlenie współrzędnych piksela
                Debug.WriteLine($"Pozycja kliknięcia: X={tapPos?.X}, Y={tapPos?.Y}");
                Debug.WriteLine($"Współrzędne piksela: X={pixelX}, Y={pixelY}");
                c++;
                if (c == 1)
                {
                    p1.X = pixelX;
                    p1.Y = pixelY;
                }
                if (c == 2)
                {
                    p2.X = pixelX;
                    p2.Y = pixelY;
                    Main.ProfileLine(index, p1, p2);
                    this.winImgBox.GestureRecognizers.Clear();
                }
            }

        };
        
    }


}