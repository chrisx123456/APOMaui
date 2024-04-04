namespace APOMaui
{
    internal class WindowImageObject : IDisposable
    {
        private bool disposed = false;
        public WinIMG winImg;
        public Window window;
        public Chart? chart;
        public WindowImageObject(WinIMG img, Window window)
        {
            this.winImg = img;
            this.window = window;
        }
        public void Dispose()
        {
            winImg = null;
            window = null;
            chart = null;
            GC.Collect();
        }


    }
}
