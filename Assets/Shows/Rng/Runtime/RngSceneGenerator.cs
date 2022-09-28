using Plml.Rng.Audio;
using Plml.Rng.Dmx;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public class RngSceneGenerator : MonoBehaviour
    {
        public RngScene[] GenerateScenes(RngShowSettings settings, RngIntroOutroSettings ioSettings)
        {
            RngSceneCollection sceneCollection = GetComponentInChildren<RngSceneCollection>();

            sceneCollection.ClearChildren();

            SceneDurationProviderCollection durationProviderCollection = GetComponentInChildren<SceneDurationProviderCollection>();
            DmxTrackProviderCollection trackProviderCollection = GetComponentInChildren<DmxTrackProviderCollection>();
            AudioProviderCollection audioProviderCollection = GetComponentInChildren<AudioProviderCollection>();

            int sceneCount = URandom.Range(settings.minScenes, settings.maxScenes);
            int totalSceneCount = sceneCount + 2;
            RngScene[] scenes = new RngScene[sceneCount + 2];

            float totalDuration = settings.showDuration;
            float blackoutDuration = settings.blackoutDuration;

            float effectiveDuration = totalDuration - sceneCount * blackoutDuration;
            float targetDuration = effectiveDuration / sceneCount;

            scenes[0] = GenerateIntroOutroScene(true);
            for (int i = 0; i < sceneCount; i++)
            {
                GameObject sceneObj = new();
                RngScene scene = sceneObj.AddComponent<RngScene>();

                sceneObj.AttachTo(sceneCollection);

                scene.track = trackProviderCollection
                    .GetNextElement()
                    .AttachTo(sceneObj);

                scenes[i + 1] = scene;

                scene.sceneWindow = durationProviderCollection.GetNextElement();
            }
            scenes[sceneCount + 1] = GenerateIntroOutroScene(false);

            float totalBlackoutsDuration = (totalSceneCount - 1) * settings.blackoutDuration;
            float durationFactor = (totalDuration - totalBlackoutsDuration) / scenes.Sum(scene => scene.duration);

            float startTime = 0.0f;
            foreach (var scene in scenes)
            {
                float duration = scene.duration * durationFactor;

                scene.duration = duration;
                scene.startTime = startTime;

                startTime += duration + blackoutDuration;
            }

            int idx = 1;
            foreach (var scene in scenes.Skip(1).Take(sceneCount))
            {
                var audioData = audioProviderCollection.GetNextElement(scene.startTime, scene.duration);
                scene.audioData = audioData;

                string dmxLabel = scene.track.name;
                string audioLabel = scene.hasAudio ?
                    $"{audioData.audioClip.name} ({audioData.musicWindow.duration:0.0}s, {audioData.audioVolume:P1})" :
                    "No Audio";
                scene.name = $"{idx++}: {dmxLabel}, {scene.duration:0.0}s - {audioLabel}";
            }

            SetupIntroOutroScene(scenes[0], true);
            SetupIntroOutroScene(scenes[scenes.Length - 1], false);

            RngScene GenerateIntroOutroScene(bool intro)
            {
                GameObject sceneObj = new();
                RngScene scene = sceneObj.AddComponent<RngScene>();

                sceneObj.AttachTo(sceneCollection);

                scene.track = ioSettings.dmxProvider
                    .GetNextElement()
                    .AttachTo(sceneObj);

                TimeWindow window = ioSettings.durationProvider.GetNextElement();
                scene.sceneWindow = window;

                scene.audioData = new()
                {
                    audioClip = ioSettings.music,
                    audioVolume = ioSettings.volume,
                };

                return scene;
            }

            void SetupIntroOutroScene(RngScene scene, bool intro)
            {
                scene.audioData.musicWindow = scene.sceneWindow;

                scene.name = $"{(intro ? "Intro" : "Outro")}: {scene.duration:0.0}s - {ioSettings.music.name}";
            }

            return scenes;
        }
    }
}
