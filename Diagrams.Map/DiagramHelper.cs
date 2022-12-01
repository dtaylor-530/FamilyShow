using Abstractions;
using Diagrams.WPF;
using Microsoft.FamilyShow;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Diagrams.Logic
{
    internal static class DiagramHelper
    {
        /// <summary>
        /// Return a list of parents for the people in the specified row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static IList<INode> GetParents(this DiagramRow row)
        {
            // List that is returned.
            IList<INode> list = new Collection<INode>();

            // Get possible children in the row.
            foreach (INode person in GetPrimaryAndRelatedPeople(row))
            {
                // Add each parent to the list, make sure the parent is only added once.
                foreach (INode parent in person.Parents())
                {
                    if (!list.Contains(parent))
                        list.Add(parent);
                }
            }

            return list;
        }

        /// <summary>
        /// Return a list of children for the people in the specified row.
        /// </summary>
        public static IList<INode> GetChildren(this DiagramRow row)
        {
            // List that is returned.
            IList<INode> list = new List<INode>();

            // Get possible parents in the row
            foreach (INode person in GetPrimaryAndRelatedPeople(row))
            {
                // Add each child to the list, make sure the child is only added once
                foreach (INode child in person.Children())
                {
                    if (!list.Contains(child))
                        list.Add(child);
                }
            }

            return list;
        }

        /// <summary>
        /// Return list of people in the row that are primary or related node types.
        /// </summary>
        public static IList<INode> GetPrimaryAndRelatedPeople(this DiagramRow row)
        {
            IList<INode> list = new List<INode>();
            foreach (DiagramGroup group in row.Groups)
            {
                foreach (DiagramNode node in group.Nodes)
                {
                    if (node.Type == NodeType.Related || node.Type == NodeType.Primary)
                        list.Add(node.Model as INode);
                }
            }

            return list;
        }

        public static void RemoveDuplicates<T>(this IList<T> people, IList<T> other)
        {
            foreach (T person in other)
                people.Remove(person);
        }

        public static void RemoveDuplicates(this IList<IRelationship> relationships, IList<INode> other)
        {
            foreach (INode person in other)
            {
                if (relationships.SingleOrDefault(a => a.RelationTo == person) is { } item)
                    relationships.Remove(item);
            }
        }
    }
}