using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinFlipped.Animations;
using WinFlipped.Helpers;
using Bitmap = System.Drawing.Bitmap;

namespace WinFlipped
{
    public partial class MainWindow : Window
    {
        private void MainWindow_KeyUp(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Tab && OpenWindows is not null && !eventArgs.IsRepeat)
            {
                // Cycle through windows
                (nint handle, string title, Bitmap screenshot) hiddenWindow;
                                                                    
                if (OpenWindows.Count() < 2)
                {
                    return;
                }
                else if (OpenWindows.Count() < WINDOWS_SHOW_LIMIT)
                {
                    hiddenWindow = OpenWindows.Last();
                } 
                else
                {
                    hiddenWindow = OpenWindows.SkipLast(WINDOWS_SHOW_LIMIT).Last();
                }

                // Move animation
                foreach (Label label in canvas.Children.OfType<Label>())
                {
                    label.MoveBy(50, 50);
                }

                foreach (Image image in canvas.Children.OfType<Image>())
                {
                    image.MoveBy(50, 50);
                    Canvas.SetZIndex(image, Canvas.GetZIndex(image) + 1);
                    image.ScaleBy(1.1);
                }

                // Fade out animation
                var lastLabel = canvas
                    .Children.OfType<Label>()
                    .Where((control) => control.Name == '_' + OpenWindows.Last().MainWindowHandle.ToString())
                    .Last();
                lastLabel.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: true));
                canvas.Children.Remove(lastLabel);

                var lastImage = canvas
                    .Children.OfType<Image>()
                    .Where((control) => control.Name == '_' + OpenWindows.Last().MainWindowHandle.ToString()).Last();
                lastImage.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: true));
                canvas.Children.Remove(lastImage);

                // Add hidden window
                (Label title, Image screenshot, Image icon) = canvas.DrawWindow(hiddenWindow.handle, hiddenWindow.title, hiddenWindow.screenshot, 50, 50, 1, 1, hidden: true);

                // Fade in animation
                title.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: false));
                screenshot.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: false));
                icon.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: false));

                OpenWindows = OpenWindows.Prepend(OpenWindows.Last()).SkipLast(1);
            }
            else if (eventArgs.Key == Key.Enter)
            {
                // Show selected window, and quit program
                Application.Current.Shutdown();
                ShowWindow(OpenWindows?.Last().MainWindowHandle ?? 0, 1);
                OpenWindows = null;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}