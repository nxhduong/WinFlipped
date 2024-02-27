using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;

namespace WinFlipped.Helpers
{
    internal static class BitmapManip
    {
        /// <summary>Convert Bitmap to ImageSource</summary>
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

        /// <summary>
        /// Combine two bitmaps into one.
        /// </summary>
        /// <param name="baseBmp">The bitmap that you want to add another bitmap to</param>
        /// <param name="addedBmp">The bitmap to be added</param>
        /// <param name="baseWidth">New width of baseBmp, if desired</param>
        /// <param name="baseHeight">New height of baseBmp, if desired</param>
        /// <param name="addedWidth">New width of addedBmp, if desired</param>
        /// <param name="addedHeight">New height of addedBmp, if desired</param>
        public static Bitmap MergeBitmapSideBySide(this Bitmap baseBmp, Bitmap addedBmp, int? baseWidth = null, int? baseHeight = null, int? addedWidth = null, int? addedHeight = null)
        {
            if (baseWidth is not null && baseHeight is not null) {
                baseBmp = new Bitmap(baseBmp, baseWidth, baseHeight);
            }
            if (addedWidth is not null && addedHeight is not null) {
                addedBmp = new Bitmap(addedBmp, addedWidth, addedHeight);
            }
            Bitmap mergedBmp = new(baseBmp.Width + addedBmp.Width, Math.Max(baseBmp.Height, addedBmp.Height), PixelFormat.Format32bppArgb);

            for (int x = 0; x < addedBmp.Width; x++)
            {
                for (int y = 0; y < addedBmp.Height; y++)
                {
                    mergedBmp.SetPixel(x, y, addedBmp.GetPixel(x, y));
                }
            }

            for (int x = addedBmp.Width; x < mergedBmp.Width; x++)
            {
                for (int y = 0; y < baseBmp.Height; y++)
                {
                    mergedBmp.SetPixel(x, y, baseBmp.GetPixel(x - addedBmp.Width, y));
                }
            }

            mergedBmp.MakeTransparent();
            return mergedBmp;
        }
    }
}