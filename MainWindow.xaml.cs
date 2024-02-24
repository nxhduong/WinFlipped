using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Bookflipper.Helpers;

namespace Bookflipper
{
    public partial class MainWindow : Window
    {
        private Dictionary<nint, string>? VisibleWindows;
        private nint? SelectedWindow;

        [LibraryImport("user32.dll")]
        private static partial nint ShowWindow(nint hWnd, int nCmdShow);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial uint GetWindowThreadProcessId(nint hWnd, out uint pdwProcessId);

        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab) 
            {
                if (VisibleWindows is null)
                {
                    var imageTop = 100;
                    var imageLeft = 100;

                    canvas.Children.Clear();
                    Hide();

                    foreach (KeyValuePair<nint, string> window in OpenWindowGetter.GetOpenWindows())
                    {
                        // Ignore self window
                        if (new WindowInteropHelper(this).Handle == window.Key)
                        {
                            continue;
                        }

                        // Get window representation by screen grabbing
                        ShowWindow(window.Key, 1);
                        GetWindowThreadProcessId(window.Key, out uint process);

                        Label title = new()
                        {
                            // Window handles are for debugging purposes
                            Content = window.Value + ' ' + window.Key,
                            Foreground = new BrushConverter().ConvertFromString("#ffffff") as Brush
                        };

                        Image image = new()
                        {
                            Height = 100,
                            Width = 200,
                            Source = BitmapManipulator.SkewBitmap(ScreenGrabber.CaptureActiveWindow()).ToBitmapImage()
                        };

                        Image icon = new()
                        {
                            Height = 50,
                            Width = 50,
                            Source = Imaging.CreateBitmapSourceFromHIcon(
                                System.Drawing.Icon.ExtractAssociatedIcon(
                                    Process.GetProcessById((int)process).MainModule?.FileName ?? "")?.Handle ?? 0,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions()
                                )
                        };

                        // Minimize window
                        ShowWindow(window.Key, 2);

                        canvas.Children.Add(image);
                        Canvas.SetTop(image, imageTop);
                        Canvas.SetLeft(image, imageLeft);

                        canvas.Children.Add(icon);
                        Canvas.SetTop(icon, imageTop);
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
                    // TODO: switch selected window
                }
            } 
            else if (e.Key == Key.Enter)
            {
                // Show selected window
                Hide();
                ShowWindow(SelectedWindow ?? 0, 1);
                VisibleWindows = null;
                SelectedWindow = null;
            }
        }
    }
}