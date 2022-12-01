using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Diagrams.Views
{
    /// <summary>
    /// Interaction logic for TimeControl.xaml
    /// </summary>
    public partial class TimeControl : UserControl
    {
        public static readonly RoutedEvent ValueChangedEvent =
        System.Windows.Controls.Primitives.RangeBase.ValueChangedEvent.AddOwner(typeof(TimeControl));

        double oldValue;

        public TimeControl()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            TimeSlider.ValueChanged += (s, e) => Time = e.NewValue;
            TimeSlider.MouseDoubleClick += (s, e) => GoToMaximum();
            base.OnInitialized(e);
        }

        /// <summary>
        /// Update the time slider max, min and large change values.
        /// </summary>
        public void UpdateTimeSlider(double minimumYear, double maximumYear)
        {
            // The max is always this year.
            TimeSlider.Maximum = maximumYear;

            // Min is the more previous date in the diagram, this comes
            // from birth dates and marriages. Use a default of 10 years
            // if the diagram does not contain any dates.
            TimeSlider.Minimum = minimumYear;

            // Adjust the large change tick based on the range of the min / max values.
            TimeSlider.LargeChange = Math.Max(2,
                (int)((TimeSlider.Maximum - TimeSlider.Minimum) / 10));
        }

        public void MouseWheelChange(MouseWheelEventArgs e)
        {
            // Time slider
            if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                e.Handled = true;
                Time += (e.Delta > 0) ? TimeSlider.LargeChange : -TimeSlider.LargeChange;
            }
        }

        public void GoToMaximum()
        {
            Time = TimeSlider.Maximum;
        }


        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }


        public double Time
        {
            get { return TimeSlider.Value; }
            private set
            {
                if (value >= TimeSlider.Minimum && value <= TimeSlider.Maximum)
                {
                    TimeSlider.Value = value;
                    RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, value, ValueChangedEvent));
                    oldValue = value;
                }
            }
        }

    }
}

