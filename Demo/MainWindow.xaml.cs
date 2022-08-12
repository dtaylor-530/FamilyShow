using Abstractions;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PeopleCollection<INode> family = new PeopleCollection<INode>();
        private DiagramLogic model;

        public MainWindow()
        {
            model = new DiagramLogic(family);
            
            family.Current = new Person();
            family.Add(family.Current);
            family.AddChild(family.Current,  new Person(), ParentChildModifier.Natural);
            family.AddChild(family.Current, new Person() { }, ParentChildModifier.Natural);
            InitializeComponent();
            DiagramView1.Logic = model;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            family.AddChild(family.Current, new Person(), ParentChildModifier.Adopted);
            DiagramView1.TheDiagram.OnFamilyCurrentChanged(default, default);
            //model.primaryRowSubject.OnNext(new System.Reactive.Unit());
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
        }
    }

    static class Helper
    {
        #region Add new people / relationships

        /// <summary>
        /// Adds Parent-Child relationship between person and child with the provided parent-child relationship type.
        /// </summary>
        public static void AddChild(this PeopleCollection<INode> people, INode parent, Person child, ParentChildModifier parentChildType)
        {
            //add child relationship to person
            parent.Relationships.Add(new ChildRelationship(child, parentChildType));

            //add person as parent of child
            child.Relationships.Add(new ParentRelationship(parent, parentChildType));

            //add the child to the main people list
            if (!people.Contains(child))
            {
                people.Add(child);
            }
        }

        /// <summary>
        /// Add Spouse relationship between the person and the spouse with the provided spouse relationship type.
        /// </summary>
        public static void AddSpouse(this PeopleCollection<INode> people, INode person, Person spouse, SpouseModifier spouseType)
        {
            //assign spouses to each other    
            person.Relationships.Add(new SpouseRelationship(spouse, spouseType));
            spouse.Relationships.Add(new SpouseRelationship(person, spouseType));

            //add the spouse to the main people list
            if (!people.Contains(spouse))
            {
                people.Add(spouse);
            }
        }

        /// <summary>
        /// Adds sibling relation between the person and the sibling
        /// </summary>
        public static void AddSibling(this PeopleCollection<INode> people, Person person, Person sibling)
        {
            //assign sibling to each other    
            person.Relationships.Add(new SiblingRelationship(sibling));
            sibling.Relationships.Add(new SiblingRelationship(person));

            //add the sibling to the main people list
            if (!people.Contains(sibling))
            {
                people.Add(sibling);
            }
        }

        #endregion
    }
}
