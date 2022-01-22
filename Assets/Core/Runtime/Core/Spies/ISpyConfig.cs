using Plml;
using System;
using System.Linq.Expressions;

namespace Plml
{
    public interface ISpyConfig
    {
        ISpySetupPropertiesNode<TModel> When<TModel>(Func<TModel> model);

        ISpyValueArrayNode<TStruct> WhenAny<TStruct>(Func<TStruct[]> arraySource) where TStruct : struct;

        IBindPropertiesSourceNode<TProp> Bind<TTarget, TProp>(TTarget target, Expression<Func<TTarget, TProp>> propSelector);

        IBindPropertiesSourceNode<TProp1, TProp2> Bind<TTarget, TProp1, TProp2>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2);

        IBindPropertiesSourceNode<TProp1, TProp2, TProp3> Bind<TTarget, TProp1, TProp2, TProp3>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2,
            Expression<Func<TTarget, TProp3>> propSelector3);

        IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4> Bind<TTarget, TProp1, TProp2, TProp3, TProp4>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2,
            Expression<Func<TTarget, TProp3>> propSelector3,
            Expression<Func<TTarget, TProp4>> propSelector4);

        IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2,
            Expression<Func<TTarget, TProp3>> propSelector3,
            Expression<Func<TTarget, TProp4>> propSelector4,
            Expression<Func<TTarget, TProp5>> propSelector5);

        IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2,
            Expression<Func<TTarget, TProp3>> propSelector3,
            Expression<Func<TTarget, TProp4>> propSelector4,
            Expression<Func<TTarget, TProp5>> propSelector5,
            Expression<Func<TTarget, TProp6>> propSelector6);

        IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2,
            Expression<Func<TTarget, TProp3>> propSelector3,
            Expression<Func<TTarget, TProp4>> propSelector4,
            Expression<Func<TTarget, TProp5>> propSelector5,
            Expression<Func<TTarget, TProp6>> propSelector6,
            Expression<Func<TTarget, TProp7>> propSelector7);

        IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8>(
            TTarget target,
            Expression<Func<TTarget, TProp1>> propSelector1,
            Expression<Func<TTarget, TProp2>> propSelector2,
            Expression<Func<TTarget, TProp3>> propSelector3,
            Expression<Func<TTarget, TProp4>> propSelector4,
            Expression<Func<TTarget, TProp5>> propSelector5,
            Expression<Func<TTarget, TProp6>> propSelector6,
            Expression<Func<TTarget, TProp7>> propSelector7,
            Expression<Func<TTarget, TProp8>> propSelector8);
    }
}