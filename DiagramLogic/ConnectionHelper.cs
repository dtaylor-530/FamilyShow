using Abstractions;
using Microsoft.FamilyShow.Controls.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram.Logic
{
    public static class ConnectionHelper
    {
        ///// <summary>
        ///// Add connections for each person and the person's children in the list.
        ///// </summary>
        //public static void AddChildConnections(this DiagramLogic logic, IList<INode> parents)
        //{
        //    foreach (INode parent in parents)
        //        AddChildConnections(logic, parent);
        //}

        ///// <summary>
        ///// Add connections between the child and child's parents.
        ///// </summary>
        //public static void AddParentConnections(this DiagramLogic logic, INode child)
        //{
        //    foreach (INode parent in child.Parents)
        //    {
        //        if (logic.PersonLookup.ContainsKey(parent) &&
        //            logic.PersonLookup.ContainsKey(child))
        //        {
        //            logic.Connections.Add(new ChildDiagramConnector(
        //                logic.PersonLookup[parent], logic.PersonLookup[child]));
        //        }
        //    }
        //}

        ///// <summary>
        ///// Add connections between the parent and parent’s children.
        ///// </summary>
        //public static void AddChildConnections(this DiagramLogic logic, INode parent)
        //{
        //    foreach (INode child in parent.Children)
        //    {
        //        if (logic.PersonLookup.ContainsKey(parent) &&
        //             logic.PersonLookup.ContainsKey(child))
        //        {
        //            logic.Connections.Add(new ChildDiagramConnector(
        //                logic.PersonLookup[parent], logic.PersonLookup[child]));
        //        }
        //    }
        //}
    }
}
