using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Globalization;

namespace Demo.Custom.Infrastructure
{
    public class ConnectorConverter : IConnectorConverter
    {
        public DateTime MinimumDate(IRelationship relationship)
        {
            if (relationship?.StartDate != null)
                return relationship.StartDate.Value;

            throw new Exception("dsfd sfd");
        }

        public string Text(INode obj1, INode obj2)
        {
            var rel = obj1.GetSpouseRelationship(obj2);
            if (rel != null)
            {
                return this.MinimumDate(rel).Year.ToString(CultureInfo.CurrentCulture);
            }
            return "No Text";
        }
    }
}