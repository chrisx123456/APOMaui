
using Emgu.CV;
using Emgu.CV.Structure;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Diagnostics;
namespace APOMaui;

public partial class ImagePage : ContentPage
{
    //Add backup 4 color images!
    private Image<Gray, Byte>? backupGray;
    private Image<Bgr, Byte>? colorImage;
    private Image<Gray, Byte>? grayImage;
    public ImageSource ImageSource { get; set; }

    string title;
    public string path;
    public string GetTitle
    {
        get => title;
    }
    public int index;
    double imgScale;
    public ImgType? Type;

    private SKPoint? previousPoint;

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
        
        this.title = title;
        this.Type = ImgType.RGB;
        this.colorImage = img;
        this.index = index;
        this.ImageSource = EmguImgToImageSource(colorImage);
        this.imgScale = (double)img.Height / (double)img.Width;
        ToggleDrawBox(false);
        //setDrawBoxSize(img.Width, img.Height);
        BindingContext = this;
    }
    public ImagePage(Image<Gray, Byte> img, int index, string title)
    {
        InitializeComponent();
        this.title = title;
        this.Type = ImgType.Gray;
        this.grayImage = img;
        this.index = index;
        this.ImageSource = EmguImgToImageSource(grayImage);
        this.imgScale = (double)img.Height / (double)img.Width;
        //setDrawBoxSize(img.Width, img.Height);
        ToggleDrawBox(false);
        BindingContext = this;

    }

    public static ImageSource EmguImgToImageSource(Image<Bgr, Byte> img)
    {

        System.Drawing.Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    public static ImageSource EmguImgToImageSource(Image<Gray, Byte> img)
    {
        System.Drawing.Bitmap bitmap = Emgu.CV.BitmapExtension.ToBitmap(img);
        MemoryStream stream = new();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Position = 0;
        return ImageSource.FromStream(() => stream);
    }
    public void ToggleDrawBox(bool value)
    {
        drawBox.IsVisible = value;
        drawBox.IsEnabled = value;
    }
    public bool GetDrawBoxState()
    {
        return drawBox.IsVisible && drawBox.IsEnabled;
    }
    private void setDrawBoxSize(int w, int h)
    {
        this.drawBox.HeightRequest = h;
        this.drawBox.WidthRequest = w;
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
            if (ImageProc.OpenedImagesList[index].HistogramChart != null)
            {

                byte[] bytes = grayImage.Bytes;
                int[] hist = ImageProc.CalcHistValues(bytes);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                ImageProc.OpenedImagesList[index].HistogramChart.UpdateChart(hist);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                System.Diagnostics.Debug.WriteLine("Task chart updating done");
            }
        });

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ImageProc.OnCloseImagePage(index);
    }
    private void ZoomIn(object sender, EventArgs e)
    {
        ImageProc.OpenedImagesList[index].ImagePageWindow.Height += (int)((20 * imgScale) + 0.5); // Approx. Height
        ImageProc.OpenedImagesList[index].ImagePageWindow.Width += 20;
        winImgBox.WidthRequest = winImgBox.Width + 20;

    }
    private void ZoomOut(object sender, EventArgs e)
    {
        ImageProc.OpenedImagesList[index].ImagePageWindow.Height -= (int)((20 * imgScale) + 0.5); // Approx. Height
        ImageProc.OpenedImagesList[index].ImagePageWindow.Width -= 20;
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

            ImageProc.ChangeSelectedImagePage(this.index);
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
                    ImageProc.ProfileLine(index, p1, p2);
                    this.winImgBox.GestureRecognizers.Clear();
                }
            }

        };
        
    }
    private void GrabCut(object sender, EventArgs e)
    {
        this.winImgBox.GestureRecognizers.Clear();
        if (colorImage == null) return;
        int c = 0;
        Debug.WriteLine("GrabCut click");
        TapGestureRecognizer tgr = new TapGestureRecognizer();
        winImgBox.GestureRecognizers.Add(tgr);
        System.Drawing.Point p1 = new System.Drawing.Point();
        System.Drawing.Point p2 = new System.Drawing.Point();
        tgr.Tapped += (s, e) =>
        {

            ImageProc.ChangeSelectedImagePage(this.index);
            var img = s as Microsoft.Maui.Controls.Image;
            var tappedEvent = e as TappedEventArgs;
            Microsoft.Maui.Graphics.Point? tapPos = e.GetPosition(img);
            if (img != null && tapPos != null && colorImage != null)
            {
                
                // Rozmiar obrazka
                var imageSize = new System.Drawing.Size((int)img.Width, (int)img.Height);

                // Rozmiar obrazu źródłowego
                var sourceSize = new System.Drawing.Size(colorImage.Width, colorImage.Height);

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
                    int width = Math.Abs(p2.X - p1.X);
                    int height = Math.Abs(p2.Y - p1.Y);

                    // Znajdujemy lewy górny róg prostokąta
                    int x = Math.Min(p1.X, p2.X);
                    int y = Math.Min(p1.Y, p2.Y);

                    // Tworzymy Rectangle na podstawie obliczonych wartości
                    System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(x, y, width, height);
                    Debug.WriteLine("Calling GrabCut in main");

                    ImageProc.GrabCut(index, rectangle);
                    this.winImgBox.GestureRecognizers.Clear();
                }
            }

        };
    }
    void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
    }
    public void OnPaint(object sender, SKTouchEventArgs e)
    {
        e.Handled = true;
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                Debug.WriteLine("Pressed");
                previousPoint = null;
                previousPoint = e.Location; // Ustaw bieżący punkt na początkowy
                e.Handled = true;
                break;
            case SKTouchAction.Moved:
                e.Handled = true;
                if (previousPoint.HasValue)
                {
                    DrawLine(previousPoint.Value, e.Location); // Narysuj linię między poprzednim punktem a bieżącym punktem
                    previousPoint = e.Location; // Ustaw bieżący punkt na poprzedni
                    Debug.WriteLine("Moved");
                    drawBox.InvalidateSurface();
                }
                break;
            case SKTouchAction.Cancelled:
                Debug.WriteLine("Cancelled");
                return;
            case SKTouchAction.Exited:
                Debug.WriteLine("Exited");
                break;
            case SKTouchAction.Entered:
                Debug.WriteLine("Entered");
                break;
            case SKTouchAction.Released:
                Debug.WriteLine("Rel");
                e.Handled = true;
                drawBox.InvalidateSurface();
                previousPoint = null; // Zresetuj poprzedni punkt po zwolnieniu dotyku
                break;
        }
        e.Handled = true;
    }
    private void DrawLine(SKPoint startPoint, SKPoint endPoint)
    {
        // Narysuj linię między dwoma punktami na płótnie SKCanvasView
        drawBox.PaintSurface += (s, args) =>
        {
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.Style = SKPaintStyle.Fill;
                paint.StrokeWidth = 10;

                args.Surface.Canvas.DrawLine(startPoint, endPoint, paint);
            }
        };

    }
    public async Task<Image<Gray, Byte>> GetImageFromCanvas()
    {
        var img = await drawBox.CaptureAsync();
        Stream s = await img.OpenReadAsync();
        s.Position = 0;
        SKBitmap skb = SKBitmap.Decode(s);
        Image<Gray, Byte> res = ConvertSKBitmapToGrayByteImage(skb);
        //Main.OpenNewWindowWinIMG(res, "Test");
        return res;

    }
    public Image<Gray, Byte> ConvertSKBitmapToGrayByteImage(SKBitmap skBitmap)
    {
        var image = new Image<Gray, byte>(skBitmap.Width, skBitmap.Height, new Gray(0));
            for (int y = 0; y < skBitmap.Height; y++)
            {
                for (int x = 0; x < skBitmap.Width; x++)
                {
                    var color = skBitmap.GetPixel(x, y);

                    byte grayValue = (byte)((color.Red + color.Green + color.Blue) / 3);

                    image[y, x] = new Gray(grayValue);
                }
        }

        if (colorImage != null)
        {
            return image.Resize(colorImage.Width, colorImage.Height, Emgu.CV.CvEnum.Inter.NearestExact);
        }
        else if (grayImage != null)
        {
            return image.Resize(grayImage.Width, grayImage.Height, Emgu.CV.CvEnum.Inter.NearestExact);
        }
        else return image;
    }
}