using System;

namespace Plml.Jace.Compilation
{
    public interface IAbstractFuncBuilder { }
    public interface IFuncBuilder<TFunc> : IAbstractFuncBuilder where TFunc : Delegate
    {
        TFunc Build();
    }

    public interface ICanAddParameter<TNewBuilder> where TNewBuilder : IAbstractFuncBuilder
    {
        TNewBuilder Parameter(string name);
    }

    public interface IFormulaBuilder :
        IFuncBuilder<Func<float>>,
        ICanAddParameter<IFormulaBuilder1>
    {
    }

    public interface IFormulaBuilder1 :
        IFuncBuilder<Func<float, float>>,
        ICanAddParameter<IFormulaBuilder2>
    {

    }

    public interface IFormulaBuilder2 :
          IFuncBuilder<Func<float, float, float>>,
          ICanAddParameter<IFormulaBuilder3>
    {

    }

    public interface IFormulaBuilder3 :
          IFuncBuilder<Func<float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder4>
    {

    }

    public interface IFormulaBuilder4 :
          IFuncBuilder<Func<float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder5>
    {

    }

    public interface IFormulaBuilder5 :
          IFuncBuilder<Func<float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder6>
    {

    }

    public interface IFormulaBuilder6 :
          IFuncBuilder<Func<float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder7>
    {

    }

    public interface IFormulaBuilder7 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder8>
    {

    }

    public interface IFormulaBuilder8 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder9>
    {

    }

    public interface IFormulaBuilder9 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder10>
    {

    }

    public interface IFormulaBuilder10 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder11>
    {

    }

    public interface IFormulaBuilder11 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder12>
    {

    }

    public interface IFormulaBuilder12 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder13>
    {

    }

    public interface IFormulaBuilder13 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder14>
    {

    }

    public interface IFormulaBuilder14 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder15>
    {

    }

    public interface IFormulaBuilder15 :
          IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>,
          ICanAddParameter<IFormulaBuilder16>
    {

    }

    public interface IFormulaBuilder16 :
        IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>
    {

    }
}