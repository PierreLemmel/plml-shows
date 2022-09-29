using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Plml.Midi
{
    public sealed class MidiInputListener : MidiInputListenerBase
    {
        #region Actions
        [SerializeField, EditTimeOnly]
        private NoteOnData[] notesOn;
        [SerializeField, EditTimeOnly]
        private NoteOffData[] notesOff;
        [SerializeField, EditTimeOnly]
        private PolyphonicKeyPressureData[] polyphonicKeysPressure;
        [SerializeField, EditTimeOnly]
        private ControlChangedData[] controlsChanged;
        [SerializeField, EditTimeOnly]
        private ProgramChangedData[] programsChanged;


        [SerializeField]
        private UnityEvent<MidiNote, byte> onNoteOn;

        [SerializeField]
        private UnityEvent<MidiNote, byte> onNoteOff;

        [SerializeField]
        private UnityEvent<int, byte> onPolyphonicKeyPressure;

        [SerializeField]
        private UnityEvent<int, byte> onControlChanged;

        [SerializeField]
        private UnityEvent<int> onProgramChanged;

        [SerializeField]
        private UnityEvent<byte> onChannelPressure;

        [SerializeField]
        private UnityEvent<int> onPitchBend;


        private IDictionary<MidiNote, UnityEvent<byte>> notesOnDic;
        private IDictionary<MidiNote, UnityEvent<byte>> notesOffDic;
        private IDictionary<int, UnityEvent<byte>> pkpDic;
        private IDictionary<int, UnityEvent<byte>> ccDic;
        private IDictionary<int, UnityEvent> pcDic;
        #endregion

        #region Lifecycle
        private void Awake()
        {
            notesOnDic = notesOn.ToDictionary(
                no => no.note,
                no => no.action
            );

            notesOffDic = notesOff.ToDictionary(
                no => no.note,
                no => no.action
            );

            pkpDic = polyphonicKeysPressure.ToDictionary(
                pkp => pkp.key,
                pkp => pkp.action
            );

            ccDic = controlsChanged.ToDictionary(
                cc => cc.controller,
                cc => cc.action
            );

            pcDic = programsChanged.ToDictionary(
                pc => pc.program,
                pc => pc.action
            );
        }
        #endregion

        #region Add/Remove Listeners
        public void AddNoteOnListener(MidiNote note, UnityAction<byte> action)
        {
            if (!notesOnDic.TryGetValue(note, out UnityEvent<byte> ue))
            {
                ue = new();
                notesOnDic.Add(note, ue);
            }

            ue.AddListener(action);
        }

        public void AddNoteOffListener(MidiNote note, UnityAction<byte> action)
        {
            if (!notesOffDic.TryGetValue(note, out UnityEvent<byte> ue))
            {
                ue = new();
                notesOffDic.Add(note, ue);
            }

            ue.AddListener(action);
        }

        public void AddPolyphonicKeyPressureListener(int keyNumber, UnityAction<byte> action)
        {
            if (!pkpDic.TryGetValue(keyNumber, out UnityEvent<byte> ue))
            {
                ue = new();
                pkpDic.Add(keyNumber, ue);
            }

            ue.AddListener(action);
        }

        public void AddControlChangedListener(int controllerNumber, UnityAction<byte> action)
        {
            if (!ccDic.TryGetValue(controllerNumber, out UnityEvent<byte> ue))
            {
                ue = new();
                ccDic.Add(controllerNumber, ue);
            }

            ue.AddListener(action);
        }

        public void AddProgramChangedListener(int program, UnityAction action)
        {
            if (!pcDic.TryGetValue(program, out UnityEvent ue))
            {
                ue = new();
                pcDic.Add(program, ue);
            }

            ue.AddListener(action);
        }

        public void RemoveNoteOnListener(MidiNote note, UnityAction<byte> action) => notesOnDic[note].RemoveListener(action);
        public void RemoveNoteOffListener(MidiNote note, UnityAction<byte> action) => notesOffDic[note].RemoveListener(action);
        public void RemovePolyphonicKeyPressureListener(int keyNumber, UnityAction<byte> action) => pkpDic[keyNumber].RemoveListener(action);
        public void RemoveControlChangedListener(int controllerNumber, UnityAction<byte> action) => ccDic[controllerNumber].RemoveListener(action);
        public void RemoveProgramChangedListener(int program, UnityAction action) => pcDic[program].RemoveListener(action);
        #endregion

        #region Overrides
        public override void OnNoteOn(MidiNote note, byte velocity)
        {
            onNoteOn?.Invoke(note, velocity);

            if (notesOnDic != null && notesOnDic.TryGetValue(note, out var handler))
            {

                handler.Invoke(velocity);
            }
        }

        public override void OnNoteOff(MidiNote note, byte velocity)
        {
            onNoteOff?.Invoke(note, velocity);

            if (notesOnDic != null && notesOffDic.TryGetValue(note, out var handler))
                handler.Invoke(velocity);
        }

        public override void OnPolyphonicKeyPressure(int keyNumber, byte pressure)
        {
            onPolyphonicKeyPressure?.Invoke(keyNumber, pressure);

            if (notesOnDic != null && pkpDic.TryGetValue(keyNumber, out var handler))
                handler.Invoke(pressure);
        }

        public override void OnControlChanged(int controllerNumber, byte value)
        {
            onControlChanged?.Invoke(controllerNumber, value);

            if (ccDic != null && ccDic.TryGetValue(controllerNumber, out var handler))
                handler?.Invoke(value);
        }

        public override void OnProgramChanged(int program)
        {
            onProgramChanged?.Invoke(program);

            if (pcDic != null && pcDic.TryGetValue(program, out var handler))
                handler?.Invoke();
        }

        public override void OnChannelPressure(byte pressure) => onChannelPressure?.Invoke(pressure);
        public override void OnPitchBend(int pitch) => onPitchBend?.Invoke(pitch);
        #endregion

        #region Structs
        [Serializable]
        public struct NoteOnData
        {
            public MidiNote note;
            public UnityEvent<byte> action;
        }

        [Serializable]
        public struct NoteOffData
        {
            public MidiNote note;
            public UnityEvent<byte> action;
        }

        [Serializable]
        public struct PolyphonicKeyPressureData
        {
            public int key;
            public UnityEvent<byte> action;
        }

        [Serializable]
        public struct ControlChangedData
        {
            public int controller;
            public UnityEvent<byte> action;
        }

        [Serializable]
        public struct ProgramChangedData
        {
            public int program;
            public UnityEvent action;
        }
        #endregion
    }
}