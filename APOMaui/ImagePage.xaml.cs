
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Platform;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;
namespace APOMaui;

public partial class ImagePage : ContentPage, IDisposable
{
    //Add backup 4 color images!
    private Image<Gray, Byte>? backupGray;
    private Image<Bgr, Byte>? colorImage;
    private Image<Gray, Byte>? grayImage;
    private string? _cacheImageSourcePath = null;
    public ImageSource ImageSource { get; set; }

    public string path;
    public int index;
    double imgScale;
    public ImgType? Type;

    private bool _readyToDraw = false;
    private System.Drawing.Point p1 = new System.Drawing.Point();
    private System.Drawing.Point p2 = new System.Drawing.Point();
    public Image<Bgr, Byte> ColorImage
    {
        get
        {
            if (colorImage != null) return colorImage;
            else
            {
                System.Diagnostics.Debug.WriteLine("WINImg.xaml.cs: Tried to get color image while image is GrayScale");
                return colorImage;
            }
        }
        set
        {
            BindingContext = null;
            if (grayImage != null) grayImage.Dispose();
            if (colorImage != null) colorImage.Dispose();
            grayImage = null;
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
                return grayImage;
            }
        }
        set
        {
            OnSetGrayImage(value);
            System.Diagnostics.Debug.WriteLine("Setter Done");
        }
    }   // Setter&Getter to refresh content after RGB->Grayscale, Dispose other img, Set new ImageSource
    public ImagePage(Image<Bgr, Byte> img, int index, string title)
    {
        InitializeComponent();
        this.Title = title;
        this.Type = ImgType.RGB;
        this.colorImage = img;
        this.index = index;
        this.ImageSource = EmguImgToImageSource(colorImage);
        this.imgScale = (double)img.Height / (double)img.Width;
        BindingContext = this;
    }
    public ImagePage(Image<Gray, Byte> img, int index, string title)
    {
        InitializeComponent();
        this.Title = title;
        this.Type = ImgType.Gray;
        this.grayImage = img;
        this.index = index;
        this.ImageSource = EmguImgToImageSource(grayImage);
        this.imgScale = (double)img.Height / (double)img.Width;
        BindingContext = this;

    }
    public void Dispose()
    {
        if(backupGray != null) backupGray.Dispose();
        if(colorImage != null) colorImage.Dispose();
        if(grayImage != null) grayImage.Dispose();
        if(_cacheImageSourcePath != null)
        {
            File.Delete(_cacheImageSourcePath);
        }
        this.ClearLogicalChildren();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        //WindowFileManager.ChangeSelectedImagePage(this.index);
#endif
    }

    public ImageSource EmguImgToImageSource(Image<Bgr, Byte> img)
    {
        if(this._cacheImageSourcePath != null)
        {
            File.Delete(this._cacheImageSourcePath);
        }
        string imagePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.bmp");
        this._cacheImageSourcePath = imagePath;
        img.Save(imagePath);
        ImageSource src;
        src = ImageSource.FromFile(imagePath);
        return src;
    }
    public ImageSource EmguImgToImageSource(Image<Gray, Byte> img)
    {
        if (this._cacheImageSourcePath != null)
        {
            File.Delete(this._cacheImageSourcePath);
        }
        string imagePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.bmp");
        this._cacheImageSourcePath = imagePath;
        img.Save(imagePath);
        ImageSource src;
        src = ImageSource.FromFile(imagePath);
        return src;
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
        colorImage = null;
        grayImage = value;
        this.ImageSource = EmguImgToImageSource(grayImage);
        BindingContext = this;
        Type = ImgType.Gray;
        _ = Task.Run(() =>
        {
            if (WindowFileManager.OpenedImagesList[index].CollectivePage.HistogramChart != null)
            {

                byte[] bytes = grayImage.Bytes;
                int[] hist = ImageProc.CalcHistValues(bytes);

                WindowFileManager.OpenedImagesList[index].CollectivePage.HistogramChart.UpdateChart(hist);
                System.Diagnostics.Debug.WriteLine("Task chart updating done");
            }
        });

    }
    private void ZoomIn(object sender, EventArgs e)
    {
        if(WindowFileManager.OpenedImagesList[index].CollectivePageWindow != null)
        {
            WindowFileManager.OpenedImagesList[index].CollectivePageWindow.Height += (int)((20 * imgScale) + 0.5); // Approx. Height
            WindowFileManager.OpenedImagesList[index].CollectivePageWindow.Width += 20;
        }
        winImgBox.WidthRequest = winImgBox.Width + 20;

    }
    private void ZoomOut(object sender, EventArgs e)
    {
        if(WindowFileManager.OpenedImagesList[index].CollectivePageWindow != null)
        {
            WindowFileManager.OpenedImagesList[index].CollectivePageWindow.Height -= (int)((20 * imgScale) + 0.5); // Approx. Height
            WindowFileManager.OpenedImagesList[index].CollectivePageWindow.Width -= 20;
        }
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
    private async void ProfileLine(object sender, EventArgs e)
    {
        System.Drawing.Point p1Draw = new System.Drawing.Point();
        System.Drawing.Point p2Draw = new System.Drawing.Point();

        winImgBox.GestureRecognizers.Clear();
        if (grayImage == null) return;
        if (_readyToDraw)
        {
            string res = await DisplayActionSheet("Profile Line", null, null, new string[] { "Accept", "Cancel" });
            if(res == "Accept")
            {
                ImageProc.ProfileLine(index, p1, p2);        
                _readyToDraw = false;
            }
            _readyToDraw = false;
            this.DrawBox.Source = null;
            return;
        }
        int c = 0;
        TapGestureRecognizer tgr = new TapGestureRecognizer();
        winImgBox.GestureRecognizers.Add(tgr);
        tgr.Tapped += (s, e) =>
        {

            WindowFileManager.ChangeSelectedImagePage(this.index);
            var img = s as Microsoft.Maui.Controls.Image;
            var tappedEvent = e as TappedEventArgs;
            Microsoft.Maui.Graphics.Point? tapPos = e.GetPosition(img);
            if (img != null && tapPos != null && grayImage != null)
            {
                double scale = Math.Max(winImgBox.Width / grayImage.Width,
                      winImgBox.Height / grayImage.Height);
                int pixelX = (int)(tapPos?.X / scale);
                int pixelY = (int)(tapPos?.Y / scale);
                Debug.WriteLine($"Pozycja kliknięcia: X={tapPos?.X}, Y={tapPos?.Y}");
                Debug.WriteLine($"Współrzędne piksela: X={pixelX}, Y={pixelY}");
                c++;
                if (c == 1)
                {
                    p1.X = pixelX;
                    p1.Y = pixelY;
                    p1Draw.X = (int)tapPos?.X;
                    p1Draw.Y = (int)tapPos?.Y;
                }
                if (c == 2)
                {
                    p2.X = pixelX;
                    p2.Y = pixelY;
                    p2Draw.X = (int)tapPos?.X;
                    p2Draw.Y = (int)tapPos?.Y;
                    DrawLine(p2Draw.X, p2Draw.Y, p1Draw.X, p1Draw.Y);
                    _readyToDraw = true;
                    this.winImgBox.GestureRecognizers.Clear();
                    
                }
            }

        };
        
    }

    private async void GrabCut(object sender, EventArgs e)
    {
        System.Drawing.Point p1Draw = new System.Drawing.Point();
        System.Drawing.Point p2Draw = new System.Drawing.Point();

        winImgBox.GestureRecognizers.Clear();
        if (colorImage == null) return;
        if (_readyToDraw)
        {
            string res = await DisplayActionSheet("Grab Cut", null, null, new string[] { "Accept", "Cancel" });
            if (res == "Accept")
            {
                int width = Math.Abs(p2.X - p1.X);
                int height = Math.Abs(p2.Y - p1.Y);
                int x = Math.Min(p1.X, p2.X);
                int y = Math.Min(p1.Y, p2.Y);
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(x, y, width, height);
                Debug.WriteLine("Calling GrabCut in main");

                ImageProc.GrabCut(index, rectangle);
                _readyToDraw = false;
            }
            _readyToDraw = false;
            this.DrawBox.Source = null;
            return;
        }
        int c = 0;
        Debug.WriteLine("GrabCut click");
        TapGestureRecognizer tgr = new TapGestureRecognizer();
        winImgBox.GestureRecognizers.Add(tgr);
        tgr.Tapped += (s, e) =>
        {

            WindowFileManager.ChangeSelectedImagePage(this.index);
            var img = s as Microsoft.Maui.Controls.Image;
            var tappedEvent = e as TappedEventArgs;
            Microsoft.Maui.Graphics.Point? tapPos = e.GetPosition(img);
            if (img != null && tapPos != null && colorImage != null)
            {
                double scale = Math.Max(winImgBox.Width / colorImage.Width,
                      winImgBox.Height / colorImage.Height);
                int pixelX = (int)(tapPos?.X / scale);
                int pixelY = (int)(tapPos?.Y / scale);
                Debug.WriteLine($"Pozycja kliknięcia: X={tapPos?.X}, Y={tapPos?.Y}");
                Debug.WriteLine($"Współrzędne piksela: X={pixelX}, Y={pixelY}");
                c++;
                if (c == 1)
                {
                    p1.X = pixelX;
                    p1.Y = pixelY;
                    p1Draw.X = (int)tapPos?.X;
                    p1Draw.Y = (int)tapPos?.Y;
                }
                if (c == 2)
                {
                    p2.X = pixelX;
                    p2.Y = pixelY;
                    p2Draw.X = (int)tapPos?.X;
                    p2Draw.Y = (int)tapPos?.Y;
                    DrawSquare(p1Draw.X, p1Draw.Y, p2Draw.X, p2Draw.Y);
                    _readyToDraw = true;
                    this.winImgBox.GestureRecognizers.Clear();
                }
            }

        };
    }
    private void DrawLine(float startX, float startY, float endX, float endY)
    {
        int width = (int)this.winImgBox.Width;
        int height = (int)this.winImgBox.Height;
        SKBitmap bitmap = new SKBitmap(width, height, false);
        using (SKCanvas canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.Transparent);
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 2.5f
            };
            canvas.DrawLine(startX, startY, endX, endY, paint);
        }
        var imageSource = ConvertSKBitmapToImageSource(bitmap);
        this.DrawBox.Source = imageSource;
    }
    private void DrawSquare(float startX, float startY, float endX, float endY)
    {
        int width = (int)this.winImgBox.Width;
        int height = (int)this.winImgBox.Height;
        SKBitmap bitmap = new SKBitmap(width, height, false);
        using (SKCanvas canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.Transparent);
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 2.5f
            };
            SKRect rect = new SKRect(
                Math.Min(startX, endX),
                Math.Min(startY, endY),
                Math.Max(startX, endX),
                Math.Max(startY, endY)
            );
            canvas.DrawRect(rect, paint);
        }
        var imageSource = ConvertSKBitmapToImageSource(bitmap);
        this.DrawBox.Source = imageSource;
    }

    private ImageSource ConvertSKBitmapToImageSource(SKBitmap skBitmap)
    {
        var image = SKImage.FromBitmap(skBitmap);
        var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return ImageSource.FromStream(() => new MemoryStream(data.ToArray()));
    }

    private void winImgBox_SizeChanged(object sender, EventArgs e)
    {
        if (winImgBox != null && DrawBox != null)
        {
            DrawBox.WidthRequest = winImgBox.Width;
            DrawBox.HeightRequest = winImgBox.Height;
        }
    }
}