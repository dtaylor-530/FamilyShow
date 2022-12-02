namespace Abstractions
{
    public enum RelationshipType
    {
        // one parent node
        Child,
        // one child node
        Parent,
        // two or more child nodes; one parent
        Sibling,
        // two or more parent nodes; one child
        Spouse,
    }


    public interface IRelationship
    {
        RelationshipType RelationshipType { get; }
        INode From { get; }
        INode To { get; }
        DateTime Start { get; }
        DateTime? End { get; }
    }
}