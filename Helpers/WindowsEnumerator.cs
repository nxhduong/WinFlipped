using SharpDX;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using WindowsDesktop;

namespace WinFlipped.Helpers
{
    internal partial class WindowsEnumerator
    {
        private delegate bool EnumWindowsCallback(nint hWnd, int lParam);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumWindows(EnumWindowsCallback enumFunc, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [SuppressMessage("Interoperability", 
            "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", 
            Justification = "LibraryImport won't work with this method and will raise error"
        )]
        private static extern int GetWindowTextLength(nint hWnd);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsWindowVisible(nint hWnd);

        [LibraryImport("user32.dll")]
        private static partial nint GetShellWindow();

        [LibraryImport("dwmapi.dll")]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Currently not needed")]
        private static partial nint DwmGetWindowAttribute(nint hWnd, nint dwAttribute, out nint pvAttribute, nint cbAttribute);

        [LibraryImport("user32.dll")]
        public static partial nint ShowWindow(nint hWnd, int nCmdShow);

        [LibraryImport("user32.dll")]
        private static partial nint SwitchToThisWindow(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool fUnknown);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetForegroundWindow(nint hWnd);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cX, int cY, uint uFlags);

        [LibraryImport("user32.dll")]
        private static partial nint BringWindowToTop(nint hWnd);

        [LibraryImport("user32.dll")]
        private static partial nint IsIconic(nint hWnd);

        /// <summary>
        /// Returns an IEnumerable that contains the handle, title and screenshots of all the open windows.
        /// </summary>
        public static IEnumerable<(nint MainWindowHandle, string MainWindowTitle, Bitmap screenshot)> GetOpenWindows()
        {
            nint shellWindow = GetShellWindow();
            List<(nint, string, Bitmap)> windows = [];

            EnumWindows(delegate (nint hWnd, int lParam)
            {
                int titleLength = GetWindowTextLength(hWnd);
                if (hWnd == shellWindow || !IsWindowVisible(hWnd) || titleLength == 0 || VirtualDesktop.FromHwnd(hWnd) is null)
                {
                    return true;
                }

                StringBuilder builder = new(titleLength);
                _ = GetWindowText(hWnd, builder, titleLength + 1);

                // Redundancy to ensure that the window is shown
                ShowWindow(hWnd, IsIconic(hWnd) != 0 ? 9 : 5);
                SetForegroundWindow(hWnd);
                SetWindowPos(hWnd, -1, 0, 0, 0, 0, 0x0040 | 0x0001 | 0x0002);
                BringWindowToTop(hWnd);
                SwitchToThisWindow(hWnd, true);

                Thread.Sleep(800);

                windows.Add((hWnd, builder.ToString(), ScreenGrabber.CaptureWindow(hWnd)));

                // Minimize window
                ShowWindow(hWnd, 2);

                return true;

            }, 0);

            return windows.AsEnumerable();
        }

        // https://stackoverflow.com/questions/32149880/how-to-identify-windows-10-background-store-processes-that-have-non-displayed-wi
        /* DWMWA_CLOAKED?
        public static bool IsInvisibleWin10BackgroundAppWindow(nint hWnd)
        {
            int CloakedVal = 0;
            nint hRes = DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED (0x1 | 0x2 | 0x4)?, out CloakedVal, Marshal.SizeOf(CloakedVal));

            if (hRes != 0)
            {
                return false;
            }
            return CloakedVal != 0;
        }
        */
    }
}