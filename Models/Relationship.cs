using Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Models
{ 
    /// <summary>
    /// Describes the kinship between person objects
    /// </summary>
    [Serializable]
    public abstract class Relationship : IRelationship
    {
        private RelationshipType relationshipType;

        private Model relationTo;

        // The person's Id will be serialized instead of the relationTo person object to avoid
        // circular references during Xml Serialization. When family data is loaded, the corresponding
        // person object will be assigned to the relationTo property (please see app.xaml.cs).
        //private string personId;

        // Store the person's name with the Id to make the xml file more readable
        //private string personFullname;

        /// <summary>
        /// The Type of relationship.  Parent, child, sibling, or spouse
        /// </summary>
        public RelationshipType RelationshipType
        {
            get { return relationshipType; }
            set { relationshipType = value; }
        }

        /// <summary>
        /// The person id the relationship is to. See comment on personId above.
        /// </summary>
        [XmlIgnore]
        public INode RelationTo
        {
            get { return relationTo; }
            set
            {
                relationTo = value as Model;
                //personId = ((IPerson)value).Id;
                //personFullname = ((IPerson)value).FullName;
            }
        }

        public Existence Existence { get; set; }

        // public IPerson Model { get; set; }

        //public string PersonId
        //{
        //    get { return personId; }
        //    set { personId = value; }
        //}

        //public string PersonFullname
        //{
        //    get { return personFullname; }
        //    set { personFullname = value; }
        //}

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }



    /// <summary>
    /// Collection of relationship for a person object
    /// </summary>
    [Serializable]
    public class RelationshipCollection : ObservableCollection<IRelationship> { }


    /// <summary>
    /// Describes the kinship between a child and parent.
    /// </summary>
    [Serializable]
    public class ParentRelationship : Relationship
    {
        // Paramaterless constructor required for XML serialization
        public ParentRelationship() { }

        public ParentRelationship(Model personId)
        {
            RelationshipType = RelationshipType.Parent;
            RelationTo = personId;   
        }

        public override string ToString()
        {
            return (RelationTo as Model).Key;
        }
    }

    /// <summary>
    /// Describes the kindship between a parent and child.
    /// </summary>
    [Serializable]
    public class ChildRelationship : Relationship
    {
        // Paramaterless constructor required for XML serialization
        public ChildRelationship() { }

        public ChildRelationship(Model person)
        {
            RelationshipType = RelationshipType.Child;
            RelationTo = person;
        }
    }

    /// <summary>
    /// Describes the kindship between a couple
    /// </summary>
    [Serializable]
    public class SpouseRelationship : Relationship
    {
        private DateTime? marriageDate;
        private DateTime? divorceDate;

        public DateTime? MarriageDate
        {
            get { return marriageDate; }
            set { marriageDate = value; }
        }

        public DateTime? DivorceDate
        {
            get { return divorceDate; }
            set { divorceDate = value; }
        }

        // Paramaterless constructor required for XML serialization
        public SpouseRelationship() { }

        public SpouseRelationship(Model person)
        {
            RelationshipType = RelationshipType.Spouse;
            RelationTo = person;
        }
    }

    /// <summary>
    /// Describes the kindship between a siblings
    /// </summary>
    [Serializable]
    public class SiblingRelationship : Relationship
    {
        // Paramaterless constructor required for XML serialization
        public SiblingRelationship() { }

        public SiblingRelationship(Model person)
        {
            RelationshipType = RelationshipType.Sibling;
            RelationTo = person;
        }
    }




}