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
using System.Windows;

namespace Diagrams.WPF.Infrastructure
{
    public interface INodeLimits
    {
        Point BottomCenter(DiagramNode control) => new(control.Location.X + control.DesiredSize.Width / 2, control.Location.Y + control.DesiredSize.Height);

        Point Center(DiagramNode control) => new(control.Location.X + control.DesiredSize.Width / 2, control.Location.Y + control.DesiredSize.Height / 2);

        Point LeftCenter(DiagramNode control) => new(control.Location.X, control.Location.Y + control.DesiredSize.Height / 2);

        Point RightCenter(DiagramNode control) => new(control.Location.X + control.DesiredSize.Width, control.Location.Y + control.DesiredSize.Height / 2);

        Point TopCenter(DiagramNode control) => new(control.Location.X + control.DesiredSize.Width / 2, control.Location.Y);

        Point TopLeft(DiagramNode control) => new(control.Location.X, control.Location.Y);

        Point TopRight(DiagramNode control) => new(control.Location.X + control.DesiredSize.Width, control.Location.Y);
    }
}