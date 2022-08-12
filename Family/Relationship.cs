using Abstractions;
using System;
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
                //personId = ((IPerson)value).Id;
                //personFullname = ((IPerson)value).FullName;
            }
        }

        public SpouseModifier SpouseModifier { get; }

        // public IPerson Person { get; set; }

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
    public ParentRelationship() { }

    public ParentRelationship(INode personId, ParentChildModifier parentChildType)
    {
      RelationshipType = RelationshipType.Parent;
      RelationTo = personId;
      parentChildModifier = parentChildType;
    }

    public override string ToString()
    {
      return RelationTo.Name;
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
    public ChildRelationship() { }

    public ChildRelationship(INode person, ParentChildModifier parentChildType)
    {
      RelationshipType = RelationshipType.Child;
      RelationTo = person;
      parentChildModifier = parentChildType;
    }
  }

  /// <summary>
  /// Describes the kindship between a couple
  /// </summary>
  [Serializable]
  public class SpouseRelationship : Relationship
  {
    private SpouseModifier spouseModifier;
    private DateTime? marriageDate;
    private DateTime? divorceDate;
    private String marriagePlace;

    public SpouseModifier SpouseModifier
    {
      get { return spouseModifier; }
      set { spouseModifier = value; }
    }

    //public DateTime? StartDate
    //{
    //  get { return marriageDate; }
    //  set { marriageDate = value; }
    //}

    //public DateTime? EndDate
    //{
    //  get { return divorceDate; }
    //  set { divorceDate = value; }
    //}
    public String MarriagePlace
    {
      get { return marriagePlace; }
      set { marriagePlace = value; }
    }


    // Paramaterless constructor required for XML serialization
    public SpouseRelationship() { }

    public SpouseRelationship(INode person, SpouseModifier spouseType)
    {
      RelationshipType = RelationshipType.Spouse;
      spouseModifier = spouseType;
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

    public SiblingRelationship(INode person)
    {
      RelationshipType = RelationshipType.Sibling;
      RelationTo = person;
    }
  }

  #endregion

  #region Relationships collection

  /// <summary>
  /// Collection of relationship for a person object
  /// </summary>
  [Serializable]
  public class RelationshipCollection : ObservableCollection<IRelationship> { }

  #endregion

  #region Relationship Type/Modifier Enums

  /// <summary>
  /// Enumeration of connection types between person objects
  /// </summary>
  //public enum RelationshipType
  //{
  //  Child,
  //  Parent,
  //  Sibling,
  //  Spouse
  //}

  //public enum SpouseModifier
  //{
  //  Current,
  //  Former
  //}

  //public enum ParentChildModifier
  //{
  //  Natural,
  //  Adopted,
  //  Foster
  //}

  #endregion
}