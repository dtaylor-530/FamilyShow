using Abstractions;
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

        private INode relationTo;

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
                relationTo = value;
            }
        }

        public ExistenceState Existence { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Collection of relationship for a person object
    /// </summary>
    [Serializable]
    public class RelationshipCollection : ObservableCollection<IRelationship>
    { }

    /// <summary>
    /// Describes the kinship between a child and parent.
    /// </summary>
    [Serializable]
    public class ParentRelationship : Relationship
    {
        // Paramaterless constructor required for XML serialization
        public ParentRelationship()
        { }

        public ParentRelationship(INode personId)
        {
            RelationshipType = RelationshipType.Parent;
            RelationTo = personId;
        }
    }

    /// <summary>
    /// Describes the kindship between a parent and child.
    /// </summary>
    [Serializable]
    public class ChildRelationship : Relationship
    {
        // Paramaterless constructor required for XML serialization
        public ChildRelationship()
        { }

        public ChildRelationship(INode person)
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
        public SpouseRelationship()
        { }

        public SpouseRelationship(INode person)
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
        public SiblingRelationship()
        { }

        public SiblingRelationship(INode person)
        {
            RelationshipType = RelationshipType.Sibling;
            RelationTo = person;
        }
    }
}