using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Globalization;

namespace Demo
{
    public class ConnectorConverter : IConnectorConverter
    {
        public DateTime? MinimumDate(INode person1, INode person2)
        {
            var rel = person1.GetSpouseRelationship(person2);
            if (rel != null)
                return rel.StartDate;

            return default;
            ////if (!married)
            ////{
            ////    SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
            ////    if (rel != null)
            ////        return rel.DivorceDate;
            ////}

            //// Marriage date.
            ////DateTime? date = connector.MarriedDate;
            ////if (date != null)
            ////    minimumYear = Math.Min(minimumYear, date.Value.Year);

            //// Previous marriage date.
            //date = connector.PreviousMarriedDate;
            //if (date != null)
            //    minimumYear = Math.Min(minimumYear, date.Value.Year);

            //DateTime? date = person.BirthDate;

            //return date;
        }

        public IRelationship? Relationship(INode person1, INode person2)
        {
            return person1.GetSpouseRelationship(person2);
        }

        public string Text(INode person1, INode person2)
        {
            string text = default;

            var rel = Relationship(person1, person2);
            if (rel != null)
            {
                // Marriage date.
                if (rel.StartDate.HasValue)
                {
                    text = rel.StartDate.Value.Year.ToString(CultureInfo.CurrentCulture);
                }
                if (rel.EndDate.HasValue)
                {
                    text = rel.EndDate.Value.Year.ToString(CultureInfo.CurrentCulture);
                }
            }
            return text;
        }

        /// <summary>
        /// Gets the married date for the connector. Can be null.
        /// </summary>
        //override public DateTime? MarriedDate
        //{
        //  get
        //  {
        //    //if (married)
        //    //{
        //    //  SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
        //    //  if (rel != null)
        //    //    return rel.MarriageDate;
        //    //}
        //    return null;
        //  }
        //}

        ///// <summary>
        ///// Get the previous married date for the connector. Can be null.
        ///// </summary>
        //override public DateTime? PreviousMarriedDate
        //{
        //  get
        //  {
        //    //if (!married)
        //    //{
        //    //  SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
        //    //  if (rel != null)
        //    //    return rel.DivorceDate;
        //    //}
        //    return null;
        //  }
        //}
    }
}