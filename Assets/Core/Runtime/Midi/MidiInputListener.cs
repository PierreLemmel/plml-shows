using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Midi
{
    public abstract class MidiInputListener : MonoBehaviour
    {
        public void OnShortMessage(byte status, byte data1, byte data2)
        {
            switch (status / 0x10)
            {
                case 0x8:
                    OnNoteOff(data1, data2);
                    break;
                case 0x9:
                    OnNoteOn(data1, data2);
                    break;
                case 0xa:
                    OnPolyphonicKeyPressure(data1, data2);
                    break;
                case 0xb:
                    OnControlChanged(data1, data2);
                    break;
                case 0xc:
                    OnProgramChanged(data1);
                    break;
                case 0xd:
                    OnChannelPressure(data1);
                    break;
                case 0xe:
                    int pitch = 0x100 * data1 + data2;
                    OnPitchBend(pitch);
                    break;
            }
        }

        public virtual void OnNoteOn(byte keyNumber, byte velocity) { }
        public virtual void OnNoteOff(byte keyNumber, byte velocity) { }
        public virtual void OnPolyphonicKeyPressure(int keyNumber, byte pressure) { }
        public virtual void OnControlChanged(int controllerNumber, byte value) { }
        public virtual void OnProgramChanged(int program) { }
        public virtual void OnChannelPressure(byte pressure) { }
        public virtual void OnPitchBend(int pitch) { }
    }
}