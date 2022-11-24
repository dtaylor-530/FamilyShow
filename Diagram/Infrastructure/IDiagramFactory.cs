//using Abstractions;
using Abstractions;
using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Collections.Generic;

namespace Diagram.Logic
{
    public interface IDiagramFactory
    {
        event Action<INode> CurrentNode;

        DiagramRow CreateChildrenRow(IList<INode> children, double scale, double scaleRelated);

        DiagramNode CreateNode(INode person, NodeType type, bool clickEvent, double? scale = null);

        DiagramRow CreateParentRow(IList<INode> parents, double scale, double scaleRelated);

        DiagramRow CreatePrimaryRow(object person, double scale, double scaleRelated);

        void DestroyNode(DiagramNode node);

        IList<INode> GetParents(DiagramRow parentRow);

        IList<INode> GetChildren(DiagramRow childRow);
    }
}