//using Abstractions;
using Abstractions;
using Microsoft.FamilyShowLib;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Demo
{
    public static class RelationshipHelper
    {
        /// <summary>
        /// Performs the business logic for adding the Child relationship between the Model and the child.
        /// </summary>
        public static Model AddChild(Model model, Model child)
        {
            // Add the new child as a sibling to any existing children
            foreach (Model existingSibling in model.Children)
            {
                Helper.AddSibling(existingSibling as Model, child);
            }


            Helper.AddChild(model, child);


            return child;
        }

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the Model and the parent.
        /// </summary>
        public static Model AddParent(Model model, Model parent, DateTime? startDate = default)
        {
            // A Model can only have 2 parents, do nothing
            if (model.Parents.Count() == 2)
            {
                throw new Exception("dfg gdff343");
            }

            switch (model.Parents.Count())
            {
                // No exisitng parents
                case 0:
                    Helper.AddChild(parent, model);
                    break;

                // An existing parent
    
            }

            // Handle siblings
         
                // Make siblings the child of the new parent
                foreach (Model sibling in model.Siblings)
                {
                    Helper.AddChild(parent, sibling);
                }
            

            return parent;
        }

        /// <summary>
        /// Performs the business logic for adding the Spousal relationship between the Model and the spouse.
        /// </summary>
        public static Model AddSpouse(Model Model, Model spouse, DateTime startDate)
        {
            // Assume the spouse's gender based on the counterpart of the Model's gender



            Helper.AddSpouse(Model, spouse, startDate);

            return spouse;
        }



        /// <summary>
        /// Performs the business logic for adding the Sibling relationship between the Model and the sibling.
        /// </summary>
        public static Model AddSibling(Model model, Model sibling)
        {
            // Handle siblings
    
                // Connect the siblings to each other.
                foreach (Model existingSibling in model.Siblings)
                {
                    Helper.AddSibling(existingSibling as Model, sibling);
                }
            

            Helper.AddSibling(model, sibling);

            return sibling;
        }



        /// <summary>
        /// Return a list of children for the parent set.
        /// </summary>
        private static IEnumerable<Model> GetChildren(IParentSet parentSet)
        {
            // Get list of both parents.
            List<Model> firstParentChildren = new(parentSet.FirstParent.Children.Cast<Model>());
            List<Model> secondParentChildren = new(parentSet.SecondParent.Children.Cast<Model>());

            // Go through and add the children that have both parents.            
            foreach (Model child in firstParentChildren)
            {
                if (secondParentChildren.Contains(child))
                {
                    yield return (child);
                }
            }
        }

        ///// <summary>
        ///// Performs the business logic for updating the spouse status
        ///// </summary>
        public static void UpdateSpouseStatus(Model Model, Model spouse, Existence modifier)
        {
            foreach (Relationship relationship in Model.Relationships.Cast<Relationship>())
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).Existence = modifier;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(Model))
                {
                    ((SpouseRelationship)relationship).Existence = modifier;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the marriage date
        /// </summary>
        public static void UpdateMarriageDate(Model Model, Model spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in Model.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).StartDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(Model))
                {
                    ((SpouseRelationship)relationship).StartDate = dateTime;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the divorce date
        /// </summary>
        public static void UpdateDivorceDate(Model Model, Model spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in Model.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).EndDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(Model))
                {
                    ((SpouseRelationship)relationship).EndDate = dateTime;
                    break;
                }
            }
        }

        ///// <summary>
        ///// Performs the business logic for changing the Model parents
        ///// </summary>
        //public static void ChangeParents(Model Model, IParentSet newParentSet)
        //{
        //    // Don't do anything if there is nothing to change or if the parents are the same
        //    if (Model.ParentSet == null || newParentSet == null || Model.ParentSet.Equals(newParentSet))
        //        return;

        //    // store the current parent set which will be removed
        //    IParentSet formerParentSet = Model.ParentSet;

        //    // Remove the first parent
        //    RemoveParentChildRelationship(Model, formerParentSet.FirstParent as Model);

        //    // Remove the Model as a child of the second parent
        //    RemoveParentChildRelationship(Model, formerParentSet.SecondParent as Model);

        //    // Remove the sibling relationships
        //    RemoveSiblingRelationships(Model);

        //    // Add the new parents
        //    AddParent(Model, newParentSet);


        //    /// <summary>
        //    /// Performs the business logic for adding the Parent relationship between the Model and the parents.
        //    /// </summary>
        //    static void AddParent(Model Model, IParentSet parentSet)
        //    {
        //        // First add child to parents.
        //        Helper.AddChild(parentSet.FirstParent as Model, Model, ParentChildModifier.Natural);
        //        Helper.AddChild(parentSet.SecondParent as Model, Model, ParentChildModifier.Natural);

        //        // Next update the siblings. Get the list of full siblings for the Model. 
        //        // A full sibling is a sibling that has both parents in common.            
        //        foreach (Model sibling in GetChildren(parentSet))
        //        {
        //            if (sibling != Model)
        //            {
        //                Helper.AddSibling(Model, sibling);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Helper function for removing sibling relationships
        /// </summary>
        private static void RemoveSiblingRelationships(Model Model)
        {
            foreach (var rel in Model.Relationships.Reverse())
            {
                if ((rel as Relationship).RelationshipType == RelationshipType.Sibling)
                {
                    Model.Remove(rel as Relationship);
                }
            }
        }

        /// <summary>
        /// Helper function for removing a parent relationship
        /// </summary>
        private static void RemoveParentChildRelationship(Model Model, Model parent)
        {
            foreach (Relationship relationship in Model.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Parent && relationship.RelationTo.Equals(parent))
                {
                    Model.Remove(relationship);
                    break;
                }
            }

            foreach (Relationship relationship in parent.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Child && relationship.RelationTo.Equals(Model))
                {
                    parent.Remove(relationship);
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for changing the deleting the Model
        /// </summary>
        //public static void DeleteModel(ICollection<Model> family, Model ModelToDelete)
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
        //                (relationship.RelationTo as Model).Remove(rel as Relationship);
        //                break;
        //            }
        //        }
        //    }

        //    // Delete the Model's photos and story
        //    ModelToDelete.DeletePhotos();
        //    ModelToDelete.DeleteStory();

        //    family.Remove(ModelToDelete);
        //}

        static class Helper
        {
            class Package
            {
                public Package(Relationship relationship, Model? Model)
                {
                    Relationship = relationship;
                    Model = Model;
                }

                public Relationship Relationship { get; }
                public Model? Model { get; }
            }
            /// <summary>
            /// Adds Parent-Child relationship between Model and child with the provided parent-child relationship type.
            /// </summary>
            public static void AddChild(Model parent, Model child)
            {
                //add child relationship to Model
                parent.Add(new ChildRelationship(child));

                //add Model as parent of child
                child.Add(new ParentRelationship(parent));
            }

            /// <summary>
            /// Add Spouse relationship between the Model and the spouse with the provided spouse relationship type.
            /// </summary>
            public static void AddSpouse(Model model, Model spouse, DateTime startDate)
            {
                //assign spouses to each other    
                model.Add(new SpouseRelationship(spouse) { StartDate = startDate });
                spouse.Add(new SpouseRelationship(model) { StartDate = startDate });
            }

            /// <summary>
            /// Adds sibling relation between the Model and the sibling
            /// </summary>
            public static void AddSibling(Model Model, Model sibling)
            {
                //assign sibling to each other    
                Model.Add(new SiblingRelationship(sibling));
                sibling.Add(new SiblingRelationship(Model));


            }
        }
    }
}

