namespace Plml.Rng.Audio
{
    public class NoAudioProvider : AudioProvider
    {
        public override RngAudioData GetNextElement(float startTime, float sceneDuration) => null;
    }
}