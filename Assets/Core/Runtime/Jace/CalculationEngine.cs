using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Plml.Jace.Compilation;
using Plml.Jace.Execution;
using Plml.Jace.Maths;
using Plml.Jace.Operations;
using Plml.Jace.Parsing;

namespace Plml.Jace
{
    public class CalculationEngine
    {
        private readonly ExpressionTreeBuilder expressionBuilder;
        private readonly Optimizer optimizer;
        private readonly IDictionary<string, Delegate> formulaCache;

        public CalculationEngine()
        {
            this.formulaCache = new Dictionary<string, Delegate>();
            this.FunctionRegistry = new FunctionRegistry();
            this.ConstantRegistry = new ConstantRegistry();

            expressionBuilder = new ExpressionTreeBuilder();

            optimizer = new Optimizer(new Interpreter()); // We run the optimizer with the interpreter 

            RegisterDefaultConstants();
            RegisterDefaultFunctions();
        }

        internal IFunctionRegistry FunctionRegistry { get; }

        internal IConstantRegistry ConstantRegistry { get; }

        public IEnumerable<FunctionInfo> Functions => FunctionRegistry;
        public IEnumerable<ConstantInfo> Constants => ConstantRegistry;

        public IFormulaBuilder Formula(string formulaText)
        {
            if (string.IsNullOrWhiteSpace(formulaText))
                throw new ArgumentNullException(nameof(formulaText));

            string cleanFormula = formulaText.Replace(" ", "");
            return new FormulaBuilder(cleanFormula, this);
        }

        internal TFunc Build<TFunc>(string formula, IList<string> parameters)
            where TFunc : Delegate
        {
            if (string.IsNullOrEmpty(formula))
                throw new ArgumentNullException(nameof(formula));

            TFunc func;
            if (formulaCache.TryGetValue(formula, out Delegate del))
            {
                func = (TFunc)del;
            }
            else
            {
                func = CompileFunction<TFunc>(formula, parameters);
                formulaCache.Add(formula, func);
            }

            return func;
        }

        public void AddFunction(string functionName, Func<float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);
        public void AddFunction(string functionName, Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> function) => FunctionRegistry.RegisterFunction(functionName, function);

        public void AddConstant(string constantName, float value) => ConstantRegistry.RegisterConstant(constantName, value);

        private void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", new Func<float, float>(Mathf.Sin));
            FunctionRegistry.RegisterFunction("cos", new Func<float, float>(Mathf.Cos));
            FunctionRegistry.RegisterFunction("csc", new Func<float, float>(Mathf.Csc));
            FunctionRegistry.RegisterFunction("sec", new Func<float, float>(Mathf.Sec));
            FunctionRegistry.RegisterFunction("asin", new Func<float, float>(Mathf.Asin));
            FunctionRegistry.RegisterFunction("acos", new Func<float, float>(Mathf.Acos));
            FunctionRegistry.RegisterFunction("tan", new Func<float, float>(Mathf.Tan));
            FunctionRegistry.RegisterFunction("cot", new Func<float, float>(Mathf.Cot));
            FunctionRegistry.RegisterFunction("atan", new Func<float, float>(Mathf.Atan));
            FunctionRegistry.RegisterFunction("acot", new Func<float, float>(Mathf.Acot));
            FunctionRegistry.RegisterFunction("loge", new Func<float, float>(Mathf.Log));
            FunctionRegistry.RegisterFunction("log10", new Func<float, float>(Mathf.Log10));
            FunctionRegistry.RegisterFunction("sqrt", new Func<float, float>(Mathf.Sqrt));
            FunctionRegistry.RegisterFunction("abs", new Func<float, float>(Math.Abs));
            FunctionRegistry.RegisterFunction("ceiling", new Func<float, float>(Mathf.Ceiling));
            FunctionRegistry.RegisterFunction("floor", new Func<float, float>(Mathf.Floor));
            FunctionRegistry.RegisterFunction("truncate", new Func<float, float>(Mathf.Truncate));
            FunctionRegistry.RegisterFunction("round", new Func<float, float>(Mathf.Round));

            FunctionRegistry.RegisterFunction("logn", new Func<float, float, float>(Mathf.Logn));

            FunctionRegistry.RegisterFunction("if", (Func<float, float, float, float>)((a, b, c) => (a != 0.0f ? b : c)));
            FunctionRegistry.RegisterFunction("ifless", (Func<float, float, float, float, float>)((a, b, c, d) => (a < b ? c : d)));
            FunctionRegistry.RegisterFunction("ifmore", (Func<float, float, float, float, float>)((a, b, c, d) => (a > b ? c : d)));
            FunctionRegistry.RegisterFunction("ifequal", (Func<float, float, float, float, float>)((a, b, c, d) => (a == b ? c : d)));
        }

        private void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Mathf.E, false);
            ConstantRegistry.RegisterConstant("pi", Mathf.PI, false);
        }

        private Operation BuildAbstractSyntaxTree(string formulaText)
        {
            TokenReader tokenReader = new TokenReader();
            List<Token> tokens = tokenReader.Read(formulaText);

            AstBuilder astBuilder = new AstBuilder(FunctionRegistry);
            Operation operation = astBuilder.Build(tokens);

            Operation optimized = optimizer.Optimize(operation, this.FunctionRegistry);
            return optimized;
        }

        private TFunc CompileFunction<TFunc>(string formulaText, IList<string> parameters)
            where TFunc : Delegate
        {
            Operation syntaxTree = BuildAbstractSyntaxTree(formulaText);
            Expression<TFunc> expressionTree = expressionBuilder.BuildExpressionTree<TFunc>(syntaxTree,
                FunctionRegistry,
                ConstantRegistry,
                parameters);
            TFunc result = expressionTree.Compile();
            return result;
        }
    }
}