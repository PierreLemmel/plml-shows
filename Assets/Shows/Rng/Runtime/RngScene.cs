using Plml.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng
{
    public class RngScene : MonoBehaviour
    {
        public float duration
        {
            get => sceneWindow.duration;
            set => sceneWindow.duration = value;
        }

        [EditTimeOnly]
        public TimeWindow sceneWindow;

        [EditTimeOnly]
        public bool hasMusic;

        [EditTimeOnly]
        public TimeWindow musicWindow;

        public DmxTrack track;
    }
}