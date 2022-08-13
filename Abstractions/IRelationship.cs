namespace Abstractions
{
    public enum RelationshipType
    {
        Child,
        Parent,
        Sibling,
        Spouse
    }
    public enum Existence
    {
        Current,
        Former
    }


    public interface IRelationship
    {
        //string PersonFullname { get; set; }
        //IPerson Person { get; set; }
        RelationshipType RelationshipType { get;  }
        INode RelationTo { get;  }
        Existence Existence { get; }
        DateTime? StartDate { get;  }
        DateTime? EndDate { get;  }
    }

}
