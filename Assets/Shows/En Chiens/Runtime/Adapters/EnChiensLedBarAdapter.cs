using Plml.Dmx;
using System;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.EnChiens
{
    public class EnChiensLedBarAdapter : EnChiensAdapter
    {
        [HideInPlayMode]
        public DmxTrackElement ledBar1;

        [HideInPlayMode]
        public DmxTrackElement ledBar2;

        private Color32[] bar1Colors;
        private Color32[] bar2Colors;

        [Range(0f, 1f)]
        public float flickerAmplitudeFactor = 1f;

        [Range(2, 12)]
        public int flickeringDurationInFrames = 2;

        [Range(2, 12)]
        public int chasingStepDurationInFrames = 9;

        [Range(2, 12)]
        public int chasingStrobeEffectDurationInFrames = 2;

        [RangeBounds(2, 16)]
        public IntRange chasingWidth = new(4, 8);

        [RangeBounds(1, 6)]
        public IntRange chasingNumber = new(2, 4);

        [Range(1, 12)]
        public int pianoDurationInFrames = 6;


        [HideInPlayMode]
        public float bpm = 120f;

        [QuadraticRange(0.1f, 1f)]
        public float pulsationSmoothTime = 0.4f;

        [Range(1, 16)]
        public int pianoWidth = 2;
        
        [Range(1, 16)]
        public int pianoStep = 4;


        private void Awake()
        {
            bar1Colors = ledBar1.GetColors();
            bar2Colors = ledBar2.GetColors();

            float bps = bpm / 60f;
            pulseInterval = 1f / bps;
            nextPulseTimeTarget = Time.time + pulseInterval;

            chasePattern = Arrays.Create(bar1Colors.Length + bar2Colors.Length, false);
        }

        public override void ResetLights()
        {
            Color32 black = new(0, 0, 0, 0);
            bar1Colors.Set(black);
            bar2Colors.Set(black);
        }

        public override void SetupOthers(int others)
        {
            byte b = (byte)others;
            Color32 color = new(b, b, b, b);

            ApplyColorGlobally(color);
        }

        private bool[] chasePattern;
        private int chaseFrame = 100_000;
        private int chaseStrobeFrame;
        private bool chaseStrobeIsOn = true;
        public override void Chase(int strobe)
        {
            if (++chaseFrame >= chasingStepDurationInFrames)
            {
                RegeneratechasePattern();
                chaseFrame = 0;
            }

            if (++chaseStrobeFrame >= chasingStrobeEffectDurationInFrames)
            {
                chaseStrobeIsOn = !chaseStrobeIsOn;
                chaseStrobeFrame = 0;
            }

            if (chaseStrobeIsOn)
            {
                Color32 color = new(0xff, 0xff, 0xff, 0xff);
                int l1 = bar1Colors.Length;
                for (int i = 0; i < l1; i++)
                {
                    if (chasePattern[i])
                        bar1Colors[i] = color;
                }

                int l2 = bar2Colors.Length;
                for (int i = 0; i < l2; i++)
                {
                    if (chasePattern[i + l1])
                        bar2Colors[i] = color;
                }
            }
        }

        public override void Flicker(int amplitude, int strobe)
        {
            if (IsFlickering())
            {
                Color32 flickerColor = flickerAmplitudeFactor * Color.white;
                ApplyColorGlobally(flickerColor);
            }
        }

        private float pulseTarget01 = 0f;
        private float pulseValue01 = 0f;
        private float pulseVelocity = 0f;

        private float pulseInterval;
        private float nextPulseTimeTarget;
        public override void UpdatePulsations(Color color, float pulsationMinValue, float pulsationMaxValue)
        {
            if (Time.time >= nextPulseTimeTarget)
            {
                pulseTarget01 = pulseTarget01 == 0.0f ? 1.0f : 0.0f;
                nextPulseTimeTarget += pulseInterval;
            }

            pulseValue01 = Mathf.SmoothDamp(pulseValue01, pulseTarget01, ref pulseVelocity, pulsationSmoothTime);

            byte val = (byte) Mathf.RoundToInt(pulsationMinValue + pulseValue01 * (pulsationMaxValue - pulsationMinValue));

            Color32 pulseColor = new(val, val, val, 0xff);
            ApplyColorGlobally(pulseColor);
        }

        public override void PlayPiano(int strobe)
        {
            int l1 = bar1Colors.Length;
            int l2 = bar2Colors.Length;

            int pianoIndex = GetPianoIndex();

            Color32 color = new(0xff, 0xff, 0xff, 0xff);
            
            if (pianoIndex < l1)
            {
                int bound = Math.Min(l1, pianoIndex + pianoWidth);
                for (int i = pianoIndex; i < bound; i++)
                {
                    bar1Colors[i] = color;
                }
            }

            if (pianoIndex + pianoWidth >= l1)
            {
                int lb = Math.Max(0, pianoIndex - l1);
                int ub = pianoIndex + pianoWidth - l1;
                for (int i = lb; i < ub; i++)
                {
                    bar2Colors[i] = color;
                }
            }
        }

        public override void CommitValues()
        {
            ledBar1.SetColors(bar1Colors);
            ledBar2.SetColors(bar2Colors);
        }

        private void ApplyColorToArray(Color32[] array, Color32 color) => array.Set(old => Colors.Max(old, color));
        private void ApplyColorGlobally(Color32 color)
        {
            ApplyColorToArray(bar1Colors, color);
            ApplyColorToArray(bar2Colors, color);
        }

        private bool flickerResult = true;
        private int flickerFrameCount = 0;
        private bool IsFlickering()
        {
            if (++flickerFrameCount >= flickeringDurationInFrames)
            {
                flickerResult = !flickerResult;
                flickerFrameCount = 0;
            }

            return flickerResult;
        }

        private void RegeneratechasePattern()
        {
            chasePattern.Set(false);

            int nb = MoreRandom.Range(chasingNumber);
            int l = chasePattern.Length;

            for (int i = 0; i < nb; i++)
            {
                int w = MoreRandom.Range(chasingWidth);

                int start = URandom.Range(0, l - w);

                for (int k = start; k < start + w; k++)
                    chasePattern[k] = true;
            }
        }

        private bool pianoAscending = true;
        private int pianoFrameCount = 0;
        private int pianoIndex;
        private int GetPianoIndex()
        {
            if (++pianoFrameCount >= pianoDurationInFrames)
            {
                if (pianoAscending)
                {
                    pianoIndex += pianoStep;

                    if (pianoIndex >= bar1Colors.Length + bar2Colors.Length - pianoWidth)
                        pianoAscending = false;
                }
                else
                {
                    pianoIndex -= pianoStep;

                    if (pianoIndex <= 0)
                        pianoAscending = true;
                }
                pianoFrameCount = 0;
            }

            return pianoIndex;
        }
    }
}
