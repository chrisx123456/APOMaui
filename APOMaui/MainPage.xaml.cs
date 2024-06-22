namespace APOMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Emgu.CV.Platform.Maui.MauiInvoke.Init();
#if ANDROID
            WindowFileManager.AndroidImageView = new AndroidTabbedPage();
            Window window = new Window(WindowFileManager.AndroidImageView);
            Application.Current?.OpenWindow(window);
#endif
        }

        private async void OnOpenButtonClicked(object sender, EventArgs e)
        {
#if ANDROID
            if(WindowFileManager.OpenedImagesList.Count >= 5)
            {
                await DisplayAlert("Alert", "Cannot open more than 5 photos", "Ok");
                return;
            }
#endif
            WindowFileManager.CreateImagePage();
        }
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (WindowFileManager.selectedWindow == null)
            {
                await DisplayAlert("Alert", "None image is selected!", "Ok");
                return;
            }
            int index = (int)WindowFileManager.selectedWindow;
            try
            {
                WindowFileManager.SaveImage(index, false);
            }
            catch(InvalidOperationException ex)
            {
                await DisplayAlert("Alert", ex.Message, "Ok");
            }
        }
        private async void OnSaveAsButtonClicked(object sender, EventArgs e)
        {
            if (WindowFileManager.selectedWindow == null)
            {
                await DisplayAlert("Alert", "None image is selected!", "Ok");
                return;
            }
            int index = (int)WindowFileManager.selectedWindow;
            WindowFileManager.SaveImage(index, true);
        }
        private async void OnAboutButtonClicked(object sender, EventArgs e)
        {
            string msg = "APO Projekt \n\nAutor: Maciej Lacek";
            await DisplayAlert("About", msg, "Ok");
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
#if ANDROID
            if(!await CheckNeededPermissions())
            {
                await DisplayAlert("Permissions alert", "Not all permissions granted", "Quit");
                Application.Current?.Quit();
            }
#endif
        }
#if ANDROID
        private async Task<bool> CheckNeededPermissions()
        {
            PermissionStatus camera = await CheckPermission<Permissions.Camera>();
            PermissionStatus read = await CheckPermission<Permissions.StorageRead>();
            PermissionStatus write = await CheckPermission<Permissions.StorageWrite>();
            return IsGranted(camera) && IsGranted(read) && IsGranted(write);
        }
        private async Task<PermissionStatus> CheckPermission<TPermission>() where TPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            PermissionStatus status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<TPermission>();
            if (status != PermissionStatus.Granted)
            {
                status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<TPermission>();
            }
            return status;
        }
        private static bool IsGranted(PermissionStatus status)
        {
            return status == PermissionStatus.Granted || status == PermissionStatus.Limited;
        }
#endif
    }

}
