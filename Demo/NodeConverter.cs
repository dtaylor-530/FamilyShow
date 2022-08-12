using Abstractions;
using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Globalization;
using System.Linq;

namespace Diagram.Logic
{
    public class NodeConverter : INodeConverter
    {
        public NodeConverter()
        {
        }

        public bool IsFiltered(object obj, double displayYear)
        {
            if (obj is INode { } person)
            {
                return person.BirthDate != null && person.BirthDate.Value.Year > displayYear;
            }
            return false;
        }

        public bool ShouldDisplayGroupIndicator(object obj, NodeType nodeType)
        {
            if (obj is INode { } person)
            {
                switch (nodeType)
                {
                    // Primary and related nodes never display the group indicator.
                    case NodeType.Primary:
                    case NodeType.Related:
                        return false;
                    // Spouse - if have parents, siblings, or ex spouses.
                    case NodeType.Spouse when (person.Parents.Count() > 0 || person.Siblings.Count() > 0 || person.PreviousSpouses.Count() > 0):
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
            }
            return false;
        }

        public string DateInformation(object obj, double displayYear)
        {
            if (obj is INode { } person)
            {
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
            }
            return string.Empty;
        }


        public string NodeTemplate(object obj, NodeType type)
        {
            if (obj is not INode { } person)
            {
                throw new Exception("Tfbgdgfdgf");
            }
            // Determine the node template based on node properties.
            string template = string.Format(
            CultureInfo.InvariantCulture, "{0}{1}NodeTemplate",
            (person.Gender == Gender.Female) ? "Female" : "Male",
            (type == NodeType.Primary) ? "Primary" : "");
            return template;
        }
        public string NodeTemplate()
        {
            return "Person";
        }

        public DateTime? MinimumDate(object obj)
        {
            if (obj is not INode { } person)
            {
                throw new Exception("Tfbgdgfdgf");
            }

            DateTime? date = person.BirthDate;
   
           return date;
        }

        public object BrushResource(object? model, NodeType type, string part)
        {
            if (model is not INode { } person)
            {
                throw new Exception("Tfbgdgfdgf");
            }

            string resourceName = string.Format(
        CultureInfo.InvariantCulture, "{0}{1}{2}{3}",
        (person.Gender == Gender.Female) ? "Female" : "Male",
        type.ToString(),
        person.IsLiving ? "Living" : "Deceased",
        part);
            return resourceName;
        }
    }
}
