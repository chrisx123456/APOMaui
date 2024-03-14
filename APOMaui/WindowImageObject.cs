namespace APOMaui
{
    internal class WindowImageObject
    {
        public WinIMG winImg;
        public Window window;
        public Chart? chart;
        public WindowImageObject(WinIMG img, Window window)
        {
            this.winImg = img;
            this.window = window;
        }
    }
}
