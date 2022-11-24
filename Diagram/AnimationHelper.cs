using System;
using System.Windows.Input;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public class AnimationHelper
    {
        /// <summary>
        /// Return the animation duration. The duration is extended
        /// if special keys are currently pressed (for demo purposes)
        /// otherwise the specified duration is returned.
        /// </summary>
        public static TimeSpan GetAnimationDuration(double milliseconds)
        {
            return TimeSpan.FromMilliseconds(Keyboard.IsKeyDown(Key.F12) ? milliseconds * 5 : milliseconds);
        }
    }
}