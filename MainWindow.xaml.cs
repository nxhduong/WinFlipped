using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using WinFlipped.Helpers;

namespace WinFlipped
{
    public partial class MainWindow : Window
    {
        
        private IEnumerable<Process>? OpenWindows; // SortedDictionary ?
        private int SelectedWindowIndex = 0;

        [LibraryImport("user32.dll")]
        private static partial nint ShowWindow(nint hWnd, int nCmdShow);
        [LibraryImport("user32.dll")]
        private static partial nint SwitchToThisWindow(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool fUnknown);
        [LibraryImport("user32.dll")]
        private static partial nint SetForegroundWindow(nint hWnd);
        [LibraryImport("user32.dll")]
        private static partial nint SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cX, int cY, uint uFlags);
        [LibraryImport("user32.dll")]
        private static partial nint BringWindowToTop(nint hWnd);
        [LibraryImport("user32.dll")]
        private static partial nint IsIconic(nint hWnd);

        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab) 
            {
                if (OpenWindows is null)
                {
                    var imageTop = 100;
                    var imageLeft = 100;
                    OpenWindows = Process.GetProcesses().Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)
                    && new WindowInteropHelper(this).Handle != p.MainWindowHandle);

                    canvas.Children.Clear();
                    Hide();

                    foreach (Process window in OpenWindows)
                    {
                        // Get window representation by taking screenshot
                        // Redundancy to ensure that the window is shown
                        ShowWindow(window.MainWindowHandle, IsIconic(window.MainWindowHandle) != 0? 9 : 5);
                        _ = SetForegroundWindow(window.MainWindowHandle);
                        _ = SetWindowPos(window.MainWindowHandle, -1, 0, 0, 0, 0, 0x0040 | 0x0001 | 0x0002);
                        BringWindowToTop(window.MainWindowHandle);
                        SwitchToThisWindow(window.MainWindowHandle, true);

                        Label title = new()
                        {
                            // Window handles are for debugging purposes
                            Content = window.MainWindowTitle + ' ' + window.MainWindowHandle,
                            Name = '_' + window.MainWindowHandle.ToString(),
                            Foreground = new BrushConverter().ConvertFromString("#ffffff") as Brush,
                        };

                        Image image = new()
                        {
                            Height = 100,
                            Width = 200,
                            Source = ScreenGrabber.CaptureWindow(window.MainWindowHandle).ToBitmapImage()
                        };

                        Image icon = new()
                        {
                            Height = 50,
                            Width = 50,
                            Source = Imaging.CreateBitmapSourceFromHIcon(
                                System.Drawing.Icon.ExtractAssociatedIcon(window.MainModule?.FileName ?? "")?.Handle ?? 0,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions()
                                )
                        };

                        // Minimize window
                        ShowWindow(window.MainWindowHandle, 2);

                        canvas.Children.Add(image);
                        Canvas.SetTop(image, imageTop);
                        Canvas.SetLeft(image, imageLeft);

                        canvas.Children.Add(icon);
                        Canvas.SetTop(icon, imageTop - 10);
                        Canvas.SetLeft(icon, imageLeft);

                        canvas.Children.Add(title);
                        Canvas.SetTop(title, imageTop - 25);
                        Canvas.SetLeft(title, imageLeft);

                        // Change position of subsequent window
                        imageLeft += 100;
                        imageTop += 50;
                    }
                    Show();
                } 
                else
                {
                    if (SelectedWindowIndex < OpenWindows.Count() - 1)
                    {
                        SelectedWindowIndex++;
                    }
                    else
                    {
                        SelectedWindowIndex = 0;
                    }

                    // Bold the label of selected window
                    // Todo: animation
                    canvas
                        .Children.OfType<Label>()
                        .Where((control) => control.Name == '_' + OpenWindows.ElementAt(SelectedWindowIndex).MainWindowHandle.ToString())
                        .First().FontWeight = FontWeights.ExtraBlack;
                }
            } 
            else if (e.Key == Key.Enter)
            {
                // Show selected window
                Hide();
                ShowWindow(OpenWindows?.ElementAt(SelectedWindowIndex).MainWindowHandle ?? 0, 1);
                OpenWindows = null;
                SelectedWindowIndex = 0;
            }
        }
    }
}