using System.Collections.Generic;

namespace Plml.Dmx.Scripting
{
    public class LightScriptContext : ILightScriptContext
    {
        public void AddToContext(string key, DmxTrackElement fixture) => fixtures.Add(key, fixture);
        public void AddToContext(string key, int intValue) => integers.Add(key, intValue);
        public void AddToContext(string key, float floatValue) => floats.Add(key, floatValue);
        public void AddToContext(string key, Color24 color) => colors.Add(key, color);

        private readonly Dictionary<string, DmxTrackElement> fixtures = new();
        private readonly Dictionary<string, int> integers = new();
        private readonly Dictionary<string, float> floats = new();
        private readonly Dictionary<string, Color24> colors = new();

        public IDictionary<string, DmxTrackElement> Fixtures => fixtures;
        public IDictionary<string, int> Integers => integers;
        public IDictionary<string, float> Floats => floats;
        public IDictionary<string, Color24> Colors => colors;
    }
}