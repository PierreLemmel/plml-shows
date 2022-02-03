using System;
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
    }
}