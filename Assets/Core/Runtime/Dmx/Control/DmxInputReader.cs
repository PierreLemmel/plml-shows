using System;
using UnityEngine;

namespace Plml.Dmx
{
    public class DmxInputReader : MonoBehaviour
    {
        public ReadDmxInterfaceType dmxInterface;

        private IReadDmxInterface _dmxInterface;


        private ReadDmxInterfaceType lastType = ReadDmxInterfaceType.None;
        private IReadDmxInterface GetDmxInterface()
        {
            if (_dmxInterface == null || dmxInterface != lastType)
            {
                _dmxInterface = DmxInterfaces.CreateReadInterface(dmxInterface);
                lastType = dmxInterface;
            }

            return _dmxInterface;
        }

        private void OnEnable() => StartListening();
        private void OnDisable() => StopListening();

        private void StartListening()
        {
            var dmxInterface = GetDmxInterface();

            dmxInterface.Start();
        }

        private void StopListening()
        {
            var dmxInterface = GetDmxInterface();

            dmxInterface.Stop();
            dmxInterface.Dispose();
        }

#if UNITY_EDITOR
        public void StartListeningFromEditor() => StartListening();
        public void StopListeningFromEditor() => StopListening();
#endif
    }
}