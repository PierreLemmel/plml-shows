using UnityEngine;

namespace Plml.Midi
{
    public class LogMidiListener : MidiInputListenerBase
    {
        public override void OnNoteOn(MidiNote note, byte velocity) => Debug.Log($"On note on: {note} {velocity}");
        public override void OnNoteOff(MidiNote note, byte velocity) => Debug.Log($"On note off: {note} {velocity}");
        public override void OnPolyphonicKeyPressure(int keyNumber, byte pressure) => Debug.Log($"On polyphonic key pressure: {keyNumber} {pressure}");
        public override void OnControlChanged(int controllerNumber, byte value) => Debug.Log($"On control changed: {controllerNumber} {value}");
        public override void OnProgramChanged(int program) => Debug.Log($"On program changed: {program}");
        public override void OnChannelPressure(byte pressure) => Debug.Log($"Channel pressure: {pressure}");
        public override void OnPitchBend(int pitch) => Debug.Log($"Pitch bend: {pitch}");
    }
}