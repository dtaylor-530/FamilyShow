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

using Diagrams.WPF;
using Diagrams.WPF.Infrastructure;
using Microsoft.FamilyShow;
using System.Windows;

namespace Demo.Family.Infrastructure
{
    public class NodeLimits : INodeLimits
    {
        /// <summary>
        /// Get the top center of the node. The center is shifted to the left since the
        /// person drawing is not located in the true center of the node, it's shifted
        /// to the left due to the shadow.
        /// </summary>
        public Point TopCenter(DiagramNode node)
        {
            // The real center of the node.
            Point point = new(node.Location.X + node.DesiredSize.Width / 2, node.Location.Y);

            // Shift the center to the left. This is an estimate since we don't
            // know the exact position of the person drawing within the node.
            FrameworkElement? personElement = node.Template.FindName("Person", node) as FrameworkElement;
            double offset = node.Type == NodeType.Primary ? 12 : 5;
            point.X -= (personElement?.ActualWidth ?? offset) / offset;
            return point;
        }
    }
}