/*
 * One node in the diagram. The control is a Button with a redefined control template.
 *
 * The control templates (and other resources) are specified in Skins/<name>/Resources/DiagramResources.xaml.
 * This is a resource dictionary that is part of the application resources. If the resources
 * were specified as part of the control, every instance of the control would allocate memory
 * for each resource. Specifying the resources at the application level only allocates one copy
 * of the resources which are shared with all instances of the control. The resources could be
 * specified in the application's generic dictionary also (themes/generic.xaml).
 *
 * Instead of specifying every possible node combination as a resource, only four control templates
 * are specified: female, male, primary female, and primary male. Then different brushes are used
 * depending on the node state. For example: sibling brush, related deceased brush, spouse brush.
 * This reduces the amount of code in the XAML file, but requires code that determines what control
 * template and brush resource to use based on the node's state.
 *
*/

//using Abstractions;
using System.Windows;

namespace Microsoft.FamilyShow
{
    public class NodeLimits : INodeLimits
    {
        private DiagramNode control;

        // Location of the node, relative to its parent group.
        private Point location = new();

        public NodeLimits()
        {
        }

        public DiagramNode Control { get => control; set => control = value; }

        /// <summary>
        /// Location of the node relative to the parent group.
        /// </summary>
        public Point Location
        {
            get => location;
            set => location = value;
        }

        /// <summary>
        /// Get the center of the node.
        /// </summary>
        public virtual Point Center => new(location.X + (control.DesiredSize.Width / 2), location.Y + (control.DesiredSize.Height / 2));

        /// <summary>
        /// Get the top center of the node. The center is shifted to the left since the
        /// person drawing is not located in the true center of the node, it's shifted
        /// to the left due to the shadow.
        /// </summary>
        public Point TopCenter => new(location.X + (control.DesiredSize.Width / 2), location.Y);

        /// <summary>
        /// Get the top right of the node.
        /// </summary>
        public Point TopRight => new(location.X + control.DesiredSize.Width, location.Y);

        /// <summary>
        /// Get the top left of the node.
        /// </summary>
        public Point TopLeft => new(location.X, location.Y);

        /// <summary>
        /// Get the bottom center of the node.
        /// </summary>
        public Point BottomCenter => new(location.X + (control.DesiredSize.Width / 2), location.Y + control.DesiredSize.Height);

        /// <summary>
        /// Get the left center of the node.
        /// </summary>
        public Point LeftCenter => new(location.X, location.Y + (control.DesiredSize.Height / 2));

        /// <summary>
        /// Get the right center of the node.
        /// </summary>
        public Point RightCenter => new(location.X + control.DesiredSize.Width, location.Y + (control.DesiredSize.Height / 2));
    }
}