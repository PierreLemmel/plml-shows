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

        internal void AddVariable(LightScriptVariable variable) => _variables.Add(variable);
        internal void AddFunction(LightScriptFunction function) => _functions.Add(function);

        public bool TryGetVariable(string name, out LightScriptVariable variable) => (variable = _variables.FirstOrDefault(v => v.Name == name)) != null;
        public bool TryGetFunction(string name, out LightScriptFunction function, params LightScriptType[] arguments) =>
            (function = GetFunctions(name).FirstOrDefault(f => f.Arguments
                .Select(arg => arg.Type)
                .Zip(arguments, (lhs, rhs) => lhs == rhs)
                .All(r => r))
            ) != null;

        public IEnumerable<LightScriptFunction> GetFunctions(string name) => _functions.Where(f => f.Name == name);
    }

    public static class LightScriptCompilationExtensions
    {
        public static bool HasFunction(this ILightScriptCompilationContext context, string name) => context.GetFunctions(name).Any();
    }
}