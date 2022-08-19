using Abstractions;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Models
{
    public class Model : INotifyPropertyChanged, IEquatable<Model>, INode
    {
        private List<IRelationship> relationships = new();
        public readonly DateTime? Created;


        public Model(string key)
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
            return base.Equals(obj as Model);
        }

        public bool Equals(Model? other)
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



    }
}