using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;   
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

    internal class MoveAnimation { }
    internal class MoveToBottomAnimation { }
}
