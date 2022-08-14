using Abstractions;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Models
{
    public class Model : INotifyPropertyChanged, IEquatable<Model>, INode
    {
        private List<Relationship> relationships = new();
        public readonly DateTime? Created;


        public Model(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public void Add(Relationship relationship)
        {
            relationships.Add(relationship);
        }
        public void Remove(Relationship relationship)
        {
            relationships.Remove(relationship);
        }

        public IEnumerable<IRelationship> Relationships => relationships;
        public IEnumerable<INode> Children => Relations(RelationshipType.Child);
        public IEnumerable<INode> Spouses =>Relations(RelationshipType.Spouse);
        public IEnumerable<INode> Parents => Relations(RelationshipType.Parent);
        public IEnumerable<INode> Siblings => Relations(RelationshipType.Sibling);
    
        public event PropertyChangedEventHandler? PropertyChanged;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj as Model);
        }

        public bool Equals(Model? other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        private IEnumerable<Model> Relations(RelationshipType relationshipType)
        {
            return Relationships(relationshipType).Select(a => a.RelationTo).Cast<Model>();

            IEnumerable<Relationship> Relationships(RelationshipType relationshipType)
            {
                foreach (Relationship relationship in relationships)
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