//using Abstractions;
using Abstractions;
using Family;
using Microsoft.FamilyShowLib;

namespace Demo
{
    public static class RelationshipHelper
    {
        /// <summary>
        /// Performs the business logic for adding the Child relationship between the person and the child.
        /// </summary>
        public static Person AddChild(Person person, Person child)
        {
            // Add the new child as a sibling to any existing children
            foreach (Person existingSibling in person.Children)
            {
                Helper.AddSibling(existingSibling as Person, child);
            }

            switch (person.Spouses.Count())
            {
                // Single parent, add the child to the person
                case 0:
                    Helper.AddChild(person, child, ParentChildModifier.Natural);
                    break;

                // Has existing spouse, add the child to the person's spouse as well.
                case 1:
                    Helper.AddChild(person, child, ParentChildModifier.Natural);
                    Helper.AddChild(person.Spouses.First() as Person, child, ParentChildModifier.Natural);
                    break;
            }

            return child;
        }

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the person and the parent.
        /// </summary>
        public static Person AddParent(Person person, Person parent, DateTime? startDate = default)
        {
            // A person can only have 2 parents, do nothing
            if (person.Parents.Count() == 2)
            {
                throw new Exception("dfg gdff343");
            }

            switch (person.Parents.Count())
            {
                // No exisitng parents
                case 0:
                    Helper.AddChild(parent, person, ParentChildModifier.Natural);
                    break;

                // An existing parent
                case 1:
                    Helper.AddChild(parent, person, ParentChildModifier.Natural);
                    if (person.Parents.First().Relationships.SingleOrDefault() is not Relationship { StartDate: DateTime date } relationship)
                    {
                        if (startDate.HasValue == false)
                            throw new Exception("$T fghgfh");
                        date = startDate.Value;
                    }
                    Helper.AddSpouse(parent, person.Parents.First() as Person, ExistenceState.Current, date);
                    break;
            }

            // Handle siblings
            if (person.FullSiblings.Count() > 0)
            {
                // Make siblings the child of the new parent
                foreach (Person sibling in person.FullSiblings)
                {
                    Helper.AddChild(parent, sibling, ParentChildModifier.Natural);
                }
            }

            // Setter for property change notification
            person.HasParents = true;
            return parent;
        }

        /// <summary>
        /// Performs the business logic for adding the Spousal relationship between the person and the spouse.
        /// </summary>
        public static Person AddSpouse(Person person, Person spouse, ExistenceState modifier, DateTime startDate, DateTime? endDate = default)
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

            switch (person.Spouses.Count())
            {
                // No existing spouse
                case 0:
                    Helper.AddSpouse(person, spouse, modifier, startDate);

                    // Add any of the children as the child of the spouse.
                    if (person.Children != null || person.Children.Count() > 0)
                    {
                        foreach (Person child in person.Children)
                        {
                            Helper.AddChild(spouse, child, ParentChildModifier.Natural);
                        }
                    }
                    break;

                // Existing spouse(s)
                default:
                    // If specifying a new married spouse, make existing spouses former.
                    if (modifier == ExistenceState.Current)
                    {
                        foreach (Relationship relationship in person.Relationships)
                        {
                            if (relationship.RelationshipType == RelationshipType.Spouse)
                            {
                                ((SpouseRelationship)relationship).Existence = ExistenceState.Former;
                                ((SpouseRelationship)relationship).EndDate = endDate ?? throw new Exception("sdf sd f");
                            }
                        }
                    }

                    Helper.AddSpouse(person, spouse, modifier, startDate);
                    break;
            }
            // Setter for property change notification
            person.HasSpouse = true;
            return spouse;
        }

        /// <summary>
        /// Performs the business logic for adding the Sibling relationship between the person and the sibling.
        /// </summary>
        public static Person AddSibling(Person person, Person sibling)
        {
            // Handle siblings
            if (person.FullSiblings.Count() > 0)
            {
                // Connect the siblings to each other.
                foreach (Person existingSibling in person.FullSiblings)
                {
                    Helper.AddSibling(existingSibling as Person, sibling);
                }
            }

            if (person.Parents != null)
            {
                switch (person.Parents.Count())
                {
                    // No parents
                    case 0:
                        Helper.AddSibling(person, sibling);
                        break;

                    // Single parent
                    case 1:
                        Helper.AddSibling(person, sibling);
                        Helper.AddChild(person.Parents.First() as Person, sibling, ParentChildModifier.Natural);
                        break;

                    // 2 parents
                    case 2:
                        // Add the sibling as a child of the same parents
                        foreach (Person parent in person.Parents)
                        {
                            Helper.AddChild(parent, sibling, ParentChildModifier.Natural);
                        }

                        Helper.AddSibling(person, sibling);
                        break;

                    default:
                        Helper.AddSibling(person, sibling);
                        break;
                }
            }
            return sibling;
        }

        /// <summary>
        /// Return a list of children for the parent set.
        /// </summary>
        private static IEnumerable<Person> GetChildren(IParentSet parentSet)
        {
            // Get list of both parents.
            List<Person> firstParentChildren = new(parentSet.FirstParent.Children().Cast<Person>());
            List<Person> secondParentChildren = new(parentSet.SecondParent.Children().Cast<Person>());

            // Go through and add the children that have both parents.
            foreach (Person child in firstParentChildren)
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
        public static void UpdateSpouseStatus(Person person, Person spouse, ExistenceState modifier)
        {
            foreach (Relationship relationship in person.Relationships.Cast<Relationship>())
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).Existence = modifier;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).Existence = modifier;
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for updating the marriage date
        /// </summary>
        public static void UpdateMarriageDate(Person person, Person spouse, DateTime? dateTime)
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
        public static void UpdateDivorceDate(Person person, Person spouse, DateTime? dateTime)
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
        public static void ChangeParents(Person person, IParentSet newParentSet)
        {
            // Don't do anything if there is nothing to change or if the parents are the same
            if (person.ParentSet == null || newParentSet == null || person.ParentSet.Equals(newParentSet))
                return;

            // store the current parent set which will be removed
            IParentSet formerParentSet = person.ParentSet;

            // Remove the first parent
            RemoveParentChildRelationship(person, formerParentSet.FirstParent as Person);

            // Remove the person as a child of the second parent
            RemoveParentChildRelationship(person, formerParentSet.SecondParent as Person);

            // Remove the sibling relationships
            RemoveSiblingRelationships(person);

            // Add the new parents
            AddParent(person, newParentSet);

            /// <summary>
            /// Performs the business logic for adding the Parent relationship between the person and the parents.
            /// </summary>
            static void AddParent(Person person, IParentSet parentSet)
            {
                // First add child to parents.
                Helper.AddChild(parentSet.FirstParent as Person, person, ParentChildModifier.Natural);
                Helper.AddChild(parentSet.SecondParent as Person, person, ParentChildModifier.Natural);

                // Next update the siblings. Get the list of full siblings for the person.
                // A full sibling is a sibling that has both parents in common.
                foreach (Person sibling in GetChildren(parentSet))
                {
                    if (sibling != person)
                    {
                        Helper.AddSibling(person, sibling);
                    }
                }
            }
        }

        /// <summary>
        /// Helper function for removing sibling relationships
        /// </summary>
        private static void RemoveSiblingRelationships(Person person)
        {
            foreach (var rel in person.Relationships.Reverse())
            {
                if ((rel as Relationship).RelationshipType == RelationshipType.Sibling)
                {
                    person.Remove(rel);
                }
            }
        }

        /// <summary>
        /// Helper function for removing a parent relationship
        /// </summary>
        private static void RemoveParentChildRelationship(Person person, Person parent)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Parent && relationship.RelationTo.Equals(parent))
                {
                    person.Remove(relationship);
                    break;
                }
            }

            foreach (Relationship relationship in parent.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Child && relationship.RelationTo.Equals(person))
                {
                    parent.Remove(relationship);
                    break;
                }
            }
        }

        /// <summary>
        /// Performs the business logic for changing the deleting the person
        /// </summary>
        public static void DeletePerson(ICollection<Person> family, Person personToDelete)
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
                        (relationship.RelationTo as Person).Remove(rel);
                        break;
                    }
                }
            }

            // Delete the person's photos and story
            personToDelete.DeletePhotos();
            personToDelete.DeleteStory();

            family.Remove(personToDelete);
        }

        private static class Helper
        {
            private class Package
            {
                public Package(Relationship relationship, Person? person)
                {
                    Relationship = relationship;
                    Person = person;
                }

                public Relationship Relationship { get; }
                public Person? Person { get; }
            }

            /// <summary>
            /// Adds Parent-Child relationship between person and child with the provided parent-child relationship type.
            /// </summary>
            public static void AddChild(Person parent, Person child, ParentChildModifier parentChildType)
            {
                //add child relationship to person
                parent.Add(new ChildRelationship(child, parentChildType) { StartDate = child.Created });

                //add person as parent of child
                child.Add(new ParentRelationship(parent, parentChildType) { StartDate = child.Created });
            }

            /// <summary>
            /// Add Spouse relationship between the person and the spouse with the provided spouse relationship type.
            /// </summary>
            public static void AddSpouse(Person person, Person spouse, ExistenceState existence, DateTime startDate)
            {
                //assign spouses to each other
                person.Add(new SpouseRelationship(spouse, existence) { StartDate = startDate });
                spouse.Add(new SpouseRelationship(person, existence) { StartDate = startDate });
            }

            /// <summary>
            /// Adds sibling relation between the person and the sibling
            /// </summary>
            public static void AddSibling(Person person, Person sibling)
            {
                //assign sibling to each other
                var max = new DateTime(Math.Max(sibling.Created.Ticks, person.Created.Ticks));
                person.Add(new SiblingRelationship(sibling) { StartDate = max });
                sibling.Add(new SiblingRelationship(person) { StartDate = max });
            }
        }
    }
}