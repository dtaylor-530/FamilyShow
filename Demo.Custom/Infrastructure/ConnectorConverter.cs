using Abstractions;
using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Media.TextFormatting;
using System.Windows.Media;
using System.Windows;
using Diagram.Logic;
using Demo;

namespace Demo.Custom.Infrastructure
{
    public class ConnectorConverter : IConnectorConverter
    {
        public DateTime? MinimumDate(object obj1, object obj2)
        {
            if (obj1 is not INode { } Model1)
            {
                throw new Exception("Tfbgdgfdgf");
            }
            if (obj2 is not INode { } Model2)
            {
                throw new Exception("Tfbgdddgfdgf");
            }

            var rel = Model1.GetSpouseRelationship(Model2);
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
        public IRelationship? Relationship(object obj1, object obj2)
        {
            if (obj1 is not INode { } Model1)
            {
                throw new Exception("Tfbgdgfdgf");
            }
            if (obj2 is not INode { } Model2)
            {
                throw new Exception("Tfbgdddgfdgf");
            }

            return Model1.GetSpouseRelationship(Model2);

        }
        public string Text(object obj1, object obj2)
        {
            string text = default;

            var rel = Relationship(obj1, obj2);
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
