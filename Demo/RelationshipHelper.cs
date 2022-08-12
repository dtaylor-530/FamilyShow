﻿using Abstractions;
using Microsoft.FamilyShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public static class RelationshipHelper
    {
        /// <summary>
        /// Performs the business logic for adding the Child relationship between the person and the child.
        /// </summary>
        public static void AddChild(this PeopleCollection<INode> family, Person person, Person child)
        {
            // Add the new child as a sibling to any existing children
            foreach (INode existingSibling in person.Children)
            {
                Helper.AddSibling(family, existingSibling as Person, child);
            }

            switch (person.Spouses.Count())
            {
                // Single parent, add the child to the person
                case 0:
                    Helper.AddChild(family, person, child, ParentChildModifier.Natural);
                    break;

                // Has existing spouse, add the child to the person's spouse as well.
                case 1:
                    Helper.AddChild(family, person, child, ParentChildModifier.Natural);
                    Helper.AddChild(family, person.Spouses.First(), child, ParentChildModifier.Natural);
                    break;
            }
        }

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the person and the parent.
        /// </summary>
        public static void AddParent(this PeopleCollection<INode> family, Person person, Person parent)
        {
            // A person can only have 2 parents, do nothing
            if (person.Parents.Count() == 2)
            {
                return;
            }

            // Add the parent to the main collection of people.
            family.Add(parent);

            switch (person.Parents.Count())
            {
                // No exisitng parents
                case 0:
                    Helper.AddChild(family, parent, person, ParentChildModifier.Natural);
                    break;

                // An existing parent
                case 1:
                    Helper.AddChild(family, parent, person, ParentChildModifier.Natural);
                    Helper.AddSpouse(family, parent, person.Parents.First() as Person, SpouseModifier.Current);
                    break;
            }

            // Handle siblings
            if (person.Siblings.Count() > 0)
            {
                // Make siblings the child of the new parent
                foreach (INode sibling in person.Siblings)
                {
                    Helper.AddChild(family, parent, sibling, ParentChildModifier.Natural);
                }
            }

            // Setter for property change notification
            person.HasParents = true;
        }

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the person and the parents.
        /// </summary>
        public static void AddParent(this PeopleCollection<INode> family, Person person, IParentSet parentSet)
        {
            // First add child to parents.
            Helper.AddChild(family, parentSet.FirstParent, person, ParentChildModifier.Natural);
            Helper.AddChild(family, parentSet.SecondParent, person, ParentChildModifier.Natural);

            // Next update the siblings. Get the list of full siblings for the person. 
            // A full sibling is a sibling that has both parents in common. 
            List<INode> siblings = GetChildren(parentSet);
            foreach (INode sibling in siblings)
            {
                if (sibling != person)
                {
                    family.AddSibling(person, sibling as Person);
                }
            }
        }

        /// <summary>
        /// Return a list of children for the parent set.
        /// </summary>
        private static List<INode> GetChildren(IParentSet parentSet)
        {
            // Get list of both parents.
            List<INode> firstParentChildren = new List<INode>(parentSet.FirstParent.Children);
            List<INode> secondParentChildren = new List<INode>(parentSet.SecondParent.Children);

            // Combined children list that is returned.
            List<INode> children = new List<INode>();

            // Go through and add the children that have both parents.            
            foreach (INode child in firstParentChildren)
            {
                if (secondParentChildren.Contains(child))
                {
                    children.Add(child);
                }
            }

            return children;
        }

        /// <summary>
        /// Performs the business logic for adding the Spousal relationship between the person and the spouse.
        /// </summary>
        public static void AddSpouse(this PeopleCollection<INode> family, Person person, Person spouse, SpouseModifier modifier)
        {
            // Assume the spouse's gender based on the counterpart of the person's gender
            if (person.Gender == Gender.Male)
            {
                spouse.Gender = Gender.Female;
            }
            else
            {
                spouse.Gender = Gender.Male;
            }

            if (person.Spouses != null)
            {
                switch (person.Spouses.Count())
                {
                    // No existing spouse	
                    case 0:
                        Helper.AddSpouse(family, person, spouse, modifier);

                        // Add any of the children as the child of the spouse.
                        if (person.Children != null || person.Children.Count() > 0)
                        {
                            foreach (INode child in person.Children)
                            {
                                Helper.AddChild(family, spouse, child, ParentChildModifier.Natural);
                            }
                        }
                        break;

                    // Existing spouse(s)
                    default:
                        // If specifying a new married spouse, make existing spouses former.
                        if (modifier == SpouseModifier.Current)
                        {
                            foreach (Relationship relationship in person.Relationships)
                            {
                                if (relationship.RelationshipType == RelationshipType.Spouse)
                                {
                                    ((SpouseRelationship)relationship).SpouseModifier = SpouseModifier.Former;
                                }
                            }
                        }

                        Helper.AddSpouse(family, person, spouse, modifier);
                        break;
                }

                // Setter for property change notification
                person.HasSpouse = true;
            }
        }

        /// <summary>
        /// Performs the business logic for adding the Sibling relationship between the person and the sibling.
        /// </summary>
        public static void AddSibling(this PeopleCollection<INode> family, Person person, Person sibling)
        {
            // Handle siblings
            if (person.Siblings.Count() > 0)
            {
                // Make the siblings siblings to each other.
                foreach (INode existingSibling in person.Siblings)
                {
                    Helper.AddSibling(family, existingSibling as Person, sibling);
                }
            }

            if (person.Parents != null)
            {
                switch (person.Parents.Count())
                {
                    // No parents
                    case 0:
                        Helper.AddSibling(family, person, sibling);
                        break;

                    // Single parent
                    case 1:
                        Helper.AddSibling(family, person, sibling);
                        Helper.AddChild(family, person.Parents.First(), sibling, ParentChildModifier.Natural);
                        break;

                    // 2 parents
                    case 2:
                        // Add the sibling as a child of the same parents
                        foreach (INode parent in person.Parents)
                        {
                            Helper.AddChild(family, parent, sibling, ParentChildModifier.Natural);
                        }

                        Helper.AddSibling(family, person, sibling);
                        break;

                    default:
                        Helper.AddSibling(family, person, sibling);
                        break;
                }
            }
        }

        ///// <summary>
        ///// Performs the business logic for updating the spouse status
        ///// </summary>
        public static void UpdateSpouseStatus(INode person, INode spouse, SpouseModifier modifier)
        {
            foreach (IRelationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).SpouseModifier = modifier;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).SpouseModifier = modifier;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the marriage date
        /// </summary>
        public static void UpdateMarriageDate(INode person, INode spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).StartDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).StartDate = dateTime;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the divorce date
        /// </summary>
        public static void UpdateDivorceDate(INode person, INode spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).EndDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).EndDate = dateTime;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for changing the person parents
        /// </summary>
        public static void ChangeParents(this PeopleCollection<INode> family, Person person, IParentSet newParentSet)
        {
            // Don't do anything if there is nothing to change or if the parents are the same
            if (person.ParentSet == null || newParentSet == null || person.ParentSet.Equals(newParentSet))
                return;

            // store the current parent set which will be removed
            IParentSet formerParentSet = person.ParentSet;

            // Remove the first parent
            RemoveParentChildRelationship(person, formerParentSet.FirstParent);

            // Remove the person as a child of the second parent
            RemoveParentChildRelationship(person, formerParentSet.SecondParent);

            // Remove the sibling relationships
            RemoveSiblingRelationships(person);

            // Add the new parents
            AddParent(family, person, newParentSet);
        }

        /// <summary>
        /// Helper function for removing sibling relationships
        /// </summary>
        private static void RemoveSiblingRelationships(INode person)
        {
            for (int i = person.Relationships.Count - 1; i >= 0; i--)
            {
                if (person.Relationships[i].RelationshipType == RelationshipType.Sibling)
                {
                    person.Relationships.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Helper function for removing a parent relationship
        /// </summary>
        private static void RemoveParentChildRelationship(INode person, INode parent)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Parent && relationship.RelationTo.Equals(parent))
                {
                    person.Relationships.Remove(relationship);
                    break;
                }
            }

            foreach (Relationship relationship in parent.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Child && relationship.RelationTo.Equals(person))
                {
                    parent.Relationships.Remove(relationship);
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for changing the deleting the person
        /// </summary>
        public static void DeletePerson(PeopleCollection<INode> family, Person personToDelete)
        {
            if (!personToDelete.IsDeletable)
            {
                return;
            }

            // Remove the personToDelete from the relationships that contains the personToDelete.
            foreach (Relationship relationship in personToDelete.Relationships)
            {
                foreach (Relationship rel in relationship.RelationTo.Relationships)
                {
                    if (rel.RelationTo.Equals(personToDelete))
                    {
                        relationship.RelationTo.Relationships.Remove(rel);
                        break;
                    }
                }
            }

            // Delete the person's photos and story
            personToDelete.DeletePhotos();
            personToDelete.DeleteStory();

            family.Remove(personToDelete);
        }

        static class Helper
        {

            /// <summary>
            /// Adds Parent-Child relationship between person and child with the provided parent-child relationship type.
            /// </summary>
            public static void AddChild(PeopleCollection<INode> people, INode parent, INode child, ParentChildModifier parentChildType)
            {
                //add child relationship to person
                parent.Relationships.Add(new ChildRelationship(child, parentChildType));

                //add person as parent of child
                child.Relationships.Add(new ParentRelationship(parent, parentChildType));

                //add the child to the main people list
                if (!people.Contains(child))
                {
                    people.Add(child);
                }
            }

            /// <summary>
            /// Add Spouse relationship between the person and the spouse with the provided spouse relationship type.
            /// </summary>
            public static void AddSpouse(PeopleCollection<INode> people, INode person, INode spouse, SpouseModifier spouseType)
            {
                //assign spouses to each other    
                person.Relationships.Add(new SpouseRelationship(spouse, spouseType));
                spouse.Relationships.Add(new SpouseRelationship(person, spouseType));

                //add the spouse to the main people list
                if (!people.Contains(spouse))
                {
                    people.Add(spouse);
                }
            }

            /// <summary>
            /// Adds sibling relation between the person and the sibling
            /// </summary>
            public static void AddSibling(PeopleCollection<INode> people, INode person, INode sibling)
            {
                //assign sibling to each other    
                person.Relationships.Add(new SiblingRelationship(sibling));
                sibling.Relationships.Add(new SiblingRelationship(person));

                //add the sibling to the main people list
                if (!people.Contains(sibling))
                {
                    people.Add(sibling);
                }
            }
        }
    }
}

