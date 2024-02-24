using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;


namespace Bookflipper.Helpers
{
    internal static class BitmapManipulator
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using MemoryStream memory = new();
            bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }

        // Skew image so it looks 3D
        public static Bitmap SkewBitmap(Bitmap bitmap) {
            var skewedBitmap = new Bitmap(bitmap.Width, 2 * bitmap.Height, PixelFormat.Format32bppArgb);
            var space = bitmap.Height;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    skewedBitmap.SetPixel(x, y + space, pixelColor);
                }
                if (space <= 0)
                {
                    return skewedBitmap;
                } else
                {
                    space--;
                }
            }
            return skewedBitmap;
        }
    }
}
