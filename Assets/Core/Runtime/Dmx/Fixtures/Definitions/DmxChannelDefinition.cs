using System;
using UnityEngine;

namespace Plml.Dmx
{
    [Serializable]
    public class DmxChannelDefinition
    {
        [Range(0, 32)]
        public int channel;
        
        public DmxChannelType type;
    }
}