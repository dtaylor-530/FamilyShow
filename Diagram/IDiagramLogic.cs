/*
 * Contains the logic to populate the diagram. Populates rows with 
 * groups and nodes based on the node relationships. 
*/

using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.FamilyShow.Controls.Diagram
{

    public class ContentChangedEventArgs : EventArgs
    {
        private object newPerson;

        public object NewPerson
        {
            get { return newPerson; }
        }

        public ContentChangedEventArgs(object newPerson)
        {
            this.newPerson = newPerson;
        }        
    }

    public interface IDiagramLogic
    {
        List<DiagramConnector> Connections { get; }
        double DisplayYear { set; }
        //PeopleCollection Family { get; }
        double MinimumYear { get; }
        EventHandler NodeClickHandler { get; set; }
        Dictionary<object, DiagramConnectorNode> PersonLookup { get; }
        EventHandler<ContentChangedEventArgs> ContentChanged { get; set; }
        EventHandler CurrentChanged { get; set; }
        object Current { get; set; }
        //object Family { get; }

        void Clear();
        //DiagramRow CreateChildrenRow(List<object> children, double scale, double scaleRelated);
        //DiagramRow CreateParentRow(System.Collections.ObjectModel.Collection<object> parents, double scale, double scaleRelated);
        //DiagramRow CreatePrimaryRow(object person, double scale, double scaleRelated);
        DiagramNode GetDiagramNode(object person);
        Rect GetNodeBounds(object person);
        void UpdateDiagram(Diagram diagram);
    }
}