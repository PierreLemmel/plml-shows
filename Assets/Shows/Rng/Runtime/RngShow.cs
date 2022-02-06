using Plml.Dmx;
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

            int sceneCount = URandom.Range(settings.minScenes, settings.maxScenes);
            scenes = new RngScene[sceneCount];

            float targetDuration = totalDuration / sceneCount;

            float startTime = 0.0f;
            for (int i = 0; i < sceneCount; i++)
            {
                GameObject sceneObj = new($"Rng Scene {i}");
                RngScene scene = sceneObj.AddComponent<RngScene>();

                sceneObj.AttachTo(sceneCollection);

                scene.track = trackProviderCollection
                    .GetNextElement()
                    .AttachTo(sceneObj);

                scenes[i] = scene;

                scene.sceneWindow = durationProviderCollection.GetNextElement(startTime);

                startTime += scene.duration;
            }

            float durationFactor = totalDuration / scenes.Sum(scene => scene.duration);

            foreach (RngScene scene in scenes)
                scene.duration *= durationFactor;
        }

        public void StartShow() => isPlaying = true;
        public void StopShow() => isPlaying = false;
    }
}