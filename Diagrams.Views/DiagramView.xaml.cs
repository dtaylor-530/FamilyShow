/*
 * Adds zooming and scrolling to the diagram control. 
*/

using Diagrams;
using Diagrams.WPF.Infrastructure;
using Diagrams.WPF.UI_Infrastructure;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Diagrams.Views
{
    /// <summary>
    /// Adds zooming and scrolling to the diagram control.
    /// </summary>
    public partial class DiagramView : UserControl
    {
        public static readonly DependencyProperty LogicProperty = DependencyProperty.Register("Logic", typeof(IDiagramLogic), typeof(DiagramView), new PropertyMetadata(null, ModelChanged));
        public static readonly DependencyProperty MinimumYearProperty = DependencyProperty.Register("MinimumYear", typeof(double), typeof(DiagramView), new PropertyMetadata());
        public static readonly DependencyProperty MaximumYearProperty = DependencyProperty.Register("MaximumYear", typeof(double), typeof(DiagramView), new PropertyMetadata());
        public static readonly DependencyProperty YearProperty = DependencyProperty.Register("Year", typeof(double), typeof(DiagramView), new PropertyMetadata());

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IDiagramLogic model || d is not DiagramView diagramView)
                return;
            diagramView.Diagram.Logic = model;
            // Initialize the time filter.
            diagramView.TimeControl.UpdateTimeSlider(diagramView.MinimumYear, diagramView.MaximumYear);
            diagramView.TimeControl.GoToMaximum();
        }

        private static class Const
        {
            // Amount of the diagram that cannot be scrolled, this 
            // ensures that some of the diagram is always visible.
            public static double PanMargin = 50;

            // Duration to pause before auto centering the diagram.
            public static double AutoCenterAnimationPauseDuration = 1000;

            // Duration of the auto center animation.
            public static double AutoCenterAnimationDuration = 600;
        }

        // Used when manually scrolling.
        private Point scrollStartPoint;
        private Point scrollStartOffset;

        // Stores the top-left offset of the diagram. Used to auto-scroll
        // the new primary node to the location of the previous selected node.
        private Point previousTopLeftOffset;

        // Timer that is used when animating a new diagram.
        private DispatcherTimer autoCenterTimer;

        public DiagramView()
        {
            InitializeComponent();
            this.SizeChanged += _SizeChanged;
        }

        #region properties

        public IDiagramLogic Logic
        {
            get { return (IDiagramLogic)GetValue(LogicProperty); }
            set { SetValue(LogicProperty, value); }
        }

        public double MinimumYear
        {
            get { return (double)GetValue(MinimumYearProperty); }
            set { SetValue(MinimumYearProperty, value); }
        }

        public double MaximumYear
        {
            get { return (double)GetValue(MaximumYearProperty); }
            set { SetValue(MaximumYearProperty, value); }
        }

        public double Year
        {
            get { return (double)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        #endregion


        #region event handlers

        protected override void OnInitialized(EventArgs e)
        {
            // Timer used for animations.
            autoCenterTimer = new DispatcherTimer();

            // Events.
            Diagram.Loaded += new RoutedEventHandler(Diagram_Loaded);
            Diagram.SizeChanged += new SizeChangedEventHandler(Diagram_SizeChanged);
            Diagram.DiagramUpdated += new EventHandler(Diagram_DiagramUpdated);
            Diagram.DiagramPopulated += new EventHandler(Diagram_DiagramPopulated);

            TimeControl.ValueChanged += TimeSlider_ValueChanged;
            ZoomControl.ValueChanged += ZoomControl_ValueChanged;
            SizeChanged += new SizeChangedEventHandler(Diagram_SizeChanged);
            ScrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);

            base.OnInitialized(e);
        }

        private void ZoomControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Diagram.Scale = e.NewValue;
            UpdateScrollSize(e.NewValue);
        }

        private void Diagram_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the display after the diagram has been loaded,
            // set the scroll size and center the diagram in the display area.
            UpdateScrollSize(ZoomControl.Zoom);
            AutoScrollToSelected(ZoomControl.Zoom);
        }

        private void Diagram_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update the scroll size, this is necessary to make sure the
            // user cannot scroll the entire diagram off the display area.
            UpdateScrollSize(ZoomControl.Zoom);
        }

        private void Diagram_DiagramUpdated(object sender, EventArgs e)
        {
            // The diagram changed, update the time slider min and max values.
            TimeControl.UpdateTimeSlider(this.MinimumYear, this.MaximumYear);
        }

        private void Diagram_DiagramPopulated(object sender, EventArgs e)
        {
            // The diagram was populated. Need to force the diagram to layout 
            // since the diagram values are required to perform animations (need 
            // to know exactly where the primary node is located).

            // Save the current top-left offset before force the layout,
            // this is used later when animating the diagram.
            Point offset = GetTopLeftScrollOffset(ZoomControl.Zoom);
            previousTopLeftOffset = new Point(
                Grid.ActualWidth - ScrollViewer.HorizontalOffset - offset.X,
                Grid.ActualHeight - ScrollViewer.VerticalOffset - offset.Y);

            // Force the layout.
            UpdateLayout();

            // Now auto-scroll so the primary node appears at the previous
            // selected node location.
            AutoScrollToSelected(ZoomControl.Zoom);

            // Reset the time slider.
            TimeControl.GoToMaximum();
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Diagram.Populate();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Need to adjust the scroll position when zoom changes to keep 
            // the diagram centered. The ScrollChanged event occurs when
            // zooming since the diagram's extent changes.
            if (e.ExtentWidthChange != 0 && e.ExtentWidthChange != e.ExtentWidth)
            {
                // Keep centered horizontaly.
                double percent = e.ExtentWidthChange / (e.ExtentWidth - e.ExtentWidthChange);
                double middle = e.HorizontalOffset + (e.ViewportWidth / 2);
                ScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset + (middle * percent));
            }

            if (e.ExtentHeightChange != 0 && e.ExtentHeightChange != e.ExtentHeight)
            {
                // Keep centered verically.
                double percent = e.ExtentHeightChange / (e.ExtentHeight - e.ExtentHeightChange);
                double middle = e.VerticalOffset + (e.ViewportHeight / 2);
                ScrollViewer.ScrollToVerticalOffset(e.VerticalOffset + (middle * percent));
            }
        }

        /// <summary>
        /// Adjust the time slider for Shift + MouseWheel, 
        /// adjust the zoom slider for Ctrl + MouseWheel.
        /// </summary>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            ZoomControl.MouseWheelChange(e);
            TimeControl.MouseWheelChange(e);
            base.OnPreviewMouseWheel(e);
        }


        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Tell the diagram which information to filter.
            Year = e.NewValue;
        }
        #endregion

        /// <summary>
        /// Return the offset that positions the diagram in the top-left 
        /// corner, takes into account the zoom level.
        /// </summary>
        private Point GetTopLeftScrollOffset(double zoom)
        {
            // Offset that is returned.
            Point offset = new Point();
            
            // Empty offset if the diagram is empty.
            if (Diagram.ActualWidth == 0 || Diagram.ActualHeight == 0)
                return offset;

            // Get the size of the diagram.
            Size diagramSize = new Size(
                Diagram.ActualWidth * zoom,
                Diagram.ActualHeight * zoom);

            // Calcualte the offset that positions the diagram in the top-left corner.
            offset.X = ActualWidth + diagramSize.Width - (Const.PanMargin / 2);
            offset.Y = ActualHeight + diagramSize.Height - (Const.PanMargin / 2);

            return offset;
        }

        /// <summary>
        /// Update the scroll area so the diagram can be scrolled from edge to edge.
        /// </summary>
        private void UpdateScrollSize(double zoom)
        {
            // Nothing to do if the diagram is empty.
            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            Size diagramSize = new Size(
                Diagram.ActualWidth * zoom,
                Diagram.ActualHeight * zoom);

            // The grid contains the diagram, set the size of the grid so it's
            // large enough to allow the diagram to scroll from edge to edge.
            Grid.Width = Math.Max(0, (ActualWidth * 2) + diagramSize.Width - Const.PanMargin);
            Grid.Height = Math.Max(0, (ActualHeight * 2) + diagramSize.Height - Const.PanMargin);
        }

        /// <summary>
        /// Scroll the diagram so the primary node appears at the location of the selected node.
        /// </summary>
        private void AutoScrollToSelected(double zoom)
        {
            // Don't scroll if the diagram is empty.
            if (Diagram.ActualWidth == 0 || Diagram.ActualHeight == 0)
                return;

            // This is the offset that will be scrolled. First get the offset 
            // that positions the diagram in the top-left corner.
            Point offset = GetTopLeftScrollOffset(ZoomControl.Zoom);

            // Get the location of the node that was selected.
            Rect selectedBounds = Diagram.SelectedNodeBounds;

            // See if this is the first time the diagram is being displayed.            
            if (selectedBounds.IsEmpty)
            {
                // First time, center the diagram in the display area.
                offset.X += ((ActualWidth - (Diagram.ActualWidth * zoom)) / 2);
                offset.Y += ((ActualHeight - (Diagram.ActualHeight * zoom)) / 2);
            }
            else
            {
                // Scroll the diagram so the new primary node is at the location
                // of the previous selected node. 

                // Offset the distance the diagram is scrolled from the 
                // previous top-left position.
                offset.X += previousTopLeftOffset.X;
                offset.Y += previousTopLeftOffset.Y;

                // Determine the distance between the two nodes.
                Rect primaryBounds = Diagram.PrimaryNodeBounds;
                Point nodeDelta = new Point();
                nodeDelta.X = (primaryBounds.Left + (primaryBounds.Width / 2)) -
                    (selectedBounds.Left + (selectedBounds.Width / 2));
                nodeDelta.Y = (primaryBounds.Top + (primaryBounds.Height / 2)) -
                    (selectedBounds.Top + (selectedBounds.Height / 2));

                // Offset the distance between the two nodes.
                offset.X -= (nodeDelta.X * zoom);
                offset.Y -= (nodeDelta.Y * zoom);
            }

            // Scroll the diagram.
            ScrollViewer.ScrollToHorizontalOffset(Grid.Width - offset.X);
            ScrollViewer.ScrollToVerticalOffset(Grid.Height - offset.Y);

            // Set a timer so there is a pause before centering the diagram.
            autoCenterTimer.Interval = AnimationHelper.GetAnimationDuration(Const.AutoCenterAnimationPauseDuration);
            autoCenterTimer.Tick += new EventHandler(OnAutoCenterPauseTimer);
            autoCenterTimer.IsEnabled = true;
        }

        void OnAutoCenterPauseTimer(object sender, EventArgs e)
        {
            // Timer only fires once.
            autoCenterTimer.IsEnabled = false;

            // Scroll the diagram so it's centered in the display area.
            AutoScrollToCenter(ZoomControl.Zoom);
        }

        /// <summary>
        /// Center the diagram in the display area.
        /// </summary>
        private void AutoScrollToCenter(double zoom)
        {
            // Adjust the offset so the diagram appears in the center of 
            // the display area. First get the top-left offset.
            Point offset = GetTopLeftScrollOffset(zoom);

            // Now adjust the offset so the diagram is centered.
            offset.X += ((ActualWidth - (Diagram.ActualWidth * zoom)) / 2);
            offset.Y += ((ActualHeight - (Diagram.ActualHeight * zoom)) / 2);

            // Before auto scroll, determine the start and end 
            // points so the scrolling can be animated.
            Point startLocation = new Point(
                ScrollViewer.HorizontalOffset,
                ScrollViewer.VerticalOffset);

            Point endLocation = new Point(
                Grid.Width - offset.X - startLocation.X,
                Grid.Height - offset.Y - startLocation.Y);

            // Auto scroll the diagram.
            ScrollViewer.ScrollToHorizontalOffset(Grid.Width - offset.X);
            ScrollViewer.ScrollToVerticalOffset(Grid.Height - offset.Y);

            // Animate the scrollings.
            AnimateDiagram(endLocation);
        }

        /// <summary>
        /// Animate the diagram by moving from the startLocation to the endLocation.
        /// </summary>
        private void AnimateDiagram(Point endLocation)
        {
            // Create the animations, nonlinear by using accelration and deceleration.
            DoubleAnimation horzAnim = new DoubleAnimation(endLocation.X, 0,
                
               AnimationHelper.GetAnimationDuration(Const.AutoCenterAnimationDuration));
            horzAnim.AccelerationRatio = .5;
            horzAnim.DecelerationRatio = .5;

            DoubleAnimation vertAnim = new DoubleAnimation(endLocation.Y, 0,
              AnimationHelper.GetAnimationDuration(Const.AutoCenterAnimationDuration));
            vertAnim.AccelerationRatio = .5;
            vertAnim.DecelerationRatio = .5;

            // Animate the transform to make it appear like the diagram is moving.
            TranslateTransform transform = new TranslateTransform();
            transform.BeginAnimation(TranslateTransform.XProperty, horzAnim);
            transform.BeginAnimation(TranslateTransform.YProperty, vertAnim);

            // Animate the grid (that contains the diagram) instead of the 
            // diagram, otherwise nodes are clipped in the diagram.
            Grid.RenderTransform = transform;
        }

  
        #region manual scroll

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ScrollViewer.IsMouseOver && !Diagram.IsMouseOver)
            {
                // Save starting point, used later when determining how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewer.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewer.VerticalOffset;

                // Update the cursor if can scroll or not.
                Cursor = (ScrollViewer.ExtentWidth > ScrollViewer.ViewportWidth) ||
                    (ScrollViewer.ExtentHeight > ScrollViewer.ViewportHeight) ?
                    Cursors.ScrollAll : Cursors.Arrow;

                CaptureMouse();
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                // Get the new scroll position.
                Point point = e.GetPosition(this);

                // Determine the new amount to scroll.
                Point delta = new Point(
                    (point.X > scrollStartPoint.X) ?
                        -(point.X - scrollStartPoint.X) : (scrollStartPoint.X - point.X),
                    (point.Y > scrollStartPoint.Y) ?
                        -(point.Y - scrollStartPoint.Y) : (scrollStartPoint.Y - point.Y));

                // Scroll to the new position.
                ScrollViewer.ScrollToHorizontalOffset(scrollStartOffset.X + delta.X);
                ScrollViewer.ScrollToVerticalOffset(scrollStartOffset.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Cursor = Cursors.Arrow;
                ReleaseMouseCapture();
            }

            base.OnPreviewMouseUp(e);
        }

        #endregion

    }
}
