namespace APOMaui
{
    internal class WindowImageObject : IDisposable
    {
        public CollectivePage CollectivePage;
        public Window CollectivePageWindow;
        public WindowImageObject(CollectivePage cv, Window window)
        {
            this.CollectivePage = cv;
            this.CollectivePageWindow = window;
        }

        //public ImagePage ImagePage;
        //public Window ImagePageWindow;

        //public HistogramChart? HistogramChart;
        //public Window? HistogramChartWindow;

        //public WindowImageObject(ImagePage img, Window window)
        //{
        //    this.ImagePage = img;
        //    this.ImagePageWindow = window;
        //}
        public void Dispose()
        {
            this.CollectivePage.Dispose();
            this.CollectivePageWindow.ClearLogicalChildren();
            Application.Current?.CloseWindow(CollectivePageWindow);
        }


    }
}
