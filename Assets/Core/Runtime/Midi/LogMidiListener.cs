using UnityEngine;

namespace Plml.Midi
{
    public class LogMidiListener : MidiInputListener
    {
        public override void OnNoteOn(byte keyNumber, byte velocity) => Debug.Log($"On note on: {keyNumber} {velocity}");
        public override void OnNoteOff(byte keyNumber, byte velocity) => Debug.Log($"On note off: {keyNumber} {velocity}");
        public override void OnPolyphonicKeyPressure(int keyNumber, byte pressure) => Debug.Log($"On polyphonic key pressure: {keyNumber} {pressure}");
        public override void OnControlChanged(int controllerNumber, byte value) => Debug.Log($"On control changed: {controllerNumber} {value}");
        public override void OnProgramChanged(int program) => Debug.Log($"On program changed: {program}");
        public override void OnChannelPressure(byte pressure) => Debug.Log($"Channel pressure: {pressure}");
        public override void OnPitchBend(int pitch) => Debug.Log($"Pitch bend: {pitch}");
    }
}