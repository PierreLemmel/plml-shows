using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Rng.UI
{
    [RequireComponent(typeof(Button))]
    public class RngPlayButton : MonoBehaviour
    {
        private RngShow show;
        private TextMeshProUGUI text;

        private void Awake()
        {
            show = FindObjectOfType<RngShow>();
            text = GetComponentInChildren<TextMeshProUGUI>();

            Button button = GetComponent<Button>();
            button.interactable = true;
            button.onClick.AddListener(TogglePlay);
        }

        public void TogglePlay()
        {
            if (!show.isPlaying)
            {
                show.StartShow();
            }
            else
            {
                show.StopShow();
            }
        }

        private void Update() => text.SetText(show.isPlaying ? "Stop" : "Play");
    }
}