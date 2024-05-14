using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;

namespace Test
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        SKPoint? previousPoint;

        private SKCanvasView CreateSliderControl()
        {
            var control = new SKCanvasView();
            control.HeightRequest = 100;
            control.WidthRequest = 100;
            control.PaintSurface += SKCanvasView_PaintSurface;
            control.EnableTouchEvents = true;
            control.Touch += (sender, args) =>
            {
                var pt = args.Location;
                Debug.WriteLine("sdsdsd");

                switch (args.ActionType)
                {
                    case SKTouchAction.Pressed:
                        control.InvalidateSurface();
                        Debug.WriteLine("sdsdsd");

                        break;
                    case SKTouchAction.Moved:
                        control.InvalidateSurface();
                        Debug.WriteLine("sdsdsd");

                        break;
                }
                // Let the OS know that we want to receive more touch events
                args.Handled = true;
            };
            control.InputTransparent = false;

            return control;
        }



        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
         
        }

        private void SKCanvasView_Touch(object sender, SkiaSharp.Views.Maui.SKTouchEventArgs e)
        {
            e.Handled = true;
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    Debug.WriteLine("Pressed");
                    previousPoint = null;
                    previousPoint = e.Location; // Ustaw bieżący punkt na początkowy
                    e.Handled = true;
                    break;
                case SKTouchAction.Moved:
                    e.Handled = true;
                    if (previousPoint.HasValue)
                    {
                        DrawLine(previousPoint.Value, e.Location); // Narysuj linię między poprzednim punktem a bieżącym punktem
                        previousPoint = e.Location; // Ustaw bieżący punkt na poprzedni
                        Debug.WriteLine("Moved");
                        drawBox.InvalidateSurface();
                    }
                    break;
                case SKTouchAction.Cancelled:
                    Debug.WriteLine("Cancelled");
                    return;
                case SKTouchAction.Exited:
                    Debug.WriteLine("Exited");
                    break;
                case SKTouchAction.Entered:
                    Debug.WriteLine("Entered");
                    break;
                case SKTouchAction.Released:
                    Debug.WriteLine("Rel");
                    e.Handled = true;
                    drawBox.InvalidateSurface();
                    previousPoint = null; // Zresetuj poprzedni punkt po zwolnieniu dotyku
                    break;
            }
            e.Handled = true;
        }
        private void DrawLine(SKPoint startPoint, SKPoint endPoint)
        {
            // Narysuj linię między dwoma punktami na płótnie SKCanvasView
            drawBox.PaintSurface += (s, args) =>
            {
                using (var paint = new SKPaint())
                {
                    paint.Color = SKColors.Black;
                    paint.Style = SKPaintStyle.Fill;
                    paint.StrokeWidth = 10;

                    args.Surface.Canvas.DrawLine(startPoint, endPoint, paint);
                }
            };
            
        }
    }

}
