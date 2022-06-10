using System;
using UnityEngine;

namespace Plml.Dmx
{
    [ExecuteAlways]
    public class DmxTrackElement : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxFixture fixture;

        public void InitializeFixture(DmxFixture fixture)
        {
            this.fixture = fixture;
            channels = new int[fixture.model.chanCount];
        }

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

        public void SetColors(Color32[] colors)
        {
            switch(colors.Length)
            {
                case 16:
                    SetColorArray16(colors);
                    break;
                case 32:
                    SetColorArray32(colors);
                    break;
            }
        }

        public void SetColorArray16(Color32[] colors) => SetColorArray_Internal(colors, DmxChannelType.ColorArray16);
        public void SetColorArray32(Color32[] colors) => SetColorArray_Internal(colors, DmxChannelType.ColorArray32);

        private void SetColorArray_Internal(Color32[] colors, DmxChannelType chanType)
        {
            int addr = fixture.model.GetChannelAddress(chanType);
            int expectedLength = chanType.ColorArrayCount();

            if (colors.Length != expectedLength)
                throw new InvalidOperationException($"Unexpected colors length: {colors.Length} (was expecting {expectedLength})");

            for (int i = 0; i < expectedLength; i++)
                SetColor_Internal(colors[i], addr + 3 * i);
        }

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
        public int GetChannel(DmxChannelType chanType) => channels[fixture.model.GetChannelAddress(chanType)];

        public bool TryGetChannel(DmxChannelType chanType, out int channel) => fixture.model.TryGetChannelAddress(chanType, out channel);
        public bool TryGetValue(DmxChannelType chanType, out int value)
        {
            bool hasChannel = TryGetChannel(chanType, out int channel);
            value = hasChannel ? channels[channel] : -1;
            return hasChannel;
        }
        public void SetChannel(DmxChannelType chanType, int value) => channels[fixture.model.GetChannelAddress(chanType)] = value;
        public bool TrySetChannel(DmxChannelType chanType, int value)
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

        public void SetDimmer(int value) => SetChannel(DmxChannelType.Dimmer, value);
        public void SetStroboscope(int value) => SetChannel(DmxChannelType.Stroboscope, value);
        public void SetWhite(int value) => SetChannel(DmxChannelType.White, value);
        public void SetUv(int value) => SetChannel(DmxChannelType.Uv, value);
        public void SetCold(int value) => SetChannel(DmxChannelType.Cold, value);
        public void SetWarm(int value) => SetChannel(DmxChannelType.Warm, value);
        public void SetAmber(int value) => SetChannel(DmxChannelType.Amber, value);
        public void SetPan(int value) => SetChannel(DmxChannelType.Pan, value);
        public void SetTilt(int value) => SetChannel(DmxChannelType.Tilt, value);

        public bool TrySetDimmer(int value) => TrySetChannel(DmxChannelType.Dimmer, value);
        public bool TrySetStroboscope(int value) => TrySetChannel(DmxChannelType.Stroboscope, value);
        public bool TrySetWhite(int value) => TrySetChannel(DmxChannelType.White, value);
        public bool TrySetUv(int value) => TrySetChannel(DmxChannelType.Uv, value);
        public bool TrySetCold(int value) => TrySetChannel(DmxChannelType.Cold, value);
        public bool TrySetWarm(int value) => TrySetChannel(DmxChannelType.Warm, value);
        public bool TrySetAmber(int value) => TrySetChannel(DmxChannelType.Amber, value);
        public bool TrySetPan(int value) => TrySetChannel(DmxChannelType.Pan, value);
        public bool TrySetTilt(int value) => TrySetChannel(DmxChannelType.Tilt, value);

        public Color color
        {
            get => GetColor();
            set => SetColor(value);
        }

        public int dimmer
        {
            get => GetDimmer();
            set => SetDimmer(value);
        }

        public int stroboscope
        {
            get => GetStroboscope();
            set => SetStroboscope(value);
        }

        public int white
        {
            get => GetWhite();
            set => SetWhite(value);
        }

        public int uv
        {
            get => GetUv();
            set => SetUv(value);
        }

        public int cold
        {
            get => GetCold();
            set => SetCold(value);
        }

        public int warm
        {
            get => GetWarm();
            set => SetWarm(value);
        }

        public int amber
        {
            get => GetAmber();
            set => SetAmber(value);
        }

        public int pan
        {
            get => GetPan();
            set => SetPan(value);
        }

        public int tilt
        {
            get => GetTilt();
            set => SetTilt(value);
        }
        #endregion

        private void Awake() => SetupLengthIfNeeded();

        private void Update() => SetupLengthIfNeeded();

        private void SetupLengthIfNeeded()
        {
            int chanCount = (fixture != null && fixture.model != null) ? fixture.model.chanCount : 0;
            
            if (channels?.Length != chanCount)
                channels = new int[chanCount];
        }
    }
}