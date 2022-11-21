using System.Linq;

namespace Plml.Rng
{
    public static class RngProviderCollections
    {
        public static TProvider[] GetProvidersInHierarchy<TProvider>(this IRngProviderCollection<TProvider> collection)
            where TProvider : RngProviderBase => collection.GetAllProviders()
                .SelectMany(provider => provider is IRngProviderCollection<TProvider> subcollection ?
                    GetProvidersInHierarchy(subcollection) :
                    Enumerables.Create(provider)
                )
                .ToArray();
    }
}