using Abstractions;
using Diagram.Logic;
using Diagrams.WPF.Infrastructure;
using Family;
using Microsoft.FamilyShowLib;
using System;
using System.Globalization;

namespace Demo
{
    public class ConnectorConverter : IConnectorConverter
    {
        public bool IsFiltered(IRelationship relationship)
        {
            return false;
        }

        public DateTime MinimumDate(IRelationship rel)
        {
            return NewMethod(rel) ?? throw new Exception("sd66  fg3 dgsfg.."); ;

            static DateTime? NewMethod(IRelationship rel)
            {
                if (rel is ChildRelationship)
                {
                    return rel.StartDate;
                }

                if (rel is ParentRelationship)
                {
                    return rel.StartDate;
                }

                if (rel is SpouseRelationship { })
                {
                    return rel.EndDate;
                    //if (existence == ExistenceState.Current)
                    //    return rel.StartDate;

                    //if (existence == ExistenceState.Former)
                    //    return rel.EndDate;
                }
                throw new Exception("sdfg3 dgsfg..");
            }
        }

        public string ResourcePen(IRelationship relationship)
        {
            if (relationship is not Relationship rel)
            {
                throw new Exception("dsv 33");
            }

            return rel.Existence switch
            {
                ExistenceState.Current => "Married",
                ExistenceState.Former => "Former",
                _ => throw new NotImplementedException()
            } + "ConnectionPen";
        }

        public void Subscribe(IRelationship obj)
        {

        }



        public string Text(IRelationship relationship)
        {
            return MinimumDate(relationship).Year.ToString(CultureInfo.CurrentCulture);
        }
    }
}