using System.Windows.Interop;
using System.Windows;
using WinFlipped.Helpers;
using Bitmap = System.Drawing.Bitmap;

namespace WinFlipped
{
    public partial class MainWindow : Window
    {
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var imageTop = 100;
            var imageLeft = 100;
            var scale = 1.0;
            var zIndex = 1;
            OpenWindows = WindowsEnumerator.GetOpenWindows().Where(win => win.MainWindowHandle != new WindowInteropHelper(this).Handle);

            canvas.Children.Clear();
            Hide();

            foreach ((nint mainWindowHandle, string mainWindowTitle, Bitmap mainWindowScreenshot) in OpenWindows.TakeLast(WINDOWS_SHOW_LIMIT))
            {
                // Get window representation by taking screenshot
                // Redundancy to ensure that the window is shown
                ShowWindow(mainWindowHandle, IsIconic(mainWindowHandle) != 0 ? 9 : 5);
                SetForegroundWindow(mainWindowHandle);
                SetWindowPos(mainWindowHandle, -1, 0, 0, 0, 0, 0x0040 | 0x0001 | 0x0002);
                BringWindowToTop(mainWindowHandle);
                SwitchToThisWindow(mainWindowHandle, true);

                Thread.Sleep(800);

                canvas.DrawWindow(mainWindowHandle, mainWindowTitle, mainWindowScreenshot, imageTop, imageLeft, scale, zIndex);

                // Minimize window
                ShowWindow(mainWindowHandle, 2);

                // Change position of subsequent window
                imageLeft += 100;
                imageTop += 50;
                scale += 0.1;
                zIndex++;
            }

            Show();
            Activate();
        }
    }
}