using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.Dmx
{
    public class DmxTrack : MonoBehaviour
    {
        public bool isPlaying = true;
        
        [Range(0.0f, 1.0f)]
        public float master = 1.0f;

        private DmxTrackElement[] elements;

        private void Awake()
        {
            elements = GetComponentsInChildren<DmxTrackElement>();
        }

        public IEnumerable<DmxTrackElement> Elements => elements;
    }
}