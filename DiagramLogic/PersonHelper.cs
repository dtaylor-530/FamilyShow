using Abstractions;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Collections.Generic;

namespace Diagram.Logic
{
    public static class PersonHelper
    {
        public static IEnumerable<(INode, IEnumerable<ChildDiagramConnector>)> MakeChildConnections(this Dictionary<INode, DiagramConnectorNode> logic, IList<INode> parents, IConnectorConverter converter)
        {
            foreach (INode person in parents)
                yield return (person, MakeChildConnections(logic, person, converter));
        }

        /// <summary>
        /// Add connections between the parent and parent’s children.
        /// </summary>
        public static IEnumerable<ChildDiagramConnector> MakeChildConnections(this Dictionary<INode, DiagramConnectorNode> logic, INode parent, IConnectorConverter converter)
        {
            foreach (IRelationship relationship in parent.Relationships(RelationshipType.Child))
            {
                yield return logic.AddFrom(relationship, parent, relationship.RelationTo, converter);
            }
        }

        /// <summary>
        /// Add connections between the child and child's parents.
        /// </summary>
        public static IEnumerable<ChildDiagramConnector> MakeParentConnections(this Dictionary<INode, DiagramConnectorNode> logic, INode child, IConnectorConverter converter)
        {
            foreach (IRelationship parent in child.Relationships(RelationshipType.Parent))
            {
                yield return logic.AddFrom(parent, parent.RelationTo, child, converter);
            }
        }

        public static ChildDiagramConnector AddFrom(this Dictionary<INode, DiagramConnectorNode> logic, IRelationship relationship, INode parent, INode child, IConnectorConverter converter)
        {
            if (logic.ContainsKey(parent) && logic.ContainsKey(child))
            {
                return new ChildDiagramConnector(relationship, logic[parent], logic[child], converter);
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