/*
 * Arranges nodes based on relationships. Contains a series or rows (DiagramRow),
 * each row contains a series of groups (DiagramGroup), and each group contains a
 * series of nodes (DiagramNode).
 *
 * Contains a list of connections. Each connection describes where the node
 * is located in the diagram and type of connection. The lines are draw
 * during OnRender.
 *
 * Diagram is responsible for managing the rows. The logic that populates the rows
 * and understand all of the relationships is contained in DiagramLogic.
 *
*/

using Abstractions;
using Diagrams.WPF.Infrastructure;
using Diagrams.WPF.UI_Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Diagrams.WPF
{
    /// <summary>
    /// Diagram that lays out and displays the nodes.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class Diagram : FrameworkElement
    {
        #region fields

        public static class Const
        {
            // Duration to pause before displaying new nodes.
            public static double AnimationPauseDuration = 600;

            // Duration for nodes to fade in when the diagram is repopulated.
            public static double NodeFadeInDuration = 500;

            // Duration for the new person animation.
            public static double NewPersonAnimationDuration = 250;

            // Stop adding new rows when the number of nodes exceeds the max node limit.
            public static int MaximumNodes = 50;

            // Group space.
            public static double PrimaryRowGroupSpace = 20;

            public static double ChildRowGroupSpace = 20;
            public static double ParentRowGroupSpace = 40;

            // Amount of space between each row.
            public static double RowSpace = 40;

            // Scale multiplier for spouse and siblings.
            public static double RelatedMultiplier = 0.8;

            // Scale multiplier for each future generation row.
            public static double GenerationMultiplier = 0.9;
        }

        private bool needsRepopulating;

        // List of rows in the diagram. Each row contains groups, and each group contains nodes.
        private List<DiagramRow> rows = new List<DiagramRow>();

        // Populates the rows with nodes.
        private IDiagramLogic logic;

        // Size of the diagram. Used to layout all of the nodes before the
        // control gets an actual size.
        private Size totalSize = new Size(0, 0);

        // Zoom level of the diagram.
        private double scale = 1.0;

        // Bounding area of the selected node, the selected node is the
        // non-primary node that is selected, and will become the primary node.
        private Rect selectedNodeBounds = Rect.Empty;

        // Flag if currently populating or not. Necessary since diagram populate
        // contains several parts and animations, request to update the diagram
        // are ignored when this flag is set.
        private bool populating;

        // The person that has been added to the diagram.
        private INode? newPerson;

        // Timer used with the repopulating animation.
        private DispatcherTimer animationTimer = new DispatcherTimer();

#if DEBUG

        // Flag if the row and group borders should be drawn.
        private bool displayBorder;

        private double displayYear;
#endif

        #endregion fields

        public Diagram()
        {
        }

        public event EventHandler DiagramUpdated;

        public void OnDiagramUpdated()
        {
            if (DiagramUpdated != null)
            {
                DiagramUpdated(this, EventArgs.Empty);
            }
        }

        public event EventHandler DiagramPopulated;

        public void OnDiagramPopulated()
        {
            if (DiagramPopulated != null)
            {
                DiagramPopulated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the zoom level of the diagram.
        /// </summary>
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    LayoutTransform = new ScaleTransform(scale, scale);
                }
            }
        }

        ///// <summary>
        ///// Sets the display year filter.
        ///// </summary>
        //public double DisplayYear
        //{
        //    set
        //    {
        //        // Filter nodes and connections based on the year.
        //        if (displayYear != value)
        //        {
        //            displayYear = value;
        //            //foreach (DiagramConnectorNode connectorNode in Nodes)
        //            //    connectorNode.Node.Converter.DisplayYear = displayYear;
        //        }
        //        InvalidateVisual();
        //    }
        //}

        /// <summary>
        /// Gets the bounding area (relative to the diagram) of the primary node.
        /// </summary>
        public Rect PrimaryNodeBounds => logic?.Current != null ? GetNodeBounds(logic.Current) : default;

        /// <summary>
        /// Gets the bounding area (relative to the diagram) of the selected node.
        /// The selected node is the non-primary node that was previously selected
        /// to be the primary node.
        /// </summary>
        public Rect SelectedNodeBounds => selectedNodeBounds;

        /// <summary>
        /// Gets the number of nodes in the diagram.
        /// </summary>
        //public int NodeCount
        //{
        //  get { return logic.DiagramConnectorNodes.Count; }
        //}

        public static readonly DependencyProperty LogicProperty = DependencyProperty.Register("Logic", typeof(IDiagramLogic), typeof(Diagram), new PropertyMetadata(null, ModelChanged));

        public IDiagramLogic Logic
        {
            get { return (IDiagramLogic)GetValue(LogicProperty); }
            set { SetValue(LogicProperty, value); }
        }

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IDiagramLogic logic || d is not Diagram diagram)
                return;
            {
                // Init the diagram logic, which handles all of the layout logic.
                diagram.logic = logic;
                //logic.NodeClickHandler = new EventHandler(diagram.OnNodeClick);
                // Can have an empty People collection when in design tools such as Blend.

                logic.Update += new EventHandler<ContentChangedEventArgs>(diagram.OnUpdate);
                logic.CurrentChanged += new EventHandler(diagram.OnFamilyCurrentChanged);

                diagram.Populate();
            }
        }

        private void Update()
        {
            // First reset everything.
            Clear();

            foreach (var row in logic.GenerateRows())
            {
                if (row.IsParent)
                    rows.Insert(0, row);
                else
                    rows.Add(row);
                AddVisualChild(row);
            }
            // Raise event so others know the diagram was updated.
            OnDiagramUpdated();

            // Animate the new person (optional, might not be any new people).
            AnimateNewPerson();

            void AnimateNewPerson()
            {
                // The new person is optional, can be null.
                if (newPerson == null)
                {
                    return;
                }

                // Get the UI element to animate.
                if (logic.GetDiagramConnectorNode(newPerson).Node is DiagramNode node)
                {
                    // Create the new person animation.
                    DoubleAnimation anim = new DoubleAnimation(0, 1, AnimationHelper.GetAnimationDuration(Const.NewPersonAnimationDuration));
                    // Animate the node.
                    ScaleTransform transform = new ScaleTransform();
                    transform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                    transform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                    node.RenderTransform = transform;
                }
                else
                {
                    throw new Exception("DF dfggfdg");
                }
                newPerson = null;
            }
        }

        public IEnumerable<DiagramConnectorNode> Nodes
        {
            get
            {
                foreach (var connector in Connections)
                    foreach (DiagramConnectorNode connectorNode in connector.Nodes)
                        yield return connectorNode;
            }
        }

        public IEnumerable<DiagramConnector> Connections
        {
            get
            {
                foreach (var row in rows)
                    foreach (var connector in row.Connections)
                        yield return connector;
            }
        }

        #region layout

        protected override void OnInitialized(EventArgs e)
        {
#if DEBUG
            // Context menu so can display row and group borders.
            ContextMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            ContextMenu.Items.Add(item);
            item.Header = "Show Diagram Outline";
            item.Click += new RoutedEventHandler(OnToggleBorderClick);
            item.Foreground = SystemColors.MenuTextBrush;
            item.Background = SystemColors.MenuBrush;
#endif

            //logic.UpdateDiagram(this);
            base.OnInitialized(e);
        }

        protected override int VisualChildrenCount => rows.Count;

        // Return the requested row.
        protected override Visual GetVisualChild(int index) => rows[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            // Let each row determine how large they want to be.
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (DiagramRow row in rows)
            {
                row.Measure(size);
            }

            // Return the total size of the diagram.
            return ArrangeRows(false);
        }

        // Arrange the rows in the diagram, return the total size.
        protected override Size ArrangeOverride(Size finalSize) => ArrangeRows(true);

        /// <summary>
        /// Arrange the rows in the diagram, return the total size.
        /// </summary>
        private Size ArrangeRows(bool arrange)
        {
            // Location of the row.
            double pos = 0;

            // Bounding area of the row.
            Rect bounds = new Rect();

            // Total size of the diagram.
            Size size = new Size(0, 0);

            foreach (DiagramRow row in rows)
            {
                // Row location, center the row horizontaly.
                bounds.Y = pos;
                bounds.X = totalSize.Width == 0 ? 0 :
                    bounds.X = (totalSize.Width - row.DesiredSize.Width) / 2;

                // Row Size.
                bounds.Width = row.DesiredSize.Width;
                bounds.Height = row.DesiredSize.Height;

                // Arrange the row, save the location.
                if (arrange)
                {
                    row.Arrange(bounds);
                    row.Location = bounds.TopLeft;
                }

                // Update the size of the diagram.
                size.Width = Math.Max(size.Width, bounds.Width);
                size.Height = pos + row.DesiredSize.Height;

                pos += bounds.Height;
            }

            // Store the size, this is necessary so the diagram
            // can be laid out without a valid Width property.
            totalSize = size;
            return size;
        }

        #endregion layout

        /// <summary>
        /// Draw the connector lines at a lower level (OnRender) instead
        /// of creating visual tree objects.
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
#if DEBUG
            if (displayBorder)
            {
                // Draws borders around the rows and groups.
                foreach (DiagramRow row in rows)
                {
                    // Display row border.
                    Rect bounds = new Rect(row.Location, row.DesiredSize);
                    drawingContext.DrawRectangle(null, new Pen(Brushes.DarkKhaki, 1), bounds);

                    foreach (DiagramGroup group in row.Groups)
                    {
                        // Display group border.
                        bounds = new Rect(group.Location, group.DesiredSize);
                        bounds.Offset(row.Location.X, row.Location.Y);
                        bounds.Inflate(-1, -1);
                        drawingContext.DrawRectangle(null, new Pen(Brushes.Gray, 1), bounds);
                    }
                }
            }
#endif

            // Draw child connectors first, so marriage information appears on top.

            foreach (var connector in Connections)
            {
                if (connector.IsChildConnector)
                {
                    connector.Draw(drawingContext);
                }
            }

            // Draw all other non-child connectors.
            foreach (var connector in Connections)
            {
                if (!connector.IsChildConnector)
                {
                    connector.Draw(drawingContext);
                }
            }
        }

#if DEBUG

        private void OnToggleBorderClick(object sender, RoutedEventArgs e)
        {
            // Display or hide the row and group borders.
            displayBorder = !displayBorder;

            // Update check on menu.
            MenuItem menuItem = ContextMenu.Items[0] as MenuItem;
            menuItem.IsChecked = displayBorder;

            InvalidateVisual();
        }

#endif

        /// <summary>
        /// Reset all of the data associated with the diagram.
        /// </summary>
        public void Clear()
        {
            foreach (DiagramRow row in rows)
            {
                row.Clear();
                RemoveVisualChild(row);
            }

            rows.Clear();
            logic.Clear();
        }


        /// <summary>
        /// Populate the diagram. Update the diagram and hide all non-primary nodes.
        /// Then pause, and finish the populate by fading in the new nodes.
        /// </summary>
        public void Populate()
        {
            if (populating)
            {
                needsRepopulating = true;
                //throw new Exception("sdf3 rgr");
                return;
            }
            // Set flag to ignore future updates until complete.

            if (logic == null)
                return;

            populating = true;
            needsRepopulating = false;
            // Save the bounds for the current primary person, this
            // is required later when animating the diagram.
            selectedNodeBounds = PrimaryNodeBounds;

            // Update the nodes in the diagram.
            Update();

            // First hide all of the nodes except the primary node.
            foreach (DiagramConnectorNode connectorNode in Nodes)
            {
                if (connectorNode.Node.Model != logic.Current)
                {
                    connectorNode.Node.Visibility = Visibility.Hidden;
                }
            }

            // Required to update (hide) the connector lines.
            InvalidateVisual();
            InvalidateArrange();
            InvalidateMeasure();

            // Pause before displaying the new nodes.
            animationTimer.Interval = AnimationHelper.GetAnimationDuration(Const.AnimationPauseDuration);
            animationTimer.Tick += new EventHandler(OnAnimationTimer);
            animationTimer.IsEnabled = true;

            /// <summary>
            /// The animation pause timer is complete, finish populating the diagram.
            /// </summary>
            void OnAnimationTimer(object sender, EventArgs e)
            {
                // Turn off the timer.
                animationTimer.IsEnabled = false;

                // Fade each node into view.

                foreach (DiagramConnectorNode connectorNode in Nodes)
                {
                    if (connectorNode.Node.Visibility != Visibility.Visible)
                    {
                        connectorNode.Node.Visibility = Visibility.Visible;
                        connectorNode.Node.BeginAnimation(OpacityProperty,
                            new DoubleAnimation(0, 1, AnimationHelper.GetAnimationDuration(Const.NodeFadeInDuration)));
                    }
                }

                // Redraw connector lines.
                InvalidateVisual();

                populating = false;

                if (needsRepopulating)
                {
                    Populate();
                    return;
                }
                // Let other controls know the diagram has been repopulated.
                OnDiagramPopulated();
            }
        }

        /// <summary>
        /// Called when the current person in the main People collection changes.
        /// This means the diagram should be updated based on the new selected person.
        /// </summary>
        private void OnFamilyCurrentChanged(object sender, EventArgs e)
        {
            // Repopulate the diagram.
            Populate();
        }

        /// <summary>
        /// Return the bounds (relative to the diagram) for the specified person.
        /// </summary>
        public Rect GetNodeBounds(INode person)
        {
            DiagramConnectorNode connector = logic.GetDiagramConnectorNode(person);
            var bounds = new Rect(connector.TopLeft.X, connector.TopLeft.Y,
                connector.Node.ActualWidth, connector.Node.ActualHeight);
            return bounds;
        }

        /// <summary>
        /// Called when data changed in the main People collection. This can be
        /// a new node added to the collection, updated Person details, and
        /// updated relationship data.
        /// </summary>
        private void OnUpdate(object sender, ContentChangedEventArgs e)
        {
            // Ignore if currently repopulating the diagram.
            if (populating)
            {
                return;
            }

            // Save the person that is being added to the diagram.
            // This is optional and can be null.
            newPerson = e.NewPerson;

            // Redraw the diagram.
            Update();
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        /// <summary>
        /// Animate the new person that was added to the diagram.
        /// </summary>
        /// <summary>
        /// Reset the diagram with the nodes. This is accomplished by creating a series of rows.
        /// Each row contains a series of groups, and each group contains the nodes. The elements
        /// are not laid out at this time. Also creates the connections between the nodes.
        /// </summary>
    }
}