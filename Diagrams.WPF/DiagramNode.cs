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

//using Abstractions;
using Abstractions;
using Diagrams.WPF.Infrastructure;
using Diagrams.WPF.UI_Infrastructure;
using Microsoft.FamilyShow;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diagrams.WPF
{
    /// <summary>
    /// Node in the diagram.
    /// </summary>
    public partial class DiagramNode : Button
    {
        public static class Const
        {
            public static double OpacityFiltered = 0.15;
            public static double OpacityNormal = 1.0;
            public static double AnimationDuration = 300;
            public static double Scale = 1;
        }

        public static readonly DependencyProperty BottomLabelProperty = DependencyProperty.Register("BottomLabel", typeof(string), typeof(DiagramNode));

        #region fields

        // Person object associated with the node.
        private INode model;

        // The type of node.
        private NodeType type = NodeType.Related;
        private readonly INodeConverter converter;

        // The amount to scale the node.
        private double scale = Const.Scale;

        //// The current display year, this is used for the time filter.
        //private double displayYear = DateTime.Now.Year;

        // Flag, true if this node is currently filtered. This means
        // its still displayed but in a dim state.
        private bool isFiltered;

        private Point location;

        #endregion fields

        public DiagramNode(INode model, NodeType type, INodeConverter converter, INodeLimits nodeLimits)
        {
            this.model = model;
            this.type = type;
            this.converter = converter;
            Limits = nodeLimits;
            DataContext = this;

            // Update the template to reflect the gender.
            UpdateTemplate();
            UpdateLabel();
        }

        public INode Model => model;

        //public INodeConverter Converter { get; set; }

        public INodeLimits Limits { get; }

        /// <summary>
        /// Set the scale value of the node.
        /// </summary>
        public double Scale
        {
            get => scale;
            set
            {
                // Save the scale value, used later after apply the node template.
                scale = value;
            }
        }

        /// <summary>
        /// Get the fill brush for the node based on the node type.
        /// </summary>
        public Brush NodeFill => GetBrushResource("Fill");

        /// <summary>
        /// Get the hover fill brush for the node based on the node type.
        /// </summary>
        public Brush NodeHoverFill => GetBrushResource("HoverFill");

        /// <summary>
        /// Get the stroke brush for the node based on the node type.
        /// </summary>
        public Brush NodeStroke => GetBrushResource("Stroke");

        /// <summary>
        /// Get the fill brush for the group indicator.
        /// </summary>
        public Brush GroupFill => GetGroupBrushResource("GroupFill");

        /// <summary>
        /// Get the stroke brush for the group indicator.
        /// </summary>
        public Brush GroupStroke => GetGroupBrushResource("GroupStroke");

        /// <summary>
        /// Location of the node relative to the parent group.
        /// </summary>
        public Point Location
        {
            get => location;
            set => location = value;
        }

        /// <summary>
        /// Get or set the display year. This filters the node based on date information.
        /// </summary>
        //public double DisplayYear
        //{
        //    get { return displayYear; }
        //    set
        //    {
        //        displayYear = value;

        //        // Update the filtered state based on the birth date.
        //        IsFiltered = Converter.IsFiltered(Model);

        //        // Recompuate the bottom label which contains the age,
        //        // the new age is relative to the new display year
        //        UpdateBottomLabel();
        //    }
        //}

        public void Refresh()
        {
            IsFiltered = converter.IsFiltered(Model);
            UpdateLabel();
        }

        /// <summary>
        /// Get or set if the node is filtered.
        /// </summary>
        public bool IsFiltered
        {
            get { return isFiltered; }
            set
            {
                if (isFiltered != value)
                {
                    // The filtered state changed, create a new animation.
                    isFiltered = value;
                    double newOpacity = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal;
                    BeginAnimation(OpacityProperty,
                        new DoubleAnimation(Opacity, newOpacity,
                        AnimationHelper.GetAnimationDuration(Const.AnimationDuration)));
                }
            }
        }


        /// <summary>
        /// The type of node.
        /// </summary>
        public NodeType Type => type;

        #region dependency properties

        /// <summary>
        /// The text displayed below the node.
        /// </summary>
        public string BottomLabel
        {
            get { return (string)GetValue(BottomLabelProperty); }
            set { SetValue(BottomLabelProperty, value); }
        }

        #endregion dependency properties

        public override void OnApplyTemplate()
        {
            converter.Scale(this, Scale);

            // The template changed, determine if the group
            // indicator should be displayed.
            converter.UpdateGroupIndicator(Model, this, type);

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Return the brush resouse based on the node type.
        /// </summary>
        private Brush GetBrushResource(string part) => (Brush)TryFindResource(converter.BrushResource(Model, type, part));

        private Brush GetGroupBrushResource(string part) => (Brush)TryFindResource(converter.GroupBrushResource(Model, type, part));

        /// <summary>
        /// Update the node template based on the node type.
        /// </summary>
        private void UpdateTemplate()
        {
            // Assign the node template.
            Template = (ControlTemplate)FindResource(converter.NodeTemplate(Model, Type));
        }

        /// <summary>
        /// Update the bottom label which contains the name, year range and age.
        /// </summary>
        private void UpdateLabel()
        {
            BottomLabel = converter.Text(Model);
        }
    }
}