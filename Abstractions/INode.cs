namespace Abstractions
{
    public interface INodeRelationshipEditor : IRelationshipEditor
    {
        public INode Node { get; }
    }

    public interface IRelationshipEditor
    {
        void Add(IRelationship relationship);

        void Remove(IRelationship relationship);
    }

    public interface INode
    {
        IEnumerable<IRelationship> Relationships { get; }

        DateTime Created { get; }
    }

    public interface IConnector
    {
    }
}