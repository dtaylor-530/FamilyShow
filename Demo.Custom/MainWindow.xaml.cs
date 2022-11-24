using Abstractions;
using Demo.Custom.Infrastructure;
using Diagram.Logic;
using Microsoft.FamilyShow;
using Microsoft.FamilyShow.Controls.Diagram;
using Models;
using Relationships;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows;

namespace Demo.Custom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Infrastructure.Models family;
        private DiagramLogic model;
        private static int i = 'A';

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            family = CreateFamily();
            model = DiagramLogic(family, out var observable);

            DiagramView1.Logic = model;
            //Diagram.Logic = model;
            ContentControl.Content = family.Current;
        }

        private static Infrastructure.Models CreateFamily()
        {
            var family = new Infrastructure.Models();
            family.Current = new Model(name);
            family.Add(family.Current as Model);
            family.Add(RelationshipHelper.AddChild(family.Current as Model, new Model(name)) as Model);
            family.Add(RelationshipHelper.AddChild(family.Current as Model, new Model(name)) as Model);
            return family;
        }

        private static DiagramLogic DiagramLogic(Infrastructure.Models family, out IObservable<object> current)
        {
            var currentChanges = new ReplaySubject<object>(1);
            var ModelLookup = new Dictionary<object, DiagramConnectorNode>();
            var factory = new DiagramFactory(ModelLookup, new NodeConverter(), new ConnectorConverter(), new NodeLimits());
            factory.CurrentNode += Factory_CurrentNode;
            var model = new DiagramLogic(family, factory, ModelLookup);
            current = currentChanges;
            return model;

            void Factory_CurrentNode(object obj)
            {
                family.Current = obj as INotifyPropertyChanged;
                currentChanges.OnNext(obj);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(() => RelationshipHelper.AddChild(family.Current as Model, new Model(name)));

            //DiagramView1.TheDiagram.OnFamilyCurrentChanged(default, default);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(() => RelationshipHelper
               .AddSpouse(family.Current as Model, new Model(name), new DateTime(2020, 2, 2)));
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(() => RelationshipHelper
            .AddParent(family.Current as Model, new Model(name), new DateTime(2020, 2, 2)));
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(() => RelationshipHelper
           .AddSibling(family.Current as Model, new Model(name)));
        }

        private Model CreateAndAddModel(Func<INode> func)
        {
            var Model = func() as Model;
            family.Add(Model);
            return Model;
        }

        private static string name => ((char)i++).ToString();
    }
}