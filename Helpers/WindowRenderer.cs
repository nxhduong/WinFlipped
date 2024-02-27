using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.InteropServices;
using Bitmap = System.Drawing.Bitmap;

namespace WinFlipped.Helpers
{
    internal static partial class WindowRenderer
    {
        [LibraryImport("user32.dll")]
        private static partial uint GetWindowThreadProcessId(nint hWnd, out uint processId);

        /// <summary>
        /// Add a window, its title, and its icon to the canvas
        /// </summary>
        /// <returns>
        /// A tuple containing 3 WPF elements: Label (title) and 2 x Image (screenshot, icon)
        /// </returns>
        public static (Label, Image, Image) DrawWindow(
            this Canvas canvas, nint windowHandle, 
            string windowTitle, 
            Bitmap windowScreenshot, 
            int top, 
            int left, 
            double scale,
            int zIndex,
            bool hidden = false
        )
        {
            Label title = new()
            {
                // Window handles are for debugging purposes
                Content = windowTitle + "|Handle " + windowHandle,
                Name = '_' + windowHandle.ToString(),
                Foreground = new BrushConverter().ConvertFromString("#FFFFFF") as Brush,
                Opacity = hidden? 0 : 1
            };

            GetWindowThreadProcessId(windowHandle, out uint pId);
            Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(
                    Process.GetProcessById((int)pId).MainModule?.FileName ?? ""
            )?.ToBitmap()?? new (0, 0);

            Image image = new()
            {
                Height = 100 * scale,
                Width = 150 * scale,
                Name = "_" + windowHandle.ToString(),
                Source = windowScreenshot.MergeBitmapSideBySide(icon).ToBitmapImage(),
                Opacity = hidden ? 0 : 1
            };

            canvas.Children.Add(image);
            Canvas.SetTop(image, top);
            Canvas.SetLeft(image, left);
            Canvas.SetZIndex(image, zIndex);

            canvas.Children.Add(title);
            Canvas.SetTop(title, top - 10);
            Canvas.SetLeft(title, left);
            Canvas.SetZIndex(title, zIndex);

            return (title, image, image);
        }
    }
}
