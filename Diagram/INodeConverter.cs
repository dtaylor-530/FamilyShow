using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.FamilyShow.Controls.Diagram
{
    public interface INodeConverter
    {
        bool ShouldDisplayGroupIndicator(object obj, NodeType nodeType);
        bool IsFiltered(object obj, double displayYear);
        string DateInformation(object obj, double displayYear);
        string NodeTemplate(object obj, NodeType type);
        string NodeTemplate();

        DateTime? MinimumDate(object obj);
        object BrushResource(object? model, NodeType type, string part);
    }  
    
    public interface IConnectorConverter
    {        
        DateTime? MinimumDate(object obj1, object obj2);
        string Text(object obj1, object obj2);
    }
}
