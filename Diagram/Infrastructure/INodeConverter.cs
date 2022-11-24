using Abstractions;
using System;
using System.Windows.Controls;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public interface INodeConverter
    {
        void Scale(Control control, double value);

        bool IsFiltered(object obj, double displayYear);

        string DateInformation(object obj, double displayYear);

        string NodeTemplate(object obj, NodeType type);

        DateTime? MinimumDate(object obj);

        string BrushResource(object obj, NodeType type, string part);

        string GroupBrushResource(object obj, NodeType type, string part);

        void UpdateGroupIndicator(object obj, Control control, NodeType type);

        string BottomLabel(object obj, double displayYear);
    }

    public interface IConnectorConverter
    {
        DateTime? MinimumDate(INode obj1, INode obj2);

        string Text(INode obj1, INode obj2);
    }
}