using Windows.Graphics.Capture;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using static System.Windows.SystemParameters;

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

        [LibraryImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool BitBlt(nint hDcDest, int xDest, int yDest, int wDest, int hDest, nint hDcSource, int xSrc, int ySrc, CopyPixelOperation rop);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ReleaseDC(nint hWnd, nint hDc);

        [LibraryImport("gdi32.dll")]
        private static partial nint DeleteDC(nint hDc);

        [LibraryImport("gdi32.dll")]
        private static partial nint DeleteObject(nint hDc);

        [LibraryImport("gdi32.dll")]
        private static partial nint CreateCompatibleBitmap(nint hDc, int nWidth, int nHeight);

        [LibraryImport("gdi32.dll")]
        private static partial nint CreateCompatibleDC(nint hDc);

        [LibraryImport("gdi32.dll")]
        private static partial nint SelectObject(nint hDc, nint bmp);

        [LibraryImport("user32.dll")]
        private static partial nint GetWindowDC(nint ptr);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool PrintWindow(nint hWnd, nint hDcBlt, int nFlags);

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

        public static int[] GetWindowSize(nint handle)
        {
            var rect = new RECT();

            GetWindowRect(handle, ref rect);

            return [rect.Right - rect.Left, rect.Bottom - rect.Top];
        }

        public static Bitmap BitBltCaptureScreen()
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

        public static Bitmap PrintWindow(nint hWnd)
        {
            RECT rc = new();
            GetWindowRect(hWnd, ref rc);

            Bitmap bmp = new(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            nint hDcBitmap = gfxBmp.GetHdc();

            PrintWindow(hWnd, hDcBitmap, 2);

            gfxBmp.ReleaseHdc(hDcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
    }
}