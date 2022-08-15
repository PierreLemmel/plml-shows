using Plml.Dmx;
using Plml.Rng.Dmx;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Rng.UI
{
    public class RngLightsTestPanel : MonoBehaviour
    {
        public TMP_Dropdown scenesDropdown;

        public Slider durationSlider;
        public TMP_Text durationLabel;

        public Button playBtn;
        public Button stopBtn;

        private DmxTrackProvider[] providers;
        private DmxTrackControler dmxControler;

        private int sceneIndex = 0;

        private void Awake()
        {
            durationSlider.onValueChanged.AddListener(UpdateDurationLabel);

            providers = FindObjectOfType<DmxTrackProviderCollection>().GetAllProviders();
            dmxControler = FindObjectOfType<DmxTrackControler>();

            scenesDropdown.ClearOptions();
            scenesDropdown.options.AddRange(providers.Select(pvd =>
            {
                string name = pvd.active ? pvd.name : $"{pvd.name} (inactive)";
                return new TMP_Dropdown.OptionData(name);
            }));

            scenesDropdown.onValueChanged.AddListener(SetSceneIndex);

            playBtn.onClick.AddListener(PlayScene);
            stopBtn.onClick.AddListener(StopScene);

            stopBtn.interactable = false;
        }

        private void OnDisable() => StopCurrentScene();

        private void UpdateDurationLabel(float duration) => durationLabel.text = $"{duration:F2} s";

        private void SetSceneIndex(int sceneIndex) => this.sceneIndex = sceneIndex;

        private Guid trackId = Guid.Empty;
        private DmxTrack currentTrack = null;

        private void StopCurrentScene()
        {
            if (currentTrack != null)
            {
                dmxControler.RemoveTrack(trackId);
                Destroy(currentTrack.gameObject);
                currentTrack = null;
            }
        }

        private void UpdateButtons(bool currentlyPlaying)
        {
            scenesDropdown.interactable = true;
            playBtn.interactable = true;
            stopBtn.interactable = currentlyPlaying;
        }

        private void PlayScene()
        {
            StopCurrentScene();

            UpdateButtons(true);

            DmxTrackProvider provider = providers[sceneIndex];
            currentTrack = provider.GetElement();
            currentTrack.AttachTo(this);

            dmxControler.AddTrack(currentTrack, out trackId);
        }

        private void StopScene()
        {
            StopCurrentScene();

            UpdateButtons(false);
        }
    }
}