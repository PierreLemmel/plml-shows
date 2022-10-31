using System;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation
{
    public interface ILightScriptCompilationContext
    {
        IReadOnlyCollection<LightScriptVariable> Variables { get; }

        bool TryGetVariable(string name, out LightScriptVariable variable);
    }
}