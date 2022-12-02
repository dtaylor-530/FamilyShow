using Abstractions;
using Diagrams.WPF;
using Diagrams.WPF.Infrastructure;
using Microsoft.FamilyShow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diagrams.Logic
{
    public class DiagramFactory : IDiagramFactory
    {
        private readonly Dictionary<INode, DiagramConnectorNode> personLookup;
        private INodeConverter nodeConverter;
        private IConnectorConverter connectorConverter;
        private readonly INodeLimits nodeLimits;

        public event Action<INode> CurrentNode;

        public DiagramFactory(Dictionary<INode, DiagramConnectorNode> personLookup, INodeConverter nodeConverter, IConnectorConverter connectorConverter, INodeLimits nodeLimits)
        {
            this.personLookup = personLookup;
            this.nodeConverter = nodeConverter;
            this.connectorConverter = connectorConverter;
            this.nodeLimits = nodeLimits;
        }

        /// <summary>
        /// Create a DiagramNode.
        /// </summary>
        public DiagramNode CreateNode(INode person, NodeType type, bool clickEvent, double? scale = default)
        {
            DiagramNode node = new(person, type, nodeConverter, nodeLimits)
            {
                Scale = scale ?? DiagramNode.Const.Scale
            };
            if (clickEvent)
                node.Click += (s, e) => CurrentNode?.Invoke(person);

            return node;
        }

        public void DestroyNode(DiagramNode node)
        {
            node.Click -= (s, e) => CurrentNode?.Invoke(node.Model as INode);
        }

        /// <summary>
        /// Add the siblings to the specified row and group.
        /// </summary>
        private void AddSiblingNodes(DiagramRow row, DiagramGroup group, IList<INode> siblings, NodeType nodeType, double scale)
        {
            foreach (INode sibling in siblings)
            {
                if (!personLookup.ContainsKey(sibling))
                {
                    // Siblings node.
                    DiagramNode node = CreateNode(sibling, nodeType, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Model, new DiagramConnectorNode(node, group, row));
                }
            }
        }

        /// <summary>
        /// Add the spouses to the specified row and group.
        /// </summary>
        private IEnumerable<DiagramConnector> AddSpouseNodes(INode person, DiagramRow row,
            DiagramGroup group, IList<IRelationship> spouses,
            NodeType nodeType, double scale)
        {
            foreach (IRelationship spouse in spouses)
            {
                if (!personLookup.ContainsKey(spouse.To))
                {
                    // Spouse node.
                    DiagramNode node = CreateNode(spouse.To, nodeType, true, scale);
                    group.Add(node);

                    // Add connection.
                    DiagramConnectorNode connectorNode = new(node, group, row);
                    personLookup.Add(node.Model, connectorNode);
                    yield return new MarriedDiagramConnector(spouse, personLookup[person], connectorNode, connectorConverter);
                }
            }
        }

        /// <summary>
        /// Creates the primary row. The row contains groups: 1) The primary-group
        /// that only contains the primary node, and 2) The optional left-group
        /// that contains spouses and siblings.
        /// </summary>
        public DiagramRow CreatePrimaryRow(object obj, double scale, double scaleRelated)
        {
            if (obj is not INode person)
                throw new Exception("sdffs");

            // The primary node contains two groups,
            DiagramGroup primaryGroup = new();
            DiagramGroup leftGroup = new();

            // Set up the row.
            DiagramRow row = new DiagramRow();

            // Add primary node.
            DiagramNode node = CreateNode(person, NodeType.Primary, false, scale);
            primaryGroup.Add(node);
            personLookup.Add(node.Model, new DiagramConnectorNode(node, primaryGroup, row));

            // Current spouses.
            var currentSpouses = person.Relationships(RelationshipType.Spouse).ToList();
            foreach (var conn in AddSpouseNodes(person, row, leftGroup, currentSpouses,
                NodeType.Spouse, scaleRelated))
                row.Add(conn);
            // Previous spouses.
            //IList<object> previousSpouses = person.PreviousSpouses.Cast<object>().ToList();
            //foreach (var conn in AddSpouseNodes(person, row, leftGroup, previousSpouses,
            //    NodeType.Spouse, scaleRelated, false))
            //    row.Add(conn);
            // Siblings.
            var siblings = person.Siblings().ToList();
            AddSiblingNodes(row, leftGroup, siblings, NodeType.Sibling, scaleRelated);

            // Half siblings.
            //IList<object> halfSiblings = person.HalfSiblings.Cast<object>().ToList();
            //AddSiblingNodes(row, leftGroup, halfSiblings, NodeType.SiblingLeft, scaleRelated);

            if (leftGroup.Nodes.Count > 0)
            {
                leftGroup.Reverse();
                row.Add(leftGroup);
            }

            row.Add(primaryGroup);

            return row;
        }

        /// <summary>
        /// Create the child row. The row contains a group for each child.
        /// Each group contains the child and spouses.
        /// </summary>
        public DiagramRow CreateChildrenRow(IList<INode> children, double scale, double scaleRelated)
        {
            // Setup the row.
            DiagramRow row = new DiagramRow() { IsParent = false };

            foreach (INode child in children)
            {
                // Each child is in their group, the group contains the child
                // and any spouses. The groups does not contain siblings.
                DiagramGroup group = new DiagramGroup();
                row.Add(group);

                // Child.
                if (!personLookup.ContainsKey(child))
                {
                    DiagramNode node = CreateNode(child, NodeType.Related, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Model, new DiagramConnectorNode(node, group, row));
                }

                // Current spouses.
                var currentSpouses = child.Relationships(RelationshipType.Spouse).ToList();
                foreach (var conn in AddSpouseNodes(child, row, group, currentSpouses,
                    NodeType.Spouse, scaleRelated))
                    group.Add(conn);
                // Previous spouses.
                //IList<object> previousSpouses = child.PreviousSpouses.Cast<object>().ToList();
                //foreach (var conn in AddSpouseNodes(child, row, group, previousSpouses,
                //    NodeType.Spouse, scaleRelated, false))
                //    group.Add(conn);
                // Connections.

                foreach (var conn in this.personLookup.MakeParentConnections(child, connectorConverter))
                {
                    group.Add(conn);
                }

                group.Reverse();
            }

            return row;
        }

        /// <summary>
        /// Create the parent row. The row contains a group for each parent.
        /// Each groups contains the parent, spouses and siblings.
        /// </summary>
        public DiagramRow CreateParentRow(IList<INode> parents, double scale, double scaleRelated)
        {
            // Set up the row.
            DiagramRow row = new DiagramRow() { IsParent = true };

            int groupCount = 0;

            foreach (INode node in parents)
            {
                // Each parent is in their group, the group contains the parent,
                // spouses and siblings.
                DiagramGroup group = new DiagramGroup();
                row.Add(group);

                // Determine if this is a left or right oriented group.
                bool left = (groupCount++ % 2 == 0) ? true : false;

                // Parent.
                if (!personLookup.ContainsKey(node))
                {
                    DiagramNode diNode = CreateNode(node, NodeType.Related, true, scale);
                    group.Add(diNode);
                    personLookup.Add(node, new DiagramConnectorNode(diNode, group, row));
                }

                // Current spouses.
                var currentSpouses = node.Relationships(RelationshipType.Spouse).ToList();
                DiagramHelper.RemoveDuplicates(currentSpouses, parents);
                foreach (var conn in AddSpouseNodes(node, row, group, currentSpouses, NodeType.Spouse, scaleRelated))
                    group.Add(conn);
                // Previous spouses.
                //IList<object> previousSpouses = person.PreviousSpouses.Cast<object>().ToList();
                //previousSpouses.RemoveDuplicates(parents);
                //foreach (var conn in AddSpouseNodes(person, row, group, previousSpouses, NodeType.Spouse, scaleRelated, false))
                //    group.Add(conn);

                // Siblings.
                var siblings = node.Siblings().ToList();
                AddSiblingNodes(row, group, siblings, NodeType.Sibling, scaleRelated);

                // Half siblings.
                //IList<object> halfSiblings = person.HalfSiblings.Cast<object>().ToList();
                //AddSiblingNodes(row, group, halfSiblings, left ?
                //    NodeType.SiblingLeft : NodeType.SiblingRight, scaleRelated);

                // Connections.
                foreach (var conn in personLookup.MakeChildConnections(node, connectorConverter))
                    group.Add(conn);
                foreach (var conn in personLookup.MakeChildConnections(currentSpouses.Select(a => a.To).ToList(), connectorConverter))
                    group.Add(conn.Item2);
                //foreach (var conn in personLookup.AddChildConnections(previousSpouses.Cast<INode>().ToList(), connectorConverter))
                //    group.Add(conn.Item2);
                if (left)
                    group.Reverse();
            }

            // Add connections that span across groups.
            foreach (var conn in AddSpouseConnections(parents.Cast<INode>().ToList()))
                row.Add(conn);

            return row;
        }

        /// <summary>
        /// Add marriage connections for the people specified in the
        /// list. Each marriage connection is only specified once.
        /// </summary>
        private IEnumerable<DiagramConnector> AddSpouseConnections(IList<INode> list)
        {
            // Iterate through the list.
            for (int current = 0; current < list.Count; current++)
            {
                // The person to check for marriages.
                INode person = list[current];

                // Check for current / former marriages in the rest of the list.
                for (int i = current + 1; i < list.Count; i++)
                {
                    INode spouse = list[i];

                    // Current marriage.
                    if (person.GetSpouseRelationship(spouse) is { } relationship)
                    {
                        if (personLookup.ContainsKey(person) && personLookup.ContainsKey(spouse))
                        {
                            yield return (new MarriedDiagramConnector(relationship,
                                personLookup[person], personLookup[spouse], connectorConverter));
                        }
                    }

                    // Former marriage
         
                }
            }
        }

        public IList<INode> GetParents(DiagramRow row)
        {
            return row.GetParents().ToArray();
        }

        public IList<INode> GetChildren(DiagramRow row)
        {
            return row.GetChildren().ToArray();
        }
    }
}