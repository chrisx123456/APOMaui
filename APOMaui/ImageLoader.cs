using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace APOMaui
{
    internal static class ImageLoader
    {
        public static dynamic LoadImage(string path)
        {
            Mat matImage = CvInvoke.Imread(path, ImreadModes.AnyColor);
            if (matImage.NumberOfChannels == 1) // Sprawdź, czy obraz jest w odcieniach szarości
            {
                matImage = CvInvoke.Imread(path, ImreadModes.Grayscale); // Jeśli tak, wczytaj ponownie jako obraz w odcieniach szarości
                return new Image<Gray, byte>(matImage);
            }
            else // Jeśli nie, obraz jest kolorowy
            {
                return new Image<Bgr, byte>(matImage);
            }
        }
    }
}
