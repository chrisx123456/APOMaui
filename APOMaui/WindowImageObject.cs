namespace APOMaui
{
    internal class WindowImageObject : IDisposable
    {
        public CollectivePage CollectivePage { get; set;  }
        public Window? CollectivePageWindow;
        public WindowImageObject(CollectivePage cv, Window window)
        {
            this.CollectivePage = cv;
            this.CollectivePageWindow = window;
        }
        public WindowImageObject(CollectivePage cv)
        {
            this.CollectivePage = cv;
        }
        public void Dispose()
        {
            this.CollectivePage.Dispose();
            if(this.CollectivePageWindow != null)
            {
                this.CollectivePageWindow.ClearLogicalChildren();
                Application.Current?.CloseWindow(CollectivePageWindow);
            }
        }


    }
}
