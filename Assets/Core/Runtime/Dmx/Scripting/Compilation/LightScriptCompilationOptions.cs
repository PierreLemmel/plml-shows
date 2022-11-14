using System;

namespace Plml.Dmx.Scripting.Compilation
{
    [Serializable]
    public class LightScriptCompilationOptions
    {
        public bool log = true;
        public LogLevel errorLogLevel = LogLevel.Warning;
    }
}
