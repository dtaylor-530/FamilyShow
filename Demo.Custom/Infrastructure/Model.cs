using Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Models
{
    public class Model : INotifyPropertyChanged, IEquatable<Model>, INodeRelationshipEditor
    {
        private List<IRelationship> relationships = new();

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

        public DateTime Created => DateTime.Now;

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