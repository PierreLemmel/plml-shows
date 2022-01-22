using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Plml.Dmx.Fixtures;

namespace Plml.EnChiens.Animation
{
    public class ContresPulsation : MonoBehaviour
    {
        [HideInPlayMode]
        public ParLedRGBW[] parsContre;

        public Color color;

        public float minValue;
        public float maxValue;
        public int bpm;
        public float smoothTime;

        private void Update()
        {
            int pulseValue = CalculatePulseValue();
            foreach (ParLedRGBW par in parsContre)
            {
                par.dimmer = pulseValue;
                par.color = color;
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

            return (int)(minValue + value01 * (maxValue - minValue));
        }
    }
}
