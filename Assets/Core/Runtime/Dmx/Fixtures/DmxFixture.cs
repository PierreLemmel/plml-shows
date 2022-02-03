using System;
using UnityEngine;

namespace Plml.Dmx
{
    public class DmxFixture : MonoBehaviour
    {
        [EditTimeOnly]
        public FixtureDefinition model;

        [EditTimeOnly]
        [Range(1, 512)]
        public int channelOffset = 1;
    }
}