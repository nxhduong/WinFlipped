using SharpDX;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using WindowsDesktop;

namespace WinFlipped.Helpers
{
    public partial class WindowsEnumerator
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
        [Obsolete]
        private static partial nint DwmGetWindowAttribute(nint hWnd, nint dwAttribute, out nint pvAttribute, nint cbAttribute);

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        /// 
        public static IEnumerable<(nint MainWindowHandle, string MainWindowTitle)> GetOpenWindows()
        {
            nint shellWindow = GetShellWindow();
            List<(nint, string)> windows = [];

            EnumWindows(delegate (nint hWnd, int lParam)
            {
                int titleLength = GetWindowTextLength(hWnd);
                if (hWnd == shellWindow || !IsWindowVisible(hWnd) || titleLength == 0 || VirtualDesktop.FromHwnd(hWnd) is null)
                {
                    return true;
                }

                StringBuilder builder = new(titleLength);
                _ = GetWindowText(hWnd, builder, titleLength + 1);

                windows.Add((hWnd, builder.ToString()));
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