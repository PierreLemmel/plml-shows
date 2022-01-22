using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Plml.Midi.Fixtures
{
    public class AkaiLPD8Dispatcher : MidiInputListener
    {
        public AkaiPreset preset1;
        public AkaiPreset preset2;
        public AkaiPreset preset3;
        public AkaiPreset preset4;

        private AkaiPreset[] pages;

        private void Awake()
        {
            pages = new[] { preset1, preset2, preset3, preset4 };
        }

        public override void OnControlChanged(int controllerNumber, byte value)
        {
            int page = (controllerNumber - 1) / 16;
            int index = (controllerNumber - 1) % 16;

            AkaiPreset preset = pages[page];
            if (index < 8)
            {
                var toggle = preset.GetToggle(index);

                if (value > 0)
                {
                    if (!toggle.started)
                    {
                        toggle.onStarted.Invoke();
                        toggle.started = true;
                    }
                }
                else
                {
                    if (toggle.started)
                    {
                        toggle.onStopped.Invoke();
                        toggle.started = false;
                    }
                }
            }
            else
            {
                var knob = preset.GetKnob(index - 8);
                knob.onValueChanged.Invoke(value);
            }
        }

        public override void OnProgramChanged(int program)
        {
            int page = program / 8;
            int index = program % 8;

            var handler = pages[page].GetProgram(index);
            handler.onLaunched.Invoke();
        }

        [Serializable]
        public class AkaiPreset
        {
            public ControlToggleChangedHandler toggle1;
            public ControlToggleChangedHandler toggle2;
            public ControlToggleChangedHandler toggle3;
            public ControlToggleChangedHandler toggle4;
            public ControlToggleChangedHandler toggle5;
            public ControlToggleChangedHandler toggle6;
            public ControlToggleChangedHandler toggle7;
            public ControlToggleChangedHandler toggle8;


            public ControlKnobHandler knob1;
            public ControlKnobHandler knob2;
            public ControlKnobHandler knob3;
            public ControlKnobHandler knob4;
            public ControlKnobHandler knob5;
            public ControlKnobHandler knob6;
            public ControlKnobHandler knob7;
            public ControlKnobHandler knob8;


            public ProgramHandler program1;
            public ProgramHandler program2;
            public ProgramHandler program3;
            public ProgramHandler program4;
            public ProgramHandler program5;
            public ProgramHandler program6;
            public ProgramHandler program7;
            public ProgramHandler program8;


            private ControlToggleChangedHandler[] toggles;
            private ControlKnobHandler[] knobs;
            private ProgramHandler[] programs;

            public ControlToggleChangedHandler GetToggle(int index)
            {
                InitIfNeeded();
                return toggles[index];
            }

            public ControlKnobHandler GetKnob(int index)
            {
                InitIfNeeded();
                return knobs[index];
            }

            public ProgramHandler GetProgram(int index)
            {
                InitIfNeeded();
                return programs[index];
            }

            private bool initialized = false;
            private void InitIfNeeded()
            {
                if (!initialized)
                {
                    toggles = new ControlToggleChangedHandler[]
                    {
                        toggle1, toggle2, toggle3, toggle4,
                        toggle5, toggle6, toggle7, toggle8,
                    };

                    knobs = new ControlKnobHandler[]
                    {
                        knob1, knob2, knob3, knob4,
                        knob5, knob6, knob7, knob8,
                    };

                    programs = new ProgramHandler[]
                    {
                        program1, program2, program3, program4,
                        program5, program6, program7, program8,
                    };

                    initialized = true;
                }
            }
        }

        [Serializable]
        public class ControlToggleChangedHandler
        {
            public UnityEvent onStarted;
            public UnityEvent onStopped;

            [NonSerialized]
            public bool started = false;
        }

        [Serializable]
        public class ControlKnobHandler
        {
            public UnityEvent<byte> onValueChanged;
        }

        [Serializable]
        public class ProgramHandler
        {
            public UnityEvent onLaunched;
        }
    }
}
