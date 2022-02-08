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

        #region Getters / Setters
        public bool TrySetColor(Color32 color32)
        {
            bool hasColor = fixture.model.TryGetChannelAddress(DmxChannelType.Color, out int channel);
            if (hasColor)
            {
                SetColor_Internal(color32, channel);
            }
            return hasColor;
        }

        public void SetColor(Color32 color32) => SetColor_Internal(color32, fixture.model.GetChannelAddress(DmxChannelType.Color));

        private void SetColor_Internal(Color32 color32, int colorChan)
        {
            byte r = color32.r;
            byte g = color32.g;
            byte b = color32.b;

            channels[colorChan] = r;
            channels[colorChan + 1] = g;
            channels[colorChan + 2] = b;
        }

        public bool HasColor() => HasChannel(DmxChannelType.Color);

        public Color32 GetColor() => GetColor_Internal(fixture.model.GetChannelAddress(DmxChannelType.Color));

        public bool TryGetColor(out Color32 color32)
        {
            bool hasColor = TryGetChannel(DmxChannelType.Color, out int colorChan);
            color32 = hasColor ? GetColor_Internal(colorChan) : default;
            return hasColor;
        }

        private Color32 GetColor_Internal(int colorChan)
        {
            byte r = (byte)channels[colorChan];
            byte g = (byte)channels[colorChan + 1];
            byte b = (byte)channels[colorChan + 2];

            return new(r, g, b, 0xff);
        }

        public bool HasChannel(DmxChannelType chanType) => fixture.model.HasChannel(chanType);
        public int GetChannel(DmxChannelType chanType)
        {
            try
            {
                return channels[fixture.model.GetChannelAddress(chanType)];
            }
            catch (Exception)
            {

                Debug.Log(channels.Length);
                Debug.Log(chanType);
                Debug.Log(name);
                throw;
            }
        }

        public bool TryGetChannel(DmxChannelType chanType, out int channel) => fixture.model.TryGetChannelAddress(chanType, out channel);
        public bool TryGetValue(DmxChannelType chanType, out int value)
        {
            bool hasChannel = TryGetChannel(chanType, out int channel);
            value = hasChannel ? channels[channel] : -1;
            return hasChannel;
        }
        public void SetChannel(DmxChannelType chanType, byte value) => channels[fixture.model.GetChannelAddress(chanType)] = value;
        public bool TrySetChannel(DmxChannelType chanType, byte value)
        {
            bool hasChannel = TryGetChannel(chanType, out int channel);
            if (hasChannel)
            {
                channels[channel] = value;
            }
            return hasChannel;
        }


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

        public bool TryGetDimmer(out int value) => TryGetValue(DmxChannelType.Dimmer, out value);
        public bool TryGetStroboscope(out int value) => TryGetValue(DmxChannelType.Stroboscope, out value);
        public bool TryGetWhite(out int value) => TryGetValue(DmxChannelType.White, out value);
        public bool TryGetUv(out int value) => TryGetValue(DmxChannelType.Uv, out value);
        public bool TryGetCold(out int value) => TryGetValue(DmxChannelType.Cold, out value);
        public bool TryGetWarm(out int value) => TryGetValue(DmxChannelType.Warm, out value);
        public bool TryGetAmber(out int value) => TryGetValue(DmxChannelType.Amber, out value);
        public bool TryGetPan(out int value) => TryGetValue(DmxChannelType.Pan, out value);
        public bool TryGetTilt(out int value) => TryGetValue(DmxChannelType.Tilt, out value);

        public void SetDimmer(byte value) => SetChannel(DmxChannelType.Dimmer, value);
        public void SetStroboscope(byte value) => SetChannel(DmxChannelType.Stroboscope, value);
        public void SetWhite(byte value) => SetChannel(DmxChannelType.White, value);
        public void SetUv(byte value) => SetChannel(DmxChannelType.Uv, value);
        public void SetCold(byte value) => SetChannel(DmxChannelType.Cold, value);
        public void SetWarm(byte value) => SetChannel(DmxChannelType.Warm, value);
        public void SetAmber(byte value) => SetChannel(DmxChannelType.Amber, value);
        public void SetPan(byte value) => SetChannel(DmxChannelType.Pan, value);
        public void SetTilt(byte value) => SetChannel(DmxChannelType.Tilt, value);

        public bool TrySetDimmer(byte value) => TrySetChannel(DmxChannelType.Dimmer, value);
        public bool TrySetStroboscope(byte value) => TrySetChannel(DmxChannelType.Stroboscope, value);
        public bool TrySetWhite(byte value) => TrySetChannel(DmxChannelType.White, value);
        public bool TrySetUv(byte value) => TrySetChannel(DmxChannelType.Uv, value);
        public bool TrySetCold(byte value) => TrySetChannel(DmxChannelType.Cold, value);
        public bool TrySetWarm(byte value) => TrySetChannel(DmxChannelType.Warm, value);
        public bool TrySetAmber(byte value) => TrySetChannel(DmxChannelType.Amber, value);
        public bool TrySetPan(byte value) => TrySetChannel(DmxChannelType.Pan, value);
        public bool TrySetTilt(byte value) => TrySetChannel(DmxChannelType.Tilt, value);
        #endregion

        private void Awake()
        {
            Debug.Log($"Awake {name}");
            SetupLengthIfNeeded();
        }

        private void Update() => SetupLengthIfNeeded();

        private void SetupLengthIfNeeded()
        {
            int chanCount = fixture?.model?.chanCount ?? 0;
            Debug.Log($"{name} {chanCount}");
            if (channels?.Length != chanCount)
                channels = new int[chanCount];
        }
    }
}