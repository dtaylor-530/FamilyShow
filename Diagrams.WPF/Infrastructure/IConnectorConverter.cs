using Abstractions;

namespace Diagrams.WPF.Infrastructure
{
    public interface IConnectorConverter
    {
        //DateTime MinimumDate(IRelationship relationship);

        string ResourcePen(IRelationship relationship);

        bool IsFiltered(IRelationship relationship);

        string Text(IRelationship relationship);

        void Subscribe(IRelationship obj);
    }
}