using Emgu.CV.Util;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Drawing;
using Point = System.Drawing.Point;
using CommunityToolkit.Maui.Storage;
namespace APOMaui
{
    internal static class Main
    {
        public static event Action? OnImageClosingOpeningEvent;
        public static event Action? OnImageSelectionChanged;

        public static List<WindowImageObject> OpenedImagesList = new();
        public static int? selectedWindow = null;
        public const int windowHeightFix = 100;
        public const int windowWidthFix = 26;
        public static async void SaveImage(int index, bool saveAs)
        {
            switch (saveAs)
            {
                case false:
                    if (OpenedImagesList[index].ImagePage.path == String.Empty) throw new InvalidOperationException(@"Image has no path. Save this image via ""Save as""");
                    if (OpenedImagesList[index].ImagePage.ColorImage != null && OpenedImagesList[index].ImagePage.Type == ImgType.RGB)
                    {
                        OpenedImagesList[index].ImagePage.ColorImage.Save(OpenedImagesList[index].ImagePage.path);
                    }
                    if (OpenedImagesList[index].ImagePage.GrayImage != null && OpenedImagesList[index].ImagePage.Type == ImgType.Gray)
                    {
                        OpenedImagesList[index].ImagePage.GrayImage.Save(OpenedImagesList[index].ImagePage.path);
                    }
                    break;
                case true:
                    if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        string filename = await Application.Current.MainPage.DisplayPromptAsync("File Name", "Type File Name");
                        string extension = await Application.Current.MainPage.DisplayActionSheet("File extension", null, null, new string[] { ".bmp", ".jpg", ".png" });
                        if (filename == null || filename == String.Empty || extension == null || extension == String.Empty)
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert", "Invalid filename/extension", "Cancel");
                            return;
                        }
                        var fp = await FolderPicker.Default.PickAsync();
                        string path;
                        if (fp.IsSuccessful)
                        {
                            path = fp.Folder.Path;
                            string fullPath = path + "\\" + filename + extension;
                            if (OpenedImagesList[index].ImagePage.ColorImage != null && OpenedImagesList[index].ImagePage.Type == ImgType.RGB)
                            {
                                OpenedImagesList[index].ImagePage.ColorImage.Save(fullPath);
                            }
                            if (OpenedImagesList[index].ImagePage.GrayImage != null && OpenedImagesList[index].ImagePage.Type == ImgType.Gray)
                            {
                                OpenedImagesList[index].ImagePage.GrayImage.Save(fullPath);
                            }
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert", "Path not valid", "Cancel");
                            return;
                        }
                    }
                    break;
            }

        }
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
                int duplicates = 0;
                foreach(WindowImageObject wio in OpenedImagesList) //Sketchy
                {
                    string s = wio.ImagePage.GetTitle;
                    int dotIndex = s.LastIndexOf('.');
                    if (dotIndex == -1) break;
                    string output = s.Substring(0, dotIndex);
                    if (output == fileName) duplicates++;
                }
                fileName += $".{duplicates}";
                OpenNewWindowWinIMG(img, fileName, FileFullPath);
                OnImageClosingOpeningEvent?.Invoke();

            }
            else if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                //TODO
            }
        }
        public static void OpenNewWindowWinIMG(dynamic img, string title, string path) //Argument is dynamic cuz' dont want to make overload for GrayScale/Color.
        {
            ImagePage page = new ImagePage(img, OpenedImagesList.Count, title);
            page.path = path;
            Window newWindow = new Window
            {
                Page = page,
                Title = title,
                Width = img.Width + windowWidthFix,
                Height = img.Height + windowHeightFix,
                MinimumWidth = 0,
                MinimumHeight = 0,
            };
            page.Content.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Main.ChangeSelectedtWinIMG(page.index);
                })
            });

            WindowImageObject windowImageObject = new(page, newWindow);
            OpenedImagesList.Add(windowImageObject);
            ChangeSelectedtWinIMG(OpenedImagesList.Count - 1);
#pragma warning disable 8602
            Application.Current.OpenWindow(OpenedImagesList[(int)selectedWindow].ImagePageWindow);
#pragma warning restore 8602
        }
        public static void OnCloseEventWinIMG(int index)
        {
            System.Diagnostics.Debug.WriteLine($"CloseEventWinIMG: {index}");
            selectedWindow = null;
#pragma warning disable 8602, 8604
            if (OpenedImagesList[index].HistogramChart != null && OpenedImagesList[index].HistogramChartWindow != null)
            {
                Application.Current.CloseWindow(OpenedImagesList[index].HistogramChartWindow);
            }
#pragma warning restore 8602, 8604
            for(int i = index; i < OpenedImagesList.Count; i++)
            {
                OpenedImagesList[i].ImagePage.index--;
            }
            OpenedImagesList[index].ImagePage = null;
            OpenedImagesList[index].ImagePageWindow.ClearLogicalChildren();
            OpenedImagesList[index].HistogramChart = null;
            Application.Current.CloseWindow(OpenedImagesList[index].ImagePageWindow);
            OpenedImagesList[index].Dispose();
            OpenedImagesList.RemoveAt(index);
            GC.Collect();
            OnImageClosingOpeningEvent?.Invoke();
        }
        public static void ChangeSelectedtWinIMG(int index)
        {
            selectedWindow = index;
            System.Diagnostics.Debug.WriteLine($"Selected {index}");
            OnImageSelectionChanged?.Invoke();
        }
        public async static void CompressRLE(int index)
        {
            Image<Gray, Byte> image = Main.OpenedImagesList[index].ImagePage.GrayImage;
            var compressed = new List<(byte, byte)>();
            byte[] data = image.Bytes;
            byte currPixel = data[0];
            byte samePixelCount = 1;
            for(int i = 0; i < data.Length-1; i++)
            {
                byte nextPixel= data[i+1];
                if (nextPixel == currPixel && samePixelCount < byte.MaxValue)
                {
                    samePixelCount++;
                }
                else
                {
                    compressed.Add((currPixel, samePixelCount));
                    samePixelCount = 1;
                    currPixel = nextPixel;
                }

            }
            compressed.Add((currPixel, samePixelCount)); //Last Pixel
            int orgSize = image.Bytes.Length;
            int compressedSize = compressed.Count * (sizeof(byte) + sizeof(ushort));
            double sk = (double)orgSize / (double)compressedSize; //??
            string msg = $"Original size: {orgSize} Bytes\n\nCompressed size: {compressedSize} Bytes\n\nSK: {sk}";
            await Application.Current.MainPage.DisplayAlert("SK", msg, "Ok");
        }
        public static int[] CalcHistValues(byte[] rawData)
        {
            int threads = Environment.ProcessorCount;
            int[] final = new int[256];
            int subsetSize = rawData.Length / threads;
            Task<int[]>[]? tasks = new Task<int[]>[threads];
            for(int i = 0; i< threads; i++)
            {
                int startIndex = i * subsetSize;
                int endIndex = (i+1)*subsetSize;
                if (i == threads - 1) endIndex = rawData.Length; //If rawData is not divisible by threads no. 
                tasks[i] = new Task<int[]>(() =>
                {
                    int[] subHist = new int[256];
                    for(int j = startIndex; j < endIndex; j++) { subHist[rawData[j]]++; };
                    return subHist;
                });
            }
            foreach (Task<int[]> task in tasks) task.Start();
            Task.WaitAll(tasks);
            foreach (Task < int[]> task in tasks)
            {
                final = final.Zip(task.Result, (a, b) => a + b).ToArray();
            }
            tasks = null;
            return final;
        }
        public static void CreateHistogramChart(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage; //Cant be null, checked in Charts.xaml.cs
            byte[] rawData = img.Bytes;
            int[] GrayScaleHist = CalcHistValues(rawData);
            var page = new HistogramChart(GrayScaleHist, index);
            OpenedImagesList[index].HistogramChart = page;
            int width = 830, height = 350;
            var newWindow = new Window
            {
                Page = OpenedImagesList[index].HistogramChart,
                Width = width,
                Height = height,
                MinimumWidth = width,
                MaximumWidth = width,
                MinimumHeight = height,
                MaximumHeight = height,
                Title = OpenedImagesList[index].ImagePageWindow.Title + " Histogram"
            };
            OpenedImagesList[index].HistogramChartWindow = newWindow;
            Application.Current?.OpenWindow(OpenedImagesList[index].HistogramChartWindow);
        }
        public static void ConvertRgbToGray(int index)
        {
            Image<Gray, Byte> grayImage = new(OpenedImagesList[index].ImagePage.ColorImage.Width, OpenedImagesList[index].ImagePage.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesList[index].ImagePage.ColorImage, grayImage, ColorConversion.Bgr2Gray);
            OpenedImagesList[index].ImagePage.GrayImage = grayImage;
            OpenedImagesList[index].ImagePage.Type = ImgType.Gray;
            OnImageSelectionChanged?.Invoke();
        }
        public static void ConvertRgbToHsv(int index) 
        {
            Image<Hsv, Byte> hsvimg = new(OpenedImagesList[index].ImagePage.ColorImage.Width, OpenedImagesList[index].ImagePage.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesList[index].ImagePage.ColorImage, hsvimg, ColorConversion.Bgr2HsvFull);
            Image<Gray, Byte>[] channels = hsvimg.Split();
            OpenNewWindowWinIMG(channels[0], OpenedImagesList[index].ImagePageWindow.Title + " Hue", String.Empty);
            OpenNewWindowWinIMG(channels[1], OpenedImagesList[index].ImagePageWindow.Title + " Saturation", String.Empty);
            OpenNewWindowWinIMG(channels[2], OpenedImagesList[index].ImagePageWindow.Title + " Value", String.Empty);
        }
        public static void ConvertRgbToLab(int index)
        {
            Image<Lab, Byte> labimg = new(OpenedImagesList[index].ImagePage.ColorImage.Width, OpenedImagesList[index].ImagePage.ColorImage.Height);
            CvInvoke.CvtColor(OpenedImagesList[index].ImagePage.ColorImage, labimg, ColorConversion.Bgr2Lab);
            Image<Gray, Byte>[] channels = labimg.Split();
            OpenNewWindowWinIMG(channels[0], OpenedImagesList[index].ImagePageWindow.Title + " L", String.Empty);
            OpenNewWindowWinIMG(channels[1], OpenedImagesList[index].ImagePageWindow.Title + " a", String.Empty);
            OpenNewWindowWinIMG(channels[2], OpenedImagesList[index].ImagePageWindow.Title + " b", String.Empty);
        }
        public static void SplitChannels(int index)
        {
            Image<Gray, Byte>[] channels = OpenedImagesList[index].ImagePage.ColorImage.Split();
            OpenNewWindowWinIMG(channels[0], OpenedImagesList[index].ImagePageWindow.Title + " Blue", String.Empty);
            OpenNewWindowWinIMG(channels[1], OpenedImagesList[index].ImagePageWindow.Title + " Green", String.Empty);
            OpenNewWindowWinIMG(channels[2], OpenedImagesList[index].ImagePageWindow.Title + " Red", String.Empty);
        }
        public static void HistEqualization(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage; 
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
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
            img.Dispose();
        }
        public static void HistStretch(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
           
            byte[] rawData = img.Bytes;
            byte max = rawData.Max();
            byte min = rawData.Min();
            for (int i = 0; i<rawData.Length; i++)
            {
                rawData[i] = (byte)((255 / (float)(max - min)) * (rawData[i] - min));
            }
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
            img.Dispose();
        }
        public static void ImageNegative(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            byte[] rawData = img.Bytes;
            for (int i = 0; i < rawData.Length; i++) rawData[i] = (byte)(255 - rawData[i]);
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
            img.Dispose();

        }
        public static void HistStretchInRange(int index, byte p1, byte p2, byte q3, byte q4) //TODO: Optimize//fix
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            byte[] rawData = img.Bytes;

            for (int i = 0; i < rawData.Length; i++)
            {
                byte pixelValue = rawData[i];

                if (pixelValue >= p1 && pixelValue <= p2)
                {
                    rawData[i] = (byte)((pixelValue - p1) * (q4 - q3) / (p2 - p1) + q3);
                }
            }
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
            img.Dispose();

        }
        public static void Posterize(int index, byte levels)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
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
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
            img.Dispose();
        }
        public static void MedianFilter(int index, int ksize, Emgu.CV.CvEnum.BorderType border, int pixelOffset)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Image<Gray, Byte> temp = new( img.Width+2*pixelOffset, img.Height+2*pixelOffset);

            //Image<Gray, Byte> res = new(img.Width, img.Height);
            Mat res = new(new System.Drawing.Size(img.Width, img.Height), DepthType.Cv8U, 1);

            CvInvoke.CopyMakeBorder(img, temp, pixelOffset, pixelOffset, pixelOffset, pixelOffset, border, default);
            CvInvoke.MedianBlur(temp, res, ksize);

            Main.OpenedImagesList[index].ImagePage.GrayImage = new Image<Gray, Byte>(res);
            //Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;

            img.Dispose();
            temp.Dispose();
        }
        public static void Canny(int index, Emgu.CV.CvEnum.BorderType border, int ths1, int ths2)
        {
            //Sobel X, Sobel Y, LaplacianEdge, Canny
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Mat res = new(new System.Drawing.Size(img.Width, img.Height), DepthType.Cv64F, 1);
            Mat cannyWBorder = new Mat();
            CvInvoke.CopyMakeBorder(img, cannyWBorder, 1, 1, 1, 1, border, default);
            CvInvoke.Canny(cannyWBorder, res, ths1, ths2, 3, false);
            Mat final = new();
            CvInvoke.ConvertScaleAbs(res, final, 1, 0);
            Main.OpenedImagesList[index].ImagePage.GrayImage = new Image<Gray, Byte>(final);
            img.Dispose();

        }
        public static void ApplyKernel(int index, float[,] kernel, Emgu.CV.CvEnum.BorderType border)
        {
            float sum = kernel.Cast<float>().Sum();
            if(sum != 0)
                for (int i = 0; i < kernel.GetLength(0); i++)
                    for (int j = 0; j < kernel.GetLength(1); j++)
                        kernel[i, j] = kernel[i, j] / sum;

            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Mat res = new(new System.Drawing.Size(img.Width, img.Height), DepthType.Cv64F, 1);

            Matrix<float> inputArray = new(kernel);
            CvInvoke.Filter2D(img, res, inputArray, new System.Drawing.Point(-1, -1), 0, border);

            Mat final = new();
            CvInvoke.ConvertScaleAbs(res, final, 1, 0);
            Main.OpenedImagesList[index].ImagePage.GrayImage = new Image<Gray, Byte>(final);
            img.Dispose();
        }
        public static void TwoArgsOperations(int index1, int index2, TwoArgsOps arg, double w1, double w2)
        {
            Image<Gray, Byte> img1 = Main.OpenedImagesList[index1].ImagePage.GrayImage;
            Image<Gray, Byte> img2 = Main.OpenedImagesList[index2].ImagePage.GrayImage;
            Image<Gray, Byte> res = new(img1.Width, img1.Height);
            switch (arg)
            {
                case TwoArgsOps.ADD:
                    CvInvoke.Add(img1, img2, res, null, DepthType.Cv8U);
                    break;
                case TwoArgsOps.SUBTRACT:
                    CvInvoke.Subtract(img1, img2, res, null, DepthType.Cv8U);
                    break;
                case TwoArgsOps.BLEND:
                    CvInvoke.AddWeighted(img1, w1, img2, w2, gamma: 0, res, DepthType.Cv8U);
                    break;
                case TwoArgsOps.AND:
                    CvInvoke.BitwiseAnd(img1, img2, res, null);
                    break;
                case TwoArgsOps.OR:
                    CvInvoke.BitwiseOr(img1, img2, res, null);
                    break;
                case TwoArgsOps.XOR:
                    CvInvoke.BitwiseXor(img1, img2, res, null);
                    break;
                case TwoArgsOps.NOT:
                    CvInvoke.BitwiseNot(img1, res, null);
                    Main.OpenedImagesList[index1].ImagePage.GrayImage = res;
                    return;
            }
            string title = Main.OpenedImagesList[index1].ImagePage.GetTitle + " " + arg.ToString() + " " + Main.OpenedImagesList[index2].ImagePage.GetTitle;
            OpenNewWindowWinIMG(res, title, String.Empty);
        }
        public static void TwoStage233(int index, BorderType border, BuiltInFilters stage1, Matrix<float> stage2)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Image<Gray, Byte> stage1res = new(img.Width, img.Height);
            Image<Gray, Byte> final = new(img.Width, img.Height);
            switch (stage1)
            {
                case BuiltInFilters.Blur:
                    CvInvoke.Blur(img, stage1res, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1,-1), border);
                    break;
                case BuiltInFilters.GaussianBlur:
                    CvInvoke.GaussianBlur(img, stage1res, new System.Drawing.Size(3, 3), 0, 0, border);
                    break;
                default:
                    Debug.WriteLine("Wrong builtinfilter at TwoStage233 main.cs");
                    return;
            }
            CvInvoke.Filter2D(stage1res, final, stage2, new System.Drawing.Point(-1, -1), 0, border);
            Main.OpenedImagesList[index].ImagePage.GrayImage = final;
        }
        public static void TwoStage55(int index, BorderType border, BuiltInFilters stage1, Matrix<float> stage2)
        {
            Matrix<float> blur5x5 = new(new float[,] {
            { 0f, 0f, 0f, 0f, 0f },
            { 0f, 1f/9f, 1f/9f, 1f/9f, 0f },
            { 0f, 1f/9f, 1f/9f, 1f/9f, 0f },
            { 0f, 1f/9f, 1f/9f, 1f/9f, 0f },
            { 0f, 0f, 0f, 0f, 0f }});
            Matrix<float> gblur5x5 = new(new float[,] {
            { 0f, 0f, 0f, 0f, 0f },
            { 0f, 1f/16f, 1f/8f, 1f/16f, 0f },
            { 0f, 1f/8f, 1f/4f, 1f/8f, 0f },
            { 0f, 1f/16f, 1f/8f, 1f/16f, 0f },
            { 0f, 0f, 0f, 0f, 0f }});
            Matrix<float> stage2kernel5x5 = new(new float[5,5]);
            CvInvoke.CopyMakeBorder(stage2, stage2kernel5x5, 1, 1, 1, 1, BorderType.Constant, new MCvScalar(0.0, 0.0, 0.0, 0.0));
            Matrix<float> finalkernel5x5 = new(new float[5, 5]);
            
            switch (stage1)
            {
                case BuiltInFilters.Blur:
                    CvInvoke.Filter2D(blur5x5, finalkernel5x5, stage2kernel5x5, new System.Drawing.Point(-1, -1), 0, BorderType.Isolated);
                    break;
                case BuiltInFilters.GaussianBlur:
                    CvInvoke.Filter2D(gblur5x5, finalkernel5x5, stage2kernel5x5, new System.Drawing.Point(-1, -1), 0, BorderType.Isolated);
                    break;
                default:
                    Debug.WriteLine("Wrong builtinfilter at TwoStage55 main.cs");
                    return ;
            }
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Image<Gray, Byte> res = new(img.Width, img.Height);
            CvInvoke.Filter2D(img, res, finalkernel5x5, new System.Drawing.Point(-1, -1), 0, border);
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;

        }
        public static void ProfileLine(int index, System.Drawing.Point pt1, System.Drawing.Point pt2)
        {
            //Debug.WriteLine($"{pt1.X}, {pt1.Y}, {pt2.X}, {pt2.Y}");
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage.Clone();
            //CvInvoke.Line(img, pt1, pt2, new MCvScalar(0,0,255), 1, LineType.EightConnected, 0);
            LineIterator it = new(img.Mat, pt1, pt2, 8, false);
            byte[,,] data = img.Data;
            int[] res = new int[it.Count];
            for(int i = 0; i < it.Count; i++)
            {
                //chuj wie czy yx czy xy
                res[i] = data[it.Pos.Y, it.Pos.X, 0];
                it.MoveNext();
            }
            ProfileLineChart plc = new ProfileLineChart(res);
            int width = 700, height = 350;
            Window plcw = new Window
            {
                Page = plc,
                Width = width,
                Height = height,
                MinimumWidth = width,
                MaximumWidth = width,
                MinimumHeight = height,
                MaximumHeight = height,
                Title = OpenedImagesList[index].ImagePageWindow.Title + " Profile Line"
            };
            Application.Current.OpenWindow(plcw);

        }
        public static void MathMorph(int index, MorphOp mop, ElementShape es, BorderType border, MCvScalar constant)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Image<Gray, Byte> res = new(img.Width, img.Height);
            Mat structElement = CvInvoke.GetStructuringElement(es, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));

            CvInvoke.MorphologyEx(img, res, mop, structElement, new System.Drawing.Point(-1, -1), 1, border, constant);
            
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
        }
        public static void Skeletonize(int index, ElementShape es, BorderType border, MCvScalar constant)
        {
            Image<Gray, byte> imgcopy = Main.OpenedImagesList[index].ImagePage.GrayImage.Clone();
            Image<Gray, byte> skel = new(imgcopy.Size);
            skel.SetValue(0);

            var ese = CvInvoke.GetStructuringElement(es, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            while (true)
            {
                Image<Gray, byte> imopen = new(imgcopy.Size);
                CvInvoke.MorphologyEx(imgcopy, imopen, MorphOp.Open, ese, new System.Drawing.Point(-1, -1), 1, border, constant);

                Image<Gray, byte> temp = new(imgcopy.Size);
                CvInvoke.Subtract(imgcopy, imopen, temp);

                Image<Gray, byte> eroded = new(imgcopy.Size);
                CvInvoke.Erode(imgcopy, eroded, ese, new System.Drawing.Point(-1, -1), 1, border, constant);

                CvInvoke.BitwiseOr(skel, temp, skel);

                imgcopy.Dispose();
                imgcopy = eroded.Clone();
                if (CvInvoke.CountNonZero(imgcopy)==0) break;
            }
            Main.OpenedImagesList[index].ImagePage.GrayImage = skel;
           
        }
        public static void Watershed(int index)
        {

            Image<Gray, Byte> input = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Image<Bgr, Byte> img = new(input.Size);
            CvInvoke.CvtColor(input, img, ColorConversion.Gray2Bgr, 0);
            Image<Gray, byte> gray = img.Convert<Gray, Byte>();
            Image<Gray, byte> thresh = gray.CopyBlank();
            CvInvoke.Threshold(gray, thresh, 0, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv | Emgu.CV.CvEnum.ThresholdType.Otsu);

            Matrix<byte> kernel = new Matrix<byte>(new Byte[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }); //https://stackoverflow.com/a/33646626/4926757
            Image<Gray, Byte> opening = thresh.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Open, kernel, new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
            Image<Gray, Byte> sureBg = opening.Dilate(3);

            Mat distanceTransform = new Mat();
            CvInvoke.DistanceTransform(opening, distanceTransform, null, Emgu.CV.CvEnum.DistType.L2, 5);

            double minVal = 0, maxVal = 0;
            Point minLoc = new Point(), maxLoc = new Point();
            CvInvoke.MinMaxLoc(distanceTransform, ref minVal, ref maxVal, ref minLoc, ref maxLoc);  //Find distanceTransform.max()

            Mat sureFg0 = new Mat();
            CvInvoke.Threshold(distanceTransform, sureFg0, 0.7 * maxVal, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
            Mat sureFg = new Mat();
            sureFg0.ConvertTo(sureFg, Emgu.CV.CvEnum.DepthType.Cv8U); //Convert from float to Byte

            Mat unknown = new Mat();
            CvInvoke.Subtract(sureBg, sureFg, unknown);

            Mat markers = new Mat();
            CvInvoke.ConnectedComponents(sureFg, markers);
            markers = markers + 1;

            Mat zeros = markers - markers; 
            zeros.CopyTo(markers, unknown); 

            //Mat finalMarkers = new Mat(); //????
            CvInvoke.Watershed(img, markers);

            Mat mask = new Mat();
            zeros.SetTo(new MCvScalar(-1)); 
            CvInvoke.Compare(markers, zeros, mask, CmpType.Equal);
            mask.ConvertTo(mask, Emgu.CV.CvEnum.DepthType.Cv8U); 

            Main.OpenedImagesList[index].ImagePage.GrayImage = mask.ToImage<Gray, Byte>();

        } //Chyba dziala, na pewno musze dodac komenatrze i zrozumiec co tu sie wgle dzieje
        public static void HoughLines(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
            Mat canny = new();
            Image<Bgr, Byte> res = new(img.Size);
            CvInvoke.CvtColor(img, res, ColorConversion.Gray2Bgr, 0);
            CvInvoke.CopyMakeBorder(img, canny, 1, 1, 1, 1, BorderType.Reflect101, default);
            CvInvoke.Canny(canny, canny, 100, 200, 3, false);
            CvInvoke.ConvertScaleAbs(canny, canny, 1, 0);


            LineSegment2D[] lines;
            using (var vector = new VectorOfPointF())
            {
                CvInvoke.HoughLines(canny, vector, 1.0, Math.PI / 180.0, 250);
                var linesList = new List<LineSegment2D>();
                for (var i = 0; i < vector.Size; i++)
                {
                    var rho = vector[i].X;
                    var theta = vector[i].Y;
                    var pt1 = new System.Drawing.Point();
                    var pt2 = new System.Drawing.Point();
                    var a = Math.Cos(theta);
                    var b = Math.Sin(theta);
                    var x0 = a * rho;
                    var y0 = b * rho;
                    pt1.X = (int)Math.Round(x0 + img.Width * (-b));
                    pt1.Y = (int)Math.Round(y0 + img.Height * (a));
                    pt2.X = (int)Math.Round(x0 - img.Width * (-b));
                    pt2.Y = (int)Math.Round(y0 - img.Height * (a));

                    res.Draw(new LineSegment2D(pt1, pt2), new Bgr(50,50,230), 3);
                    linesList.Add(new LineSegment2D(pt1, pt2));
                }

                lines = linesList.ToArray();
            }
            Main.OpenedImagesList[index].ImagePage.ColorImage = res;





        } 
        public static void Pyramid(int index, PyramidType pt)
        {
            Mat img = new Mat();
            Mat res = new();
            if (Main.OpenedImagesList[index].ImagePage.Type == ImgType.RGB)
            {
                img = Main.OpenedImagesList[index].ImagePage.ColorImage.Mat;
                if(pt == PyramidType.UP)
                {
                    CvInvoke.PyrUp(img, res, BorderType.Reflect101);
                }
                else
                {
                    CvInvoke.PyrDown(img, res, BorderType.Reflect101);
                }
                Main.OpenedImagesList[index].ImagePage.ColorImage = res.ToImage<Bgr, Byte>();
            }
            else
            {
                img = Main.OpenedImagesList[index].ImagePage.GrayImage.Mat;
                if (pt == PyramidType.UP)
                {
                    CvInvoke.PyrUp(img, res, BorderType.Reflect101);
                }
                else
                {
                    CvInvoke.PyrDown(img, res, BorderType.Reflect101);
                }
                Main.OpenedImagesList[index].ImagePage.GrayImage = res.ToImage<Gray, Byte>();
            }
            Main.OpenedImagesList[index].ImagePageWindow.Height = res.Height + windowHeightFix;
            Main.OpenedImagesList[index].ImagePageWindow.Width = res.Width + windowWidthFix;
        }
        public static void GrabCut(int index, Rectangle rect)
        {
            Image<Bgr, Byte> img = Main.OpenedImagesList[index].ImagePage.ColorImage;
            Image<Gray, Byte> mask = new Image<Gray, Byte>(img.Size);
            mask.SetZero();
            Matrix<double> bg = new Matrix<double>(1, 65);
            bg.SetZero();
            Matrix<double> fg = new Matrix<double>(1, 65);
            fg.SetZero();

            CvInvoke.GrabCut(img, mask, rect, bg, fg, 3, GrabcutInitType.InitWithRect);

            for (int x = 0; x < mask.Cols; x++)
            {
                for (int y = 0; y < mask.Rows; y++)
                {
                    if (mask[y, x].Intensity == new Gray(1).Intensity || mask[y, x].Intensity == new Gray(3).Intensity)
                    {
                        mask[y, x] = new Gray(1);
                    }
                    else
                    {
                        mask[y, x] = new Gray(0);
                    }
                }

            }
            img = img.Mul(mask.Convert<Bgr, Byte>());
            Main.OpenedImagesList[index].ImagePage.ColorImage = img.Clone();
            }
        public static async void Inpainting(int index)
        {
            if (!Main.OpenedImagesList[index].ImagePage.GetDrawBoxState())
            {
                Main.OpenedImagesList[index].ImagePage.ToggleDrawBox(true);
            }
            else
            {
                Debug.WriteLine("Getting Canvas Image");
                Image<Gray, Byte> mask = await Main.OpenedImagesList[index].ImagePage.GetImageFromCanvas();
                //CvInvoke.Imshow("dsd", mask);
                if (Main.OpenedImagesList[index].ImagePage.ColorImage != null)
                {
                    Image<Bgr, Byte> img = Main.OpenedImagesList[index].ImagePage.ColorImage;
                    Image<Bgr, Byte> res = new(img.Size);
                    //CvInvoke.Inpaint(img, mask, res, 3.0, InpaintType.Telea);
                    //Main.OpenedImagesWindowsList[index].winImg.ColorImage = res;
                }
                else if(Main.OpenedImagesList[index].ImagePage.GrayImage != null)
                {
                    Image<Gray, Byte> img = Main.OpenedImagesList[index].ImagePage.GrayImage;
                    Image<Gray, Byte> res = new(img.Size);
                    //CvInvoke.Inpaint(img, mask, res, 3.0, InpaintType.Telea);
                    //Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
                }
            }

        }
        public static void AnalizeImage(int index)
        {
            List<AnalysisResult> results = new List<AnalysisResult>();
            Mat img = new();
            string title = Main.OpenedImagesList[index].ImagePage.Title;
            if (Main.OpenedImagesList[index].ImagePage.ColorImage != null)
            {
                CvInvoke.CvtColor(Main.OpenedImagesList[index].ImagePage.ColorImage, img, ColorConversion.Bgr2Gray, 0);
            }
            else
            {
                img = Main.OpenedImagesList[index].ImagePage.GrayImage.Mat;
            }
            Mat cntImg = new(img.Size, DepthType.Cv8U, 3);

            CvInvoke.Threshold(img, img, 127, 255, ThresholdType.Binary);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(img, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);
            for (int i = 0; i < contours.Size; i++)
            {
                Random r = new Random();
                CvInvoke.DrawContours(cntImg, new VectorOfVectorOfPoint(contours[i]), -1, new MCvScalar(r.Next(255), r.Next(255), r.Next(255)), 1, LineType.EightConnected, null, -1, default);
                double area = CvInvoke.ContourArea(contours[i]);
                double perimeter = CvInvoke.ArcLength(contours[i], true);
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                double aspectRatio = (double)rect.Width / (double)rect.Height;
                double extent = (double)area / (double)(rect.Width + rect.Height);

                var vf = new System.Drawing.PointF[contours[i].Size];
                for (int ii = 0; ii < contours[i].Size; ii++) vf[ii] = contours[i][ii];
                var hull = CvInvoke.ConvexHull(vf);
                double hull_area = CvInvoke.ContourArea(new VectorOfPointF(hull), true);
                double solidity = area / hull_area;
                double equivalentDiameter = Math.Sqrt(4*area / Math.PI);

                Debug.WriteLine("Obiekt {0}:", i);
                Debug.WriteLine("Momenty: {0}", contours[i].Size);
                Debug.WriteLine("Pole powierzchni: {0}", area);
                Debug.WriteLine("Obwód: {0}", perimeter);
                Debug.WriteLine("Współczynniki kształtu:");
                Debug.WriteLine("  AspectRatio: {0}", aspectRatio);
                Debug.WriteLine("  Extent: {0}", extent);
                Debug.WriteLine("  Solidity: {0}", solidity);
                Debug.WriteLine("  EquivalentDiameter: {0}", equivalentDiameter);
                results.Add(new AnalysisResult(i, contours[i].Size, area, perimeter, aspectRatio, extent, solidity, equivalentDiameter));
            }
            AnalysisResultPage arp = new AnalysisResultPage(results);
            Window w = new Window
            {
                Page = arp,
                Height = 300,
                Width = 700
            };
            //OpenNewWindowWinIMG(new Image<Bgr, Byte>(cntImg), $"{title} Contours");
            Main.OpenedImagesList[index].ImagePage.ColorImage = new Image<Bgr, Byte>(cntImg);
            Application.Current.OpenWindow(w);
            //CvInvoke.Imshow("sd", cntImg);
        }
        public static void Thresh(Image<Gray, Byte> img, int index, ThreshType type, int t1, int t2)
        {
            Image<Gray, Byte> res = new Image<Gray, Byte>(img.Size);
            switch (type)
            {
                case ThreshType.MANUAL:
                    CvInvoke.Threshold(img, res, t1, t2, ThresholdType.Binary);
                    break;
                case ThreshType.ADAPTIVE:
                    CvInvoke.AdaptiveThreshold(img, res, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 3, 1); //Ostatnie idk co to jest
                    break;
                case ThreshType.OTSU:
                    CvInvoke.Threshold(img, res, 0, 255, ThresholdType.Otsu);
                    break;
                default:
                    break;
            }
            Main.OpenedImagesList[index].ImagePage.GrayImage = res;
        }
    }
}

