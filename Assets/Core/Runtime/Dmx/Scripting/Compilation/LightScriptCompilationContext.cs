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
        public bool TryGetFunction(string name, out LightScriptFunction function, params LightScriptType[] arguments) =>
            (function = GetFunctions(name).SingleOrDefault(f => f.Arguments
                .Select(arg => arg.Type)
                .Zip(arguments, (lhs, rhs) => lhs == rhs)
                .All(r => r))
            ) != null;
        public bool TryGetConstant(string name, out LightScriptConstant constant) => (constant = _constants.FirstOrDefault(v => v.Name == name)) != null;

        public IEnumerable<LightScriptFunction> GetFunctions(string name) => _functions.Where(f => f.Name == name);
    }

    public static class LightScriptCompilationExtensions
    {
        public static bool HasFunction(this ILightScriptCompilationContext context, string name) => context.GetFunctions(name).Any();
    }
}