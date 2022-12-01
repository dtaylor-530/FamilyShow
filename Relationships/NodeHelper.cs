using Abstractions;
using Models;

namespace Relationships
{
    internal class NodeHelper
    {
        private static void RemoveSiblingRelationships(INodeRelationshipEditor node)
        {
            foreach (var rel in node.Node.Relationships.Reverse())
            {
                if ((rel).RelationshipType == RelationshipType.Sibling)
                {
                    node.Remove(rel);
                }
            }
        }

        /// <summary>
        /// Helper function for removing a parent relationship
        /// </summary>
        private static void RemoveParentChildRelationship(INodeRelationshipEditor node, INodeRelationshipEditor parent)
        {
            foreach (Relationship relationship in node.Node.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Parent && relationship.RelationTo.Equals(parent))
                {
                    node.Remove(relationship);
                    break;
                }
            }

            foreach (Relationship relationship in parent.Node.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Child && relationship.RelationTo.Equals(node))
                {
                    parent.Remove(relationship);
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for changing the deleting the INode
        /// </summary>
        //public static void DeleteModel(ICollection<INode> family, INode ModelToDelete)
        //{
        //    if (!ModelToDelete.IsDeletable)
        //    {
        //        return;
        //    }

        //    // Remove the ModelToDelete from the relationships that contains the ModelToDelete.
        //    foreach (Relationship relationship in ModelToDelete.Relationships)
        //    {
        //        foreach (Relationship rel in relationship.RelationTo.Relationships)
        //        {
        //            if (rel.RelationTo.Equals(ModelToDelete))
        //            {
        //                (relationship.RelationTo as INode).Remove(rel as Relationship);
        //                break;
        //            }
        //        }
        //    }

        //    // Delete the INode's photos and story
        //    ModelToDelete.DeletePhotos();
        //    ModelToDelete.DeleteStory();

        //    family.Remove(ModelToDelete);
        //}

        /// <summary>
        /// Adds Parent-Child relationship between INode and child with the provided parent-child relationship type.
        /// </summary>
        public static void AddChildRelationships(INodeRelationshipEditor parent, INodeRelationshipEditor child)
        {
            //add child relationship to INode
            parent.Add(new ChildRelationship(child.Node) { StartDate = child.Node.Created });

            //add INode as parent of child
            child.Add(new ParentRelationship(parent.Node) { StartDate = child.Node.Created });
        }

        /// <summary>
        /// Add Spouse relationship between the INode and the spouse with the provided spouse relationship type.
        /// </summary>
        public static void AddSpouseRelationships(INodeRelationshipEditor node, INodeRelationshipEditor spouse, DateTime startDate)
        {
            //assign spouses to each other
            node.Add(new SpouseRelationship(spouse.Node) { StartDate = startDate });
            spouse.Add(new SpouseRelationship(node.Node) { StartDate = startDate });
        }

        /// <summary>
        /// Adds sibling relation between the INode and the sibling
        /// </summary>
        public static void AddSiblingRelationships(INodeRelationshipEditor node, INodeRelationshipEditor sibling)
        {
            //assign sibling to each other
            var max = new DateTime(Math.Max(sibling.Node.Created.Ticks, node.Node.Created.Ticks));
            node.Add(new SiblingRelationship(sibling.Node) { StartDate = max });
            sibling.Add(new SiblingRelationship(node.Node) { StartDate = max });
        }
    }
}