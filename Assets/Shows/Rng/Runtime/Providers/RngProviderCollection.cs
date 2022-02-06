using System;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public abstract class RngProviderCollectionBase<TProvider> : MonoBehaviour
        where TProvider : RngProviderBase
    {
        public TProvider[] GetProviders() => GetComponentsInChildren<TProvider>();

        private TProvider lastProvider;

        protected TProvider GetNextProvider()
        {
            TProvider[] providers = GetProviders();

            if (lastProvider != null && !lastProvider.canChain)
            {
                providers = providers.Except(lastProvider).ToArray();
            }
            
            float totalWeight = providers.Sum(p => p.weight);

            float random = URandom.Range(0.0f, totalWeight);

            float addr = random;
            TProvider nextProvider = providers.First(p =>
            {
                addr -= p.weight;
                return addr < 0.0f;
            });

            lastProvider = nextProvider;

            return nextProvider;
        }
    }

    public abstract class RngProviderCollection<TProvider, TProvided> : RngProviderCollectionBase<TProvider>
        where TProvider : RngProvider<TProvided>
    {
        public TProvided GetNextElement() => GetNextProvider().GetElement();
    }

    public abstract class RngProviderCollection<TProvider, TProvided, TParam> : RngProviderCollectionBase<TProvider>
        where TProvider : RngProvider<TProvided, TParam>
    {
        public TProvided GetNextElement(TParam param) => GetNextProvider().GetElement(param);
    }
}