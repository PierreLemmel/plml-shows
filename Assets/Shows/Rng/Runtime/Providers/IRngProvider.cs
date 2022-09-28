using System;

namespace Plml.Rng
{
    public interface IRngProvider
    {
    }

    public interface IRngProvider<TProvided> : IRngProvider
    {
        TProvided GetNextElement();
    }

    public interface IRngProvider<TProvided, TParam> : IRngProvider
    {
        TProvided GetNextElement(TParam param);
    }

    public interface IRngProvider<TProvided, T1, T2> : IRngProvider
    {
        TProvided GetNextElement(T1 t1, T2 t2);
    }
}
