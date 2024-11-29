using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public struct TimeWindow
    {
        public float startTime;

        public float fadeInTime;

        public float duration;

        public float endTime => startTime + duration;

        public float endOfFadeIn => startTime + fadeInTime;
        public float startOfFadeOut => endTime - fadeOutTime;

        public float fadeOutTime;

        public float GetValue(float time)
        {
            if (time > startTime && time < endOfFadeIn)
                return (time - startTime) / fadeInTime;
            else if (time > endOfFadeIn && time < startOfFadeOut)
                return 1.0f;
            else if (time > startOfFadeOut && time < endTime)
                return 1 - (time - startOfFadeOut) / fadeOutTime;
            else
                return 0.0f;
        }

        public bool Contains(float time) => time >= startTime && time < endTime;

        public TimeWindow(float startTime, float duration, float fadeInTime, float fadeOutTime)
        {
            this.startTime = startTime;
            this.fadeInTime = fadeInTime;
            this.duration = duration;
            this.fadeOutTime = fadeOutTime;
        }

        public static TimeWindow operator *(float a, TimeWindow tw) => new(a * tw.startTime, a * tw.duration, a * tw.fadeInTime, a * tw.fadeOutTime);
        public static TimeWindow operator *(TimeWindow tw, float a) => a * tw;
        public static TimeWindow operator /(TimeWindow tw, float a) => (1.0f / a) * tw;

        public static TimeWindow Empty => new();


        public TimeWindow Translate(float time) => new(startTime + time, duration, fadeInTime, fadeOutTime);

        public TimeWindow ShiftLeft(float time) => new(startTime + time, duration - time, fadeInTime, fadeOutTime);
        public TimeWindow ShiftRight(float time) => new(startTime, duration - time, fadeInTime, fadeOutTime);
    }
}