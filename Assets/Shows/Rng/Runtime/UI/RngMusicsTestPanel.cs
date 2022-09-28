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

        public Slider volumeSlider;
        public TMP_Text volumeLabel;

        public Button playBtn;
        public Button stopBtn;

        private AudioSource audioSource;
        private AudioProvider[] providers;
        private AudioProvider currentProvider;

        private AudioClip[] clips = Array.Empty<AudioClip>();
        private AudioClip nextClip = null;

        private float minVolume = 0f;
        private float maxVolume = 1f;

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

            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);

            musicsDropdown.onValueChanged.AddListener(UpdateClip);
            SetupMusicDropdown();
        }

        private void OnDisable()
        {
            StopCurrentMusic();
            audioSource.volume = 1f;
        }

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

        private void OnSliderValueChanged(float value)
        {
            audioSource.volume = value;
            ClampSliderValue();
            UpdateSliderLabel();
        }

        private void ClampSliderValue()
        {
            float value = volumeSlider.value;
            float clampedValue = Mathf.Clamp(value, minVolume, maxVolume);

            if (clampedValue != value)
            {
                volumeSlider.value = clampedValue;
                audioSource.volume = clampedValue;
            }
        }

        private string PctFormat(float number) => Mathf.Round(100 * number) + "%";
        private void UpdateSliderLabel() => volumeLabel.text = $"{PctFormat(volumeSlider.value)} [{PctFormat(minVolume)}-{PctFormat(maxVolume)}]";

        private void SetupMusicDropdown()
        {
            musicsDropdown.options.Clear();

            if (currentProvider is AudioClipProvider acp)
            {
                musicsDropdown.interactable = true;
                playBtn.interactable = true;
                clips = acp.clips;
                musicsDropdown.options.AddRange(clips.Select(c => new TMP_Dropdown.OptionData(c.name)));
                UpdateClip(0);

                minVolume = acp.minVolume;
                maxVolume = acp.maxVolume;
            }
            else
            {
                playBtn.interactable = false;
                musicsDropdown.interactable = false;

                minVolume = 0f;
                maxVolume = 1f;
            }

            ClampSliderValue();
            UpdateSliderLabel();

            musicsDropdown.value = 0;
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
