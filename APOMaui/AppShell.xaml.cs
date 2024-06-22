using System.Diagnostics;
using System.Windows.Input;

namespace APOMaui
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();

        }

        private void OnCloseAllClicked(object sender, EventArgs e)
        {
            if(WindowFileManager.selectedWindow ==  null) return;
            int index = (int)WindowFileManager.selectedWindow;
            if(WindowFileManager.OpenedImagesList[index].CollectivePageWindow != null) Application.Current?.CloseWindow(WindowFileManager.OpenedImagesList[index].CollectivePageWindow);
#if ANDROID
            WindowFileManager.OnCloseImagePage(index);
#endif

        }
        private void OnCloseClicked(object sender, EventArgs e)
        {
            if (WindowFileManager.selectedWindow == null) return;
            int index = (int)WindowFileManager.selectedWindow;
            WindowFileManager.OpenedImagesList[index].CollectivePage.RemoveCurrPage();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var files = Directory.GetFiles(FileSystem.CacheDirectory);
            foreach (var file in files) File.Delete(file);
        }

    }
}
