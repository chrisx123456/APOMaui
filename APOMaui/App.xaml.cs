namespace APOMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }


#if WINDOWS
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Destroying += Window_Destroying;

            const int newWidth = 450;
            const int newHeight = 600;

            window.Width = newWidth;
            window.Height = newHeight;

            //window.MaximumWidth = newWidth;
            //window.MaximumHeight = newHeight;

            //window.MinimumHeight = newHeight;
            //window.MinimumWidth = newWidth;
            return window;
        }

        private void Window_Destroying(object? sender, EventArgs e)
        {

        }
#endif
    }
}
