namespace Plml.EnChiens
{
    public static class Bpm
    {
        public static float ToFrameCount(int bpm)
        {
            float bps = bpm / 60.0f;
            return 60 / bps;
        }
    }
}