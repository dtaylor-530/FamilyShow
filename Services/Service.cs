using Abstractions;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Models
{
    public class Service : INotifyPropertyChanged, IEquatable<Service>, INode
    {
        private List<IRelationship> relationships = new();
        public readonly DateTime? Created;


        public Service(string key)
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
        public IEnumerable<INode> Children => this.Children();
        public IEnumerable<INode> Spouses => this.Spouses();
        public IEnumerable<INode> Parents => this.Parents();
        public IEnumerable<INode> Siblings => this.Siblings();

        public event PropertyChangedEventHandler? PropertyChanged;

        public override bool Equals(object? obj)
{
            return base.Equals(obj as Service);
}

        public bool Equals(Service? other)
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

        private IEnumerable<Service> Relations(RelationshipType relationshipType)
        {
            return Relationships(relationshipType).Select(a => a.RelationTo).Cast<Service>();

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