using System;
using System.Runtime.InteropServices;

namespace Plml.Midi
{
    internal static unsafe class WinMMDll
    {
        private const string LibName = "winmm.dll";

        public delegate void MidiCallBack(IntPtr handle, MidiMsg msg, int instance, int param1, int param2);


        [DllImport(LibName, EntryPoint = "midiInGetNumDevs")]
        public static extern int MidiInGetNumDevs();

        [DllImport(LibName, EntryPoint = "midiInOpen")]
        public static extern Mmsyserr MidiInOpen(ref IntPtr handle, int deviceID, MidiCallBack proc, int instance, int flags);

        [DllImport(LibName, EntryPoint = "midiInStart")]
        public static extern Mmsyserr MidiInStart(IntPtr handle);

        [DllImport(LibName, EntryPoint = "midiInReset")]
        public static extern Mmsyserr MidiInReset(IntPtr handle);

        [DllImport(LibName, EntryPoint = "midiInStop")]
        public static extern Mmsyserr MidiInStop(IntPtr handle);

        [DllImport(LibName, EntryPoint = "midiInClose")]
        public static extern Mmsyserr MidiInClose(IntPtr handle);
    }
}
