using System.Collections.Generic;

namespace Plml.Dmx.Scripting
{
    public interface ILightScriptContext
    {
        public void AddToContext(string key, DmxTrackElement fixture);
        public void AddToContext(string key, int intValue);
        public void AddToContext(string key, float floatValue);
        public void AddToContext(string key, Color24 color);

        public IReadOnlyDictionary<string, DmxTrackElement> Fixtures { get; }
        public IReadOnlyDictionary<string, int> Integers { get; }
        public IReadOnlyDictionary<string, float> Floats { get; }
        public IReadOnlyDictionary<string, Color24> Colors { get; }
    }
}