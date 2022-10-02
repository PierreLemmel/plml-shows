

using System.Collections.Generic;
using System.Linq;

namespace Plml.Rng
{
    public static class RngProviderCollections
    {
        public static bool IsRoot<TCollection>(this TCollection collection)
            where TCollection : RngProviderBase, IRngProviderCollection => collection.HasComponentInParents(collection.GetType());

        public static TProvider[] GetProvidersInHierarchy<TProvider>(this IRngProviderCollection<TProvider> collection)
            where TProvider : RngProviderBase => collection.GetAllProviders()
                .SelectMany(provider => provider is IRngProviderCollection<TProvider> subcollection ?
                    GetProvidersInHierarchy(subcollection) :
                    Enumerables.Create(provider)
                )
                .ToArray();
    }
}