using Plml.Dmx;
using Plml.Rng.Audio;
using Plml.Rng.Dmx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public class RngShow : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxTrackControler controler;

        public Transform sceneObject;

        [EditTimeOnly]
        public float totalDuration = 3600.0f;

        [PlayTimeOnly]
        public float currentTime = 0.0f;

        [ReadOnly]
        public bool isPlaying = false;

        [EditTimeOnly]
        public RngSettings settings;

        [ReadOnly]
        public RngScene[] scenes = Array.Empty<RngScene>();

        private void Awake()
        {
            controler = FindObjectOfType<DmxTrackControler>();
        }

        private void Start()
        {
            if (scenes.IsEmpty())
                RegenerateScenes();
        }

        public void RegenerateScenes()
        {
            RngSceneCollection sceneCollection = GetComponentInChildren<RngSceneCollection>();

            sceneCollection.ClearChildren();

            SceneDurationProviderCollection durationProviderCollection = GetComponentInChildren<SceneDurationProviderCollection>();
            DmxTrackProviderCollection trackProviderCollection = GetComponentInChildren<DmxTrackProviderCollection>();
            AudioProviderCollection audioProviderCollection = GetComponentInChildren<AudioProviderCollection>();

            int sceneCount = URandom.Range(settings.minScenes, settings.maxScenes);
            scenes = new RngScene[sceneCount];

            float targetDuration = totalDuration / sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                GameObject sceneObj = new();
                RngScene scene = sceneObj.AddComponent<RngScene>();

                sceneObj.AttachTo(sceneCollection);

                scene.track = trackProviderCollection
                    .GetNextElement()
                    .AttachTo(sceneObj);

                scenes[i] = scene;

                scene.sceneWindow = durationProviderCollection.GetNextElement();

            }

            float durationFactor = totalDuration / scenes.Sum(scene => scene.duration);
            Debug.Log(durationFactor);
            float startTime = 0.0f;

            int idx = 1;
            foreach (RngScene scene in scenes)
            {
                float duration = scene.duration * durationFactor;

                scene.duration = duration;
                scene.startTime = startTime;

                startTime += duration;

                var audioData = audioProviderCollection.GetNextElement(scene.startTime, duration);
                scene.audioData = audioData;

                string dmxLabel = scene.track.name;
                string audioLabel = scene.hasAudio ?
                    $"{audioData.audioClip.name} ({audioData.musicWindow.duration:0.0}s, {audioData.audioVolume:P1})" :
                    "No Audio";
                scene.name = $"{idx++}: {dmxLabel}, {duration:0.0}s - {audioLabel}";
            }
        }

        public void StartShow() => isPlaying = true;
        public void StopShow() => isPlaying = false;
    }
}