using Abstractions;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationships
{
    internal class NodeHelper
    {
        private static void RemoveSiblingRelationships(INode node)
        {
            foreach (var rel in node.Relationships.Reverse())
            {
                if ((rel as Relationship).RelationshipType == RelationshipType.Sibling)
                {
                    node.Remove(rel as Relationship);
                }
            }
        }

        /// <summary>
        /// Helper function for removing a parent relationship
        /// </summary>
        private static void RemoveParentChildRelationship(INode node, INode parent)
        {
            foreach (Relationship relationship in node.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Parent && relationship.RelationTo.Equals(parent))
                {
                    node.Remove(relationship);
                    break;
                }
            }

            foreach (Relationship relationship in parent.Relationships)
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
        public static void AddChildRelationships(INode parent, INode child)
        {
            //add child relationship to INode
            parent.Add(new ChildRelationship(child));

            //add INode as parent of child
            child.Add(new ParentRelationship(parent));
        }

        /// <summary>
        /// Add Spouse relationship between the INode and the spouse with the provided spouse relationship type.
        /// </summary>
        public static void AddSpouseRelationships(INode node, INode spouse, DateTime startDate)
        {
            //assign spouses to each other    
            node.Add(new SpouseRelationship(spouse) { StartDate = startDate });
            spouse.Add(new SpouseRelationship(node) { StartDate = startDate });
        }

        /// <summary>
        /// Adds sibling relation between the INode and the sibling
        /// </summary>
        public static void AddSiblingRelationships(INode node, INode sibling)
        {
            //assign sibling to each other    
            node.Add(new SiblingRelationship(sibling));
            sibling.Add(new SiblingRelationship(node));


        }

    }
}
