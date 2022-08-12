using Abstractions;
using Diagram.Logic;
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
            var personLookup = new Dictionary<object, DiagramConnectorNode>();
            model = new DiagramLogic(family, new DiagramFactory(personLookup, new NodeConverter(), new ConnectorConverter()), personLookup);
            family.CollectionChanged += Family_CollectionChanged;
            family.Current = new Person();
            family.Add(family as INode);
            family.AddChild(family.Current as Person,  new Person());
            family.AddChild(family.Current as Person, new Person() { });
            InitializeComponent();
            DiagramView1.Logic = model;
        }

        private void Family_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DiagramView1?.TheDiagram.Populate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            family.AddChild(family.Current as Person, new Person());
            //DiagramView1.TheDiagram.OnFamilyCurrentChanged(default, default);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            family.AddSpouse(family.Current as Person, new Person(), SpouseModifier.Current);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            family.AddParent(family.Current as Person, new Person());
        }
    }
}
