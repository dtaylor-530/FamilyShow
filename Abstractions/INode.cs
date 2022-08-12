using Microsoft.FamilyShowLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{

    public interface INode
    {
        public string Id { get; set; }
        string Name { get; }
        string FullName { get; }
        DateTime? BirthDate { get; }
        ObservableCollection<IRelationship> Relationships { get; }

        IEnumerable<INode> Children { get; }
        IEnumerable<INode> Spouses { get; }
        IEnumerable<INode> Parents { get; }
        IEnumerable<INode> Siblings { get; }
        bool HasParents { get; set; }
        IEnumerable<INode> PreviousSpouses { get; }
        IEnumerable<INode> CurrentSpouses { get; }
        IEnumerable<INode> HalfSiblings { get; }
        IParentSet ParentSet { get; }
        Gender Gender { get; set; }
        bool HasSpouse { get; set; }
    }

    public enum Gender
    {
        Male, Female
    }

    public enum RelationshipType
    {
        Child,
        Parent,
        Sibling,
        Spouse
    }
    public enum SpouseModifier
    {
        Current,
        Former
    }

    public enum ParentChildModifier
    {
        Natural,
        Adopted,
        Foster
    }
}
