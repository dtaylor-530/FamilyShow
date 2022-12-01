/*
 * A connector consists of two nodes and a connection type. A connection has a
 * filtered state. The opacity is reduced when drawing a connection that is
 * filtered. An animation is applied to the brush when the filtered state changes.
*/

using System.Windows;

namespace Diagrams.WPF
{
    /// <summary>
    /// One of the nodes in a connection.
    /// </summary>
    public class DiagramConnectorNode
    {
        #region fields

        // Node location in the diagram.
        private DiagramRow row;

        private DiagramGroup group;
        private DiagramNode node;

        #endregion fields

        #region properties

        /// <summary>
        /// Node for this connection point.
        /// </summary>
        public DiagramNode Node
        {
            get { return node; }
        }

        /// <summary>
        /// Center of the node relative to the diagram.
        /// </summary>
        public Point Center
        {
            get { return GetPoint(node.Limits.Center(node)); }
        }

        /// <summary>
        /// LeftCenter of the node relative to the diagram.
        /// </summary>
        public Point LeftCenter
        {
            get { return GetPoint(node.Limits.LeftCenter(node)); }
        }

        /// <summary>
        /// RightCenter of the node relative to the diagram.
        /// </summary>
        public Point RightCenter
        {
            get { return GetPoint(node.Limits.RightCenter(node)); }
        }

        /// <summary>
        /// TopCenter of the node relative to the diagram.
        /// </summary>
        public Point TopCenter
        {
            get { return GetPoint(node.Limits.TopCenter(node)); }
        }

        /// <summary>
        /// TopRight of the node relative to the diagram.
        /// </summary>
        public Point TopRight
        {
            get { return GetPoint(node.Limits.TopRight(node)); }
        }

        /// <summary>
        /// TopLeft of the node relative to the diagram.
        /// </summary>
        public Point TopLeft
        {
            get { return GetPoint(node.Limits.TopLeft(node)); }
        }

        #endregion properties

        public DiagramConnectorNode(DiagramNode node, DiagramGroup group, DiagramRow row)
        {
            this.node = node;
            this.group = group;
            this.row = row;
        }

        /// <summary>
        /// Return the point shifted by the row and group location.
        /// </summary>
        private Point GetPoint(Point point)
        {
            point.Offset(
                row.Location.X + group.Location.X,
                row.Location.Y + group.Location.Y);

            return point;
        }
    }
}