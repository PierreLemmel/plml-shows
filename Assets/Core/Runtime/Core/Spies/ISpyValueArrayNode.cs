using Plml;

namespace Plml
{
    public interface ISpyValueArrayNode<TStruct> where TStruct : struct
    {
        ISpySetupActionNode<TStruct[]> HasChanged();
    }
}