using Plml.Dmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Plml.EnChiens.Animations.TheatreDuTemps
{
    public class TheatreDuTempsContresPulsation : MonoBehaviour
    {
        [HideInPlayMode]
        public DmxTrackElement[] beamColors;

        public float minValue;
        public float maxValue;
        public float bpm;
        public float smoothTime;

        private void Update()
        {
            int pulseValue = CalculatePulseValue();
            foreach (var bar in beamColors)
            {
                bar.dimmer = pulseValue;
            }
        }

        private float target01 = 0.0f;
        private float value01 = 0.0f;
        private float velocity = 0.0f;

        private float nextTimeTarget = 0.0f;
        private float timeInterval;

        private void OnEnable()
        {
            float bps = bpm / 60.0f;
            timeInterval = 1.0f / bps;

            nextTimeTarget = Time.time + timeInterval;
            target01 = 0.0f;
        }

        private int CalculatePulseValue()
        {
            if (Time.time >= nextTimeTarget)
            {
                target01 = target01 == 0.0f ? 1.0f : 0.0f;
                nextTimeTarget += timeInterval;
            }

            value01 = Mathf.SmoothDamp(value01, target01, ref velocity, smoothTime);

            return Mathf.RoundToInt(minValue + value01 * (maxValue - minValue));
        }
    }
}
