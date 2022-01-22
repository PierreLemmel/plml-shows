using System;
using System.Linq.Expressions;

namespace Plml
{
    public interface IBindPropertiesSourceNode<TProp>
    {
        ISpyConfig To<TSource>(TSource source, Expression<Func<TSource, TProp>> propSelector);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2, TProp3>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2,
            Expression<Func<TSource, TProp3>> propSelector3);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2,
            Expression<Func<TSource, TProp3>> propSelector3,
            Expression<Func<TSource, TProp4>> propSelector4);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2,
            Expression<Func<TSource, TProp3>> propSelector3,
            Expression<Func<TSource, TProp4>> propSelector4,
            Expression<Func<TSource, TProp5>> propSelector5);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2,
            Expression<Func<TSource, TProp3>> propSelector3,
            Expression<Func<TSource, TProp4>> propSelector4,
            Expression<Func<TSource, TProp5>> propSelector5,
            Expression<Func<TSource, TProp6>> propSelector6);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2,
            Expression<Func<TSource, TProp3>> propSelector3,
            Expression<Func<TSource, TProp4>> propSelector4,
            Expression<Func<TSource, TProp5>> propSelector5,
            Expression<Func<TSource, TProp6>> propSelector6,
            Expression<Func<TSource, TProp7>> propSelector7);
    }

    public interface IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8>
    {
        ISpyConfig To<TSource>(TSource source,
            Expression<Func<TSource, TProp1>> propSelector1,
            Expression<Func<TSource, TProp2>> propSelector2,
            Expression<Func<TSource, TProp3>> propSelector3,
            Expression<Func<TSource, TProp4>> propSelector4,
            Expression<Func<TSource, TProp5>> propSelector5,
            Expression<Func<TSource, TProp6>> propSelector6,
            Expression<Func<TSource, TProp7>> propSelector7,
            Expression<Func<TSource, TProp8>> propSelector8);
    }
}