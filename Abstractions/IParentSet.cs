using Abstractions;

namespace Microsoft.FamilyShowLib
{
    public interface IParentSet
    {
        INode FirstParent { get; set; }
        string Name { get; }
        INode SecondParent { get; set; }

        bool Equals(IParentSet other);
    }
}