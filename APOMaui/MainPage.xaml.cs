namespace APOMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
        }
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {

        }
        private void OnFileButtonClicked(object sender, EventArgs e)
        {
            if (FileTab.IsVisible)
            {
                FileTab.IsVisible = false;
            } else FileTab.IsVisible = true;
            //Application.Current.OpenWindow(new Window(new MainPage()));
        }
        private void OnCloseButtonClicked(object sender, EventArgs e)
        {

        }

        private void OnOpenButtonClicked(object sender, EventArgs e)
        {
            Main.OpenPhotoInNewWindowEMGU();
            //Main.OpenPhotoInNewWindow();
        }



    }

}
