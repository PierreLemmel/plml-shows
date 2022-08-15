using Plml.Rng.Audio;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Rng.UI
{
    public class RngMusicsTestPanel : MonoBehaviour
    {
        public TMP_Dropdown providersDropdown;
        public TMP_Dropdown musicsDropdown;

        public Button playBtn;
        public Button stopBtn;

        private AudioSource audioSource;
        private AudioProvider[] providers;
        private AudioProvider currentProvider;

        private float sceneDuration = 180.0f;

        private void Awake()
        {
            providers = FindObjectOfType<AudioProviderCollection>().GetActiveProviders();
            audioSource = FindObjectOfType<AudioSource>();

            playBtn.onClick.AddListener(PlayAudio);
            stopBtn.onClick.AddListener(StopAudio);

            providersDropdown.onValueChanged.AddListener(UpdateProviderIndex);

            currentProvider = providers[0];
            SetupMusicDropdown();
        }

        private void OnDisable() => StopCurrentMusic();

        private void UpdateProviderIndex(int idx)
        {
            currentProvider = providers[idx];
            SetupMusicDropdown();
        }

        private void SetupMusicDropdown()
        {

        }

        private void StopCurrentMusic()
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        private void PlayAudio()
        {
            StopCurrentMusic();

            RngAudioData data = currentProvider.GetElement(0f, sceneDuration);

            audioSource.volume = data.audioVolume;
            audioSource.clip = data.audioClip;
        }

        private void StopAudio() => StopCurrentMusic();
    }
}
