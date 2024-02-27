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

        public static Bitmap MergeBitmapSideBySide(this Bitmap originBmp, Bitmap addedBmp)
        {
            originBmp = new Bitmap(originBmp, 300, 200);
            addedBmp = new Bitmap(addedBmp, 50, 50);
            Bitmap mergedBmp = new(originBmp.Width + addedBmp.Width, Math.Max(originBmp.Height, addedBmp.Height), PixelFormat.Format32bppArgb);

            for (int x = 0; x < addedBmp.Width; x++)
            {
                for (int y = 0; y < addedBmp.Height; y++)
                {
                    mergedBmp.SetPixel(x, y, addedBmp.GetPixel(x, y));
                }
            }

            for (int x = addedBmp.Width; x < mergedBmp.Width; x++)
            {
                for (int y = 0; y < originBmp.Height; y++)
                {
                    mergedBmp.SetPixel(x, y, originBmp.GetPixel(x - addedBmp.Width, y));
                }
            }

            mergedBmp.MakeTransparent();
            return mergedBmp;
        }
    }
}