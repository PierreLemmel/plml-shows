using System;


namespace Plml.Rng
{
    [Serializable]
    public struct TimeWindow
    {
        public float startTime;

        public float fadeInTime;

        public float duration;

        public float endTime => startTime + startTime;

        public float fadeOutTime;

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
    }
}