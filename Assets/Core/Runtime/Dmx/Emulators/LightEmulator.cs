using Plml.Dmx;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using UText = UnityEngine.UI.Text;

namespace Plml.Dmx.Emulators
{
    public abstract class LightEmulator : MonoBehaviour
    {
        protected static readonly Color coldColor = Color.white;
        protected static readonly Color warmColor = Color.yellow;
        protected static readonly Color amberColor = new Color(1.0f, 0.47f, 0.012f);

        private Image image;
        private UText text;

        public DmxTrackElement fixture;

        private void Awake()
        {
            image = GetComponent<Image>();
            text = GetComponentInChildren<UText>();
        }

        private int flickerCount = 0;
        private void Update()
        {
            int intensity = GetIntensity();
            int strobe = GetStrobe();

            Color32 color = GetColor();

            Color32 bgColor = SumColors(
                ColorMix(intensity, color),
                Color.black
            );

            if (strobe > 0)
            {
                if (((flickerCount++ / FlickerFrames(strobe)) % 2) == 1)
                    bgColor = Color.black;
            }
            else
            {
                flickerCount = 0;
            }
            image.color = bgColor;

            string textContent = string.Join(Environment.NewLine, new[]
            {
                name,
                $"{color.r}, {color.g}, {color.b}",
                $"dim: {intensity} / str: {strobe}",
            });

            text.text = textContent;

            bool isBackgroundDark = (bgColor.r + bgColor.g + bgColor.b < 3 * 255 / 2);
            text.color = isBackgroundDark ? Color.white : Color.black;
        }

        protected abstract Color32 GetColor();
        protected abstract int GetIntensity();
        protected abstract int GetStrobe();

        private static int FlickerFrames(int strobe)
        {
            if (strobe < 50)
                return 4;
            else if (strobe < 100)
                return 3;
            else if (strobe < 150)
                return 2;
            else
                return 1;
        }

        protected static Color ColorMix(int dimmer, Color color) => dimmer / 255.0f * color;
        protected static Color SumColors(params Color[] colors) => colors.Aggregate((acc, curr) => acc + curr);
    }
}