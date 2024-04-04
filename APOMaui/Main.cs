using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
namespace APOMaui
{
    internal static class Main
    {
        public static List<WindowImageObject> OpenedImagesWindowsList = new();
        public static int? selectedWindow = null;
        public static readonly int InitWidth = 400;
       // public static readonly int MaxWidth = 800;
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
                string fileName = FileFullPath.Substring(FileFullPath.LastIndexOf('\\') + 1);
                OpenNewWindowWinIMG(img, fileName);


            }
            else if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                //TODO
            }
        }
        public static void OpenNewWindowWinIMG(dynamic img, string title) //Argument is dynamic cuz' dont want to make overload for GrayScale/Color.
        {
            var page = new WinIMG(img, OpenedImagesWindowsList.Count, img.Width, img.Height);
            var newWindow = new Window
            {
                Page = page,
                Width = InitWidth,
                Height = ((double)InitWidth / (double)img.Width) * (double)img.Height + windowHeightFix,
                Title = title
            };

            WindowImageObject windowImageObject = new(page, newWindow);
            OpenedImagesWindowsList.Add(windowImageObject);
            selectedWindow = OpenedImagesWindowsList.Count - 1;
#pragma warning disable 8602
            Application.Current.OpenWindow(OpenedImagesWindowsList[(int)selectedWindow].window);
#pragma warning restore 8602
        }

        public static void ResizeWindow(int index, double newwidth, double realwidth, double realheight)
        {
            OpenedImagesWindowsList[index].window.Width = newwidth;
            OpenedImagesWindowsList[index].window.Height = (((newwidth / realwidth) * realheight) + windowHeightFix);
        }
        public static void OnCloseEventWinIMG(int index)
        {
            System.Diagnostics.Debug.WriteLine($"CloseEventWinIMG: {index}");
            selectedWindow = null;
#pragma warning disable 8602, 8604
            if (OpenedImagesWindowsList[index].chart != null && OpenedImagesWindowsList[index].chart.window != null)
            {
                Application.Current.CloseWindow(OpenedImagesWindowsList[index].chart.window);
            }
#pragma warning restore 8602, 8604
            for(int i = index; i < OpenedImagesWindowsList.Count; i++)
            {
                OpenedImagesWindowsList[i].winImg.index--;
            }
            OpenedImagesWindowsList[index].winImg = null;
            OpenedImagesWindowsList[index].window.ClearLogicalChildren();
            OpenedImagesWindowsList[index].chart = null;
            Application.Current.CloseWindow(OpenedImagesWindowsList[index].window);
            OpenedImagesWindowsList[index].Dispose();
            OpenedImagesWindowsList.RemoveAt(index);
            GC.Collect();
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
                Title = OpenedImagesWindowsList[index].window.Title + " Histogram"
                
            };
            page.window = newWindow;
#pragma warning disable 8602
            Application.Current.OpenWindow(page.window);
#pragma warning restore 8602
        }
        public static void ConvertRgbToGray(int index)
        {
            Image<Gray, Byte> grayImage = new(OpenedImagesWindowsList[index].winImg.ColorImage.Width, OpenedImagesWindowsList[index].winImg.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesWindowsList[index].winImg.ColorImage, grayImage, ColorConversion.Bgr2Gray);
            OpenedImagesWindowsList[index].winImg.GrayImage = grayImage;
            OpenedImagesWindowsList[index].winImg.Type = ImgType.Gray;
        }
        public static void ConvertRgbToHsv(int index) 
        {
            Image<Hsv, Byte> hsvimg = new(OpenedImagesWindowsList[index].winImg.ColorImage.Width, OpenedImagesWindowsList[index].winImg.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesWindowsList[index].winImg.ColorImage, hsvimg, ColorConversion.Bgr2HsvFull);
            Image<Gray, Byte>[] channels = hsvimg.Split();
            OpenNewWindowWinIMG(channels[0], OpenedImagesWindowsList[index].window.Title + " Hue");
            OpenNewWindowWinIMG(channels[1], OpenedImagesWindowsList[index].window.Title + " Saturation");
            OpenNewWindowWinIMG(channels[2], OpenedImagesWindowsList[index].window.Title + " Value");
        }
        public static void ConvertRgbToLab(int index)
        {
            Image<Lab, Byte> labimg = new(OpenedImagesWindowsList[index].winImg.ColorImage.Width, OpenedImagesWindowsList[index].winImg.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesWindowsList[index].winImg.ColorImage, labimg, ColorConversion.Bgr2Lab);
            Image<Gray, Byte>[] channels = labimg.Split();
            OpenNewWindowWinIMG(channels[0], OpenedImagesWindowsList[index].window.Title + " L");
            OpenNewWindowWinIMG(channels[1], OpenedImagesWindowsList[index].window.Title + " a");
            OpenNewWindowWinIMG(channels[2], OpenedImagesWindowsList[index].window.Title + " b");
        }
        public static void SplitChannels(int index)
        {
            Image<Gray, Byte>[] channels = OpenedImagesWindowsList[index].winImg.ColorImage.Split();
            OpenNewWindowWinIMG(channels[0], OpenedImagesWindowsList[index].window.Title + " Blue");
            OpenNewWindowWinIMG(channels[1], OpenedImagesWindowsList[index].window.Title + " Green");
            OpenNewWindowWinIMG(channels[2], OpenedImagesWindowsList[index].window.Title + " Red");
        }
        public static void HistEqualization(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage; 
            byte[] rawData = img.Bytes;
            int[] count = new int[256];
            int[] CDF = new int[256];

            CDF[0] = count[0];
            foreach (byte b in rawData) count[b]++;
            for (int i = 1; i < 256; i++) CDF[i] = CDF[i - 1] + count[i];

            for (int i = 0; i < 256; i++) CDF[i] = (int)((CDF[i] / (float)CDF[255]) * 255.0f); //CDF[255] is sum

            for (int i = 0; i < rawData.Length; i++)
            {
                rawData[i] = (byte)CDF[rawData[i]];
            }

            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
        }
        public static void HistStretch(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
           
            byte[] rawData = img.Bytes;
            byte max = rawData.Max();
            byte min = rawData.Min();
            for (int i = 0; i<rawData.Length; i++)
            {
                rawData[i] = (byte)((255 / (float)(max - min)) * (rawData[i] - min));
            }
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
        }
        public static void ImageNegative(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            byte[] rawData = img.Bytes;
            for (int i = 0; i < rawData.Length; i++) rawData[i] = (byte)(255 - rawData[i]);
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
        }
        public static void HistStretchInRange(int index, int Lmin, int Lmax) //TODO: Optimize//fix
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            byte[] rawData = img.Bytes;
            byte min = rawData.Min();
            byte max = rawData.Max();
            for(int i=0; i < rawData.Length; i++)
            {
                if (rawData[i] < min)
                {
                    rawData[i] = (byte)Lmin;
                } 
                else if (rawData[i] <= max) 
                {
                    rawData[i] = (byte)((rawData[i] - min) * Lmax / (max - min));
                }
                else //rawdata[i] > max
                {
                    rawData[i] = (byte)Lmax;
                }
            }
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;

        }
        public static void Posterize(int index, byte levels)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            byte[] rawData = img.Bytes;
            int step = 255 / levels;
            byte[] borders = new byte[levels];
            byte[] levelsOfBrigtness = new byte[levels];
            byte[] LUT = new byte[256];

            for (int i = 1; i < levels; i++)
            {
                borders[i - 1] = (byte)(step * i);
            }
            borders[levels-1] = 255;
            for(int i = 0; i < levels; i++)
            {
                if (i == 0)
                {
                    levelsOfBrigtness[i] = 0;
                }
                if (i == levels - 1)
                {
                    levelsOfBrigtness[i] = 255;
                }
                else
                {
                    levelsOfBrigtness[i] = (byte)((255 / (levels - 1)) * i);
                }
            }

            int currpos = 0;
            for (int i = 0; i < levels; i++)
            {
                for (int j = currpos; j <= borders[i]; j++, currpos++)
                {
                    LUT[j] = levelsOfBrigtness[i];
                }
            }
            
            for(int i = 0; i < rawData.Length; i++)
            {
                rawData[i] = LUT[rawData[i]];
            }

            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
        }
    }
}
