using Abstractions;
using Microsoft.FamilyShowLib;
using System;
using System.Windows.Input;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public class App
    {
        public static People FamilyCollection = new People();
        public static PeopleCollection<INode> Family = FamilyCollection.PeopleCollection;
    }
}
