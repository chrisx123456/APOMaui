namespace APOMaui
{
    internal class WindowImageObject : IDisposable
    {
        public ImagePage ImagePage;
        public Window ImagePageWindow;

        public HistogramChart? HistogramChart;
        public Window? HistogramChartWindow;

        public WindowImageObject(ImagePage img, Window window)
        {
            this.ImagePage = img;
            this.ImagePageWindow = window;
        }
        public void Dispose()
        {
            ImagePage = null;
            ImagePageWindow = null;
            HistogramChart = null;
            GC.Collect();
        }


    }
}
