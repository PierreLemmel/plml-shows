using System;
using UnityEngine;

namespace Plml.Rng
{
    public abstract class RngProviderBase : MonoBehaviour
    {
        public const float MinWeight = 1.0f;
        public const float MaxWeight = 100.0f;

        public bool active = true;

        [CubicRange(MinWeight, MaxWeight)]
        public float weight = 10.0f;

        public bool canChain = true;
    }

    public abstract class RngProvider<TProvided> : RngProviderBase, IRngProvider<TProvided>
    {
        public abstract TProvided GetNextElement();
    }

    public abstract class RngProvider<TProvided, TParam> : RngProviderBase, IRngProvider<TProvided, TParam>
    {
        public abstract TProvided GetNextElement(TParam param);
    }

    public abstract class RngProvider<TProvided, T1, T2> : RngProviderBase, IRngProvider<TProvided, T1, T2>
    {
        public abstract TProvided GetNextElement(T1 p1, T2 p2);
    }
}