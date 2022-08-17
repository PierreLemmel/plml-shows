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
    [RequireComponent(typeof(RngSceneGenerator))]
    [RequireComponent(typeof(RngScenePlayer))]
    public class RngShow : MonoBehaviour
    {
        [PlayTimeOnly]
        public float currentTime = 0.0f;

        [ReadOnly]
        public bool isPlaying = false;

        [ReadOnly]
        public bool done = false;

        [EditTimeOnly]
        public RngSettings settings;


        [EditTimeOnly]
        public SceneDurationProvider introOutroDurationProvider;

        [EditTimeOnly]
        public DmxTrackProvider introOutroDmxProvider;

        [EditTimeOnly]
        public AudioClip introOutroMusic;

        [EditTimeOnly, Range(0.0f, 1.0f)]
        public float introOutroVolume;

        [ReadOnly]
        public RngScene[] scenes = Array.Empty<RngScene>();

        [PlayTimeOnly, ReadOnly]
        public int currentSceneIndex = -1;

        [PlayTimeOnly]
        public RngScene currentScene;

        private DmxTrackControler dmxControler;
        private AudioSource audioSource;

        private RngSceneGenerator sceneGenerator;
        private RngScenePlayer scenePlayer;
        
        private DmxTrack currentTrack;
        private Guid currentTrackId;
        
        private bool audioPlaying;

        private void Awake()
        {
            dmxControler = FindObjectOfType<DmxTrackControler>();
            audioSource = FindObjectOfType<AudioSource>();

            sceneGenerator = GetComponent<RngSceneGenerator>();
            scenePlayer = GetComponent<RngScenePlayer>();
            
            ResetTrackingVariables();

            isPlaying = false;
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
            scenes = new RngScene[sceneCount + 2];

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


            float durationFactor = totalDuration / scenes.Sum(scene => scene.duration);
            
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

                scene.track = introOutroDmxProvider
                    .GetElement()
                    .AttachTo(sceneObj);

                TimeWindow window = introOutroDurationProvider.GetElement();
                scene.sceneWindow = window;

                scene.audioData = new()
                {
                    audioClip = introOutroMusic,
                    audioVolume = introOutroVolume
                };

                return scene;
            }

            void SetupIntroOutroScene(RngScene scene, bool intro)
            {
                scene.audioData.musicWindow = scene.sceneWindow;

                scene.name = $"{(intro ? "Intro" : "Outro")}: {scene.duration:0.0}s - {introOutroMusic.name}";
            }
        }

        public void Update()
        {
            if (!isPlaying || done) return;

            #region Scene Management
            if (currentScene == null || currentScene.endTime < currentTime)
            {
                currentSceneIndex++;

                if (currentSceneIndex == scenes.Length)
                {
                    done = true;
                    return;
                }
                else
                {
                    currentScene = scenes[currentSceneIndex];
                }

                StopLights();

                if (currentTrack != null)
                    Log($"End of track: '{currentTrack.name}'");

                StopAudio();

                currentTrack = dmxControler.AddTrack(currentScene.track, out currentTrackId);
                Log($"Start of track: '{currentTrack.name}'");
            }
            #endregion

            #region Dmx
            float dmxMaster = currentScene.sceneWindow.GetValue(currentTime);
            currentTrack.master = dmxMaster;
            #endregion

            #region Audio
            if (currentScene.hasAudio)
            {
                var audioData = currentScene.audioData;
                var musicWindow = audioData.musicWindow;
            
                if (musicWindow.Contains(currentTime))
                {
                    if (!audioPlaying)
                    {
                        audioPlaying = true;
                        audioSource.clip = audioData.audioClip;

                        Log($"Playing clip '{audioSource.clip.name}'");
                        audioSource.Play();
                    }
                    
                    audioSource.volume = musicWindow.GetValue(currentTime) * audioData.audioVolume;
                }
                else
                {
                    if (audioPlaying)
                    {
                        Log($"End of clip '{audioSource.clip.name}'");
                        StopAudio();
                    }
                }
                
            }
            #endregion

            currentTime += Time.deltaTime;
        }

        private void ResetTrackingVariables()
        {
            currentTrack = null;
            currentTrackId = Guid.Empty;
            currentSceneIndex = -1;
            currentScene = null;
            
            audioPlaying = false;
        }

        private void StopLights()
        {
            if (currentTrack != null)
            {
                dmxControler.RemoveTrack(currentTrackId);
            }
        }

        private void StopAudio()
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioPlaying = false;
        }

        private void Log(string msg) => Debug.Log($"{currentTime:####:##} - {msg}");

        public void StartShow()
        {
            isPlaying = true;
            Log("Show started");
        }

        public void StopShow()
        {
            currentTime = 0.0f;
            isPlaying = false;

            StopAudio();
            StopLights();

            ResetTrackingVariables();

            Log("Show stopped");
        }
    }
}