using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace WinFlipped
{
    public partial class MainWindow : Window
    {
        private IEnumerable<(nint MainWindowHandle, string MainWindowTitle)>? OpenWindows; // SortedDictionary?
        // Rough approximation of maximum number of windows that this program can show
        private readonly int WINDOWS_SHOW_LIMIT = (int)Math.Min(
                SystemParameters.FullPrimaryScreenHeight / 100,
                SystemParameters.FullPrimaryScreenWidth / 200
                );

        [LibraryImport("user32.dll")]
        private static partial nint ShowWindow(nint hWnd, int nCmdShow);

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

        public MainWindow()
        {
            InitializeComponent();
            KeyUp += new KeyEventHandler(MainWindow_KeyUp);
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }
    }
}