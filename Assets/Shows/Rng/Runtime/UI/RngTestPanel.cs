using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Plml.Rng.UI
{
    public class RngTestPanel : MonoBehaviour
    {
        private TMP_Dropdown dropdown;

        public RngTestPanelData[] panelData;

        private void Awake()
        {
            dropdown = GetComponentInChildren<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(HandleChoice);

            dropdown.options.Clear();
            dropdown.options.AddRange(panelData.Select(pde => new TMP_Dropdown.OptionData(pde.label)));

            HandleChoice(0);
        }

        private void HandleChoice(int choice) => panelData
            .ForEach((pde, i) => pde.gameObject.SetActive(i == choice));

        [Serializable]
        public class RngTestPanelData
        {
            public string label;
            public GameObject gameObject;
        }
    }
}