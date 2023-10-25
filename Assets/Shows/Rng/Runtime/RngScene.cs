using Plml.Dmx;
using Plml.Rng.Audio;
using Plml.Rng.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng
{
    public class RngScene : MonoBehaviour
    {
        public RngSceneType type;

        public float duration
        {
            get => sceneWindow.duration;
            set => sceneWindow.duration = value;
        }

        public float startTime
        {
            get => sceneWindow.startTime;
            set => sceneWindow.startTime = value;
        }

        public float endTime => sceneWindow.endTime;

        [EditTimeOnly]
        public TimeWindow sceneWindow;

        public bool hasAudio => audioData != null;

        [EditTimeOnly]
        public RngAudioData audioData;

        [EditTimeOnly]
        public DmxTrack track;
    }
}