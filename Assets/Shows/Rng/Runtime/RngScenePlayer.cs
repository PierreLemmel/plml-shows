using Plml.Dmx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Rng
{
    public class RngScenePlayer : MonoBehaviour
    {
        [PlayTimeOnly]
        public float currentTime = 0.0f;

        [ReadOnly]
        public bool isPlaying = false;

        public RngPlayState playState = RngPlayState.PreShow;

        public RngSceneContent content = null;


        public RngScene currentScene;

        public TimeWindow nextBlackoutWindow;

        private DmxTrackControler dmxControler;
        private AudioSource audioSource;

        private DmxTrack currentTrack;
        private Guid currentTrackId;

        private bool audioPlaying;

        public float showDuration;

        private void Awake()
        {
            dmxControler = FindObjectOfType<DmxTrackControler>();
            audioSource = FindObjectOfType<AudioSource>();

            ResetTrackingVariables();

            isPlaying = false;
        }

        public void Update()
        {
            if (!isPlaying) return;

            switch (playState)
            {
                case RngPlayState.PreShow:

                    break;

                case RngPlayState.PreShowBlackout:
                    currentTime += Time.deltaTime;
                    if (currentTime >= 0.0f)
                        SetState(RngPlayState.Show);
                    break;

                case RngPlayState.Show:

                    // Scene
                    if (currentScene != null)
                    {
                        // Continues
                        if (currentTime <= currentScene.endTime)
                        {
                            float dmxMaster = currentScene.sceneWindow.GetValue(currentTime);
                            currentTrack.master = dmxMaster;

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
                        }
                        // Ends
                        else
                        {
                            if (currentTime >= showDuration)
                            {
                                nextBlackoutWindow = content.blackout.sceneWindow.Translate(currentScene.endTime);
                                currentScene = null;
                                SetCurrentTrack(content.blackout.track);
                            }
                            
                        }
                    }
                    // blackout
                    else
                    {
                        if (currentTime <= nextBlackoutWindow.endTime)
                        {
                            float dmxMaster = nextBlackoutWindow.GetValue(currentTime);
                            currentTrack.master = dmxMaster;
                        }
                        else
                        {
                            if (currentTime <= showDuration)
                            {
                                currentScene = content.scenes
                                    .Prepend(content.intro)
                                    .Append(content.outro)
                                    .First(sc => currentTime >= sc.startTime && currentTime <= sc.endTime);
                                SetCurrentTrack(currentScene.track);
                            }
                            else
                            {
                                SetState(RngPlayState.PostShowBlackout);
                            }
                        }
                    }

                    currentTime += Time.deltaTime;
                    break;

                case RngPlayState.PostShowBlackout:

                    if (currentTime > nextBlackoutWindow.endTime)
                        SetState(RngPlayState.PostShow);

                    currentTime += Time.deltaTime;
                    break;

                case RngPlayState.PostShow:
                    break;
            }            
        }

        private void SetCurrentTrack(DmxTrack track)
        {
            StopAudio();
            StopLights();

            if (currentTrack != null)
                Log($"End of track: '{currentTrack.name}'");

            currentTrack = dmxControler.AddTrack(track, out currentTrackId);

            Log($"Start of track: '{currentTrack.name}'");
        }

        private void SetState(RngPlayState newState)
        {
            Debug.Log($"End of state: {playState}");
            Debug.Log($"Stating new state: {newState}");
            switch (newState)
            {
                case RngPlayState.PreShow:
                    SetCurrentTrack(content.preShow.track);
                    break;

                case RngPlayState.PreShowBlackout:
                    currentTime = -content.blackout.duration;
                    SetCurrentTrack(content.blackout.track);
                    break;

                case RngPlayState.Show:
                    StopLights();
                    break;

                case RngPlayState.PostShowBlackout:
                    SetCurrentTrack(content.blackout.track);
                    nextBlackoutWindow = content.blackout.sceneWindow.Translate(currentTime);
                    break;

                case RngPlayState.PostShow:
                    SetCurrentTrack(content.postShow.track);
                    break;
            }

            playState = newState;

            Debug.Log($"New state started: {newState}");
        }

        private void ResetTrackingVariables()
        {
            currentTrack = null;
            currentTrackId = Guid.Empty;
            nextBlackoutWindow = new();
            currentScene = null;
            playState = RngPlayState.PreShow;

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

        public void StartShow(RngSceneContent content)
        {
            ResetTrackingVariables();
            showDuration = content.outro.endTime;

            this.content = content;
            SetState(RngPlayState.PreShow);
            isPlaying = true;
        }

        public void Play() => SetState(RngPlayState.PreShowBlackout);

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
