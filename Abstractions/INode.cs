namespace Abstractions
{

    public interface INode
    {
        public string Key { get; }
        IEnumerable<IRelationship> Relationships { get; }
        IEnumerable<INode> Children { get; }
        IEnumerable<INode> Spouses { get; }
        IEnumerable<INode> Parents { get; }
        IEnumerable<INode> Siblings { get; }
        void Add(IRelationship relationship);
        void Remove(IRelationship relationship);

        //IEnumerable<INode> FullSiblings { get; }
      //  //IEnumerable<INode> PreviousSpouses { get; }
        //IEnumerable<INode> CurrentSpouses { get; }
        //IEnumerable<INode> HalfSiblings { get; }


        //IParentSet ParentSet { get; }
        //public string Id { get; set; }

        //string Name { get; }
        //string FullName { get; }
        //DateTime? BirthDate { get; }
        //Gender Gender { get; set; }
        //bool HasSpouse { get; set; }
        //bool IsLiving { get; }
        //int? Age { get; }
        //DateTime? DeathDate { get; }
    }


 
}
