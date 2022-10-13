using System;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting
{
    public interface ILightScriptContext
    {
        IReadOnlyCollection<LightScriptVariable> Variables { get; }

        bool TryGetVariable(string name, out LightScriptVariable variable);
    }
}