using Plml.Midi;
using System;
using UnityEngine;

namespace Plml.EnChiens.Animations
{
    public class SidesButtonsActivation : MonoBehaviour
    {
        public float side1 => s1Value;
        public float side2 => s2Value;

        public float smoothTime;

        private float s1Target = 0f;
        private float s1Value = 0f;
        private float s1Vel = 0f;

        private float s2Target = 0f;
        private float s2Value = 0f;
        private float s2Vel = 0f;

        private bool setup = false;
        private MidiNote side1Note;
        private MidiNote side2Note;

        private KeyCode side1Key;
        private KeyCode side2Key;

        public void SetupSideNotes(MidiNote side1Note, MidiNote side2Note)
        {
            CleanupListeners();

            MidiInputListener mil = FindObjectOfType<MidiInputListener>();

            mil.AddNoteOnListener(side1Note, SwitchOnSide1);
            mil.AddNoteOffListener(side1Note, SwitchOffSide1);

            mil.AddNoteOnListener(side2Note, SwitchOnSide2);
            mil.AddNoteOffListener(side2Note, SwitchOffSide2);

            setup = true;
            this.side1Note = side1Note;
            this.side2Note = side2Note;
        }

        public void SetupSideKeys(KeyCode side1Key, KeyCode side2Key)
        {
            this.side1Key = side1Key;
            this.side2Key = side2Key;
        }

        private void OnEnable()
        {
            if (setup)
                SetupSideNotes(side1Note, side2Note);
        }

        private void OnDisable() => CleanupListeners();

        private void CleanupListeners()
        {
            if (!setup) return;

            MidiInputListener mil = FindObjectOfType<MidiInputListener>();

            if (mil == null) return;

            mil.RemoveNoteOnListener(side1Note, SwitchOnSide1);
            mil.RemoveNoteOffListener(side1Note, SwitchOffSide1);

            mil.RemoveNoteOnListener(side2Note, SwitchOnSide2);
            mil.RemoveNoteOffListener(side2Note, SwitchOffSide2);
        }

        public void SwitchOnSide1(byte _) => s1Target = 1f;
        public void SwitchOffSide1(byte _) => s1Target = 0f;
        public void SwitchOnSide2(byte _) => s2Target = 1f;
        public void SwitchOffSide2(byte _) => s2Target = 0f;

        private void Update()
        {
            s1Value = Mathf.SmoothDamp(s1Value, s1Target, ref s1Vel, smoothTime);
            s2Value = Mathf.SmoothDamp(s2Value, s2Target, ref s2Vel, smoothTime);

            if (Input.GetKeyDown(side1Key))
                s1Target = s1Target == 1f ? 0f : 1f;

            if (Input.GetKeyDown(side2Key))
                s2Target = s2Target == 1f ? 0f : 1f;
        }
    }
}