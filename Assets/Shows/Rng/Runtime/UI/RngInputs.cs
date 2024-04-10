using UnityEngine;

namespace Plml.Rng.UI
{
    public class RngInputs : MonoBehaviour
    {
        public GameObject uiObject;

        public KeyCode uiToggleKey = KeyCode.T;
        public KeyCode startKey = KeyCode.Alpha1;
        public KeyCode playKey = KeyCode.Alpha2;

        private RngShow show;

        private void Awake()
        {
            show = FindObjectOfType<RngShow>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(uiToggleKey))
                uiObject.SetActive(!uiObject.activeSelf);

            if (Input.GetKeyDown(startKey))
                show.StartShow();

            if (Input.GetKeyDown(playKey))
                show.Play();
        }
    } 
}