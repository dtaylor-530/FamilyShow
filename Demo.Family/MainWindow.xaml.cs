using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Demo.Family
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private People family = new People();
        private DiagramLogic model;
        int i = 'A';
        public MainWindow()
        {
            InitializeComponent();
        
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var personLookup = new Dictionary<object, DiagramConnectorNode>();
            var factory = new DiagramFactory(personLookup, new NodeConverter(), new ConnectorConverter());
            factory.CurrentNode += Factory_CurrentNode;
            model = new DiagramLogic(family, factory, personLookup);
           // family.CollectionChanged += Family_CollectionChanged;
            family.Current = new Person() { FirstName = name };
            family.Add(family.Current as Person);
            family.Add(RelationshipHelper.AddChild(family.Current as Person, new Person() { FirstName = name }));
            family.Add(RelationshipHelper.AddChild(family.Current as Person, new Person() { FirstName = name }));

            DiagramView1.Logic = model;
            Diagram.Logic = model;
            ContentControl.Content = family.Current;
        }

        private void Factory_CurrentNode(object obj)
        {
            family.Current = obj as INotifyPropertyChanged;
            ContentControl.Content = obj;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateAndAddPerson(() => RelationshipHelper
                        .AddChild(family.Current as Person, new Person() { FirstName = name }));           

            //DiagramView1.TheDiagram.OnFamilyCurrentChanged(default, default);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            CreateAndAddPerson(() => RelationshipHelper
               .AddSpouse(family.Current as Person, new Person() { FirstName = name }, Existence.Current, new DateTime(2020, 2, 2)));
                
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            CreateAndAddPerson(()=>RelationshipHelper
            .AddParent(family.Current as Person, new Person() { FirstName = name }, new DateTime(2020, 2, 2)));
        }
        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            CreateAndAddPerson(() => RelationshipHelper
           .AddSibling(family.Current as Person, new Person() { FirstName = name }));
        }

        private Person CreateAndAddPerson(Func<Person> func)
        {
            var person = func();
            family.Add(person);
            return person;
        }

        string name => ((char)i++).ToString();

    }
}
