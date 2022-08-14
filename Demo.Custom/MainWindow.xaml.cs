using Abstractions;
using Diagram.Logic;
using Microsoft.FamilyShow.Controls.Diagram;
using Microsoft.FamilyShowLib;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Demo.Custom.Infrastructure;
using Demo;

namespace Demo.Custom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Infrastructure.Models family = new Infrastructure.Models();
        private DiagramLogic model;
        int i = 'A';
        public MainWindow()
        {
            InitializeComponent();
        
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var ModelLookup = new Dictionary<object, DiagramConnectorNode>();
            var factory = new DiagramFactory(ModelLookup, new NodeConverter(), new ConnectorConverter());
            factory.CurrentNode += Factory_CurrentNode;
            model = new DiagramLogic(family, factory, ModelLookup);
           // family.CollectionChanged += Family_CollectionChanged;
            family.Current = new Model(name);
            family.Add(family.Current as Model);
            family.Add(RelationshipHelper.AddChild(family.Current as Model, new Model(name)));
            family.Add(RelationshipHelper.AddChild(family.Current as Model, new Model(name)));

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
            CreateAndAddModel(() => RelationshipHelper
                        .AddChild(family.Current as Model, new Model(name)));           

            //DiagramView1.TheDiagram.OnFamilyCurrentChanged(default, default);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(() => RelationshipHelper
               .AddSpouse(family.Current as Model, new Model(name), new DateTime(2020, 2, 2)));
                
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(()=>RelationshipHelper
            .AddParent(family.Current as Model, new Model(name), new DateTime(2020, 2, 2)));
        }
        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            CreateAndAddModel(() => RelationshipHelper
           .AddSibling(family.Current as Model, new Model(name)));
        }

        private Model CreateAndAddModel(Func<Model> func)
        {
            var Model = func();
            family.Add(Model);
            return Model;
        }

        string name => ((char)i++).ToString();

    }
}
