using Plml.Midi;
using UnityEngine;

namespace Plml.EnChiens.Animations
{
    public class SidesButtonsActivation : MonoBehaviour
    {
        public MidiNote side1Note;
        public MidiNote side2Note;

        public float side1 => s1Value;
        public float side2 => s2Value;

        public float smoothTime;

        private float s1Target = 0f;
        private float s1Value = 0f;
        private float s1Vel = 0f;

        private float s2Target = 0f;
        private float s2Value = 0f;
        private float s2Vel = 0f;

        private void Awake()
        {
            MidiInputListener mil = FindObjectOfType<MidiInputListener>();

            mil.AddNoteOnListener(side1Note, SwitchOnSide1);
            mil.AddNoteOffListener(side1Note, SwitchOffSide1);

            mil.AddNoteOnListener(side2Note, SwitchOnSide2);
            mil.AddNoteOffListener(side2Note, SwitchOffSide2);
        }

        public void SwitchOnSide1(byte _) => s1Target = 1f;
        public void SwitchOffSide1(byte _) => s1Target = 0f;
        public void SwitchOnSide2(byte _) => s2Target = 1f;
        public void SwitchOffSide2(byte _) => s2Target = 0f;

        private void Update()
        {
            s1Value = Mathf.SmoothDamp(s1Value, s1Target, ref s1Vel, smoothTime);
            s2Value = Mathf.SmoothDamp(s2Value, s2Target, ref s2Vel, smoothTime);
        }
    }
}