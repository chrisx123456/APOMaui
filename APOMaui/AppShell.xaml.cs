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
            Application.Current?.CloseWindow(WindowFileManager.OpenedImagesList[index].CollectivePageWindow);
        }
        private void OnCloseClicked(object sender, EventArgs e)
        {
            if (WindowFileManager.selectedWindow == null) return;
            int index = (int)WindowFileManager.selectedWindow;
            WindowFileManager.OpenedImagesList[index].CollectivePage.RemoveCurrPage();
        }

    }
}
