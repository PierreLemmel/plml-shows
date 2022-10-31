using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation
{
    internal class LightScriptCompilationContext : ILightScriptCompilationContext
    {
        public IReadOnlyCollection<LightScriptVariable> Variables => _variables;

        private List<LightScriptVariable> _variables = new();

        internal void AddVariable(LightScriptVariable variable) => _variables.Add(variable);

        public bool TryGetVariable(string name, out LightScriptVariable variable) => (variable = _variables.FirstOrDefault(v => v.Name == name)) != null;
    }
}