namespace Plml.Rng.Audio
{
    public class NoAudioProvider : AudioProvider
    {
        public override RngAudioData GetElement(float startTime, float sceneDuration) => null;
    }
}