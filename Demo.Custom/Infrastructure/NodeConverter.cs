using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using Models;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace Demo.Custom.Infrastructure
{
    public class NodeConverter : INodeConverter
    {
        public NodeConverter()
        {
        }

        public bool IsFiltered(object obj, double displayYear)
        {
            if (obj is not Model { } model)
            {
                throw new Exception("dsgd ,,f");
            }

            return model.Created.Year > displayYear;
        }

        public string DateInformation(object obj, double displayYear)
        {
            if (obj is not Model { } model)
            {
                throw new Exception("dsgd ,,f");
            }

            return "This is date information";
        }

        public string NodeTemplate(object obj, NodeType type)
        {
            if (obj is not Model { } model)
            {
                throw new Exception("dsgd ,,f");
            }
            // Determine the node template based on node properties.
            string template = "StackPanel";
            return template;
        }

        public DateTime? MinimumDate(object obj)
        {
            if (obj is not Model { } model)
            {
                throw new Exception("dsgd ,,f");
            }
            DateTime? date = model.Created;

            return date;
        }

        public string BrushResource(object obj, NodeType type, string part)
        {
            return "PressedBrush";
        }

        public string GroupBrushResource(object obj, NodeType type, string part)
        {
            // Format string, the resource is in the XAML file.
            string resourceName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", type.ToString(), part);
            return resourceName;
        }

        public void Scale(Control control, double value)
        {
        }

        public void UpdateGroupIndicator(object obj, Control control, NodeType type)
        {
        }

        public string BottomLabel(object obj, double displayYear)
        {
            return "Bottom Label";
        }
    }
}