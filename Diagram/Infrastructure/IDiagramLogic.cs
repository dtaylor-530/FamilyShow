/*
 * Contains the logic to populate the diagram. Populates rows with
 * groups and nodes based on the node relationships.
*/

using Abstractions;
using System;
using System.Collections.Generic;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public class ContentChangedEventArgs : EventArgs
    {
        private INode newPerson;

        public ContentChangedEventArgs(INode newPerson)
        {
            this.newPerson = newPerson;
        }

        public INode NewPerson
        {
            get { return newPerson; }
        }
    }

    public interface IDiagramLogic
    {
        EventHandler<ContentChangedEventArgs> ContentChanged { get; set; }
        EventHandler CurrentChanged { get; set; }

        //List<DiagramConnectorNode> DiagramConnectorNodes { get; }
        //List<DiagramConnector> Connections { get; }

        double DisplayYear { set; }

        //double MinimumYear { get; }
        INode Current { get; }

        void Clear();

        DiagramConnectorNode GetDiagramConnectorNode(INode person);

        IEnumerable<DiagramRow> GenerateRows();
    }
}