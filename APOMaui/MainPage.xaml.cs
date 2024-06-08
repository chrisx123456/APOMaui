﻿namespace APOMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Emgu.CV.Platform.Maui.MauiInvoke.Init();
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
            WindowFileManager.CreateImagePage();
            //Main.OpenPhotoInNewWindow();
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
        private async void OnRLEButtonClicked(object sender, EventArgs e)
        {
            if (WindowFileManager.selectedWindow == null)
            {
                await DisplayAlert("Alert", "None image is selected!", "Ok");
                return;
            }
            int index = (int)WindowFileManager.selectedWindow;
            ImageProc.CompressRLE(index);
        }
    }

}
