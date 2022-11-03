using System;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation
{
    public interface ILightScriptCompilationContext
    {
        IReadOnlyCollection<LightScriptVariable> Variables { get; }
        IReadOnlyCollection<LightScriptFunction> Functions { get; }

        bool TryGetVariable(string name, out LightScriptVariable variable);
        bool TryGetFunction(string name, out LightScriptFunction function);
    }
}