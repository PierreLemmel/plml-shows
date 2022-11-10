using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation
{
    internal class LightScriptCompilationContext : ILightScriptCompilationContext
    {
        public IReadOnlyCollection<LightScriptVariable> Variables => _variables;

        private List<LightScriptVariable> _variables = new();

        public IReadOnlyCollection<LightScriptFunction> Functions => _functions;

        private List<LightScriptFunction> _functions = new();

        public IReadOnlyCollection<LightScriptConstant> Constants => _constants;

        private List<LightScriptConstant> _constants = new();

        internal void AddVariable(LightScriptVariable variable) => _variables.Add(variable);
        internal void AddFunction(LightScriptFunction function) => _functions.Add(function);
        internal void AddConstant(LightScriptConstant constant) => _constants.Add(constant);

        public bool TryGetVariable(string name, out LightScriptVariable variable) => (variable = _variables.FirstOrDefault(v => v.Name == name)) != null;
        public bool TryGetFunction(string name, out LightScriptFunction function, params LightScriptType[] arguments)
        {
            bool IsValidFunction(LightScriptFunction f)
            {
                int nonParamsArgc = f.Arguments.Length - (f.HasParamsArgument ? 1 : 0);

                int i = 0;
                for (; i < nonParamsArgc; i++)
                {
                    if (f.Arguments[i].Type != arguments[i])
                        return false;
                }

                if (f.HasParamsArgument)
                {
                    var foo = f.Arguments[^1].Type;
                    if (!arguments[i..].All(argType => argType == foo))
                        return false;
                }

                return true;
            }

            return (function = GetFunctions(name).SingleOrDefault(IsValidFunction)) != null;
        }

        public bool TryGetConstant(string name, out LightScriptConstant constant) => (constant = _constants.FirstOrDefault(v => v.Name == name)) != null;

        public IEnumerable<LightScriptFunction> GetFunctions(string name) => _functions.Where(f => f.Name == name);
    }

    public static class LightScriptCompilationExtensions
    {
        public static bool HasFunction(this ILightScriptCompilationContext context, string name) => context.GetFunctions(name).Any();
    }
}