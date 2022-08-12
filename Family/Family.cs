/*
 * Exports data from the People collection to a GEDCOM file.
 * 
 * More information on the GEDCOM format is at http://en.wikipedia.org/wiki/Gedcom.
 * 
 * GedcomExport class
 * Exports data from a Person collection to a GEDCOM file.
 *
 * GedcomIdMap
 * Maps a Person's ID (GUID) to a GEDCOM ID (int).
 * 
 * FamilyMap
 * Creates a list of GEDCOM family groups from the People collection.
 * 
 * Family
 * One family group in the FamilyMap list.
 * 
*/

using Abstractions;
using System.Collections.Generic;

namespace Microsoft.FamilyShowLib
{
  /// <summary>
  /// One family group. 
  /// </summary>
  class Family
  {
    #region fields

    private INode parentLeft;
    private INode parentRight;
    private IRelationship relationship;
    private List<INode> children = new List<INode>();

    #endregion

    /// <summary>
    /// Get the left-side parent.
    /// </summary>
    public INode ParentLeft
    {
      get { return parentLeft; }
    }

    /// <summary>
    /// Get the right-side parent.
    /// </summary>
    public INode ParentRight
    {
      get { return parentRight; }
    }

    /// <summary>
    /// Get or set the relationship for the two parents.
    /// </summary>
    public IRelationship Relationship
    {
      get { return relationship; }
      set { relationship = value; }
    }

    /// <summary>
    /// Get the list of children.
    /// </summary>
    public List<INode> Children
    {
      get { return children; }
    }

    public Family(INode parentLeft, INode parentRight)
    {
      this.parentLeft = parentLeft;
      this.parentRight = parentRight;
    }
  }
}