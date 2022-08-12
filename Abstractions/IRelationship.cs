namespace Abstractions
{
    public interface IRelationship
    {
        //string PersonFullname { get; set; }
        //IPerson Person { get; set; }
        RelationshipType RelationshipType { get; set; }
        INode RelationTo { get; set; }
        SpouseModifier SpouseModifier { get; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }

}
