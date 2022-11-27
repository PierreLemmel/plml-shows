using Plml.Dmx.OpenDmx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Dmx
{
    public class DmxTrackControler : MonoBehaviour
    {
        [EditTimeOnly]
        public GameObject fixturesObject;

        [EditTimeOnly]
        public DmxTrackCollection tracksObject;

        [Range(0.0f, 1.0f)]
        public float master = 1.0f;

        [CubicRange(0.0f, 60.0f)]
        public float fade = 0.0f;

        [Range(1.0f, 60.0f)]
        public float refreshRate = 30.0f;

        public DmxInterfaceType dmxInterface;

        private IDmxInterface _dmxInterface;

        private bool canRead;
        private bool canWrite;
        private IDmxInterface GetDmxInterface()
        {
            if (_dmxInterface == null)
            {
                _dmxInterface = DmxInterfaces.Create(dmxInterface);

                canRead = _dmxInterface.HasReadFeature();
                canWrite = _dmxInterface.HasWriteFeature();
            }

            return _dmxInterface;
        }

        private Dictionary<Guid, GameObject> addedTracks;

        private byte[] channels;
        private float[] currents;
        private float[] speeds;
        private float[] targets;

        private float lastTime;

        private void Awake() => Setup();

        private void OnEnable() => GetDmxInterface().CopyData(channels);

        private void Update()
        {
            RebuildDmxFrame();

            if (Time.time - lastTime < 1.0f / refreshRate) return;
            lastTime = Time.time;

            SendFrame();
        }

        private void OnDisable() => GetDmxInterface().ClearFrame();

        private void OnDestroy() => Cleanup();

        public DmxTrack AddTrack(DmxTrack track, out Guid trackId)
        {
            var clone = Instantiate(track.gameObject);
            trackId = Guid.NewGuid();
            clone.AttachTo(tracksObject);

            addedTracks.Add(trackId, clone);

            return clone.GetComponent<DmxTrack>();
        }

        public void RemoveTrack(Guid trackId)
        {
            var track = addedTracks[trackId];
            addedTracks.Remove(trackId);
            Destroy(track);
        }

        private void Setup()
        {
            int lastChannel = fixturesObject
                .GetComponentsInChildren<DmxFixture>()
                .Max(fix => fix.channelOffset + fix.model.chanCount);

            channels = new byte[lastChannel];
            currents = new float[lastChannel];
            speeds = new float[lastChannel];
            targets = new float[lastChannel];

            lastTime = Time.time;

            addedTracks = new();

            GetDmxInterface().Start();
        }

        private void Cleanup()
        {
            var dmxInterface = GetDmxInterface();

            dmxInterface.Stop();
            dmxInterface.Dispose();
        }

        private void RebuildDmxFrame()
        {
            Array.Clear(targets, 0, targets.Length);

            var tracks = tracksObject.GetComponentsInChildren<DmxTrack>();

            foreach (DmxTrack track in tracks.Where(t => t.isPlaying))
            {
                float master = track.master;

                foreach (DmxTrackElement elt in track.Elements)
                {
                    int[] channels = elt.channels;
                    for (int i = 0, address = elt.Address; i < channels.Length; i++, address++)
                    {
                        targets[address] = Math.Max(targets[address], master * channels[i]);
                    }
                }
            }

            for (int i = 0; i < channels.Length; i++)
            {
                currents[i] = Mathf.SmoothDamp(currents[i], master * targets[i], ref speeds[i], fade);
                channels[i] = (byte)currents[i];
            }
        }

        private void SendFrame()
        {
            if (canWrite)
            {
                var dmxInterface = GetDmxInterface();

            
                dmxInterface.CopyData(channels);
                dmxInterface.SendFrame();
            }
        }

#if UNITY_EDITOR
        public void SetupFromEditor() => Setup();

        public void SendCurrentFrameFromEditor()
        {
            RebuildDmxFrame();
            SendFrame();
        }

        public void StopSendingFrameFromEditor()
        {
            GetDmxInterface().ClearFrame();

            Cleanup();
        }

        public DmxFeature GetFeaturesFromEditor() => GetDmxInterface().Features;
        public byte[] GetChannelsFromEditor() => channels;
#endif
    }
}