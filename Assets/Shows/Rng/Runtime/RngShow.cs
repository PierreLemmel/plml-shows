using System;
using UnityEngine;

namespace Plml.Rng
{
    [RequireComponent(typeof(RngSceneGenerator))]
    [RequireComponent(typeof(RngScenePlayer))]
    [RequireComponent (typeof(RngSerializer))]
    public class RngShow : MonoBehaviour
    {
        [PlayTimeOnly]
        public float currentTime = 0.0f;

        public bool isPlaying => scenePlayer.isPlaying;
        public bool done => scenePlayer.done;


        public RngShowSettings showSettings;

        public RngIntroOutroSettings introOutroSettings;

        public RngPlaySettings playSettings;

        [ReadOnly]
        public RngScene[] scenes = Array.Empty<RngScene>();

        [PlayTimeOnly, ReadOnly]
        public int currentSceneIndex = -1;

        [PlayTimeOnly]
        public RngScene currentScene;

        private RngScenePlayer scenePlayer;
        

        private void Awake()
        {
            scenePlayer = GetComponent<RngScenePlayer>();
        }

        private void Start()
        {
            if (scenes.IsEmpty())
                RegenerateScenes();
        }

        public void RegenerateScenes()
        {
            RngSceneGenerator generator = GetComponent<RngSceneGenerator>();
            scenes = generator.GenerateScenes(showSettings, introOutroSettings);
        }

        public void SerializeShow()
        {
            RngSerializer serializer = GetComponent<RngSerializer>();
            serializer.SerializeShow(this);
        }

        public void StartShow()
        {
            RegenerateScenesIfNeeded();
            SaveIfNeeded();

            scenePlayer.StartShow(scenes);
        }

        private void RegenerateScenesIfNeeded()
        {
            if (playSettings.autoGenerateScenes)
                RegenerateScenes();
        }

        private void SaveIfNeeded()
        {
            if (playSettings.autoSaveShow)
                SerializeShow();
        }

        public void StopShow() => scenePlayer.StopShow();

        public void PlayIntro()
        {
            RegenerateScenesIfNeeded();

            scenePlayer.StartShow(scenes, stopIndex: Index.FromStart(1));
        }

        public void PlayOutro()
        {
            RegenerateScenesIfNeeded();

            scenePlayer.StartShow(scenes, startIndex: Index.FromEnd(1));
        }
    }
}