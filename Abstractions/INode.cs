using Microsoft.FamilyShowLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{

    public interface INode
    {
        IEnumerable<IRelationship> Relationships { get; }
        IEnumerable<INode> Children { get; }
        IEnumerable<INode> Spouses { get; }
        IEnumerable<INode> Parents { get; }
        IEnumerable<INode> Siblings { get; }
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
