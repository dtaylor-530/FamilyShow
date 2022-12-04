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
        RelationshipType Type { get; }
        INode To { get; }
        INode From { get; }
        DateTime Start { get; }
        DateTime? End { get; }
    }
}