using System;
using System.Linq.Expressions;

namespace Plml
{
    public interface ISpySetupPropertiesNode<TModel>
    {
        ISpySetupActionNode<TModel> HasChanged();
        ISpySetupActionNode<TModel> HasChangesOn<T>(Func<TModel, T> prop);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2>(Func<TModel, T1> prop1, Func<TModel, T2> prop2);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4, Func<TModel, T5> prop5);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5, T6>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4,
            Func<TModel, T5> prop5, Func<TModel, T6> prop6);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5, T6, T7>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4,
            Func<TModel, T5> prop5, Func<TModel, T6> prop6, Func<TModel, T7> prop7);
        ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5, T6, T7, T8>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4,
            Func<TModel, T5> prop5, Func<TModel, T6> prop6, Func<TModel, T7> prop7, Func<TModel, T8> prop8);
    }
}