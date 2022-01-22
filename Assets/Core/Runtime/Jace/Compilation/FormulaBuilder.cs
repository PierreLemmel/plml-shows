using System;
using System.Collections.Generic;

namespace Plml.Jace.Compilation
{
    internal class FormulaBuilder :
        IFormulaBuilder,
        IFormulaBuilder1,
        IFormulaBuilder2,
        IFormulaBuilder3,
        IFormulaBuilder4,
        IFormulaBuilder5,
        IFormulaBuilder6,
        IFormulaBuilder7,
        IFormulaBuilder8,
        IFormulaBuilder9,
        IFormulaBuilder10,
        IFormulaBuilder11,
        IFormulaBuilder12,
        IFormulaBuilder13,
        IFormulaBuilder14,
        IFormulaBuilder15,
        IFormulaBuilder16
    {
        private readonly CalculationEngine engine;

        private string formulaText;
        private IList<string> parameters;

        internal FormulaBuilder(string formulaText, CalculationEngine engine)
        {
            this.parameters = new List<string>();
            this.formulaText = formulaText;
            this.engine = engine;
        }

        private FormulaBuilder Parameter(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (engine.FunctionRegistry.IsFunctionName(name))
                throw new ArgumentException($"The name \"{name}\" is already a function name. Parameters cannot have this name.", nameof(name));

            if (engine.ConstantRegistry.IsConstantName(name))
                throw new ArgumentException($"The name \"{name}\" is already a constant name. Parameters cannot have this name.", nameof(name));

            if (parameters.Contains(name))
                throw new ArgumentException($"A parameter with the name \"{name}\" was already defined.", nameof(name));

            parameters.Add(name);
            return this;
        }

        private TFunc Build<TFunc>() where TFunc : Delegate => engine.Build<TFunc>(formulaText, parameters);

        IFormulaBuilder1 ICanAddParameter<IFormulaBuilder1>.Parameter(string name) => Parameter(name);
        IFormulaBuilder2 ICanAddParameter<IFormulaBuilder2>.Parameter(string name) => Parameter(name);
        IFormulaBuilder3 ICanAddParameter<IFormulaBuilder3>.Parameter(string name) => Parameter(name);
        IFormulaBuilder4 ICanAddParameter<IFormulaBuilder4>.Parameter(string name) => Parameter(name);
        IFormulaBuilder5 ICanAddParameter<IFormulaBuilder5>.Parameter(string name) => Parameter(name);
        IFormulaBuilder6 ICanAddParameter<IFormulaBuilder6>.Parameter(string name) => Parameter(name);
        IFormulaBuilder7 ICanAddParameter<IFormulaBuilder7>.Parameter(string name) => Parameter(name);
        IFormulaBuilder8 ICanAddParameter<IFormulaBuilder8>.Parameter(string name) => Parameter(name);
        IFormulaBuilder9 ICanAddParameter<IFormulaBuilder9>.Parameter(string name) => Parameter(name);
        IFormulaBuilder10 ICanAddParameter<IFormulaBuilder10>.Parameter(string name) => Parameter(name);
        IFormulaBuilder11 ICanAddParameter<IFormulaBuilder11>.Parameter(string name) => Parameter(name);
        IFormulaBuilder12 ICanAddParameter<IFormulaBuilder12>.Parameter(string name) => Parameter(name);
        IFormulaBuilder13 ICanAddParameter<IFormulaBuilder13>.Parameter(string name) => Parameter(name);
        IFormulaBuilder14 ICanAddParameter<IFormulaBuilder14>.Parameter(string name) => Parameter(name);
        IFormulaBuilder15 ICanAddParameter<IFormulaBuilder15>.Parameter(string name) => Parameter(name);
        IFormulaBuilder16 ICanAddParameter<IFormulaBuilder16>.Parameter(string name) => Parameter(name);

        Func<float> IFuncBuilder<Func<float>>.Build() => Build<Func<float>>();
        Func<float, float> IFuncBuilder<Func<float, float>>.Build() => Build<Func<float, float>>();
        Func<float, float, float> IFuncBuilder<Func<float, float, float>>.Build() => Build<Func<float, float, float>>();
        Func<float, float, float, float> IFuncBuilder<Func<float, float, float, float>>.Build() => Build<Func<float, float, float, float>>();
        Func<float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float>>();
        Func<float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>();
        Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> IFuncBuilder<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>.Build() => Build<Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>>();
    }
}