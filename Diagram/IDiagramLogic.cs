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

        //EventHandler NodeClickHandler { get; set; }
   

        EventHandler<ContentChangedEventArgs> ContentChanged { get; set; }
        EventHandler CurrentChanged { get; set; }

        //List<DiagramConnectorNode> DiagramConnectorNodes { get; }
        //List<DiagramConnector> Connections { get; }

        double DisplayYear { set; }
        //double MinimumYear { get; }
        object Current { get; set; }

        void Clear();       
        DiagramConnectorNode GetDiagramConnectorNode(object person);
        IEnumerable<DiagramRow> GenerateRows();
    }
}