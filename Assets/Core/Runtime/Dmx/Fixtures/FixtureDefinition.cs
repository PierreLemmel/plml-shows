using System;
using System.Linq;
using UnityEngine;

namespace Plml.Dmx
{
    public class FixtureDefinition : ScriptableObject
    {
        public string manufacturer;
        public string type;
        public string mode;

        public int chanCount;

        public DmxChannelDefinition[] channels;

        public int GetChannelAddress(DmxChannelType chanType) => TryGetChannelAddress(chanType, out var result)
            ? result : throw new InvalidOperationException($"Missing channel type '{chanType}' on fixture {name}");

        public bool TryGetChannelAddress(DmxChannelType chanType, out int result)
        {
            const int notFound = -1;

            result = channels.FirstOrDefault(cd => cd.type == chanType)?.channel ?? notFound;

            return result != notFound;
        }

        public bool HasChannel(DmxChannelType chanType) => TryGetChannelAddress(chanType, out _);
    }
}