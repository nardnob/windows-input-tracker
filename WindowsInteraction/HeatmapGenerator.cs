using System.Drawing.Imaging;
using System.Drawing;

namespace nardnob.InputTracker.WindowsInteraction
{
    public static class HeatmapGenerator
    {
        #region " Public Methods "

        public static Bitmap GenerateHeatmapBitmap(Dictionary<Point, int> clickedPoints)
        {
            var maxX = clickedPoints.Keys.Max(key => key.X);
            var maxY = clickedPoints.Keys.Max(key => key.Y);
            const int ellipseWidth = 10;

            var bitmap = new Bitmap(maxX + ellipseWidth + 1, maxY + ellipseWidth + 1);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                foreach (var point in clickedPoints.Keys)
                {
                    var rectangle = new Rectangle(point.X, point.Y, ellipseWidth, ellipseWidth);

                    if (clickedPoints.ContainsKey(point))
                    {
                        var clickCount = clickedPoints[point];
                        graphics.FillEllipse(GetBrushColorFromClickCount(clickCount), rectangle);
                    }
                }
            }

            return bitmap;
        }

        public static void SaveBitmapImage(Bitmap bitmap, string fileName)
        {
            bitmap.Save(fileName, ImageFormat.Bmp);
        }

        #endregion

        #region " Private Methods "

        private static System.Drawing.Brush GetBrushColorFromClickCount(int clickCount)
        {
            if (clickCount >= 128)
            {
                return System.Drawing.Brushes.OrangeRed;
            }

            if (clickCount >= 100)
            {
                return System.Drawing.Brushes.Red;
            }

            if (clickCount >= 86)
            {
                return System.Drawing.Brushes.Firebrick;
            }

            if (clickCount >= 64)
            {
                return System.Drawing.Brushes.DeepPink;
            }

            if (clickCount >= 48)
            {
                return System.Drawing.Brushes.Fuchsia;
            }

            if (clickCount >= 32)
            {
                return System.Drawing.Brushes.DarkViolet;
            }

            if (clickCount >= 16)
            {
                return System.Drawing.Brushes.DarkMagenta;
            }

            if (clickCount >= 8)
            {
                return System.Drawing.Brushes.DarkSlateBlue;
            }

            if (clickCount >= 4)
            {
                return System.Drawing.Brushes.Blue;
            }

            if (clickCount >= 2)
            {
                return System.Drawing.Brushes.CornflowerBlue;
            }

            if (clickCount >= 1)
            {
                return System.Drawing.Brushes.SkyBlue;
            }

            return System.Drawing.Brushes.White;
        }

        #endregion
    }
}
