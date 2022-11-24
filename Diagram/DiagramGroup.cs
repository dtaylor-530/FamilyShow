/*
 * One group in a row. Contains a collection of DiagramNode objects
 * that are arranged within the group.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    /// <summary>
    /// Group in a row that contains node objects.
    /// </summary>
    public class DiagramGroup : FrameworkElement
    {
        #region fields

        // Space between each node.
        private const double NodeSpace = 10;

        // Location of the group, relative to the row.
        private Point location = new Point();

        // List of nodes in the group.
        private List<DiagramNode> nodes = new List<DiagramNode>();

        private List<DiagramConnector> connectors = new List<DiagramConnector>();

        #endregion fields

        /// <summary>
        /// Location of the group, relative to the row.
        /// </summary>
        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// List of nodes in the group.
        /// </summary>
        public ReadOnlyCollection<DiagramNode> Nodes
        {
            get { return new ReadOnlyCollection<DiagramNode>(nodes); }
        }

        /// <summary>
        /// List of nodes in the group.
        /// </summary>
        public ReadOnlyCollection<DiagramConnector> Connectors
        {
            get { return new ReadOnlyCollection<DiagramConnector>(connectors); }
        }

        #region overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            // Let each node determine how large they want to be.
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (DiagramNode node in nodes)
                node.Measure(size);

            // Return the total size of the group.
            return ArrangeNodes(false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Arrange the nodes in the group, return the total size.
            return ArrangeNodes(true);
        }

        protected override int VisualChildrenCount
        {
            // Return the number of nodes.
            get { return nodes.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            // Return the requested node.
            return nodes[index];
        }

        #endregion overrides

        /// <summary>
        /// Add the node to the group.
        /// </summary>
        public void Add(DiagramNode node)
        {
            nodes.Add(node);
            AddVisualChild(node);
        }

        public void Add(DiagramConnector conn)
        {
            connectors.Add(conn);
        }

        public void Add(IEnumerable<ChildDiagramConnector> item2)
        {
            connectors.AddRange(item2);
        }

        /// <summary>
        /// Remove all nodes from the group.
        /// </summary>
        public void Clear()
        {
            foreach (DiagramNode node in nodes)
                RemoveVisualChild(node);

            nodes.Clear();
            connectors.Clear();
        }

        /// <summary>
        /// Reverse the order of the nodes.
        /// </summary>
        public void Reverse()
        {
            nodes.Reverse();
        }

        /// <summary>
        /// Arrange the nodes in the group, return the total size.
        /// </summary>
        private Size ArrangeNodes(bool arrange)
        {
            // Position of the next node.
            double pos = 0;

            // Total size of the group.
            Size totalSize = new(0, 0);

            foreach (DiagramNode node in nodes)
            {
                // Bounding area of the node.
                Rect bounds = new()
                {
                    // Node location.
                    X = pos,
                    Y = 0,
                    // Node size.
                    Width = node.DesiredSize.Width,
                    Height = node.DesiredSize.Height
                };

                NewMethod(arrange, pos, bounds, ref totalSize, node);

                pos += (bounds.Width + NodeSpace);
            }

            return totalSize;
        }

        private static void NewMethod(bool arrange, double pos, Rect bounds, ref Size totalSize, DiagramNode node)
        {
            // Arrange the node, save the location.
            if (arrange)
            {
                node.Arrange(bounds);
                node.Location = bounds.TopLeft;
            }

            // Update the size of the group.
            totalSize.Width = pos + node.DesiredSize.Width;
            totalSize.Height = Math.Max(totalSize.Height, node.DesiredSize.Height);
        }
    }
}