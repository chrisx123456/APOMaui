using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Numerics;
namespace APOMaui
{
    internal static class Main
    {
        public static event Action? OnWinIMGClosingEvent;

        public static List<WindowImageObject> OpenedImagesWindowsList = new();
        public static int? selectedWindow = null;
        public const int windowHeightFix = 100;
        public const int windowWidthFix = 22;
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
                foreach(WindowImageObject wio in OpenedImagesWindowsList) //Sketchy
                {
                    string s = wio.winImg.GetTitle;
                    int dotIndex = s.LastIndexOf('.');
                    string output = s.Substring(0, dotIndex);
                    if (output == fileName) duplicates++;
                }
                fileName += $".{duplicates}";
                OpenNewWindowWinIMG(img, fileName);


            }
            else if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                //TODO
            }
        }
        public static void OpenNewWindowWinIMG(dynamic img, string title) //Argument is dynamic cuz' dont want to make overload for GrayScale/Color.
        {
            WinIMG page = new WinIMG(img, OpenedImagesWindowsList.Count, title);
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
            OpenedImagesWindowsList.Add(windowImageObject);
            selectedWindow = OpenedImagesWindowsList.Count - 1;
#pragma warning disable 8602
            Application.Current.OpenWindow(OpenedImagesWindowsList[(int)selectedWindow].window);
#pragma warning restore 8602
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
            OnWinIMGClosingEvent?.Invoke();
        }
        public static void ChangeSelectedtWinIMG(int index)
        {
            selectedWindow = index;
            System.Diagnostics.Debug.WriteLine($"Selected {index}");
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
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage; //Cant be null, checked in Charts.xaml.cs
            byte[] rawData = img.Bytes;
            int[] GrayScaleHist = CalcHistValues(rawData);
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
            img.Dispose();
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
            img.Dispose();
        }
        public static void ImageNegative(int index)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            byte[] rawData = img.Bytes;
            for (int i = 0; i < rawData.Length; i++) rawData[i] = (byte)(255 - rawData[i]);
            Image<Gray, Byte> res = new(img.Width, img.Height);
            res.Bytes = rawData;
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
            img.Dispose();

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
            img.Dispose();

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
            img.Dispose();
        }
        public static void MedianFilter(int index, int ksize, Emgu.CV.CvEnum.BorderType border, int pixelOffset)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            Image<Gray, Byte> temp = new( img.Width+2*pixelOffset, img.Height+2*pixelOffset);

            //Image<Gray, Byte> res = new(img.Width, img.Height);
            Mat res = new(new System.Drawing.Size(img.Width, img.Height), DepthType.Cv8U, 1);

            CvInvoke.CopyMakeBorder(img, temp, pixelOffset, pixelOffset, pixelOffset, pixelOffset, border, default);
            CvInvoke.MedianBlur(temp, res, ksize);

            Main.OpenedImagesWindowsList[index].winImg.GrayImage = new Image<Gray, Byte>(res);
            //Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;

            img.Dispose();
            temp.Dispose();
        }
        public static void EdgeDetectionFilters(int index, BuiltInFilters filterType, Emgu.CV.CvEnum.BorderType border, int ths1, int ths2)
        {
            //Sobel X, Sobel Y, LaplacianEdge, Canny
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            Mat res = new(new System.Drawing.Size(img.Width, img.Height), DepthType.Cv64F, 1);
            switch(filterType)
            {
                case BuiltInFilters.SobelX:
                    CvInvoke.Sobel(img, res, DepthType.Cv64F, 1, 0, 3, 1, 0, border);
                        break;
                case BuiltInFilters.SobelY:
                    CvInvoke.Sobel(img, res, DepthType.Cv64F, 0, 1, 3, 1, 0, border);
                        break;
                case BuiltInFilters.LaplacianEdge:
                    CvInvoke.Laplacian(img, res, DepthType.Cv64F, 1, 1, 0, border);
                        break;
                case BuiltInFilters.Canny:
                    Mat cannyWBorder = new Mat();
                    CvInvoke.CopyMakeBorder(img, cannyWBorder, 1, 1, 1, 1, border, default);
                    CvInvoke.Canny(cannyWBorder, res, ths1, ths2, 3, false);
                        break;
            }
            Mat final = new();
            CvInvoke.ConvertScaleAbs(res, final, 1, 0);
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = new Image<Gray, Byte>(final);
            img.Dispose();

        }
        public static void BlurFilrers(int index, BuiltInFilters filterType, Emgu.CV.CvEnum.BorderType border)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            Image<Gray, Byte> res = new(img.Width, img.Height);
            switch (filterType)
            {
                case BuiltInFilters.Blur:
                    CvInvoke.Blur(img, res, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1), border);
                        break;
                case BuiltInFilters.GaussianBlur:
                    CvInvoke.GaussianBlur(img, res, new System.Drawing.Size(3, 3), 0, 0, border);
                        break;
            }
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
            img.Dispose();
        }
        public static void ApplyKernel(int index, float[,] kernel, Emgu.CV.CvEnum.BorderType border)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            Image<Gray, Byte> res = new(img.Width, img.Height);
            Matrix<float> inputArray = new(kernel);
            CvInvoke.Filter2D(img, res, inputArray, new System.Drawing.Point(-1, -1), 0, border);
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
            img.Dispose();
        }
        public static void TwoArgsOperations(int index1, int index2, TwoArgsOps arg, double w1, double w2)
        {
            Image<Gray, Byte> img1 = Main.OpenedImagesWindowsList[index1].winImg.GrayImage;
            Image<Gray, Byte> img2 = Main.OpenedImagesWindowsList[index2].winImg.GrayImage;
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
                    Main.OpenedImagesWindowsList[index1].winImg.GrayImage = res;
                    return;
            }
            string title = Main.OpenedImagesWindowsList[index1].winImg.GetTitle + " " + arg.ToString() + " " + Main.OpenedImagesWindowsList[index2].winImg.GetTitle;
            OpenNewWindowWinIMG(res, title);
        }
        public static void TwoStage233(int index, BorderType border, BuiltInFilters stage1, Matrix<float> stage2)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
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
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = final;
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
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            Image<Gray, Byte> res = new(img.Width, img.Height);
            CvInvoke.Filter2D(img, res, finalkernel5x5, new System.Drawing.Point(-1, -1), 0, border);
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;

        }
        public static void ProfileLine(int index, System.Drawing.Point pt1, System.Drawing.Point pt2)
        {
            //Debug.WriteLine($"{pt1.X}, {pt1.Y}, {pt2.X}, {pt2.Y}");
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage.Clone();
            CvInvoke.Line(img, pt1, pt2, new MCvScalar(), 1, LineType.EightConnected, 0);
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = img;
            LineIterator it = new(img.Mat, pt1, pt2, 8, false);
            byte[,,] data = img.Data;
            byte[] res = new byte[it.Count];
            for(int i = 0; i < it.Count; i++)
            {
                res[i] = data[it.Pos.X, it.Pos.Y, 0];
                it.MoveNext();
            }
            

        }
        public static void MathMorph(int index, MorphOp mop, ElementShape es, BorderType border, MCvScalar constant)
        {
            Image<Gray, Byte> img = Main.OpenedImagesWindowsList[index].winImg.GrayImage;
            Image<Gray, Byte> res = new(img.Width, img.Height);
            Mat structElement = CvInvoke.GetStructuringElement(es, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            CvInvoke.MorphologyEx(img, res, mop, structElement, new System.Drawing.Point(-1, -1), 1, border, constant);

            Main.OpenedImagesWindowsList[index].winImg.GrayImage = res;
        }
        public static void Skeletonize(int index, ElementShape es, BorderType border, MCvScalar constant)
        {
            Image<Gray, byte> imgcopy = Main.OpenedImagesWindowsList[index].winImg.GrayImage.Clone();
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
            Main.OpenedImagesWindowsList[index].winImg.GrayImage = skel;
        }
    }
}

