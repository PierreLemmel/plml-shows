using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Midi
{
    public class MidiInputDispatcher : MonoBehaviour
    {
        private IntPtr handle = IntPtr.Zero;

        private MidiInputListener[] listeners;

        // Can't access class members from another thread
        private static ConcurrentQueue<int> messageQueue = new ConcurrentQueue<int>();

        private void Awake()
        {
            listeners = FindObjectsOfType<MidiInputListener>();

            if (FindObjectsOfType<MidiInputDispatcher>().Length > 1)
                Debug.LogError("Multiple Midi dispatchers can cause crash. Don't do that.");
        }

        private void Start()
        {
            Debug.Log("Searching for Midi device");
            int numDevs = WinMMDll.MidiInGetNumDevs();
            
            if (numDevs == 0)
                throw new InvalidOperationException("No MIDI device found");
            if (numDevs > 1)
                throw new InvalidOperationException("Multiple MIDI devices found");

            Debug.Log("Midi device found");


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
            while (messageQueue.TryDequeue(out int packedMsg))
            {
                byte status = (byte)(packedMsg & 0xff);
                byte data1 = (byte)((packedMsg & 0xff00) >> 8);
                byte data2 = (byte)((packedMsg & -65536) >> 0x10);

                foreach (MidiInputListener listener in listeners)
                {
                    listener.OnShortMessage(status, data1, data2);
                }
            }
        }

        private void CheckResult(Mmsyserr result, string function)
        {
            if (result != Mmsyserr.MmsyserrNoerror)
                throw new InvalidOperationException($"{function} - Unexpected Midi result: {result} ({(int)result:X4})");
        }

        private void OnMidiMessage(IntPtr handle, MidiMsg msg, int instance, int param1, int param2)
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

        private void HandleShortMessage(int packedMsg) => messageQueue.Enqueue(packedMsg);
    }
}