using UnityEngine;

namespace Plml.Rng.Audio
{
    public class AudioClipProvider : AudioProvider
    {
        [Range(0.0f, 1.0f)]
        public float minVolume = 0.0f;

        [Range(0.0f, 1.0f)]
        public float maxVolume = 1.0f;

        [Range(0.0f, 10.0f)]
        public float minFadeTime = 1.0f;

        [Range(0.0f, 10.0f)]
        public float maxFadeTime = 10.0f;

        [Min(0.0f)]
        public float minDuration = 10.0f;

        [Min(0.0f)]
        public float maxDuration = 120.0f;

        public AudioClip[] clips;

        public override RngAudioData GetElement(float startTime, float sceneDuration)
        {
            AudioClip clip = clips.RandomElement();
            float clipDuration = Random.Range(minDuration, Mathf.Min(clip.length, maxDuration));

            return new()
            {
                audioClip = clip,
                audioVolume = Random.Range(minVolume, maxVolume),
                musicWindow = new(
                    startTime + Random.Range(0.0f, sceneDuration - clipDuration),
                    clipDuration,
                    Random.Range(minFadeTime, maxFadeTime),
                    Random.Range(minFadeTime, maxFadeTime)
                )
            };
        }
    }
}