using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Rng.UI
{
    [RequireComponent(typeof(Button))]
    public class RngRenerateScenesButton : MonoBehaviour
    {
        private RngShow show;

        private Button button;

        private void Awake()
        {
            show = FindObjectOfType<RngShow>();

            button = GetComponent<Button>();
            button.onClick.AddListener(RegenerateScenes);
        }

        public void RegenerateScenes() => show.RegenerateScenes();

        private void Update() => button.interactable = !show.isPlaying;
    }
}