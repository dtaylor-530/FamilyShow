using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Diagrams.Views;

/// <summary>
/// Interaction logic for ZoomControl.xaml
/// </summary>
public partial class ZoomControl : UserControl
{
    public static readonly RoutedEvent ValueChangedEvent =
        System.Windows.Controls.Primitives.RangeBase.ValueChangedEvent.AddOwner(typeof(ZoomControl));

    double oldValue;

    public ZoomControl()
    {
        InitializeComponent();
        // Default zoom level
        Zoom = 1;
    }

    protected override void OnInitialized(EventArgs e)
    {
        ZoomSlider.ValueChanged += (s, e) => Zoom = e.NewValue;
        ZoomSlider.MouseDoubleClick += (s, e) => Zoom = 1.0;
        base.OnInitialized(e);
    }

    public void MouseWheelChange(MouseWheelEventArgs e)
    {
        // Zoom slider
        if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
        {
            e.Handled = true;
            Zoom += (e.Delta > 0) ? ZoomSlider.LargeChange : -ZoomSlider.LargeChange;
        }
    }

    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add { AddHandler(ValueChangedEvent, value); }
        remove { RemoveHandler(ValueChangedEvent, value); }
    }

    public double Zoom
    {
        get { return ZoomSlider.Value; }
        private set
        {
            if (value >= ZoomSlider.Minimum && value <= ZoomSlider.Maximum)
            {
                ZoomSlider.Value = value;
                RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, value, ValueChangedEvent));
                oldValue = value;
            }
        }
    }
}
