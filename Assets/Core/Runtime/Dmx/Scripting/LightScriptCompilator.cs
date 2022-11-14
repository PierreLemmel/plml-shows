using Plml.Dmx.Scripting.Compilation;
using UnityEngine;

namespace Plml.Dmx.Scripting
{
    public class LightScriptCompilator : MonoBehaviour
    {
        public LightScriptCompilationOptions options = new();

        public LightScriptCompilationResult Compile(LightScriptCompilationData data)
        {
            var result = LightScriptCompilation.Compile(data, options);

            if (result.hasError)
            {
                if (options.log)
                {
                    Logs.Log(options.errorLogLevel, result.message);
                }
            }

            return result;
        }
    }
}