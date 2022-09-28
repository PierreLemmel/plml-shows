using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public abstract class RngProviderCollectionBase<TProvider> : 
        RngProviderBase, IRngProvider
        where TProvider : RngProviderBase, IRngProvider
    {
        private IEnumerable<TProvider> GetActiveProviders_Internal() => GetAllProviders()
            .Where(p => p.active);

        public TProvider[] GetAllProviders() => this.GetComponentsInDirectChildren<TProvider>();

        public TProvider[] GetActiveProviders() => GetActiveProviders_Internal()
            .ToArray();

        public bool IsRoot() => this.HasComponentInParents(GetType());

        private TProvider lastProvider;

        protected TProvider GetNextProvider()
        {
            IEnumerable<TProvider> providersEnum = GetActiveProviders_Internal();

            if (lastProvider != null && !lastProvider.canChain)
            {
                providersEnum = providersEnum.Except(lastProvider);
            }
            
            TProvider[] providers = providersEnum.ToArray();

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

    public abstract class RngProviderCollection<TProvider, TProvided> :
        RngProviderCollectionBase<TProvider>, IRngProvider<TProvided>
        where TProvider : RngProviderBase, IRngProvider<TProvided>
    {
        public TProvided GetNextElement() => GetNextProvider().GetNextElement();
    }

    public abstract class RngProviderCollection<TProvider, TProvided, TParam> :
        RngProviderCollectionBase<TProvider>, IRngProvider<TProvided, TParam>
        where TProvider : RngProviderBase, IRngProvider<TProvided, TParam>
    {
        public TProvided GetNextElement(TParam param) => GetNextProvider().GetNextElement(param);
    }

    public abstract class RngProviderCollection<TProvider, TProvided, T1, T2> :
        RngProviderCollectionBase<TProvider>, IRngProvider<TProvided, T1, T2>
        where TProvider : RngProviderBase, IRngProvider<TProvided, T1, T2>
    {
        public TProvided GetNextElement(T1 p1, T2 p2) => GetNextProvider().GetNextElement(p1, p2);
    }
}