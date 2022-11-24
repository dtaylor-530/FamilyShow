using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Globalization;

namespace Demo.Custom.Infrastructure
{
    public class ConnectorConverter : IConnectorConverter
    {
        //public DateTime? MinimumDate(INode obj1, INode obj2)
        //{
        //    var rel = obj1.GetSpouseRelationship(obj2);
        //    if (rel != null)
        //        return rel.StartDate;

        //    return default;
        //}

        public DateTime? MinimumDate(IRelationship rel)
        {
            if (rel != null)
                return rel.StartDate;

            return default;
        }

        public string Text(INode obj1, INode obj2)
        {
            var rel = obj1.GetSpouseRelationship(obj2);
            if (rel != null)
            {
                return MinimumDate(rel).Value.Year.ToString(CultureInfo.CurrentCulture);
            }
            return "No Text";
        }
    }
}