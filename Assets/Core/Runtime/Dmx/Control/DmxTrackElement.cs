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
        public bool TrySetColor(Color24 color24)
        {
            bool hasColor = fixture.model.TryGetChannelAddress(DmxChannelType.Color, out int channel);
            if (hasColor)
            {
                SetColor_Internal(color24, channel);
            }
            return hasColor;
        }

        public void SetColorOrSplitColor(Color24 color24)
        {
            if (HasColor())
                SetColor(color24);
            else if (HasSplitColor())
                SetSplitColor(color24);
            else
                throw new InvalidOperationException($"Missing Color channel on fixture '{fixture.name}'");
        }

        public void SetColor(Color24 color24) => SetColor_Internal(color24, fixture.model.GetChannelAddress(DmxChannelType.Color));

        public void SetSplitColor(Color24 color24) => SetSplitColor_Internal(color24, fixture.model.GetChannelAddress(DmxChannelType.SplitColor));

        public void SetColors(Color24[] colors)
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

        public void SetColorArray16(Color24[] colors) => SetColorArray_Internal(colors, DmxChannelType.ColorArray16);
        public void SetColorArray32(Color24[] colors) => SetColorArray_Internal(colors, DmxChannelType.ColorArray32);

        private void SetColorArray_Internal(Color24[] colors, DmxChannelType chanType)
        {
            int addr = fixture.model.GetChannelAddress(chanType);
            int expectedLength = chanType.ColorArrayCount();

            if (colors.Length != expectedLength)
                throw new InvalidOperationException($"Unexpected colors length: {colors.Length} (was expecting {expectedLength})");

            for (int i = 0; i < expectedLength; i++)
                SetColor_Internal(colors[i], addr + 3 * i);
        }

        private void SetColor_Internal(Color24 color24, int colorChan)
        {
            byte r = color24.r;
            byte g = color24.g;
            byte b = color24.b;

            channels[colorChan] = r;
            channels[colorChan + 1] = g;
            channels[colorChan + 2] = b;
        }

        private void SetSplitColor_Internal(Color24 color24, int colorChan)
        {
            byte r = color24.r;
            byte g = color24.g;
            byte b = color24.b;

            channels[colorChan] = r;
            channels[colorChan + 2] = g;
            channels[colorChan + 4] = b;
        }

        public bool HasColor() => HasChannel(DmxChannelType.Color);
        public bool HasSplitColor() => HasChannel(DmxChannelType.SplitColor);

        public Color24 GetColor() => GetColor_Internal(fixture.model.GetChannelAddress(DmxChannelType.Color));
        public Color24 GetSplitColor() => GetSplitColor_Internal(fixture.model.GetChannelAddress(DmxChannelType.SplitColor));

        public Color24[] GetColorArray16() => GetColors_Internal(DmxChannelType.ColorArray16);
        public void GetColorArray16(Color24[] buffer) => GetColors_Internal(DmxChannelType.ColorArray16, buffer);

        public Color24[] GetColorArray32() => GetColors_Internal(DmxChannelType.ColorArray32);
        public void GetColorArray32(Color24[] buffer) => GetColors_Internal(DmxChannelType.ColorArray32, buffer);

        public Color24[] GetColors()
        {
            if (fixture.model.HasChannel(DmxChannelType.ColorArray16))
                return GetColorArray16();
            else if (fixture.model.HasChannel(DmxChannelType.ColorArray32))
                return GetColorArray32();
            else
                throw new InvalidOperationException("Fixture have no color array");
        }

        public void GetColors(Color24[] buffer)
        {
            if (fixture.model.HasChannel(DmxChannelType.ColorArray16))
                GetColorArray16(buffer);
            else if (fixture.model.HasChannel(DmxChannelType.ColorArray32))
                GetColorArray32(buffer);
            else
                throw new InvalidOperationException("Fixture have no color array");
        }


        private Color24[] GetColors_Internal(DmxChannelType chanType)
        {
            int baseAddr = fixture.model.GetChannelAddress(chanType);
            int count = chanType.ColorArrayCount();
            return Arrays.Create(count, i => GetColor_Internal(baseAddr + 3 * i));
        }

        private void GetColors_Internal(DmxChannelType chanType, Color24[] buffer)
        {
            int baseAddr = fixture.model.GetChannelAddress(chanType);
            int count = chanType.ColorArrayCount();

            for (int i = 0; i < count; i++)
                buffer[i] = GetColor_Internal(baseAddr + 3 * i);
        }

        public bool TryGetColor(out Color24 color24)
        {
            bool hasColor = TryGetChannel(DmxChannelType.Color, out int colorChan);
            color24 = hasColor ? GetColor_Internal(colorChan) : default;
            return hasColor;
        }

        public bool TryGetSplitColor(out Color24 color24)
        {
            bool hasSplitColor = TryGetChannel(DmxChannelType.SplitColor, out int colorChan);
            color24 = hasSplitColor ? GetSplitColor_Internal(colorChan) : default;
            return hasSplitColor;
        }

        private Color24 GetColor_Internal(int colorChan) => new(
            channels[colorChan],
            channels[colorChan + 1],
            channels[colorChan + 2]
        );

        private Color24 GetSplitColor_Internal(int colorChan) => new(
            channels[colorChan],
            channels[colorChan + 2],
            channels[colorChan + 4]
        );

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

        public void SetChannelIfGreater(DmxChannelType chanType, int Value)
        {
            int oldVal = GetChannel(chanType);
            if (value > oldVal)
            {
                SetChannel(chanType, value);
            }
        }

        public bool IsTrad() => HasChannel(DmxChannelType.Trad);
        public bool HasDimmer() => HasChannel(DmxChannelType.Dimmer);
        public bool HasStroboscope() => HasChannel(DmxChannelType.Stroboscope);
        public bool HasWhite() => HasChannel(DmxChannelType.White);
        public bool HasUv() => HasChannel(DmxChannelType.Uv);
        public bool HasCold() => HasChannel(DmxChannelType.Cold);
        public bool HasWarm() => HasChannel(DmxChannelType.Warm);
        public bool HasAmber() => HasChannel(DmxChannelType.Amber);
        public bool HasPan() => HasChannel(DmxChannelType.Pan);
        public bool HasTilt() => HasChannel(DmxChannelType.Tilt);

        public int GetValue() => GetChannel(DmxChannelType.Trad);
        public int GetDimmer() => GetChannel(DmxChannelType.Dimmer);
        public int GetBeam() => GetChannel(DmxChannelType.Beam);
        public int GetStroboscope() => GetChannel(DmxChannelType.Stroboscope);
        public int GetWhite() => GetChannel(DmxChannelType.White);
        public int GetUv() => GetChannel(DmxChannelType.Uv);
        public int GetCold() => GetChannel(DmxChannelType.Cold);
        public int GetWarm() => GetChannel(DmxChannelType.Warm);
        public int GetAmber() => GetChannel(DmxChannelType.Amber);
        public int GetPan() => GetChannel(DmxChannelType.Pan);
        public int GetTilt() => GetChannel(DmxChannelType.Tilt);

        public bool TryGetValue(out int value) => TryGetValue(DmxChannelType.Trad, out value);
        public bool TryGetDimmer(out int value) => TryGetValue(DmxChannelType.Dimmer, out value);
        public bool TryGetStroboscope(out int value) => TryGetValue(DmxChannelType.Stroboscope, out value);
        public bool TryGetWhite(out int value) => TryGetValue(DmxChannelType.White, out value);
        public bool TryGetUv(out int value) => TryGetValue(DmxChannelType.Uv, out value);
        public bool TryGetCold(out int value) => TryGetValue(DmxChannelType.Cold, out value);
        public bool TryGetWarm(out int value) => TryGetValue(DmxChannelType.Warm, out value);
        public bool TryGetAmber(out int value) => TryGetValue(DmxChannelType.Amber, out value);
        public bool TryGetPan(out int value) => TryGetValue(DmxChannelType.Pan, out value);
        public bool TryGetTilt(out int value) => TryGetValue(DmxChannelType.Tilt, out value);

        public void SetValue(int value) => SetChannel(DmxChannelType.Trad, value);
        public void SetDimmer(int value) => SetChannel(DmxChannelType.Dimmer, value);
        public void SetBeam(int value) => SetChannel(DmxChannelType.Beam, value);
        public void SetStroboscope(int value) => SetChannel(DmxChannelType.Stroboscope, value);
        public void SetWhite(int value) => SetChannel(DmxChannelType.White, value);
        public void SetUv(int value) => SetChannel(DmxChannelType.Uv, value);
        public void SetCold(int value) => SetChannel(DmxChannelType.Cold, value);
        public void SetWarm(int value) => SetChannel(DmxChannelType.Warm, value);
        public void SetAmber(int value) => SetChannel(DmxChannelType.Amber, value);
        public void SetPan(int value) => SetChannel(DmxChannelType.Pan, value);
        public void SetTilt(int value) => SetChannel(DmxChannelType.Tilt, value);

        public bool TrySetValue(int value) => TrySetChannel(DmxChannelType.Trad, value);
        public bool TrySetDimmer(int value) => TrySetChannel(DmxChannelType.Dimmer, value);
        public bool TrySetStroboscope(int value) => TrySetChannel(DmxChannelType.Stroboscope, value);
        public bool TrySetWhite(int value) => TrySetChannel(DmxChannelType.White, value);
        public bool TrySetUv(int value) => TrySetChannel(DmxChannelType.Uv, value);
        public bool TrySetCold(int value) => TrySetChannel(DmxChannelType.Cold, value);
        public bool TrySetWarm(int value) => TrySetChannel(DmxChannelType.Warm, value);
        public bool TrySetAmber(int value) => TrySetChannel(DmxChannelType.Amber, value);
        public bool TrySetPan(int value) => TrySetChannel(DmxChannelType.Pan, value);
        public bool TrySetTilt(int value) => TrySetChannel(DmxChannelType.Tilt, value);

        public void SetValueIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Trad, value);
        public void SetDimmerIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Dimmer, value);
        public void SetBeamIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Beam, value);
        public void SetStroboscopeIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Stroboscope, value);
        public void SetWhiteIfGreater(int value) => SetChannelIfGreater(DmxChannelType.White, value);
        public void SetUvIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Uv, value);
        public void SetColdIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Cold, value);
        public void SetWarmIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Warm, value);
        public void SetAmberIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Amber, value);
        public void SetPanIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Pan, value);
        public void SetTiltIfGreater(int value) => SetChannelIfGreater(DmxChannelType.Tilt, value);

        public Color24 color
        {
            get => GetColor();
            set => SetColor(value);
        }

        public Color24 splitColor
        {
            get => GetSplitColor();
            set => SetSplitColor(value);
        }


        public int value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public int dimmer
        {
            get => GetDimmer();
            set => SetDimmer(value);
        }

        public int beam
        {
            get => GetBeam();
            set => SetBeam(value);
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