using Abstractions;
using System.ComponentModel;

namespace Models
{
    public class Node : INotifyPropertyChanged, IEquatable<Node>, INode
    {
        private List<IRelationship> relationships = new();

        public Node(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public void Add(IRelationship relationship)
        {
            if (relationships.Any(a => a.RelationshipType == relationship.RelationshipType && a.RelationTo == relationship.RelationTo))
            {
                throw new Exception("vdf fd ");
            }
            relationships.Add(relationship);
        }

        public void Remove(IRelationship relationship)
        {
            relationships.Remove(relationship);
        }

        public IEnumerable<IRelationship> Relationships => relationships;

        public DateTime Created => DateTime.Now;

        //public IEnumerable<INode> Children => this.Children();
        //public IEnumerable<INode> Spouses => this.Spouses();
        //public IEnumerable<INode> Parents => this.Parents();
        //public IEnumerable<INode> Siblings => this.Siblings();

        public event PropertyChangedEventHandler? PropertyChanged;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj as Node);
        }

        public bool Equals(Node? other)
        {
            return Key == other?.Key;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        private IEnumerable<Node> Relations(RelationshipType relationshipType)
        {
            return Relationships(relationshipType).Select(a => a.RelationTo).Cast<Node>();

            IEnumerable<IRelationship> Relationships(RelationshipType relationshipType)
            {
                foreach (IRelationship relationship in relationships)
                {
                    if (relationship.RelationshipType == relationshipType)
                    {
                        yield return (relationship);
                    }
                }
            }
        }
    }
}