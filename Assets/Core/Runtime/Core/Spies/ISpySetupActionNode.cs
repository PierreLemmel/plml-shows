using System;

namespace Plml
{
    public interface ISpySetupActionNode<TModel>
    {
        ISpyConfig Do(Action<TModel> action);
    }
}