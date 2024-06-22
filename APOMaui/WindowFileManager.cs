using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APOMaui
{
    internal static class WindowFileManager
    {
        public static event Action? OnImageOpeningEvent;
        public static event Action? OnImageClosingEvent;
        public static event Action? OnImageSelectionChanged;
        public static event Action? RGBToGrayConvertedEvent;

        public static AndroidTabbedPage? AndroidImageView;
        public static List<WindowImageObject> OpenedImagesList = new();
        public static int? selectedWindow = null;
        public const int windowHeightFix = 100;
        public const int windowWidthFix = 26;

        public static async void SaveImage(int index, bool saveAs)
        {
            switch (saveAs)
            {
                case false:
                    if (OpenedImagesList[index].CollectivePage.ImagePage.path == String.Empty) throw new InvalidOperationException(@"Image has no path. Save this image via ""Save as""");
                    if (OpenedImagesList[index].CollectivePage.ImagePage.ColorImage != null && OpenedImagesList[index].CollectivePage.ImagePage.Type == ImgType.RGB)
                    {
                        OpenedImagesList[index].CollectivePage.ImagePage.ColorImage.Save(OpenedImagesList[index].CollectivePage.ImagePage.path);
                    }
                    if (OpenedImagesList[index].CollectivePage.ImagePage.GrayImage != null && OpenedImagesList[index].CollectivePage.ImagePage.Type == ImgType.Gray)
                    {
                        OpenedImagesList[index].CollectivePage.ImagePage.GrayImage.Save(OpenedImagesList[index].CollectivePage.ImagePage.path);
                    }
                    break;
                case true:
                    string filename = await Application.Current.MainPage.DisplayPromptAsync("File Name", "Type File Name + extension(.bmp/.jpg etc.)");
                    if (filename == null || filename == String.Empty)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "Invalid filename/extension", "Cancel");
                        return;
                    }
                    var fp = await FolderPicker.Default.PickAsync();
                    string path;
                    if (fp.IsSuccessful)
                    {
                        path = fp.Folder.Path;
                        string fullPath = String.Empty;
#if WINDOWS
                        fullPath = path + "\\" + filename;
#endif
#if ANDROID
                        fullPath = path + "/" + filename;
#endif

                        if (OpenedImagesList[index].CollectivePage.ImagePage.ColorImage != null && OpenedImagesList[index].CollectivePage.ImagePage.Type == ImgType.RGB)
                        {
                            OpenedImagesList[index].CollectivePage.ImagePage.ColorImage.Save(fullPath);
                        }
                        if (OpenedImagesList[index].CollectivePage.ImagePage.GrayImage != null && OpenedImagesList[index].CollectivePage.ImagePage.Type == ImgType.Gray)
                        {
                            OpenedImagesList[index].CollectivePage.ImagePage.GrayImage.Save(fullPath);
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "Path not valid", "Cancel");
                        return;
                    }
                    break;
            }

        }
        public static async void CreateImagePage()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.Android)
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "image/*", "image/bmp" } },
                    { DevicePlatform.WinUI, new[] { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".ico", ".svg", ".webp" } }, // Windows
                    
                });
                FileResult? result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = "Choose File",
                    
                });
                
                if (result == null) return;
                string FileFullPath = result.FullPath.ToString();
                var img = ImageLoader.LoadImage(FileFullPath); // Important!! Type Image<Bgr, Byte> or Image<Gray, Byte> depends on file type detected in FileLoader
                string fileName = String.Empty;
#if WINDOWS
                fileName = FileFullPath.Substring(FileFullPath.LastIndexOf('\\') + 1);
#endif
#if ANDROID
                fileName = FileFullPath.Substring(FileFullPath.LastIndexOf('/') + 1);
#endif
                int duplicates = 0;
                foreach (WindowImageObject wio in OpenedImagesList) //Sketchy
                {
                    string s = wio.CollectivePage.Title;
                    int dotIndex = s.LastIndexOf('.');
                    if (dotIndex == -1) break;
                    string output = s.Substring(0, dotIndex);
                    if (output == fileName) duplicates++;
                }
                fileName += $".{duplicates}";
                OpenNewWindow(img, fileName, FileFullPath);
                

            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
            }
        }
        public static void OpenNewWindow(dynamic img, string title, string path) //Argument is dynamic cuz' dont want to make overload for GrayScale/Color.
        {
            ImagePage page = new ImagePage(img, OpenedImagesList.Count, title);
            page.path = path;
            page.Content.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    WindowFileManager.ChangeSelectedImagePage(page.index);
                })
            });
            CollectivePage collectivePage = new CollectivePage(page, title); //new
#if WINDOWS
            Window newWindow = new Window
            {
                Page = collectivePage,
                Title = title,
                Width = 650,
                Height = 480,
                MinimumWidth = 0,
                MinimumHeight = 0,
            };
 
            WindowImageObject windowImageObject = new(collectivePage, newWindow);
            OpenedImagesList.Add(windowImageObject);
            ChangeSelectedImagePage(OpenedImagesList.Count - 1);
            Application.Current?.OpenWindow(newWindow);
            //OnImageClosingOpeningEvent?.Invoke();
            OnImageOpeningEvent?.Invoke();
#endif
#if ANDROID
            WindowImageObject windowImageObject = new(collectivePage);
            AndroidImageView?.AddPage(collectivePage);
            OpenedImagesList.Add(windowImageObject);
            ChangeSelectedImagePage(OpenedImagesList.Count - 1);
            //OnImageClosingOpeningEvent?.Invoke();
            OnImageOpeningEvent?.Invoke();
#endif
        }
        public static void OnCloseImagePage(int index) 
        {
            System.Diagnostics.Debug.WriteLine($"Closing Collective View: {index}");
            selectedWindow = null;
            AndroidImageView?.RemovePage(OpenedImagesList[index].CollectivePage);
            for (int i = index; i < OpenedImagesList.Count; i++)
            {
                OpenedImagesList[i].CollectivePage.ImagePage.index--;
            }
            OpenedImagesList[index].Dispose();
            OpenedImagesList.RemoveAt(index);
            GC.Collect();
            //OnImageClosingOpeningEvent?.Invoke();
            OnImageClosingEvent?.Invoke();
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
            RGBToGrayConvertedEvent?.Invoke();
        }
    }
}
