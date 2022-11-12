using System;

namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptCompilationResult
    {
        public string message { get; }

        public bool isOk { get; }
        public bool hasError => !isOk;

        private LightScriptAction _action;
        public LightScriptAction action
        {
            get
            {
                if (hasError)
                    throw new InvalidOperationException($"Can't access action when compilation has error. Check the '{nameof(hasError)}' property first");

                return _action;
            }
        }

        private LightScriptCompilationResult(bool isOk, LightScriptAction action, string message)
        {
            this.isOk = isOk;
            _action = action;
            this.message = message;
        }

        public static LightScriptCompilationResult Ok(LightScriptAction action) => new(true, action, "OK");
        public static LightScriptCompilationResult Error(string message) => new(false, null, message);
    }
}
