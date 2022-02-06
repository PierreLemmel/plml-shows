using System;
using UnityEngine;

namespace Plml.Dmx
{
    [ExecuteAlways]
    public class DmxTrackElement : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxFixture fixture;

        [ReadOnly]
        public int[] channels = Array.Empty<int>();

        public int Address => fixture.channelOffset;

        public void SetColor(Color32 color32)
        {
            int colorChan = fixture.model.GetChannelAddress(DmxChannelType.Color);

            byte r = color32.r;
            byte g = color32.g;
            byte b = color32.b;

            channels[colorChan] = r;
            channels[colorChan + 1] = g;
            channels[colorChan + 2] = b;
        }
        public bool HasColor() => fixture.model.HasChannel(DmxChannelType.Color);

        public Color32 GetColor()
        {
            int colorChan = fixture.model.GetChannelAddress(DmxChannelType.Color);

            byte r = (byte) channels[colorChan];
            byte g = (byte) channels[colorChan + 1];
            byte b = (byte) channels[colorChan + 2];

            return new(r, g, b, 0xff);
        }

        public void SetChannel(DmxChannelType chanType, byte value) => channels[fixture.model.GetChannelAddress(chanType)] = value;
        public bool HasChannel(DmxChannelType chanType) => fixture.model.HasChannel(chanType);
        public int GetChannel(DmxChannelType chanType) => channels[fixture.model.GetChannelAddress(chanType)];

        public void SetDimmer(byte value) => SetChannel(DmxChannelType.Dimmer, value);
        public void SetStroboscope(byte value) => SetChannel(DmxChannelType.Stroboscope, value);
        public void SetWhite(byte value) => SetChannel(DmxChannelType.White, value);
        public void SetUv(byte value) => SetChannel(DmxChannelType.Uv, value);
        public void SetCold(byte value) => SetChannel(DmxChannelType.Cold, value);
        public void SetWarm(byte value) => SetChannel(DmxChannelType.Warm, value);
        public void SetAmber(byte value) => SetChannel(DmxChannelType.Amber, value);
        public void SetPan(byte value) => SetChannel(DmxChannelType.Pan, value);
        public void SetTilt(byte value) => SetChannel(DmxChannelType.Tilt, value);

        public bool HasDimmer() => HasChannel(DmxChannelType.Dimmer);
        public bool HasStroboscope() => HasChannel(DmxChannelType.Stroboscope);
        public bool HasWhite() => HasChannel(DmxChannelType.White);
        public bool HasUv() => HasChannel(DmxChannelType.Uv);
        public bool HasCold() => HasChannel(DmxChannelType.Cold);
        public bool HasWarm() => HasChannel(DmxChannelType.Warm);
        public bool HasAmber() => HasChannel(DmxChannelType.Amber);
        public bool HasPan() => HasChannel(DmxChannelType.Pan);
        public bool HasTilt() => HasChannel(DmxChannelType.Tilt);

        public int GetDimmer() => GetChannel(DmxChannelType.Dimmer);
        public int GetStroboscope() => GetChannel(DmxChannelType.Stroboscope);
        public int GetWhite() => GetChannel(DmxChannelType.White);
        public int GetUv() => GetChannel(DmxChannelType.Uv);
        public int GetCold() => GetChannel(DmxChannelType.Cold);
        public int GetWarm() => GetChannel(DmxChannelType.Warm);
        public int GetAmber() => GetChannel(DmxChannelType.Amber);
        public int GetPan() => GetChannel(DmxChannelType.Pan);
        public int GetTilt() => GetChannel(DmxChannelType.Tilt);


        private void Update()
        {
            int chanCount = fixture?.model.chanCount ?? 0;
            if (channels.Length != chanCount)
                channels = new int[chanCount];
        }
    }
}