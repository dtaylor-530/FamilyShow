/*
 * Contains the logic to populate the diagram. Populates rows with 
 * groups and nodes based on the node relationships. 
*/

using Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Diagram.Logic;
using Microsoft.FamilyShowLib;
using System.Linq;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public class DiagramLogic : IDiagramLogic
    {
        public void UpdateDiagram(Diagram diagram)
        {
            // Necessary for Blend.
            if (Family == null)
                return;

            // First reset everything.
            diagram.Clear();

            // Nothing to draw if there is not a primary person.
            if (Family.Current == null)
                return;

            // Primary row.
            INode primaryPerson = Family.Current;
            DiagramRow primaryRow = CreatePrimaryRow(primaryPerson, 1.0, Diagram.Const.RelatedMultiplier);
            primaryRow.GroupSpace = Diagram.Const.PrimaryRowGroupSpace;
            diagram.InvalidateVisual();
            diagram.AddRow(primaryRow);

            // Create as many rows as possible until exceed the max node limit.
            // Switch between child and parent rows to prevent only creating
            // child or parents rows (want to create as many of each as possible).
            int nodeCount = diagram.NodeCount;

            // The scale values of future generations, this makes the nodes
            // in each row slightly smaller.
            double nodeScale = 1.0;

            DiagramRow? childRow = primaryRow;
            DiagramRow? parentRow = primaryRow;

            while (nodeCount < Diagram.Const.MaximumNodes && (childRow != null || parentRow != null))
            {
                // Child Row.
                if (childRow != null)
                {
                    // Get list of children for the current row.
                    var children = childRow.GetChildren();
                    if (children.Count != 0)
                    {
                        // Add bottom space to existing row.
                        childRow.Margin = new Thickness(0, 0, 0, Diagram.Const.RowSpace);

                        // Add another row.
                        DiagramRow grandChildRow = CreateChildrenRow(children, 1.0, Diagram.Const.RelatedMultiplier);
                        grandChildRow.GroupSpace = Diagram.Const.ChildRowGroupSpace;
                        childRow = grandChildRow;
                        diagram.AddRow(grandChildRow);
                    }
                    else
                    {
                        childRow = null;
                    }
                }

                // Parent row.
                if (parentRow != null)
                {
                    nodeScale *= Diagram.Const.GenerationMultiplier;

                    // Get list of parents for the current row.
                    IList<INode> grandParents = parentRow.GetParents();
                    if (grandParents.Count != 0)
                    {
                        // Add another row.
                        DiagramRow grandParentRow = CreateParentRow(grandParents, nodeScale, nodeScale * Diagram.Const.RelatedMultiplier);

                        grandParentRow.Margin = new Thickness(0, 0, 0, Diagram.Const.RowSpace);
                        grandParentRow.GroupSpace = Diagram.Const.ParentRowGroupSpace;
                        parentRow = grandParentRow;
                        diagram.InsertRow(grandParentRow);

                    }
                    else
                    {
                        parentRow = null;
                    }
                }

                // See if reached node limit yet.                                       
                nodeCount = diagram.NodeCount;
            }

            // Raise event so others know the diagram was updated.
            diagram.OnDiagramUpdated();

            // Animate the new person (optional, might not be any new people).
            diagram.AnimateNewPerson();
        }




        #region fields

        // List of the connections, specify connections between two nodes.
        private List<DiagramConnector> connections = new List<DiagramConnector>();

        // Map that allows quick lookup of a Person object to connection information.
        // Used when setting up the connections between nodes.
        private Dictionary<object, DiagramConnectorNode> personLookup =
            new Dictionary<object, DiagramConnectorNode>();

        // List of people, global list that is shared by all objects in the application.
        private PeopleCollection<INode> family;

        // Callback when a node is clicked.
        private EventHandler nodeClickHandler;

        // Filter year for nodes and connectors.
        private double displayYear;

        #endregion

        #region properties

        /// <summary>
        /// Sets the callback that is called when a node is clicked.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public EventHandler NodeClickHandler
        {
            get => nodeClickHandler;
            set { nodeClickHandler = value; }
        }

        /// <summary>
        /// Gets the list of people in the family.
        /// </summary>
        public PeopleCollection<INode> Family
        {
            get { return family; }
        }

        /// <summary>
        /// Gets the list of connections between nodes.
        /// </summary>
        public List<DiagramConnector> Connections
        {
            get { return connections; }
        }

        /// <summary>
        /// Gets the person lookup list. This includes all of the 
        /// people and nodes that are displayed in the diagram.
        /// </summary>
        public Dictionary<object, DiagramConnectorNode> PersonLookup
        {
            get { return personLookup; }
        }

        /// <summary>
        /// Sets the year filter that filters nodes and connectors.
        /// </summary>
        public double DisplayYear
        {
            set
            {
                if (displayYear != value)
                {
                    displayYear = value;
                    foreach (DiagramConnectorNode connectorNode in personLookup.Values)
                        connectorNode.Node.DisplayYear = displayYear;
                }
            }
        }

        /// <summary>
        /// Gets the minimum year in all nodes and connectors.
        /// </summary>
        public double MinimumYear
        {
            get
            {
                // Init to current year.
                double minimumYear = DateTime.Now.Year;

                // Check birth years.
                foreach (DiagramConnectorNode connectorNode in personLookup.Values)
                {
                    DateTime? date = (connectorNode.Node.Person as INode).BirthDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);
                }

                // Check marriage years.
                foreach (DiagramConnector connector in connections)
                {
                    // Marriage date.
                    DateTime? date = connector.MarriedDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);

                    // Previous marriage date.
                    date = connector.PreviousMarriedDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);
                }

                return minimumYear;
            }
        }
        public EventHandler<ContentChangedEventArgs> ContentChanged { get; set; }
        public EventHandler CurrentChanged { get; set; }
        public object Current { get; set; }


        #endregion

        public DiagramLogic(PeopleCollection<INode> family)
        {
            // The list of people, this is a global list shared by the application.
            this.family = family;
            Clear();
        }



        /// <summary>
        /// Create a DiagramNode.
        /// </summary>
        private DiagramNode CreateNode(object person, NodeType type, bool clickEvent, double scale)
        {
            DiagramNode node = CreateNode(person, type, clickEvent);
            node.Scale = scale;
            return node;
        }

        /// <summary>
        /// Create a DiagramNode.
        /// </summary>
        private DiagramNode CreateNode(object person, NodeType type, bool clickEvent)
        {
            DiagramNode node = new DiagramNode();
            node.Person = person;
            node.Type = type;
            if (clickEvent)
                node.Click += (s, e) => nodeClickHandler(s, e);

            return node;
        }

        /// <summary>
        /// Add the siblings to the specified row and group.
        /// </summary>
        private void AddSiblingNodes(DiagramRow row, DiagramGroup group,
            IList<INode> siblings, NodeType nodeType, double scale)
        {
            foreach (INode sibling in siblings)
            {
                if (!personLookup.ContainsKey(sibling))
                {
                    // Siblings node.
                    DiagramNode node = CreateNode(sibling, nodeType, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Person as INode, new DiagramConnectorNode(node, group, row));
                }
            }
        }

        /// <summary>
        /// Add the spouses to the specified row and group.
        /// </summary>
        private void AddSpouseNodes(INode person, DiagramRow row,
            DiagramGroup group, IList<INode> spouses,
            NodeType nodeType, double scale, bool married)
        {
            foreach (INode spouse in spouses)
            {
                if (!personLookup.ContainsKey(spouse))
                {
                    // Spouse node.
                    DiagramNode node = CreateNode(spouse, nodeType, true, scale);
                    group.Add(node);

                    // Add connection.
                    DiagramConnectorNode connectorNode = new DiagramConnectorNode(node, group, row);
                    personLookup.Add(node.Person as INode, connectorNode);
                    connections.Add(new MarriedDiagramConnector(married, personLookup[person], connectorNode));
                }
            }
        }



        /// <summary>
        /// Creates the primary row. The row contains groups: 1) The primary-group 
        /// that only contains the primary node, and 2) The optional left-group 
        /// that contains spouses and siblings.
        /// </summary>
        public DiagramRow CreatePrimaryRow(INode person, double scale, double scaleRelated)
        {
            // The primary node contains two groups, 
            DiagramGroup primaryGroup = new DiagramGroup();
            DiagramGroup leftGroup = new DiagramGroup();

            // Set up the row.
            DiagramRow row = new DiagramRow();

            // Add primary node.
            DiagramNode node = CreateNode(person, NodeType.Primary, false, scale);
            primaryGroup.Add(node);
            personLookup.Add(node.Person as INode, new DiagramConnectorNode(node, primaryGroup, row));

            // Current spouses.
            IList<INode> currentSpouses = person.CurrentSpouses.ToList();
            AddSpouseNodes(person, row, leftGroup, currentSpouses,
                NodeType.Spouse, scaleRelated, true);

            // Previous spouses.
            IList<INode> previousSpouses = person.PreviousSpouses.ToList(); 
            AddSpouseNodes(person, row, leftGroup, previousSpouses,
                NodeType.Spouse, scaleRelated, false);

            // Siblings.
            IList<INode> siblings = person.Siblings.ToList();
            AddSiblingNodes(row, leftGroup, siblings, NodeType.Sibling, scaleRelated);

            // Half siblings.
            IList<INode> halfSiblings = person.HalfSiblings.ToList();
            AddSiblingNodes(row, leftGroup, halfSiblings, NodeType.SiblingLeft, scaleRelated);

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
            DiagramRow row = new DiagramRow();

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
                    personLookup.Add(node.Person as INode, new DiagramConnectorNode(node, group, row));
                }

                // Current spouses.
                IList<INode> currentSpouses = child.CurrentSpouses.ToList();
                AddSpouseNodes(child, row, group, currentSpouses,
                    NodeType.Spouse, scaleRelated, true);

                // Previous spouses.
                IList<INode> previousSpouses = child.PreviousSpouses.ToList();
                AddSpouseNodes(child, row, group, previousSpouses,
                    NodeType.Spouse, scaleRelated, false);

                // Connections.
                this.AddParentConnections(child);

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
            DiagramRow row = new DiagramRow();

            int groupCount = 0;

            foreach (INode person in parents)
            {
                // Each parent is in their group, the group contains the parent,
                // spouses and siblings.
                DiagramGroup group = new DiagramGroup();
                row.Add(group);

                // Determine if this is a left or right oriented group.
                bool left = (groupCount++ % 2 == 0) ? true : false;

                // Parent.
                if (!personLookup.ContainsKey(person))
                {
                    DiagramNode node = CreateNode(person, NodeType.Related, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Person as INode, new DiagramConnectorNode(node, group, row));
                }

                // Current spouses.
                IList<INode> currentSpouses = person.CurrentSpouses.ToList();
                DiagramHelper.RemoveDuplicates(currentSpouses, parents);
                AddSpouseNodes(person, row, group, currentSpouses,
                    NodeType.Spouse, scaleRelated, true);

                // Previous spouses.
                IList<INode> previousSpouses = person.PreviousSpouses.ToList();
                previousSpouses.RemoveDuplicates(parents);
                AddSpouseNodes(person, row, group, previousSpouses,
                    NodeType.Spouse, scaleRelated, false);

                // Siblings.
                IList<INode> siblings = person.Siblings.ToList();
                AddSiblingNodes(row, group, siblings, NodeType.Sibling, scaleRelated);

                // Half siblings.
                IList<INode> halfSiblings = person.HalfSiblings.ToList();
                AddSiblingNodes(row, group, halfSiblings, left ?
                    NodeType.SiblingLeft : NodeType.SiblingRight, scaleRelated);

                // Connections.
                this.AddChildConnections(person);
                this.AddChildConnections(currentSpouses);
                this.AddChildConnections(previousSpouses);

                if (left)
                    group.Reverse();
            }

            // Add connections that span across groups.
            AddSpouseConnections(parents);

            return row;
        }


        /// <summary>
        /// Add marriage connections for the people specified in the 
        /// list. Each marriage connection is only specified once.
        /// </summary>
        private void AddSpouseConnections(IList<INode> list)
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
                    IRelationship rel = person.GetSpouseRelationship(spouse);

                    // Current marriage.
                    if (rel != null && rel.SpouseModifier == SpouseModifier.Current)
                    {
                        if (personLookup.ContainsKey(person) &&
                            personLookup.ContainsKey(spouse))
                        {
                            connections.Add(new MarriedDiagramConnector(true,
                                personLookup[person], personLookup[spouse]));
                        }
                    }

                    // Former marriage
                    if (rel != null && rel.SpouseModifier == SpouseModifier.Former)
                    {
                        if (personLookup.ContainsKey(person) &&
                            personLookup.ContainsKey(spouse))
                        {
                            connections.Add(new MarriedDiagramConnector(false,
                                personLookup[person], personLookup[spouse]));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Clear 
        /// </summary>
        public void Clear()
        {
            // Remove any event handlers from the nodes. Otherwise 
            // the delegate maintains a reference to the object 
            // which can hinder garbage collection. 
            foreach (DiagramConnectorNode node in personLookup.Values)
                node.Node.Click -= (s, e) => nodeClickHandler(s, e);

            // Clear the connection info.
            connections.Clear();
            personLookup.Clear();

            // Time filter.
            displayYear = DateTime.Now.Year;
        }

        /// <summary>
        /// Return the DiagramNode for the specified Person.
        /// </summary>
        public DiagramNode GetDiagramNode(object person)
        {
            if (person == null)
                return null;

            if (!personLookup.ContainsKey(person))
                return null;

            return personLookup[person].Node;
        }

        /// <summary>
        /// Return the bounds (relative to the diagram) for the specified person.
        /// </summary>
        public Rect GetNodeBounds(object person)
        {
            Rect bounds = Rect.Empty;
            if (person != null && personLookup.ContainsKey(person))
            {
                DiagramConnectorNode connector = personLookup[person];
                bounds = new Rect(connector.TopLeft.X, connector.TopLeft.Y,
                    connector.Node.ActualWidth, connector.Node.ActualHeight);
            }

            return bounds;
        }
    }
}
