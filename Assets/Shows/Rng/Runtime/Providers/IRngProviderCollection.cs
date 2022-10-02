namespace Plml.Rng
{
    public interface IRngProviderCollection { }

    public interface IRngProviderCollection<TProvider> : IRngProviderCollection where TProvider : RngProviderBase
    {
        TProvider[] GetAllProviders();
        TProvider[] GetActiveProviders();
    }
}