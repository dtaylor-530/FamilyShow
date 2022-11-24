namespace Abstractions
{
    public enum RelationshipType
    {
        Child,
        Parent,
        Sibling,
        Spouse
    }

    public enum ExistenceState
    {
        Current,
        Former
    }

    public interface IRelationship
    {
        RelationshipType RelationshipType { get; }
        INode RelationTo { get; }
        ExistenceState Existence { get; }
        DateTime? StartDate { get; }
        DateTime? EndDate { get; }
    }
}