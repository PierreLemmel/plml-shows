using Plml.Rng.Audio;
using Plml.Rng.Dmx;
using System.Linq;

using UnityEngine;
using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public class RngSceneGenerator : MonoBehaviour
    {
        public RngSceneContent GenerateScenes(
            RngShowSettings showSettings,
            RngIntroOutroSettings introSettings,
            RngIntroOutroSettings outroSettings,
            RngBlackoutSettings blackoutSettings,
            RngPrePostShowSettings preShowSettings,
            RngPrePostShowSettings postShowSettings
        )
        {
            RngSceneCollection sceneCollection = GetComponentInChildren<RngSceneCollection>();

            sceneCollection.ClearChildren();

            SceneDurationProviderCollection durationProviderCollection = GetComponentInChildren<SceneDurationProviderCollection>();
            DmxTrackProviderCollection trackProviderCollection = GetComponentInChildren<DmxTrackProviderCollection>();
            AudioProviderCollection audioProviderCollection = GetComponentInChildren<AudioProviderCollection>();

            int sceneCount = URandom.Range(showSettings.minScenes, showSettings.maxScenes);

            RngScene[] scenes = new RngScene[sceneCount];

            float totalDuration = showSettings.showDuration;
            float blackoutDuration = blackoutSettings.duration;

            var blackout = GenerateBlackoutScene();
            var preShow = GeneratePrePostShowScene(true);
            var postShow = GeneratePrePostShowScene(false);
            var intro = GenerateIntroOutroScene(true);
            var outro = GenerateIntroOutroScene(false);

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

            float totalBlackoutsDuration = (sceneCount +1) * blackoutDuration;
            float durationFactor = (totalDuration - totalBlackoutsDuration - intro.duration - outro.duration)
                / scenes.Sum(scene => scene.duration);

            float startTime = intro.duration + blackoutDuration;
            foreach (var scene in scenes)
            {
                float duration = scene.duration * durationFactor;

                scene.duration = duration;
                scene.startTime = startTime;
                scene.type = RngSceneType.Scene;

                startTime += duration + blackoutDuration;
            }

            int idx = 0;
            foreach (var scene in scenes)
            {
                var audioData = audioProviderCollection.GetNextElement(scene.startTime, scene.duration);
                scene.audioData = audioData;

                string dmxLabel = scene.track.name;
                string audioLabel = scene.hasAudio ?
                    $"{audioData.audioClip.name} ({audioData.musicWindow.duration:0.0}s, {audioData.audioVolume:P1})" :
                    "No Audio";
                scene.name = $"{idx++}: {dmxLabel}, {scene.duration:0.0}s - {audioLabel}";
            }

            SetupIntroOutroScene(intro, true, 0f);
            SetupIntroOutroScene(outro, false, startTime);

            RngScene GenerateIntroOutroScene(bool intro)
            {
                GameObject sceneObj = new();
                RngScene scene = sceneObj.AddComponent<RngScene>();

                sceneObj.AttachTo(sceneCollection);

                scene.type = intro ? RngSceneType.Intro : RngSceneType.Outro;
                
                var dmxProvider = intro ? introSettings.dmxProvider : outroSettings.dmxProvider;
                scene.track = dmxProvider
                    .GetNextElement()
                    .AttachTo(sceneObj);

                var durationProvider = intro ?
                    introSettings.durationProvider :
                    outroSettings.durationProvider;
                TimeWindow window = durationProvider.GetNextElement();
                scene.sceneWindow = window;

                scene.audioData = new()
                {
                    audioClip = introSettings.music,
                    audioVolume = introSettings.volume,
                };

                return scene;
            }

            void SetupIntroOutroScene(RngScene scene, bool intro, float time)
            {
                scene.sceneWindow = scene.sceneWindow.Translate(time);
                scene.audioData.musicWindow = scene.sceneWindow;

                var timeOffset = intro ?
                    introSettings.lightOffset :
                    outroSettings.lightOffset;

                scene.lightWindow = scene.sceneWindow.ShiftLeft(timeOffset);

                scene.name = $"{(intro ? "Intro" : "Outro")}: {scene.duration:0.0}s - {introSettings.music.name}";
            }

            RngScene GeneratePrePostShowScene(bool pre)
            {
                GameObject sceneObj = new();
                RngScene scene = sceneObj.AddComponent<RngScene>();

                sceneObj.AttachTo(sceneCollection);

                scene.type = pre ? RngSceneType.PreShow : RngSceneType.PostShow;
                scene.name = pre ? "Preshow" : "Postshow";

                var settings = pre ? preShowSettings : postShowSettings;
                
                scene.track = settings
                    .dmxProvider
                    .GetNextElement()
                    .AttachTo(sceneObj);

                var duration = settings.timeGap;
                Debug.Log(pre);
                Debug.Log(duration);
                var fade = settings.fade;
                var startTime = pre ? -duration : totalDuration;
                scene.sceneWindow = new(startTime, duration, fade, fade);

                return scene;
            }

            RngScene GenerateBlackoutScene()
            {
                GameObject sceneObj = new();
                RngScene scene = sceneObj.AddComponent<RngScene>();
                scene.AttachTo(sceneCollection);

                float duration = blackoutSettings.duration;
                float fade = blackoutSettings.fade;

                scene.type = RngSceneType.Blackout;
                scene.sceneWindow = new TimeWindow(0, duration, fade, fade);
                scene.name = "Blackout";
                scene.track = blackoutSettings.dmxProvider
                    .GetNextElement()
                    .AttachTo(sceneObj);

                return scene;
            }

            RngSceneContent result = new()
            {
                scenes = scenes,
                intro = intro,
                outro = outro,
                preShow = preShow,
                postShow = postShow,
                blackout = blackout
            };

            return result;
        }
    }
}
