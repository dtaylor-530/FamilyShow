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

namespace Diagram.Logic
{
    public class ConnectorConverter : IConnectorConverter
    {
        public DateTime? MinimumDate(object obj1, object obj2)
        {
            if (obj1 is not INode { } person1)
            {
                throw new Exception("Tfbgdgfdgf");
            }
            if (obj2 is not INode { } person2)
            {
                throw new Exception("Tfbgdddgfdgf");
            }

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
        public IRelationship? Relationship(object obj1, object obj2)
        {
            if (obj1 is not INode { } person1)
            {
                throw new Exception("Tfbgdgfdgf");
            }
            if (obj2 is not INode { } person2)
            {
                throw new Exception("Tfbgdddgfdgf");
            }

            return person1.GetSpouseRelationship(person2);
   
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
