﻿using Abstractions;
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
        public static IEnumerable<(INode, IEnumerable<ChildDiagramConnector>)> AddChildConnections(this Dictionary<object, DiagramConnectorNode> logic, IList<INode> parents, IConnectorConverter converter)
        {
            foreach (INode person in parents)
                yield return (person, AddChildConnections(logic, person, converter));
        }

        /// <summary>
        /// Add connections between the parent and parent’s children.
        /// </summary>
        public static IEnumerable<ChildDiagramConnector> AddChildConnections(this Dictionary<object, DiagramConnectorNode> logic, INode parent, IConnectorConverter converter)
        {
            foreach (INode child in parent.Children)
            {
                yield return Add(logic, parent, child, converter);
            }
        }


        /// <summary>
        /// Add connections between the child and child's parents.
        /// </summary>
        public static IEnumerable<ChildDiagramConnector> AddParentConnections(this Dictionary<object, DiagramConnectorNode> logic, INode child, IConnectorConverter converter)
        {
            foreach (INode parent in child.Parents)
            {
                yield return Add(logic, parent, child, converter);
            }
        }

        public static ChildDiagramConnector Add(this Dictionary<object, DiagramConnectorNode> logic, INode parent, INode child, IConnectorConverter converter)
        {
            if (logic.ContainsKey(parent) && logic.ContainsKey(child))
            {
                return new ChildDiagramConnector(logic[parent], logic[child], converter);
            }
            throw new Exception("dfg 3424 dfgdf");
        }

        /// <summary>
        /// Get the spouse relationship for the specified spouse.
        /// </summary>
        public static IRelationship? GetSpouseRelationship(this INode person, INode spouse)
        {
            foreach (IRelationship relationship in person.Relationships)
            {

                if (relationship.RelationshipType == RelationshipType.Spouse)
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
