using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using Models;
using System;
using System.Globalization;
using System.Linq;
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
            //// Living, example: 1900 | 107
            //if (Model.IsLiving)
            //{
            //    if (Model.BirthDate == null)
            //    {
            //        return string.Empty;
            //    }

            //    if (!Model.Age.HasValue)
            //    {
            //        return string.Empty;
            //    }

            //    int age = Model.Age.Value - (DateTime.Now.Year - (int)displayYear);
            //    return string.Format(CultureInfo.CurrentUICulture, "{0} | {1}", Model.BirthDate.Value.Year, Math.Max(0, age));
            //}

            //// Deceased, example: 1900 - 1950 | 50
            //if (Model.BirthDate != null && Model.DeathDate != null)
            //{
            //    if (!Model.Age.HasValue)
            //    {
            //        return string.Empty;
            //    }

            //    int age = (displayYear >= Model.DeathDate.Value.Year) ?
            //        Model.Age.Value : Model.Age.Value - (Model.DeathDate.Value.Year - (int)displayYear);

            //    return string.Format(CultureInfo.CurrentUICulture,
            //        "{0} - {1} | {2}", Model.BirthDate.Value.Year,
            //        Model.DeathDate.Value.Year, Math.Max(0, age));
            //}

            //// Deceased, example: ? - 1950 | ?
            //if (Model.BirthDate == null && Model.DeathDate != null)
            //{
            //    return string.Format(CultureInfo.CurrentUICulture,
            //        "? - {0} | ?", Model.DeathDate.Value.Year);
            //}

            //// Deceased, example: 1900 - ? | ?
            //if (Model.BirthDate != null && Model.DeathDate == null)
            //{
            //    return string.Format(CultureInfo.CurrentUICulture,
            //        "{0} - ? | ?", Model.BirthDate.Value.Year);
            //}

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
            bool ShouldDisplayGroupIndicator(object obj, NodeType nodeType)
            {
                if (obj is Model { } Model)
                {
                    switch (nodeType)
                    {
                        // Primary and related nodes never display the group indicator.
                        case NodeType.Primary:
                        case NodeType.Related:
                            return false;
                        // Spouse - if have parents, siblings, or ex spouses.
                        case NodeType.Spouse when (Model.Parents.Count() > 0 || Model.Siblings.Count() > 0):
                            return true;
                        // Sibling - if have spouse, or children.
                        case NodeType.Sibling when (Model.Spouses.Count() > 0 || Model.Children.Count() > 0):
                            return true;
                        // Half sibling - like sibling, but also inherits the
                        // group status from all parents.
                        case NodeType.SiblingLeft when (Model.Spouses.Count() > 0 || Model.Children.Count() > 0):
                        case NodeType.SiblingRight when (Model.Spouses.Count() > 0 || Model.Children.Count() > 0):

                            return true;
                    }
                }
                return false;
            }
        }

        public string BottomLabel(object obj, double displayYear)
        {
            return "Bottom Label";
        }
    }
}