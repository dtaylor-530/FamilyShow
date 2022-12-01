using Abstractions;
using Microsoft.FamilyShow;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Diagrams.WPF.Infrastructure
{
    public interface INodeConverter
    {
        //double DisplayYear { get; set; }

        void Scale(Control control, double value);

        bool IsFiltered(INode obj);

        string DateInformation(INode obj);

        string NodeTemplate(INode obj, NodeType type);

        //DateTime? MinimumDate(object obj);

        string BrushResource(INode obj, NodeType type, string part);

        string GroupBrushResource(INode obj, NodeType type, string part);

        void UpdateGroupIndicator(INode obj, Control control, NodeType type);

        string Text(INode obj);

        void Subscribe(INode obj);
    }
}