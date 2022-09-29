using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Plml.Midi
{
    public class MidiInputDispatcher : MonoBehaviour
    {
        private IntPtr handle = IntPtr.Zero;

        private MidiInputListenerBase[] listeners;

        // Can't access class members from another thread
        private static ConcurrentQueue<int> messageQueue = new();

        private bool hasMidiInput = false;
        public bool log = true;

        private void Awake()
        {
            if (log)
            {
                this.AddChild("Log")
                    .WithComponent<LogMidiListener>();
            }

            listeners = FindObjectsOfType<MidiInputListenerBase>();

            if (FindObjectsOfType<MidiInputDispatcher>().Length > 1)
                Debug.LogError("Multiple Midi dispatchers can cause crash. Don't do that.");
        }

        private void Start()
        {
            Debug.Log("Searching for Midi device");
            int numDevs = WinMMDll.MidiInGetNumDevs();

            if (numDevs == 0)
            {
                Debug.LogWarning("No Midi device found, won't receive Midi inputs.");
            }
            else if (numDevs > 1)
                throw new InvalidOperationException("Multiple Midi devices found");
            else
            {
                hasMidiInput = true;
                Debug.Log("Midi device found");
            }

            if (!hasMidiInput) return;

            Debug.Log("Opening Midi device");
            Mmsyserr openResult = WinMMDll.MidiInOpen(ref handle, 0, OnMidiMessage, 0, 0x30000);
            CheckResult(openResult, nameof(WinMMDll.MidiInOpen));
            Debug.Log("Midi device opened");


            Debug.Log("Starting Midi device");
            Mmsyserr startResult = WinMMDll.MidiInStart(handle);
            CheckResult(startResult, nameof(WinMMDll.MidiInStart));
            Debug.Log("Midi device started");
        }

        private void OnDestroy()
        {
            if (!hasMidiInput) return;

            Debug.Log("Stopping Midi device");
            Mmsyserr stopResult = WinMMDll.MidiInStop(handle);
            CheckResult(stopResult, nameof(WinMMDll.MidiInStop));
            Debug.Log("Midi device stopped");


            Debug.Log("Closing Midi device");
            Mmsyserr closeResult = WinMMDll.MidiInClose(handle);
            CheckResult(closeResult, nameof(WinMMDll.MidiInClose));
            Debug.Log("Midi device closed");
        }

        private void Update()
        {
            if (!hasMidiInput) return;

            while (messageQueue.TryDequeue(out int packedMsg))
            {
                byte status = (byte)(packedMsg & 0xff);
                byte data1 = (byte)((packedMsg & 0xff00) >> 8);
                byte data2 = (byte)((packedMsg & -65536) >> 0x10);

                foreach (var listener in listeners)
                {
                    listener.OnShortMessage(status, data1, data2);
                }
            }
        }

        private static void CheckResult(Mmsyserr result, string function)
        {
            if (result != Mmsyserr.MmsyserrNoerror)
                throw new InvalidOperationException($"{function} - Unexpected Midi result: {result} ({(int)result:X4})");
        }

        private static void OnMidiMessage(IntPtr handle, MidiMsg msg, int instance, int param1, int param2)
        {
            switch (msg)
            {
                case MidiMsg.Open:
                case MidiMsg.Close:
                    Debug.Log($"Midi message: {msg} ({(int)msg:x4}");
                    break;
                case MidiMsg.ShortMessage1:
                    HandleShortMessage(param1);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected Midi message: {msg}");
            }
        }

        private static void HandleShortMessage(int packedMsg) => messageQueue.Enqueue(packedMsg);
    }
}