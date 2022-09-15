using UnityEngine;

namespace Plml.EnChiens.Animations
{
    public class SidesButtonsActivation : MonoBehaviour
    {
        public KeyCode side1Key;
        public KeyCode side2Key;

        public float side1 => s1Value;
        public float side2 => s2Value;

        public float smoothTime;

        private float s1Target = 0f;
        private float s1Value = 0f;
        private float s1Vel = 0f;

        private float s2Target = 0f;
        private float s2Value = 0f;
        private float s2Vel = 0f;

        private void Update()
        {
            if (Input.GetKeyDown(side1Key))
                s1Target = s1Target == 0f ? 1f : 0f;

            if (Input.GetKeyDown(side2Key))
                s2Target = s2Target == 0f ? 1f : 0f;

            s1Value = Mathf.SmoothDamp(s1Value, s1Target, ref s1Vel, smoothTime);
            s2Value = Mathf.SmoothDamp(s2Value, s2Target, ref s2Vel, smoothTime);
        }
    }
}