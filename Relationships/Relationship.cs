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
        /// <summary>
        /// The Type of relationship.  Parent, child, sibling, or spouse
        /// </summary>
        public RelationshipType Type { get; set; }

        public INode From { get; set; }

        public INode To { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }
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
            Type = RelationshipType.Parent;
            To = personId;
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
            Type = RelationshipType.Child;
            To = person;
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
            Type = RelationshipType.Spouse;
            To = person;
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
            Type = RelationshipType.Sibling;
            To = person;
        }
    }
}