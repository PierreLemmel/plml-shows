using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Rng.UI
{
    [RequireComponent(typeof(Button))]
    public class RngTestPanelButton : MonoBehaviour
    {
        private Button button;
        private RngShow show;
        private RngUI ui;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(TestPanel);

            show = FindObjectOfType<RngShow>();
            ui = FindObjectOfType<RngUI>();
        }

        public void TestPanel() => ui.TestPanel();

        private void Update() => button.interactable = !show.isPlaying;
    }
}