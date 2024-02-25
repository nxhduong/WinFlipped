using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Graphics.Capture;
using static System.Windows.SystemParameters;
using System.Drawing.Imaging;

namespace WinFlipped.Helpers
{
    internal partial class ScreenGrabber
    {
        [LibraryImport("user32.dll")]
        public static partial nint GetForegroundWindow();
        [LibraryImport("user32.dll")]
        private static partial nint GetDesktopWindow();
        [LibraryImport("user32.dll")]
        private static partial nint GetWindowRect(nint hWnd, ref RECT rect);
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(nint hdcDest, int xDest, int yDest, int wDest, int hDest, nint hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(nint hWnd, nint hDc);
        [LibraryImport("gdi32.dll")]
        private static partial nint DeleteDC(nint hDc);
        [LibraryImport("gdi32.dll")]
        private static partial nint DeleteObject(nint hDc);
        [DllImport("gdi32.dll")]
        static extern nint CreateCompatibleBitmap(nint hdc, int nWidth, int nHeight);
        [LibraryImport("gdi32.dll")]
        private static partial nint CreateCompatibleDC(nint hdc);
        [LibraryImport("gdi32.dll")]
        private static partial nint SelectObject(nint hdc, nint bmp);
        [LibraryImport("user32.dll")]
        private static partial nint GetWindowDC(nint ptr);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(nint hWnd, nint hdcBlt, int nFlags);

        public static Bitmap CaptureScreen()
        {
            int height = (int)FullPrimaryScreenHeight;
            int width = (int)FullPrimaryScreenWidth;
            Bitmap bmp = new(width, height);
            Graphics.FromImage(bmp).CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, new System.Drawing.Size
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
                graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
            }
            return result;
        }

        public static int[] GetWindowSize(nint handle)
        {
            var rect = new RECT();
            GetWindowRect(handle, ref rect);
            return [rect.Right - rect.Left, rect.Bottom - rect.Top];
        }

        public static Bitmap BitBltCaptureWindow(nint hwnd)
        {
            Size sz = new((int)PrimaryScreenWidth, (int)PrimaryScreenHeight);
            nint hDesk = GetDesktopWindow();
            nint hSrce = GetWindowDC(hDesk);
            nint hDest = CreateCompatibleDC(hSrce);
            nint hBmp = CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
            nint hOldBmp = SelectObject(hDest, hBmp);
            _ = BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            Bitmap bmp = Image.FromHbitmap(hBmp);
            SelectObject(hDest, hOldBmp);
            DeleteObject(hBmp);
            DeleteDC(hDest);
            ReleaseDC(hDesk, hSrce);
            return bmp;
        }

        public static Bitmap PrintWindow(nint hwnd)
        {
            RECT rc = new();
            GetWindowRect(hwnd, ref rc);

            Bitmap bmp = new(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            nint hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hwnd, hdcBitmap, 2);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
    }
}