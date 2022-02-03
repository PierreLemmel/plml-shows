using System;
using UnityEngine;

namespace Plml.Dmx
{
    [Serializable]
    public struct DmxChannelDefinition
    {
        [Range(0, 32)]
        public int channel;
        
        public DmxChannelType type;
    }
}