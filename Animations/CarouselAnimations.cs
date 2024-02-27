using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WinFlipped.Animations
{
    internal class FadeAnimation : DoubleAnimation
    {
        public FadeAnimation(bool fadeOut)
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            From = fadeOut ? 1.0 : 0.0;
            To = fadeOut ? 0.0 : 1.0;
            AutoReverse = false;
            RepeatBehavior = new RepeatBehavior(1);
        }
    }

    internal static class Transformations {
        /// <summary>
        /// Translate (move) an element. The element remains in new position.
        /// </summary>
        /// <typeparam name="T">Extends FrameworkElement (Label or Image)</typeparam>
        /// <param name="target">Element to be translated</param>
        /// <param name="hDist">Horizontal distance between old and new position</param>
        /// <param name="vDist">Vertical distance between old and new position</param>
        public static void MoveBy<T>(this T target, double hDist, double vDist) where T : FrameworkElement
        {
            TranslateTransform trans = new();
            target.RenderTransform = trans;

            trans.BeginAnimation(
                TranslateTransform.XProperty, 
                new DoubleAnimation(Canvas.GetLeft(target), Canvas.GetLeft(target) + hDist, TimeSpan.FromMilliseconds(500))
                {
                    IsAdditive = true
                }
            );
            trans.BeginAnimation(
                TranslateTransform.YProperty,   
                new DoubleAnimation(Canvas.GetTop(target), Canvas.GetTop(target) + vDist, TimeSpan.FromMilliseconds(500))
                {
                    IsAdditive = true
                }
            );

            Trace.WriteLine(target.Name + Canvas.GetTop(target) + ' ' + Canvas.GetLeft(target));
        }

        public static void ScaleBy(this Image target, double scale)
        {
            target.BeginAnimation(Image.HeightProperty, new DoubleAnimation(target.Height, target.Height * scale, TimeSpan.FromMilliseconds(500)));
            target.BeginAnimation(Image.WidthProperty, new DoubleAnimation(target.Width, target.Width * scale, TimeSpan.FromMilliseconds(500)));
        }
    }
}
