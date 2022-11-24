using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using System;
using System.Globalization;

namespace Demo
{
    public class ConnectorConverter : IConnectorConverter
    {
        public DateTime? MinimumDate(IRelationship rel)
        {
            if (rel is ChildRelationship)
            {
                return rel.StartDate;
            }

            if (rel is ParentRelationship)
            {
                return rel.StartDate;
            }

            if (rel is SpouseRelationship { Existence: { } existence })
            {
                if (existence == ExistenceState.Current)
                    return rel.StartDate;

                if (existence == ExistenceState.Former)
                    return rel.EndDate;
            }

            throw new Exception("sdfg3 dgsfg..");
        }

        public string Text(INode person1, INode person2)
        {
            string text = default;

            var rel = person1.GetSpouseRelationship(person2);
            if (rel != null)
            {
                return MinimumDate(rel).Value.Year.ToString(CultureInfo.CurrentCulture);
            }
            return text;
        }
    }
}