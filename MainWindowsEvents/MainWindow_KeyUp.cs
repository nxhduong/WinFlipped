using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinFlipped.Animations;

namespace WinFlipped
{
    public partial class MainWindow : Window
    {
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && OpenWindows is not null)
            {
                // Cycle through windows
                // Add hidden window


                // Move animation
                foreach (Label label in canvas.Children.OfType<Label>())
                {
                    label.MoveBy(50, 50);
                }

                foreach (Image image in canvas.Children.OfType<Image>())
                {
                    image.MoveBy(50, 50);
                }

                // Fade in animation
                canvas
                    .Children.OfType<Label>()
                    .Where((control) => control.Name == '_' + OpenWindows.First().MainWindowHandle.ToString())
                    .First().BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: false));
                canvas
                    .Children.OfType<Image>()
                    .Where((control) => control.Name == '_' + OpenWindows.First().MainWindowHandle.ToString())
                    .ToList().ForEach(i => i.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: false)));

                // Fade out animation
                var lastLabel = canvas
                    .Children.OfType<Label>()
                    .Where((control) => control.Name == '_' + OpenWindows.Last().MainWindowHandle.ToString())
                    .Last();
                lastLabel.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: true));
                canvas.Children.Remove(lastLabel);

                canvas
                    .Children.OfType<Image>()
                    .Where((control) => control.Name == '_' + OpenWindows.Last().MainWindowHandle.ToString())
                    .ToList().ForEach(i => {
                        i.BeginAnimation(OpacityProperty, new FadeAnimation(fadeOut: true));
                        canvas.Children.Remove(i);
                    });

                OpenWindows = OpenWindows.Prepend(OpenWindows.Last()).SkipLast(1);
            }
            else if (e.Key == Key.Enter)
            {
                // Show selected window, and quit program
                Application.Current.Shutdown();
                ShowWindow(OpenWindows?.Last().MainWindowHandle ?? 0, 1);
                OpenWindows = null;
            }
        }
    }
}
