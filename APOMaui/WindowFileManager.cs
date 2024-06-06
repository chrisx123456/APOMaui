using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APOMaui
{
    internal static class WindowFileManager
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
        public static async void CreateImagePage()
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
                foreach (WindowImageObject wio in OpenedImagesList) //Sketchy
                {
                    string s = wio.ImagePage.GetTitle;
                    int dotIndex = s.LastIndexOf('.');
                    if (dotIndex == -1) break;
                    string output = s.Substring(0, dotIndex);
                    if (output == fileName) duplicates++;
                }
                fileName += $".{duplicates}";
                OpenNewWindowImagePage(img, fileName, FileFullPath);
                OnImageClosingOpeningEvent?.Invoke();

            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                //TODO
            }
        }
        public static void OpenNewWindowImagePage(dynamic img, string title, string path) //Argument is dynamic cuz' dont want to make overload for GrayScale/Color.
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
                    WindowFileManager.ChangeSelectedImagePage(page.index);
                })
            });

            WindowImageObject windowImageObject = new(page, newWindow);
            OpenedImagesList.Add(windowImageObject);
            ChangeSelectedImagePage(OpenedImagesList.Count - 1);
#pragma warning disable 8602
            Application.Current.OpenWindow(OpenedImagesList[(int)selectedWindow].ImagePageWindow);
#pragma warning restore 8602
        }
        public static void OnCloseImagePage(int index)
        {
            System.Diagnostics.Debug.WriteLine($"CloseEventWinIMG: {index}");
            selectedWindow = null;
#pragma warning disable 8602, 8604
            if (OpenedImagesList[index].HistogramChart != null && OpenedImagesList[index].HistogramChartWindow != null)
            {
                Application.Current.CloseWindow(OpenedImagesList[index].HistogramChartWindow);
            }
#pragma warning restore 8602, 8604
            for (int i = index; i < OpenedImagesList.Count; i++)
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
        public static void ChangeSelectedImagePage(int index)
        {
            selectedWindow = index;
            System.Diagnostics.Debug.WriteLine($"Selected {index}");
            OnImageSelectionChanged?.Invoke();
        }
        public static void RaiseEventRGB2GrayConversion()
        {
            OnImageSelectionChanged?.Invoke();
        }
    }
}
