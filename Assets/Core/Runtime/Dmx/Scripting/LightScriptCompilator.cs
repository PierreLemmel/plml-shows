using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.Dmx.Scripting
{
    public class LightScriptCompilator : MonoBehaviour
    {
        public LightScriptCompilationOptions options = new();

        public bool TryCompile(LightScriptData data, out LightScriptAction action)
        {
            var result = LightScriptCompilation.Compile(data, options);

            if (result.isOk)
            {
                action = result.action;
                return true;
            }
            else
            {
                action = null;

                if (options.log)
                    Debug.LogError(result.message);

                return false;
            }
        }
    }
}