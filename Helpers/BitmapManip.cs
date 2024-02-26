using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;

namespace WinFlipped.Helpers
{
    internal static class BitmapManip
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
    }
}