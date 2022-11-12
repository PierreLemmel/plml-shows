using Plml.Dmx.Scripting.Types;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation
{
    public interface ILightScriptCompilationContext
    {
        IReadOnlyCollection<LightScriptVariable> Variables { get; }
        IReadOnlyCollection<LightScriptFunction> Functions { get; }
        IReadOnlyCollection<LightScriptConstant> Constants { get; }

        bool TryGetVariable(string name, out LightScriptVariable variable);
        bool TryGetFunction(string name, out LightScriptFunction function, params LightScriptType[] arguments);
        bool TryGetConstant(string name, out LightScriptConstant constant);

        IEnumerable<LightScriptFunction> GetFunctions(string name);
    }
}