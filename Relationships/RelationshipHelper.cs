using Abstractions;
using Models;

namespace Relationships
{
    public static class RelationshipHelper
    {
        /// <summary>
        /// Performs the business logic for adding the Child relationship between the INode and the child.
        /// </summary>
        public static INode AddChild(INodeRelationshipEditor model, INodeRelationshipEditor child)
        {
            // Add the new child as a sibling to any existing children
            foreach (INodeRelationshipEditor existingSibling in model.Children())
            {
                NodeHelper.AddSiblingRelationships(existingSibling, child);
            }

            NodeHelper.AddChildRelationships(model, child);

            return child;
        }

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the INode and the parent.
        /// </summary>
        public static INode AddParent(INodeRelationshipEditor model, INodeRelationshipEditor parent, DateTime? startDate = default)
        {
            // A INode can only have 2 parents, do nothing
            //if (model.Parents.Count() == 2)
            //{
            //    throw new Exception("dfg gdff343");
            //}

            var parents = model.Parents().ToArray();

            NodeHelper.AddChildRelationships(parent, model);

            foreach (INodeRelationshipEditor spouse in parents)
            {
                NodeHelper.AddSpouseRelationships(parent, spouse, startDate ?? default);
            }

            foreach (INodeRelationshipEditor sibling in model.Siblings())
            {
                NodeHelper.AddChildRelationships(parent, sibling);
                foreach (INodeRelationshipEditor spouse in parents)
                {
                    if (spouse.Children().Contains(model))
                    {
                        continue;
                    }
                    NodeHelper.AddChildRelationships(spouse, model);
                }
            }

            return parent;
        }

        /// <summary>
        /// Performs the business logic for adding the Spousal relationship between the INode and the spouse.
        /// </summary>
        public static INode AddSpouse(INodeRelationshipEditor node, INodeRelationshipEditor spouse, DateTime startDate)
        {
            NodeHelper.AddSpouseRelationships(node, spouse, startDate);

            return spouse;
        }

        /// <summary>
        /// Performs the business logic for adding the Sibling relationship between the INode and the sibling.
        /// </summary>
        public static INode AddSibling(INodeRelationshipEditor model, INodeRelationshipEditor sibling)
        {
            // Handle siblings

            // Connect the siblings to each other.
            foreach (INodeRelationshipEditor existingSibling in model.Siblings())
            {
                NodeHelper.AddSiblingRelationships(existingSibling, sibling);
            }

            NodeHelper.AddSiblingRelationships(model, sibling);

            return sibling;
        }

        /// <summary>
        /// Return a list of children for the parent set.
        /// </summary>
        //private static IEnumerable<INode> GetChildren(IParentSet parentSet)
        //{
        //    // Get list of both parents.
        //    List<INode> firstParentChildren = new(parentSet.FirstParent.Children.Cast<INode>());
        //    List<INode> secondParentChildren = new(parentSet.SecondParent.Children.Cast<INode>());

        //    // Go through and add the children that have both parents.
        //    foreach (INode child in firstParentChildren)
        //    {
        //        if (secondParentChildren.Contains(child))
        //        {
        //            yield return (child);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Performs the business logic for updating the spouse status
        ///// </summary>
        public static void UpdateSpouseStatus(INode node, INode spouse, ExistenceState modifier)
        {
            foreach (Relationship relationship in node.Relationships.Cast<Relationship>())
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).Existence = modifier;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(node))
                {
                    ((SpouseRelationship)relationship).Existence = modifier;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the marriage date
        /// </summary>
        public static void UpdateMarriageDate(INode node, INode spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in node.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).StartDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(node))
                {
                    ((SpouseRelationship)relationship).StartDate = dateTime;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the divorce date
        /// </summary>
        public static void UpdateDivorceDate(INode node, INode spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in node.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).EndDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(node))
                {
                    ((SpouseRelationship)relationship).EndDate = dateTime;
                    break;
                }
            }
        }

        ///// <summary>
        ///// Performs the business logic for changing the INode parents
        ///// </summary>
        //public static void ChangeParents(INode node, IParentSet newParentSet)
        //{
        //    // Don't do anything if there is nothing to change or if the parents are the same
        //    if (node.ParentSet == null || newParentSet == null || node.ParentSet.Equals(newParentSet))
        //        return;

        //    // store the current parent set which will be removed
        //    IParentSet formerParentSet = node.ParentSet;

        //    // Remove the first parent
        //    RemoveParentChildRelationship(INode, formerParentSet.FirstParent as INode);

        //    // Remove the INode as a child of the second parent
        //    RemoveParentChildRelationship(INode, formerParentSet.SecondParent as INode);

        //    // Remove the sibling relationships
        //    RemoveSiblingRelationships(INode);

        //    // Add the new parents
        //    AddParent(INode, newParentSet);

        //    /// <summary>
        //    /// Performs the business logic for adding the Parent relationship between the INode and the parents.
        //    /// </summary>
        //    static void AddParent(INode node, IParentSet parentSet)
        //    {
        //        // First add child to parents.
        //        Helper.AddChild(parentSet.FirstParent as INode, INode, ParentChildModifier.Natural);
        //        Helper.AddChild(parentSet.SecondParent as INode, INode, ParentChildModifier.Natural);

        //        // Next update the siblings. Get the list of full siblings for the node.
        //        // A full sibling is a sibling that has both parents in common.
        //        foreach (INode sibling in GetChildren(parentSet))
        //        {
        //            if (sibling != INode)
        //            {
        //                Helper.AddSibling(INode, sibling);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Helper function for removing sibling relationships
        /// </summary>
    }
}