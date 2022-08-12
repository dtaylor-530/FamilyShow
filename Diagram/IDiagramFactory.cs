//using Abstractions;
using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Collections.Generic;

namespace Diagram.Logic
{
    public interface IDiagramFactory
    {
        event Action<object> CurrentNode;

        DiagramRow CreateChildrenRow(IList<object> children, double scale, double scaleRelated);
        DiagramNode CreateNode(object person, NodeType type, bool clickEvent, double? scale = null);
        DiagramRow CreateParentRow(IList<object> parents, double scale, double scaleRelated);
        DiagramRow CreatePrimaryRow(object person, double scale, double scaleRelated);
        void DestroyNode(DiagramNode node);
        IList<object> GetParents(DiagramRow parentRow);
        IList<object> GetChildren(DiagramRow childRow);
    }
}