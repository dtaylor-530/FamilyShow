using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Diagram.Logic
{
    public class NodeConverter : INodeConverter
    {
        public NodeConverter()
        {
        }

        public bool IsFiltered(object obj, double displayYear)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }

            return person.BirthDate != null && person.BirthDate.Value.Year > displayYear;
        }

        public string DateInformation(object obj, double displayYear)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }
            // Living, example: 1900 | 107
            if (person.IsLiving)
            {
                if (person.BirthDate == null)
                {
                    return string.Empty;
                }

                if (!person.Age.HasValue)
                {
                    return string.Empty;
                }

                int age = person.Age.Value - (DateTime.Now.Year - (int)displayYear);
                return string.Format(CultureInfo.CurrentUICulture, "{0} | {1}", person.BirthDate.Value.Year, Math.Max(0, age));
            }

            // Deceased, example: 1900 - 1950 | 50
            if (person.BirthDate != null && person.DeathDate != null)
            {
                if (!person.Age.HasValue)
                {
                    return string.Empty;
                }

                int age = (displayYear >= person.DeathDate.Value.Year) ?
                    person.Age.Value : person.Age.Value - (person.DeathDate.Value.Year - (int)displayYear);

                return string.Format(CultureInfo.CurrentUICulture,
                    "{0} - {1} | {2}", person.BirthDate.Value.Year,
                    person.DeathDate.Value.Year, Math.Max(0, age));
            }

            // Deceased, example: ? - 1950 | ?
            if (person.BirthDate == null && person.DeathDate != null)
            {
                return string.Format(CultureInfo.CurrentUICulture,
                    "? - {0} | ?", person.DeathDate.Value.Year);
            }

            // Deceased, example: 1900 - ? | ?
            if (person.BirthDate != null && person.DeathDate == null)
            {
                return string.Format(CultureInfo.CurrentUICulture,
                    "{0} - ? | ?", person.BirthDate.Value.Year);
            }
            throw new Exception("weasfd  sdsdd");
        }

        public string NodeTemplate(object obj, NodeType type)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }
            // Determine the node template based on node properties.
            string template = string.Format(
            CultureInfo.InvariantCulture, "{0}{1}NodeTemplate",
            (person.Gender == Gender.Female) ? "Female" : "Male",
            (type == NodeType.Primary) ? "Primary" : "");
            return template;
        }

        public DateTime? MinimumDate(object obj)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }
            DateTime? date = person.BirthDate;

            return date;
        }

        public string BrushResource(object obj, NodeType type, string part)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }
            string resourceName = string.Format(
        CultureInfo.InvariantCulture, "{0}{1}{2}{3}",
        (person.Gender == Gender.Female) ? "Female" : "Male",
        type.ToString(),
        person.IsLiving ? "Living" : "Deceased",
        part);
            return resourceName;
        }

        public string GroupBrushResource(object obj, NodeType type, string part)
        {
            // Format string, the resource is in the XAML file.
            string resourceName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", type.ToString(), part);
            return resourceName;
        }

        public void Scale(Control control, double scale)
        {
            // The template has been applied to the node. See if the person drawing needs to be scaled.
            if (scale != 1)
            {
                // Scale the person drawing part of the node, not the entire node.
                if (control.Template.FindName("Person", control) is FrameworkElement personElement)
                {
                    ScaleTransform transform = new(scale, scale);
                    personElement.LayoutTransform = transform;
                }
            }
        }

        /// <summary>
        /// Hide or show the group indicator for this node.
        /// </summary>
        public void UpdateGroupIndicator(object obj, Control control, NodeType type)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }

            // Primary templates don't have the group xaml section.
            if (type == NodeType.Primary)
            {
                return;
            }

            // Determine if the group indicator should be displayed.
            bool isGrouping = ShouldDisplayGroupIndicator(type);

            if (control.Template.FindName("Group", control) is FrameworkElement element)
            {
                element.Visibility = isGrouping ? Visibility.Visible : Visibility.Collapsed;
            }

            bool ShouldDisplayGroupIndicator(NodeType nodeType)
            {
                switch (nodeType)
                {
                    // Primary and related nodes never display the group indicator.
                    case NodeType.Primary:
                    case NodeType.Related:
                        return false;
                    // Spouse - if have parents, siblings, or ex spouses.
                    case NodeType.Spouse when (person.Parents.Count() > 0 || person.FullSiblings.Count() > 0 || person.PreviousSpouses.Count() > 0):
                        return true;
                    // Sibling - if have spouse, or children.
                    case NodeType.Sibling when (person.Spouses.Count() > 0 || person.Children.Count() > 0):
                        return true;
                    // Half sibling - like sibling, but also inherits the
                    // group status from all parents.
                    case NodeType.SiblingLeft when (person.Spouses.Count() > 0 || person.Children.Count() > 0):
                    case NodeType.SiblingRight when (person.Spouses.Count() > 0 || person.Children.Count() > 0):

                        return true;
                }

                return false;
            }
        }

        public string BottomLabel(object obj, double displayYear)
        {
            if (obj is not Person { } person)
            {
                throw new Exception("dsgd ,,f");
            }
            string label = string.Format(CultureInfo.CurrentCulture, "{0}\r{1}", person.FullName,
               DateInformation(obj, displayYear));
            return label;
        }
    }
}