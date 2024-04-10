using Plml.Rng.Audio;
using System;
using UnityEngine;

namespace Plml.Rng
{
    public class RngPlaylistPlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        public AudioProvider clipSource;

        [Range(0f, 1f)]
        public float maxVolume = 0.5f;

        [Min(0f)]
        public float fade = 2.0f;

        private AudioClip currentClip;

        private State state = State.Stopped;
        private float volume = 0f;

        private void Awake()
        {
            audioSource = FindObjectOfType<AudioSource>();
        }

        public void StartPlaylist() => SetState(State.FadingIn);
        public void StopPlaylist() => SetState(State.FadingOut);

        private void Update()
        {
            switch (state)
            {
                case State.Stopped:
                    break;

                case State.FadingIn:
                    volume += Time.deltaTime / fade * maxVolume;

                    if (volume < maxVolume)
                    {
                        audioSource.volume = volume;
                    }
                    else
                    {
                        volume = maxVolume;
                        audioSource.volume = maxVolume;
                        SetState(State.Playing);
                    }

                    UpdateClip();
                    break;

                case State.Playing:
                    UpdateClip();
                    break;

                case State.FadingOut:
                    volume -= Time.deltaTime / fade * maxVolume;

                    if (volume > 0f)
                    {
                        audioSource.volume = volume;
                    }
                    else
                    {
                        volume = 0f;
                        audioSource.volume = 0f;
                        currentClip = null;
                        SetState(State.Stopped);
                    }

                    UpdateClip();
                    break;
            }
        }

        private void UpdateClip()
        {
            if (currentClip == null || audioSource.time >= currentClip.length)
            {
                AudioClip nextClip = clipSource.GetNextElement(0f, 1_000_000f).audioClip;
                currentClip = nextClip;
                audioSource.Stop();
                audioSource.clip = currentClip;
                audioSource.Play();
            }
        }

        private void SetState(State newState)
        {
            state = newState;

            switch (state)
            {
                case State.Stopped:
                    audioSource.Stop();
                    audioSource.clip = null;
                    break;

                case State.FadingIn:
                    break;

                case State.FadingOut:
                    break;

                case State.Playing:
                    break;
            }
        }

        private enum State
        {
            Stopped,
            FadingIn,
            Playing,
            FadingOut,
        }
    }
}