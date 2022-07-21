using System;
using UnityEngine;

namespace Plml.Rng.UI
{
    public class RngUI : MonoBehaviour
    {
        public GameObject mainMenuObject;
        public GameObject testPanelObject;

        public void MainMenu() => GoToMainMenu();
        public void TestPanel() => GoToTestPanel();

        private void Awake() => GoToMainMenu();

        private void GoToMainMenu()
        {
            mainMenuObject.SetActive(true);
            testPanelObject.SetActive(false);
        }

        private void GoToTestPanel()
        {
            testPanelObject.SetActive(true);
            mainMenuObject.SetActive(false);
        }
    }
}