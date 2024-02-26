using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using WinFlipped.Helpers;

namespace WinFlipped
{
    public partial class MainWindow : Window
    {
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (OpenWindows is null)
            {
                var imageTop = 100;
                var imageLeft = 100;
                OpenWindows = WindowsEnumerator.GetOpenWindows().Where(win => win.MainWindowHandle != new WindowInteropHelper(this).Handle);

                canvas.Children.Clear();
                Hide();

                foreach ((nint MainWindowHandle, string MainWindowTitle) in OpenWindows.TakeLast(WINDOWS_SHOW_LIMIT))
                {
                    // Get window representation by taking screenshot
                    // Redundancy to ensure that the window is shown
                    ShowWindow(MainWindowHandle, IsIconic(MainWindowHandle) != 0 ? 9 : 5);
                    SetForegroundWindow(MainWindowHandle);
                    SetWindowPos(MainWindowHandle, -1, 0, 0, 0, 0, 0x0040 | 0x0001 | 0x0002);
                    BringWindowToTop(MainWindowHandle);
                    SwitchToThisWindow(MainWindowHandle, true);

                    Label title = new()
                    {
                        // Window handles are for debugging purposes
                        Content = MainWindowTitle + "|Handle " + MainWindowHandle,
                        Name = '_' + MainWindowHandle.ToString(),
                        Foreground = new BrushConverter().ConvertFromString("#FFFFFF") as Brush,
                    };

                    Image image = new()
                    {
                        Height = 100,
                        Width = 200,
                        Name = "_" + MainWindowHandle.ToString(),
                        Source = ScreenGrabber.CaptureWindow(MainWindowHandle).ToBitmapImage()
                    };

                    Image icon = new()
                    {
                        Height = 50,
                        Width = 50,
                        Name = "_" + MainWindowHandle.ToString(),
                        
                    };

                    try
                    {
                        icon.Source = Imaging.CreateBitmapSourceFromHIcon(
                            System.Drawing.Icon.ExtractAssociatedIcon(
                                Process.GetProcessById(
                                    (int)MainWindowHandle).MainModule?.FileName ?? ""
                                    )?.Handle ?? 0,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions()
                            );
                    } 
                    catch(Win32Exception ex)
                    {
                        // When getting process results in 'Access is denied' error
                        Trace.TraceError(ex.Message);
                    }
                    catch(ArgumentException ex)
                    {
                        // When casting failed
                        Trace.TraceError(ex.Message);
                    }

                    // Minimize window
                    ShowWindow(MainWindowHandle, 2);

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
        }
    }
}
