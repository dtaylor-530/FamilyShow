using Abstractions;
using Diagrams.Logic;
using Diagrams.WPF.Infrastructure;
using System;
using System.Globalization;

namespace Demo.Custom.Infrastructure
{
    public class ConnectorConverter : IConnectorConverter
    {
        public bool IsFiltered(IRelationship relationship)
        {
            return false;
        }

        public DateTime MinimumDate(IRelationship relationship)
        {
            if (relationship?.StartDate != null)
                return relationship.StartDate.Value;

            throw new Exception("dsfd sfd");
        }

        public string ResourcePen(IRelationship relationship)
        {
            return "MarriedConnectionPen";
        }

        public void Subscribe(IRelationship obj)
        {
         
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

        public string Text(IRelationship relationship)
        {
            return "Relationship";
        }
    }
}