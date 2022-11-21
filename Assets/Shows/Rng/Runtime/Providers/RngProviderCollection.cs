using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public abstract class RngProviderCollection<TProvided> : 
        RngProvider<TProvided>,
        IRngProviderCollection<RngProvider<TProvided>>
    {
        private IEnumerable<RngProvider<TProvided>> GetActiveProviders_Internal() => GetAllProviders()
            .Where(p => p.active);

        public RngProvider<TProvided>[] GetAllProviders() => this.GetComponentsInDirectChildren<RngProvider<TProvided>>();

        public RngProvider<TProvided>[] GetActiveProviders() => GetActiveProviders_Internal()
            .ToArray();

        public bool IsRoot() => !this.HasComponentInParents(GetType());

        private RngProvider<TProvided> lastProvider;

        private RngProvider<TProvided> GetNextProvider()
        {
            IEnumerable<RngProvider<TProvided>> providersEnum = GetActiveProviders_Internal();

            if (lastProvider != null && !lastProvider.canChain)
            {
                providersEnum = providersEnum.Except(lastProvider);
            }

            RngProvider<TProvided>[] providers = providersEnum.ToArray();

            float totalWeight = providers.Sum(p => p.weight);

            float random = URandom.Range(0.0f, totalWeight);

            float addr = random;
            RngProvider<TProvided> nextProvider = providers.First(p =>
            {
                addr -= p.weight;
                return addr < 0.0f;
            });

            lastProvider = nextProvider;

            return nextProvider;
        }

        public override TProvided GetNextElement() => GetNextProvider().GetNextElement();
    }

    public abstract class RngProviderCollection<TProvided, TParam> :
        RngProvider<TProvided, TParam>,
        IRngProviderCollection<RngProvider<TProvided, TParam>>
    {
        private IEnumerable<RngProvider<TProvided, TParam>> GetActiveProviders_Internal() => GetAllProviders()
            .Where(p => p.active);

        public RngProvider<TProvided, TParam>[] GetAllProviders() => this.GetComponentsInDirectChildren<RngProvider<TProvided, TParam>>();

        public RngProvider<TProvided, TParam>[] GetActiveProviders() => GetActiveProviders_Internal()
            .ToArray();

        public bool IsRoot() => this.HasComponentInParents(GetType());

        private RngProvider<TProvided, TParam> lastProvider;

        private RngProvider<TProvided, TParam> GetNextProvider()
        {
            IEnumerable<RngProvider<TProvided, TParam>> providersEnum = GetActiveProviders_Internal();

            if (lastProvider != null && !lastProvider.canChain)
            {
                providersEnum = providersEnum.Except(lastProvider);
            }

            RngProvider<TProvided, TParam>[] providers = providersEnum.ToArray();

            float totalWeight = providers.Sum(p => p.weight);

            float random = URandom.Range(0.0f, totalWeight);

            float addr = random;
            RngProvider<TProvided, TParam> nextProvider = providers.First(p =>
            {
                addr -= p.weight;
                return addr < 0.0f;
            });

            lastProvider = nextProvider;

            return nextProvider;
        }

        public override TProvided GetNextElement(TParam param) => GetNextProvider().GetNextElement(param);
    }

    public abstract class RngProviderCollection<TProvided, T1, T2> :
        RngProvider<TProvided, T1, T2>,
        IRngProviderCollection<RngProvider<TProvided, T1, T2>>
    {
        private IEnumerable<RngProvider<TProvided, T1, T2>> GetActiveProviders_Internal() => GetAllProviders()
            .Where(p => p.active);

        public RngProvider<TProvided, T1, T2>[] GetAllProviders() => this.GetComponentsInDirectChildren<RngProvider<TProvided, T1, T2>>();

        public RngProvider<TProvided, T1, T2>[] GetActiveProviders() => GetActiveProviders_Internal()
            .ToArray();

        public bool IsRoot() => this.HasComponentInParents(GetType());

        private RngProvider<TProvided, T1, T2> lastProvider;

        private RngProvider<TProvided, T1, T2> GetNextProvider()
        {
            IEnumerable<RngProvider<TProvided, T1, T2>> providersEnum = GetActiveProviders_Internal();

            if (lastProvider != null && !lastProvider.canChain)
            {
                providersEnum = providersEnum.Except(lastProvider);
            }

            RngProvider<TProvided, T1, T2>[] providers = providersEnum.ToArray();

            float totalWeight = providers.Sum(p => p.weight);

            float random = URandom.Range(0.0f, totalWeight);

            float addr = random;
            RngProvider<TProvided, T1, T2> nextProvider = providers.First(p =>
            {
                addr -= p.weight;
                return addr < 0.0f;
            });

            lastProvider = nextProvider;

            return nextProvider;
        }

        public override TProvided GetNextElement(T1 t1, T2 t2) => GetNextProvider().GetNextElement(t1, t2);
    }
}