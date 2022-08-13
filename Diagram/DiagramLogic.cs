/*
 * Contains the logic to populate the diagram. Populates rows with 
 * groups and nodes based on the node relationships. 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using Microsoft.FamilyShowLib;
using Diagram.Logic;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public class DiagramLogic : IDiagramLogic
    {  
        #region fields

        //// List of the connections, specify connections between two nodes.
        //private List<DiagramConnector> connections = new List<DiagramConnector>();

        // Map that allows quick lookup of a Person object to connection information.
        // Used when setting up the connections between nodes.
        
        private Dictionary<object, DiagramConnectorNode> personLookup;

        // List of people, global list that is shared by all objects in the application.
        private CurrentCollection family;
        private readonly IDiagramFactory factory;

        // Callback when a node is clicked.
        //private EventHandler nodeClickHandler;

        // Filter year for nodes and connectors.
        private double displayYear;
        //private DiagramFactory factory;

        #endregion

        public DiagramLogic(CurrentCollection family, IDiagramFactory factory, Dictionary<object, DiagramConnectorNode> personLookup)
        {
            // The list of people, this is a global list shared by the application.
            this.family = family;
            this.personLookup = personLookup;
            this.factory = factory;
            Clear();
            factory.CurrentNode += Factory_CurrentNode;
            family.CollectionChanged += Family_CollectionChanged;
        }

        private void Family_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ContentChanged.Invoke(this, new ContentChangedEventArgs(Family.Current));
        }

        private void Factory_CurrentNode(object obj)
        {
            Current = obj;
        }


        /// <summary>
        /// Sets the callback that is called when a node is clicked.
        /// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        //public EventHandler NodeClickHandler
        //{
        //    get => nodeClickHandler;
        //    set { nodeClickHandler = value; }
        //}

        /// <summary>
        /// Gets the list of people in the family.
        /// </summary>
        public CurrentCollection Family => family;



        /// <summary>
        /// Gets the person lookup list. This includes all of the 
        /// people and nodes that are displayed in the diagram.
        /// </summary>
        //public Dictionary<object, DiagramConnectorNode> PersonLookup => personLookup;

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
    

        public EventHandler<ContentChangedEventArgs> ContentChanged { get; set; }
        public EventHandler CurrentChanged { get; set; }
        public object Current { get; set; }
 
        /// <summary>
        /// Clear 
        /// </summary>
        public void Clear()
        {
            // Remove any event handlers from the nodes. Otherwise 
            // the delegate maintains a reference to the object 
            // which can hinder garbage collection. 
            foreach (DiagramConnectorNode node in personLookup.Values)
                factory.DestroyNode(node.Node);

            // Clear the connection info.
            //connections.Clear();
            personLookup.Clear();

            // Time filter.
            displayYear = DateTime.Now.Year;
        }

        /// <summary>
        /// Return the DiagramNode for the specified Person.
        /// </summary>


        public DiagramConnectorNode? GetDiagramConnectorNode(object person)
        {
            if (person == null)
                return null;

            if (!personLookup.ContainsKey(person))
                return null;

            return personLookup[person];
        }

        public IEnumerable<DiagramRow> GenerateRows()
        {
            // Necessary for Blend.
            if (Family == null)
                yield break;

            // Nothing to draw if there is not a primary person.
            if (Family.Current == null)
                yield break;

            // Primary row.
            var primaryPerson = Family.Current;
            DiagramRow primaryRow = factory.CreatePrimaryRow(primaryPerson, 1.0, Diagram.Const.RelatedMultiplier);
            primaryRow.GroupSpace = Diagram.Const.PrimaryRowGroupSpace;
            //diagram.AddRow(primaryRow);
            yield return primaryRow;
            // Create as many rows as possible until exceed the max node limit.
            // Switch between child and parent rows to prevent only creating
            // child or parents rows (want to create as many of each as possible).
            int nodeCount = personLookup.Count;

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
                    var children = factory.GetChildren(childRow); 
                    if (children.Count != 0)
                    {
                        // Add bottom space to existing row.
                        childRow.Margin = new Thickness(0, 0, 0, Diagram.Const.RowSpace);

                        // Add another row.
                        DiagramRow grandChildRow = factory.CreateChildrenRow(children, 1.0, Diagram.Const.RelatedMultiplier);
                        grandChildRow.GroupSpace = Diagram.Const.ChildRowGroupSpace;
                        childRow = grandChildRow;
                        //diagram.AddRow(grandChildRow);
                        yield return grandChildRow;
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
                    IList<object> grandParents = factory.GetParents(parentRow);
                    if (grandParents.Count != 0)
                    {
                        // Add another row.
                        DiagramRow grandParentRow = factory.CreateParentRow(grandParents, nodeScale, nodeScale * Diagram.Const.RelatedMultiplier);
                         

                        grandParentRow.Margin = new Thickness(0, 0, 0, Diagram.Const.RowSpace);
                        grandParentRow.GroupSpace = Diagram.Const.ParentRowGroupSpace;
                        parentRow = grandParentRow;
                        //diagram.InsertRow(grandParentRow);
                        yield return grandParentRow;

                    }
                    else
                    {
                        parentRow = null;
                    }
                }

                // See if reached node limit yet.                                       
                nodeCount = personLookup.Count;
            }
        }
    }
}
