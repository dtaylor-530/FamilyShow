/*
 * Contains the logic to populate the diagram. Populates rows with
 * groups and nodes based on the node relationships.
*/

using Abstractions;
using Diagrams.WPF;
using System;
using System.Collections.Generic;

namespace Diagrams.WPF.Infrastructure
{
    //public class ContentChangedEventArgs : EventArgs
    //{
    //    private INode newPerson;

    //    public ContentChangedEventArgs(INode newPerson)
    //    {
    //        this.newPerson = newPerson;
    //    }

    //    public INode NewPerson
    //    {
    //        get { return newPerson; }
    //    }
    //}



    public interface IDiagramLogic
    {
        EventHandler<ContentChangedEventArgs> Update { get; set; }
        EventHandler CurrentChanged { get; set; }

        INode Current { get; }

        void Clear();

        DiagramConnectorNode GetDiagramConnectorNode(INode person);

        IEnumerable<DiagramRow> GenerateRows();

        //List<DiagramConnectorNode> DiagramConnectorNodes { get; }
        //List<DiagramConnector> Connections { get; }

        //double DisplayYear { set; }

        //double MinimumYear { get; }
    }
}