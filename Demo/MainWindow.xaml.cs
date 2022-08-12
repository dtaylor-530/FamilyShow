using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Microsoft.FamilyShowLib.People family = new Microsoft.FamilyShowLib.People();
        private DiagramLogic model;

        public MainWindow()
        {
            var personLookup = new Dictionary<object, DiagramConnectorNode>();
            model = new DiagramLogic(family, new DiagramFactory(personLookup, new NodeConverter(), new ConnectorConverter()), personLookup);
            family.CollectionChanged += Family_CollectionChanged;
            family.Current = new Person();
            family.Add(family.Current as Person);
            family.AddRange(family.AddChild(family.Current as Person,  new Person()));
            family.AddRange(family.AddChild(family.Current as Person, new Person() { }));
            InitializeComponent();
            DiagramView1.Logic = model;
        }

        private void Family_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DiagramView1?.TheDiagram.Populate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            family.AddRange(
                family
                .AddChild(family.Current as Person, new Person())
                .Where(a => a != default));

            //DiagramView1.TheDiagram.OnFamilyCurrentChanged(default, default);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            family.AddRange(
                family
                .AddSpouse(family.Current as Person, new Person(), SpouseModifier.Current, new DateTime(2020, 2, 2))
                .Where(a => a != default));            
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            family.AddRange(
                family
                .AddParent(family.Current as Person, new Person(), new DateTime(2020, 2, 2))
                .Where(a=>a!=default));
        }
    }
}
