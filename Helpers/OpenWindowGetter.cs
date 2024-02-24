using System.Runtime.InteropServices;
using System.Text;

namespace Bookflipper
{
    /// <summary>Contains functionality to get all the open windows.</summary>
    internal static partial class OpenWindowGetter
    {
        private delegate bool EnumWindowsProc(nint hWnd, int lParam);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(nint hWnd);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsWindowVisible(nint hWnd);

        [LibraryImport("user32.dll")]
        private static partial IntPtr GetShellWindow();

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        /// 
        public static IDictionary<nint, string> GetOpenWindows()
        {
            nint shellWindow = GetShellWindow();
            Dictionary<nint, string> windows = [];

            EnumWindows(delegate (nint hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new(length);
                _ = GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }
    }
}