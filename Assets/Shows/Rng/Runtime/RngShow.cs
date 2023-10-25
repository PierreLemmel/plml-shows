using System;
using System.Linq;
using UnityEngine;

namespace Plml.Rng
{
    [RequireComponent(typeof(RngSceneGenerator))]
    [RequireComponent(typeof(RngScenePlayer))]
    [RequireComponent (typeof(RngSerializer))]
    [RequireComponent(typeof(RngPlaylistSender))]
    public class RngShow : MonoBehaviour
    {
        [PlayTimeOnly]
        public float currentTime = 0.0f;

        public bool isPlaying => scenePlayer.isPlaying;
        public bool done => scenePlayer.playState == RngPlayState.PostShow;


        public RngShowSettings showSettings;

        public RngIntroOutroSettings introSettings;

        public RngIntroOutroSettings outroSettings;

        public RngBlackoutSettings blackoutSettings;

        public RngPrePostShowSettings preShowSettings;

        public RngPrePostShowSettings postShowSettings;



        public RngPlaySettings playSettings;


        public RngSceneContent content;
        

        [PlayTimeOnly, ReadOnly]
        public int currentSceneIndex = -1;

        [PlayTimeOnly]
        public RngScene currentScene;

        private RngScenePlayer scenePlayer;
        private RngPlaylistSender playlistSender;


        private void Awake()
        {
            scenePlayer = GetComponent<RngScenePlayer>();
            playlistSender = GetComponent<RngPlaylistSender>();
        }

        private void Start()
        {
            if (content == null)
                RegenerateScenes();
        }

        public void RegenerateScenes()
        {
            RngSceneGenerator generator = GetComponent<RngSceneGenerator>();
            var result = generator.GenerateScenes(
                showSettings, 
                introSettings,
                outroSettings,
                blackoutSettings,
                preShowSettings,
                postShowSettings
            );

            content = result;
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
            SendPlaylistIfNeeded();

            scenePlayer.StartShow(content);
        }

        public void Play()
        {
            scenePlayer.Play();
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

        private void SendPlaylistIfNeeded()
        {
            if (playSettings.autoSendPlaylist)
                SendPlaylist();
        }

        private void SendPlaylist()
        {
            string[] playlist = new string[0];

            playlist = GetComponentsInChildren<RngScene>()
                .Where(scene => scene.hasAudio)
                .Select(scene => scene.audioData.audioClip.name)
                .ToArray();

            playlistSender.SendPlaylist(playlist);
        }

        public void StopShow() => scenePlayer.StopShow();

        public void PlayIntro()
        {
            Debug.LogWarning("Play Intro not implemented");
        }

        public void PlayOutro()
        {
            Debug.LogWarning("Play Outro not implemented");
        }
    }
}