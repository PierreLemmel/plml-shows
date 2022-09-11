using System;
using UnityEngine;

namespace Plml.EnChiens
{
    public abstract class EnChiensAdapter : MonoBehaviour
    {
        [ReadOnly]
        public EnChiensSpectacle.Animation currentAnimation;

        protected T AnimationMap<T>(T onIntro, T onConclusion) => currentAnimation == EnChiensSpectacle.Animation.Introduction ?
            onIntro : onConclusion;

        public virtual void ResetLights() { }
        public virtual void CommitValues() { }

        public virtual void SetupContres(Color globalColor, Color contresColor, int contres, int contre1, int contre2, int contre3, int contre4) { }
        public virtual void SetupServo(int dimmer, int pan, int tilt) { }
        public virtual void SetupJardinCour(Color color, int jardinCour, int courJardin) { }
        public virtual void SetupOthers(int others) { }

        public virtual void Chase(int strobe) { }
        public virtual void Flicker(int amplitude, int strobe) { }
        public virtual void PlayPiano(int strobe) { }

        public virtual void UpdatePulsations(Color color, float pulsationMinValue, float pulsationMaxValue) { }
        public virtual void StopPulsations() { }

        public virtual void SetupFond(int value) { }
        public virtual void SetupEnd(int value) { }
    }
}