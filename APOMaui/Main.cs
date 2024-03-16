using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
namespace APOMaui
{
    internal static class Main
    {
        public static List<WindowImageObject> OpenedImagesWindowsList = new List<WindowImageObject>();
        public static int? selectedWindow = null;
        public static readonly int InitWidth = 400;
        public static readonly int MaxWidth = 800;
        private static readonly int windowHeightFix = 80;

        public static async void OpenPhotoWinIMG()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Wybierz plik"
                });
                if (result == null) return;
                string FileFullPath = result.FullPath.ToString();
                var img = ImageLoader.LoadImage(FileFullPath); // Important!! Type Image<Bgr, Byte> or Image<Gray, Byte> depends on file type detected in FileLoader              
                OpenNewWindowWinIMG(img);


            }
            else if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                //TODO
            }
        }
        public static void OpenNewWindowWinIMG(dynamic img) //Argument is dynamic cuz' dont want to make overload for GrayScale/Color.
        {
            var page = new WinIMG(img, OpenedImagesWindowsList.Count, img.Width, img.Height);
            var newWindow = new Window
            {
                Page = page,
                Width = InitWidth,
                Height = ((double)InitWidth / (double)img.Width) * (double)img.Height + windowHeightFix,
            };

            WindowImageObject windowImageObject = new WindowImageObject(page, newWindow);
            OpenedImagesWindowsList.Add(windowImageObject);
            selectedWindow = OpenedImagesWindowsList.Count - 1;
#pragma warning disable 8602
            Application.Current.OpenWindow(newWindow);
#pragma warning restore 8602
        }

        public static void ResizeWindow(int index, double newwidth, double realwidth, double realheight)
        {
            OpenedImagesWindowsList[index].window.Width = newwidth;
            OpenedImagesWindowsList[index].window.Height = (((newwidth / realwidth) * realheight) + windowHeightFix);
        }
        public static void OnCloseEventWinIMG(int index)
        {
#pragma warning disable 8602
            if (OpenedImagesWindowsList[index].chart != null)
            {
                Application.Current.CloseWindow(OpenedImagesWindowsList[index].chart.window);
            }
#pragma warning restore 8602
            OpenedImagesWindowsList.RemoveAt(index);
            for(int i = index; index<OpenedImagesWindowsList.Count; index++)
            {
                OpenedImagesWindowsList[i].winImg.index--;
            }
        }
        public static void OnCloseEventChart(int index)
        {
#pragma warning disable 8602
            OpenedImagesWindowsList[index].chart.Series = null;  
            OpenedImagesWindowsList[index].chart.BindingContext = null;
            OpenedImagesWindowsList[index].chart = null;
            System.Diagnostics.Debug.WriteLine("Chart Disposed");
#pragma warning restore 8602
        }
        public static void ChangeSelectedtWinIMG(int index)
        {
            selectedWindow = index;
            System.Diagnostics.Debug.WriteLine($"Selected {index}");
        }
        public static void CreateHistogramChart(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage; //Cant be null, checked in Charts.xaml.cs
            byte[] rawData = img.Bytes;
            int[] GrayScaleHist = new int[256];
            foreach (byte b in rawData) GrayScaleHist[b]++;
            var page = new Chart(GrayScaleHist, index);
            OpenedImagesWindowsList[index].chart = page;
            int width = 830, height = 350;
            var newWindow = new Window
            {
                Page = OpenedImagesWindowsList[index].chart,
                Width = width,
                Height = height,
                MinimumWidth = width,
                MaximumWidth = width,
                MinimumHeight = height,
                MaximumHeight = height,
                
            };
            page.window = newWindow;
#pragma warning disable 8602
            Application.Current.OpenWindow(newWindow);
#pragma warning restore 8602
        }
        public static void ConvertRgbToGray(int index)
        {
            Image<Gray, Byte> grayImage = new Image<Gray, Byte>(OpenedImagesWindowsList[index].winImg.ColorImage.Width, OpenedImagesWindowsList[index].winImg.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesWindowsList[index].winImg.ColorImage, grayImage, ColorConversion.Bgr2Gray);
            OpenedImagesWindowsList[index].winImg.GrayImage = grayImage;
            OpenedImagesWindowsList[index].winImg.isRGB = false;
        }
        public static void ConvertRgbToHsv(int index) //Pytanie czy to musi byc HsvFull(0-360)(Uint16) czy git dla Hsv(0-180)(Byte)
        {
            Image<Hsv, Byte> hsvimg = new Image<Hsv, Byte>(OpenedImagesWindowsList[index].winImg.ColorImage.Width, OpenedImagesWindowsList[index].winImg.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesWindowsList[index].winImg.ColorImage, hsvimg, ColorConversion.Bgr2Hsv);
            Image<Gray, Byte>[] channels = hsvimg.Split();
            OpenNewWindowWinIMG(channels[0]);
            OpenNewWindowWinIMG(channels[1]);
            OpenNewWindowWinIMG(channels[2]);


        }
        public static void ConvertRgbToLab(int index)
        {
            Image<Lab, SByte> labimg = new Image<Lab, SByte>(OpenedImagesWindowsList[index].winImg.ColorImage.Width, OpenedImagesWindowsList[index].winImg.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesWindowsList[index].winImg.ColorImage, labimg, ColorConversion.Bgr2Lab);

        }
        public static void SplitChannels(int index)
        {
            Image<Gray, Byte>[] channels = OpenedImagesWindowsList[index].winImg.ColorImage.Split();
            OpenNewWindowWinIMG(channels[0]);
            OpenNewWindowWinIMG(channels[1]);
            OpenNewWindowWinIMG(channels[2]);
        } 
    }
}
