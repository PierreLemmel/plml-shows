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
        private AudioClip[] clips = Array.Empty<AudioClip>();
        private AudioClip nextClip = null;

        private void Awake()
        {
            providers = FindObjectOfType<AudioProviderCollection>().GetActiveProviders();
            audioSource = FindObjectOfType<AudioSource>();

            playBtn.onClick.AddListener(PlayAudio);
            stopBtn.onClick.AddListener(StopAudio);

            providersDropdown.options.Clear();
            providersDropdown.options.AddRange(providers.Select(p => new TMP_Dropdown.OptionData(p.name)));

            providersDropdown.onValueChanged.AddListener(UpdateProvider);

            currentProvider = providers[0];

            musicsDropdown.onValueChanged.AddListener(UpdateClip);
            SetupMusicDropdown();
        }

        private void OnDisable() => StopCurrentMusic();

        private void Update()
        {
            stopBtn.interactable = audioSource.isPlaying;
        }

        private void UpdateProvider(int idx)
        {
            currentProvider = providers[idx];
            SetupMusicDropdown();
        }

        private void UpdateClip(int idx)
        {
            nextClip = clips[idx];
        }

        private void SetupMusicDropdown()
        {
            musicsDropdown.options.Clear();
            musicsDropdown.value = 0;

            if (currentProvider is AudioClipProvider acp)
            {
                musicsDropdown.interactable = true;
                playBtn.interactable = true;
                clips = acp.clips;
                musicsDropdown.options.AddRange(clips.Select(c => new TMP_Dropdown.OptionData(c.name)));
            }
            else
            {
                playBtn.interactable = false;
                musicsDropdown.interactable = false;
            }

            musicsDropdown.RefreshShownValue();
        }

        private void StopCurrentMusic()
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        private void PlayAudio()
        {
            audioSource.Stop();
            audioSource.clip = nextClip;
            audioSource.Play();
        }

        private void StopAudio() => StopCurrentMusic();
    }
}
