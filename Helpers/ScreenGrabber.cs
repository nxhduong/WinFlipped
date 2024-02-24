using System.Drawing;
using System.Runtime.InteropServices;
using static System.Windows.SystemParameters;

namespace Bookflipper.Helpers
{
    internal partial class ScreenGrabber
    {
        /*[StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }*/

        [LibraryImport("user32.dll")]
        private static partial nint GetForegroundWindow();
        [LibraryImport("user32.dll")]
        public static partial nint GetDesktopWindow();
        [LibraryImport("user32.dll")]
        private static partial nint GetWindowRect(nint hWnd, ref RECT rect);

        public static Bitmap CaptureScreen()
        {
            int height = (int)FullPrimaryScreenHeight;
            int width = (int)FullPrimaryScreenWidth;
            Bitmap bmp = new(width, height);
            Graphics.FromImage(bmp).CopyFromScreen(Point.Empty, Point.Empty, new Size
            {
                Width = width,
                Height = height
            });
            return bmp;
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        public static Bitmap CaptureWindow(nint handle)
        {
            var rect = new RECT();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            return result;
        }
    }
}
