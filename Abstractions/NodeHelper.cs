namespace Abstractions
{
    public static class NodeHelper
    {
        public static IEnumerable<INode> Children(this INode node) => node.Relations(RelationshipType.Child);

        public static IEnumerable<INode> Spouses(this INode node) => node.Relations(RelationshipType.Spouse);

        public static IEnumerable<INode> Parents(this INode node) => node.Relations(RelationshipType.Parent);

        public static IEnumerable<INode> Siblings(this INode node) => node.Relations(RelationshipType.Sibling);

        public static IEnumerable<INode> Relations(this INode node, RelationshipType relationshipType)
        {
            return Relationships(relationshipType).Select(a => a.To);

            IEnumerable<IRelationship> Relationships(RelationshipType relationshipType)
            {
                foreach (IRelationship relationship in node.Relationships)
                {
                    if (relationship.RelationshipType == relationshipType)
                    {
                        yield return (relationship);
                    }
                }
            }
        }

        public static IEnumerable<IRelationship> Relationships(this INode node, RelationshipType relationshipType)
        {
            foreach (IRelationship relationship in node.Relationships)
            {
                if (relationship.RelationshipType == relationshipType)
                {
                    yield return (relationship);
                }
            }
        }
    }
}