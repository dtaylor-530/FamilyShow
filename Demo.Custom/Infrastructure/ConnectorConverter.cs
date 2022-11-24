using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Globalization;

namespace Demo.Custom.Infrastructure
{
    public class ConnectorConverter : IConnectorConverter
    {
        public DateTime? MinimumDate(INode obj1, INode obj2)
        {
            var rel = obj1.GetSpouseRelationship(obj2);
            if (rel != null)
                return rel.StartDate;

            return default;
            ////if (!married)
            ////{
            ////    SpouseRelationship rel = StartNode.Node.Model.GetSpouseRelationship(EndNode.Node.Model);
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

            //DateTime? date = Model.BirthDate;

            //return date;
        }

        public IRelationship? Relationship(INode obj1, INode obj2)
        {
            return obj1.GetSpouseRelationship(obj2);
        }

        public string? Text(INode obj1, INode obj2)
        {
            var rel = Relationship(obj1, obj2);
            if (rel != null)
            {
                // Marriage date.
                if (rel.StartDate.HasValue)
                {
                    return rel.StartDate.Value.Year.ToString(CultureInfo.CurrentCulture);
                }
                if (rel.EndDate.HasValue)
                {
                    return rel.EndDate.Value.Year.ToString(CultureInfo.CurrentCulture);
                }
            }
            return default;
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
        //    //  SpouseRelationship rel = StartNode.Node.Model.GetSpouseRelationship(EndNode.Node.Model);
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
        //    //  SpouseRelationship rel = StartNode.Node.Model.GetSpouseRelationship(EndNode.Node.Model);
        //    //  if (rel != null)
        //    //    return rel.DivorceDate;
        //    //}
        //    return null;
        //  }
        //}
    }
}