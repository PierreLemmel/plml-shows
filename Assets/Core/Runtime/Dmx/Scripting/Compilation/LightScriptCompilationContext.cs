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
        public bool TryGetFunction(string name, out LightScriptFunction function) => (function = _functions.FirstOrDefault(v => v.Name == name)) != null;
    }
}