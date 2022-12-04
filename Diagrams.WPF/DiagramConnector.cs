/*
 * A connector consists of two nodes and a connection type. A connection has a
 * filtered state. The opacity is reduced when drawing a connection that is
 * filtered. An animation is applied to the brush when the filtered state changes.
*/

using Abstractions;
using Diagrams.WPF.Infrastructure;
using Diagrams.WPF.UI_Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diagrams.WPF
{
    /// <summary>
    /// Base class for child and married diagram connectors.
    /// </summary>
    public abstract class DiagramConnector : IDiagramConnector
    {
        private static class Const
        {
            // Filtered settings.
            public static double OpacityFiltered = 0.15;

            public static double OpacityNormal = 1.0;
            public static double AnimationDuration = 300;
        }

        #region fields

        // The two nodes that are connected.
        private DiagramConnectorNode start;

        private DiagramConnectorNode end;

        // Flag, if the connection is currently filtered. The
        // connection is drawn in a dim-state when filtered.
        private bool isFiltered;

        // Animation if the filtered state has changed.
        private DoubleAnimation? animation;

        // Pen to draw connector line.
        private Pen resourcePen;

        protected IConnectorConverter converter;
        #endregion fields

        /// <summary>
        /// Consturctor that specifies the two nodes that are connected.
        /// </summary>
        protected DiagramConnector(IRelationship relationship, DiagramConnectorNode startConnector, DiagramConnectorNode endConnector, IConnectorConverter converter)
        {
            Relationship = relationship;
            start = startConnector;
            end = endConnector;
            this.converter = converter;
        }

        /// <summary>
        /// Return true if this is a child connector.
        /// </summary>
        public virtual bool IsChildConnector
        {
            get { return true; }
        }

        public IRelationship Relationship { get; }

        /// <summary>
        /// Get the starting node.
        /// </summary>
        public DiagramConnectorNode StartNode
        {
            get { return start; }
        }

        /// <summary>
        /// Get the ending node.
        /// </summary>
        public DiagramConnectorNode EndNode
        {
            get { return end; }
        }

        public ICollection<DiagramConnectorNode> Nodes => new[] { StartNode, EndNode };

        /// <summary>
        /// Get or set the pen that specifies the connector line.
        /// </summary>
        protected Pen ResourcePen
        {
            get { return resourcePen; }
            set { resourcePen = value; }
        }

        /// <summary>
        /// Create the connector line pen. The opacity is set based on
        /// the current filtered state. The pen contains an animation
        /// if the filtered state has changed.
        /// </summary>
        protected Pen Pen
        {
            get
            {
                // Make a copy of the resource pen so it can
                // be modified, the resource pen is frozen.
                Pen connectorPen = ResourcePen.Clone();

                // Set opacity based on the filtered state.
                connectorPen.Brush.Opacity = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal;

                // Create animation if the filtered state has changed.
                if (animation != null)
                    connectorPen.Brush.BeginAnimation(Brush.OpacityProperty, animation);

                return connectorPen;
            }
        }

        public RelationshipType RelationshipType => Relationship.Type;

        ///// <summary>
        ///// Return true if the connection is currently filtered.
        ///// </summary>
        //private bool IsFiltered
        //{
        //    set { isFiltered = value; }
        //    get { return isFiltered; }
        //}

        /// <summary>
        /// Get the new filtered state of the connection. This depends
        /// on the connection nodes, marriage date and previous marriage date.
        /// </summary>
        //protected virtual bool NewFilteredState
        //{
        //    get
        //    {
        //        // Connection is filtered if any of the nodes are filtered.
        //        if (start.Node.IsFiltered || end.Node.IsFiltered)
        //            return true;

        //        // Connection is not filtered.
        //        return false;
        //    }
        //}

        /// <summary>
        /// Return true if should continue drawing, otherwise false.
        /// </summary>
        public virtual bool Draw(DrawingContext drawingContext)
        {
            // Don't draw if either of the nodes are filtered.
            if (start.Node.Visibility != Visibility.Visible ||
                end.Node.Visibility != Visibility.Visible)
                return false;

            // First check if the filtered state has changed, an animation
            // if created if the state has changed which is used for all
            // connection drawing.
            CheckIfFilteredChanged();

            return true;
        }

        /// <summary>
        /// Create the specified brush. The opacity is set based on the
        /// current filtered state. The brush contains an animation if
        /// the filtered state has changed.
        /// </summary>
        protected SolidColorBrush GetBrush(Color color)
        {
            // Create the brush.
            SolidColorBrush brush = new SolidColorBrush(color)
            {
                // Set the opacity based on the filtered state.
                Opacity = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal
            };

            // Create animation if the filtered state has changed.
            if (animation != null)
            {
                brush.BeginAnimation(Brush.OpacityProperty, animation);
            }

            return brush;
        }

        /// <summary>
        /// Determine if the filtered state has changed, and create
        /// the animation that is used to draw the connection.
        /// </summary>
        protected void CheckIfFilteredChanged()
        {
            // See if the filtered state has changed.
            bool newFiltered = converter.IsFiltered(Relationship);
            if (newFiltered != isFiltered)
            {
                // Filtered state did change, create the animation.
                isFiltered = newFiltered;
                animation = new DoubleAnimation
                {
                    From = isFiltered ? Const.OpacityNormal : Const.OpacityFiltered,
                    To = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal,
                    Duration = AnimationHelper.GetAnimationDuration(Const.AnimationDuration)
                };
            }
            else
            {
                // Filtered state did not change, clear the animation.
                animation = null;
            }
        }
    }

    /// <summary>
    /// Connector between a parent and child.
    /// </summary>
    public class ChildDiagramConnector : DiagramConnector
    {
        public ChildDiagramConnector(IRelationship connector, DiagramConnectorNode startConnector,
            DiagramConnectorNode endConnector, IConnectorConverter converter) : base(connector, startConnector, endConnector, converter)
        {
            // Get the pen that is used to draw the connection line.
            ResourcePen = (Pen)Application.Current.TryFindResource("ChildConnectionPen");
        }

        /// <summary>
        /// Draw the connection between the two nodes.
        /// </summary>
        public override bool Draw(DrawingContext drawingContext)
        {
            if (!base.Draw(drawingContext))
                return false;

            drawingContext.DrawLine(Pen, StartNode.Center, EndNode.Center);
            return true;
        }
    }

    /// <summary>
    /// Connector between spouses. Handles current and former spouses.
    /// </summary>
    public class MarriedDiagramConnector : DiagramConnector
    {
        #region fields

        // Connector line text.
        private double connectionTextSize;

        private Color connectionTextColor;
        private FontFamily connectionTextFont;

        // Flag if currently married or former.
        private bool married;

        #endregion fields

        #region properties

        /// <summary>
        /// Return true if this is a child connector.
        /// </summary>
        public override bool IsChildConnector
        {
            get { return false; }
        }

        /// <summary>
        /// Get the new filtered state of the connection. This depends
        /// on the connection nodes, marriage date and previous marriage date.
        /// Return true if the connection should be filtered.
        /// </summary>
        //protected override bool NewFilteredState
        //{
        //    get
        //    {
        //        // Check the two connected nodes.
        //        //if (base.NewFilteredState)
        //        //  return true;

        //        //// Check the married date for current and former spouses.
        //        //SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
        //        //if (rel != null && rel.MarriageDate != null &&
        //        //    (StartNode.Node.DisplayYear < rel.MarriageDate.Value.Year))
        //        //{
        //        //  return true;
        //        //}

        //        //// Check the divorce date for former spouses.
        //        //if (!married && rel != null && rel.DivorceDate != null &&
        //        //    (StartNode.Node.DisplayYear < rel.DivorceDate.Value.Year))
        //        //{
        //        //  return true;
        //        //}

        //        // Connection is not filtered.
        //        return false;
        //    }
        //}

        #endregion properties

        public MarriedDiagramConnector(IRelationship relationship,
            DiagramConnectorNode startConnector, DiagramConnectorNode endConnector, IConnectorConverter converter) :
            base(relationship, startConnector, endConnector, converter)
        {
            // Get resources used to draw text.
            connectionTextSize = (double)Application.Current.TryFindResource("ConnectionTextSize");
            connectionTextColor = (Color)Application.Current.TryFindResource("ConnectionTextColor");
            connectionTextFont = (FontFamily)Application.Current.TryFindResource("ConnectionTextFont");

            // Get resourced used to draw the connection line.
            ResourcePen = (Pen)Application.Current.TryFindResource(converter.ResourcePen(relationship));
        }

        /// <summary>
        /// Draw the connection between the two nodes.
        /// </summary>
        public override bool Draw(DrawingContext drawingContext)
        {
            if (!base.Draw(drawingContext))
                return false;

            DrawMarried(drawingContext);
            return true;
        }

        /// <summary>
        /// Draw married or previous married connector between nodes.
        /// </summary>
        private void DrawMarried(DrawingContext drawingContext)
        {
            const double TextSpace = 3;

            // Determine the start and ending points based on what node is on the left / right.
            Point startPoint = StartNode.TopCenter.X < EndNode.TopCenter.X ? StartNode.TopCenter : EndNode.TopCenter;
            Point endPoint = StartNode.TopCenter.X < EndNode.TopCenter.X ? EndNode.TopCenter : StartNode.TopCenter;

            // Use a higher arc when the nodes are further apart.
            double arcHeight = (endPoint.X - startPoint.X) / 4;
            Point middlePoint = new Point(startPoint.X + (endPoint.X - startPoint.X) / 2, startPoint.Y - arcHeight);

            // Draw the arc, get the bounds so can draw connection text.
            Rect bounds = DrawArc(drawingContext, Pen, startPoint, middlePoint, endPoint);

            // Get the relationship info so the dates can be displayed.
            //var rel = Converter.Relationship(StartNode.Node.Model, EndNode.Node.Model);

            string text = converter.Text(Relationship);

            FormattedText format = new FormattedText(text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, new Typeface(connectionTextFont,
                FontStyles.Normal, FontWeights.Normal, FontStretches.Normal,
                connectionTextFont), connectionTextSize, GetBrush(connectionTextColor));

            drawingContext.DrawText(format, new Point(
                bounds.Left + (bounds.Width / 2 - format.Width / 2),
                bounds.Top - format.Height - TextSpace));
        }

        /// <summary>
        /// Draw an arc connecting the two nodes.
        /// </summary>
        private static Rect DrawArc(DrawingContext drawingContext, Pen pen,
            Point startPoint, Point middlePoint, Point endPoint)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure
            {
                StartPoint = startPoint
            };

            figure.Segments.Add(new QuadraticBezierSegment(middlePoint, endPoint, true));
            geometry.Figures.Add(figure);
            drawingContext.DrawGeometry(null, pen, geometry);
            return geometry.Bounds;
        }
    }
}