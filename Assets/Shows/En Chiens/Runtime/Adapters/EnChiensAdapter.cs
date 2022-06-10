using System;
using UnityEngine;

namespace Plml.EnChiens
{
    public abstract class EnChiensAdapter : MonoBehaviour
    {
        public virtual void StartPulsations() { }
        public virtual void StopPulsations() { }
    }
}