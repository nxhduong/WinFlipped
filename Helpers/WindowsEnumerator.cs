using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace WinFlipped.Helpers
{
    public partial class WindowsEnumerator
    {
        private delegate bool EnumWindowsProc(nint hWnd, int lParam);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

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
                if (hWnd == shellWindow || !IsWindowVisible(hWnd) || titleLength == 0)
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
    }
}
