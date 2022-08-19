using Abstractions;

namespace Microsoft.FamilyShowLib
{
    public interface IParentSet
    {
        INode FirstParent { get; }
        string Name { get; }
        INode SecondParent { get;  }

        bool Equals(IParentSet other);
    }
}