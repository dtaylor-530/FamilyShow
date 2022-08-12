using Abstractions;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram.Logic
{
    public static class PersonHelper
    {
        public static void AddChildConnections(this DiagramLogic logic, IList<INode> parents)
        {
            foreach (INode person in parents)
                AddChildConnections(logic, person);
        }

        /// <summary>
        /// Add connections between the parent and parent’s children.
        /// </summary>
        public static void AddChildConnections(this DiagramLogic logic, INode parent)
        {
            foreach (INode child in parent.Children)
            {
                Add(logic, parent, child);
            }
        }


        /// <summary>
        /// Add connections between the child and child's parents.
        /// </summary>
        public static void AddParentConnections(this DiagramLogic logic, INode child)
        {
            foreach (INode parent in child.Parents)
            {
                Add(logic, parent, child);
            }
        }

        public static void Add(this DiagramLogic logic, INode parent, INode child)
        {
            if (logic.PersonLookup.ContainsKey(parent) &&
                logic.PersonLookup.ContainsKey(child))
            {
                logic.Connections.Add(new ChildDiagramConnector(
                    logic.PersonLookup[parent], logic.PersonLookup[child]));
            }
        }

        /// <summary>
        /// Get the spouse relationship for the specified spouse.
        /// </summary>
        public static IRelationship? GetSpouseRelationship(this INode person, INode spouse)
        {
            foreach (IRelationship relationship in person.Relationships)
            {
                
                if (relationship.RelationshipType== RelationshipType.Spouse)
                {
                    if (relationship.RelationTo.Equals(spouse))
                    {
                        return relationship;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Gets the combination of parent sets for this person and his/her spouses
        /// </summary>
        /// <returns></returns>
        //public static ParentSetCollection MakeParentSets(INode person)
        //{
        //    ParentSetCollection parentSets = new ParentSetCollection();

        //    foreach (INode spouse in person.Spouses)
        //    {
        //        ParentSet ps = new ParentSet(person, spouse);

        //        // Don't add the same parent set twice.
        //        if (!parentSets.Contains(ps))
        //        {
        //            parentSets.Add(ps);
        //        }
        //    }

        //    return parentSets;
        //}

    }
}
