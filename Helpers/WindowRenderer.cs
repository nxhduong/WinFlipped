using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Bitmap = System.Drawing.Bitmap;

namespace WinFlipped.Helpers
{
    internal static class WindowRenderer
    {
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

            Image image = new()
            {
                Height = 100,
                Width = 200,
                Name = "_" + windowHandle.ToString(),
                Source = windowScreenshot.ToBitmapImage(),
                Opacity = hidden ? 0 : 1
            };

            Image icon = new()
            {
                Height = 50,
                Width = 50,
                Name = "_" + windowHandle.ToString(),
                Opacity = hidden ? 0 : 1
            };

            try
            {
                icon.Source = Imaging.CreateBitmapSourceFromHIcon(
                    System.Drawing.Icon.ExtractAssociatedIcon(
                        Process.GetProcessById(
                            (int)windowHandle).MainModule?.FileName ?? ""
                        )?.Handle ?? 0,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
            catch (Win32Exception ex)
            {
                // When getting process results in 'Access is denied' error
                Trace.TraceError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // When casting fails
                Trace.TraceError(ex.Message);
            }

            canvas.Children.Add(image);
            Canvas.SetTop(image, top);
            Canvas.SetLeft(image, left);

            canvas.Children.Add(icon);
            Canvas.SetTop(icon, top - 10);
            Canvas.SetLeft(icon, left);

            canvas.Children.Add(title);
            Canvas.SetTop(title, top - 25);
            Canvas.SetLeft(title, left);

            return (title, image, icon);
        }
    }
}
