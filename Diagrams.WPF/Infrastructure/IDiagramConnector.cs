/*
 * A connector consists of two nodes and a connection type. A connection has a
 * filtered state. The opacity is reduced when drawing a connection that is 
 * filtered. An animation is applied to the brush when the filtered state changes.
*/

using System.Collections.Generic;
using Abstractions;

namespace Diagrams.WPF.Infrastructure
{
    public interface IDiagramConnector
    {
        RelationshipType RelationshipType { get; }
        ICollection<DiagramConnectorNode> Nodes { get; }
        DiagramConnectorNode StartNode { get; }
        DiagramConnectorNode EndNode { get; }

    }
}