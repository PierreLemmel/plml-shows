using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.Dmx.Animations
{
    public class DmxChanPulser : MonoBehaviour
    {
        public DmxTrackElement[] fixtures;
        public DmxChannelType channel;
        
        [DmxRange]
        public IntRange range;

        [Min(0f)]
        public float period = 1f;

        [Min(0f)]
        public float smoothTime = 0.3f;

        private bool goUp;
        private float target;
        private float current;
        private float velocity;

        private void Awake()
        {
            target = range.max;
            current = range.min;
            goUp = true;
            velocity = 0f;
        }

        private float t = 0f;
        private void Update()
        {
            t += Time.deltaTime;

            if (t > period)
            {
                t %= period;
                goUp = !goUp;
                target = goUp ? range.max : range.min;
            }

            current = Mathf.SmoothDamp(current, target, ref velocity, smoothTime);
            int val = Mathf.RoundToInt(current);

            foreach (var fixture in fixtures)
                fixture.SetChannel(channel, val);
        }
    }
}