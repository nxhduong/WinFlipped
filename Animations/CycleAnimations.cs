using System;
using System.Collections.Generic;
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
            Duration = new Duration(TimeSpan.FromMilliseconds(500));
            From = fadeOut ? 1.0 : 0.0;
            To = fadeOut ? 0.0 : 1.0;
            AutoReverse = false;
            RepeatBehavior = new RepeatBehavior(1);
        }
    }

    internal static class MoveAnimation {
        public static void MoveBy(this Image target, double hDist, double vDist)
        {
            TranslateTransform trans = new();
            target.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(target.Width, target.Width + hDist, TimeSpan.FromMilliseconds(500)));
            trans.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(target.Height, target.Height + vDist, TimeSpan.FromMilliseconds(500)));
        }

        public static void MoveBy(this Label target, double hDist, double vDist)
        {
            TranslateTransform trans = new();
            target.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(target.Width, target.Width + hDist, TimeSpan.FromMilliseconds(500)));
            trans.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(target.Height, target.Height + vDist, TimeSpan.FromMilliseconds(500)));
        }
    }
    internal class EntranceAnimation { }
}
