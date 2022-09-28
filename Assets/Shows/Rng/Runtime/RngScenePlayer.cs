using Plml.Dmx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.Rng
{
    public class RngScenePlayer : MonoBehaviour
    {
        [PlayTimeOnly]
        public float currentTime = 0.0f;

        [ReadOnly]
        public bool isPlaying = false;

        [ReadOnly]
        public bool done = false;


        [ReadOnly]
        public RngScene[] scenes = Array.Empty<RngScene>();

        [PlayTimeOnly, ReadOnly]
        public int currentSceneIndex = -1;

        [PlayTimeOnly]
        public RngScene currentScene;


        private DmxTrackControler dmxControler;
        private AudioSource audioSource;

        private DmxTrack currentTrack;
        private Guid currentTrackId;

        private bool audioPlaying;

        private void Awake()
        {
            dmxControler = FindObjectOfType<DmxTrackControler>();
            audioSource = FindObjectOfType<AudioSource>();

            ResetTrackingVariables();

            isPlaying = false;
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

        public void StartShow(RngScene[] scenes, Index? startIndex = null, Index? stopIndex = null)
        {
            if (done)
            {
                ResetTrackingVariables();
                done = false;
            }

            Range range = new(startIndex ?? Index.Start, stopIndex ?? Index.End);
            Debug.Log(range);

            this.scenes = scenes.AsSpan(range).ToArray();
            isPlaying = true;
            done = false;
            currentTime = this.scenes[0].startTime;

            Log("Show started");
        }

        public void StopShow()
        {
            currentTime = 0.0f;
            isPlaying = false;

            StopAudio();
            StopLights();

            ResetTrackingVariables();

            scenes = Array.Empty<RngScene>();

            Log("Show stopped");
        }
    }
}
