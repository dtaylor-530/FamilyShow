using Abstractions;
using Family;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Microsoft.FamilyShowLib
{
    #region Relationship classes

    /// <summary>
    /// Describes the kinship between person objects
    /// </summary>
    [Serializable]
    public abstract class Relationship : IRelationship
    {
        private RelationshipType relationshipType;

        private INode? relationTo;
        private INode? relationFrom;

        public Relationship()
        {
        }

        // The person's Id will be serialized instead of the relationTo person object to avoid
        // circular references during Xml Serialization. When family data is loaded, the corresponding
        // person object will be assigned to the relationTo property (please see app.xaml.cs).
        //private string personId;

        // Store the person's name with the Id to make the xml file more readable
        //private string personFullname;

        /// <summary>
        /// The Type of relationship.  Parent, child, sibling, or spouse
        /// </summary>
        public RelationshipType Type
        {
            get { return relationshipType; }
            set { relationshipType = value; }
        }

        /// <summary>
        /// The person id the relationship is to. See comment on personId above.
        /// </summary>
        [XmlIgnore]
        public INode To
        {
            get { return relationTo; }
            set
            {
                relationTo = value;
                //personId = ((IPerson)value).Id;
                //personFullname = ((IPerson)value).FullName;
            }
        }   
        
        [XmlIgnore]
        public INode From
        {
            get { return relationFrom; }
            set
            {
                relationFrom = value;
                //personId = ((IPerson)value).Id;
                //personFullname = ((IPerson)value).FullName;
            }
        }

        public ExistenceState Existence { get; set; } = ExistenceState.Current;

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }
    }

    /// <summary>
    /// Describes the kinship between a child and parent.
    /// </summary>
    [Serializable]
    public class ParentRelationship : Relationship
    {
        // The ParentChildModifier are not currently used by the application
        private ParentChildModifier parentChildModifier;

        public ParentChildModifier ParentChildModifier
        {
            get { return parentChildModifier; }
            set { parentChildModifier = value; }
        }

        // Paramaterless constructor required for XML serialization
        public ParentRelationship()
        { }

        public ParentRelationship(INode personId, ParentChildModifier parentChildType)
        {
            Type = RelationshipType.Parent;
            To = personId;
            parentChildModifier = parentChildType;
        }

        public override string ToString()
        {
            return (To as Person).Name;
        }
    }

    /// <summary>
    /// Describes the kindship between a parent and child.
    /// </summary>
    [Serializable]
    public class ChildRelationship : Relationship
    {
        // The ParentChildModifier are not currently used by the application
        private ParentChildModifier parentChildModifier;

        public ParentChildModifier ParentChildModifier
        {
            get { return parentChildModifier; }
            set { parentChildModifier = value; }
        }

        // Paramaterless constructor required for XML serialization
        public ChildRelationship()
        { }

        public ChildRelationship(INode person, ParentChildModifier parentChildType)
        {
            Type = RelationshipType.Child;
            To = person;
            parentChildModifier = parentChildType;
        }
    }

    /// <summary>
    /// Describes the kindship between a couple
    /// </summary>
    [Serializable]
    public class SpouseRelationship : Relationship
    {
        private string marriagePlace;

        // Paramaterless constructor required for XML serialization
        public SpouseRelationship()
        {
        }

        public string MarriagePlace
        {
            get { return marriagePlace; }
            set { marriagePlace = value; }
        }

        public SpouseRelationship(Person person, ExistenceState spouseType)
        {
            Type = RelationshipType.Spouse;
            Existence = spouseType;
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

    #endregion Relationship classes

    #region Relationships collection

    /// <summary>
    /// Collection of relationship for a person object
    /// </summary>
    [Serializable]
    public class RelationshipCollection : ObservableCollection<IRelationship>
    { }

    #endregion Relationships collection
}