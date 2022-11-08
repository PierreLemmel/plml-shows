using System.Collections.Generic;

namespace Plml.Dmx.Scripting
{
    public interface ILightScriptContext
    {
        public void AddToContext(string key, DmxTrackElement fixture);
        public void AddToContext(string key, int intValue);
        public void AddToContext(string key, float floatValue);
        public void AddToContext(string key, Color24 color);

        public IDictionary<string, DmxTrackElement> Fixtures { get; }
        public IDictionary<string, int> Integers { get; }
        public IDictionary<string, float> Floats { get; }
        public IDictionary<string, Color24> Colors { get; }
    }
}