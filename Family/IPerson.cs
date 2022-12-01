using Abstractions;

namespace Microsoft.FamilyShowLib
{
    public interface IPerson
    {
        string this[string columnName] { get; }

        int? Age { get; }
        AgeGroup AgeGroup { get; }
        string Avatar { get; set; }
        EventBaptism Baptism { get; set; }
        DateTime? BirthDate { get; set; }
        string BirthDateAndPlace { get; }
        string BirthMonthAndDay { get; }
        string BirthPlace { get; set; }
        string ChildRelationshipText { get; }
        IEnumerable<Person> Children { get; }
        string ChildrenText { get; }
        Contact Contact { get; set; }
        DateTime Created { get; }
        IEnumerable<Person> CurrentSpouses { get; }
        DateTime? DeathDate { get; set; }
        string DeathPlace { get; set; }
        string Error { get; }
        Person Father { get; }
        string FirstName { get; set; }
        string FullName { get; }
        IEnumerable<Person> FullSiblings { get; }
        Gender Gender { get; set; }
        IEnumerable<Person> HalfSiblings { get; }
        bool HasAvatar { get; }
        bool HasParents { get; set; }
        bool HasSpouse { get; set; }
        string Id { get; set; }
        bool IsDeletable { get; }
        bool IsLiving { get; set; }
        string Key { get; }
        string LastName { get; set; }
        IEnumerable<IRelationship> ListSpousesRelationShip { get; }
        string MarriedName { get; set; }
        string MiddleName { get; set; }
        Person Mother { get; }
        string Name { get; }
        string NickName { get; set; }
        string ParentRelationshipText { get; }
        IEnumerable<Person> Parents { get; }
        IParentSet? ParentSet { get; }
        string ParentsText { get; }
        PhotoCollection Photos { get; }
        ParentSetCollection PossibleParentSets { get; }
        IEnumerable<Person> PreviousSpouses { get; }
        IEnumerable<IRelationship> Relationships { get; }
        string SiblingRelationshipText { get; }
        IEnumerable<Person> Siblings { get; }
        string SiblingsText { get; }
        string SpouseRelationshipText { get; }
        IEnumerable<Person> Spouses { get; }
        string SpousesText { get; }
        Story Story { get; set; }
        string Suffix { get; set; }
        string YearOfBirth { get; }
        string YearOfDeath { get; }

        void DeletePhotos();

        void DeleteStory();
    }
}