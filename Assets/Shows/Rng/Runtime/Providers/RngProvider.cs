using System;
using UnityEngine;

namespace Plml.Rng
{
    public abstract class RngProviderBase : MonoBehaviour
    {
        public const float MinWeight = 1.0f;
        public const float MaxWeight = 100.0f;

        [CubicRange(MinWeight, MaxWeight)]
        public float weight = 10.0f;

        public bool canChain = true;
    }

    public abstract class RngProvider<TProvided> : RngProviderBase
    {
        public abstract TProvided GetElement();
    }

    public abstract class RngProvider<TProvided, TParam> : RngProviderBase
    {
        public abstract TProvided GetElement(TParam param);
    }

    public abstract class RngProvider<TProvided, T1, T2> : RngProviderBase
    {
        public abstract TProvided GetElement(T1 p1, T2 p2);
    }
}