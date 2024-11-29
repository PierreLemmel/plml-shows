using System;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Dmx.Animations
{
    public class DmxSequencer : MonoBehaviour
    {
        [RangeBounds(0.1f, 60f)]
        public FloatRange stepDurationRange = new(2f, 5f);

        [Range(0f, 20f)]
        public float transitionTime;

        public SequenceStepData[] steps;

        private float t;
        private float currentDuration;
        private SequenceStepData currentStep;
        private SequenceStepData previousStep;
        private int stepIndex = -1;

        public SequencerMode mode;

        private void GoToNextStep()
        {
            if (previousStep != null)
                ApplyStep(previousStep, 0f);

            previousStep = currentStep;

            int offset = mode == SequencerMode.Sequential ?
                1 : URandom.Range(1, steps.Length - 1);

            stepIndex = (stepIndex + offset) % steps.Length;
            currentStep = steps[stepIndex];

            currentDuration = MoreRandom.Range(stepDurationRange) * currentStep.durationFactor;
        }

        private void Awake()
        {
            foreach (var step in steps)
                ApplyStep(step, 0f);

            GoToNextStep();
        }

        private void Update()
        {
            if (t > currentDuration)
            {
                t -= currentDuration;
                GoToNextStep();
            }
            
            if (previousStep != null)
            {
                if (t <= transitionTime)
                {
                    float aPrev = (transitionTime - t) / transitionTime;
                    ApplyStep(previousStep, aPrev);
                }
                else
                {
                    ApplyStep(previousStep, 0);
                    previousStep = null;
                }
            }

            float aCurr = Mathf.Min(t / transitionTime, 1f);
            ApplyStep(currentStep, aCurr);

            t += Time.deltaTime;
        }

        private void ApplyStep(SequenceStepData step, float a)
        {
            foreach (var elt in step.elements)
            {
                var chan = elt.channel;
                int val = Mathf.RoundToInt(a * elt.value);
                foreach(var fixture in elt.fixtures)
                {
                    fixture.SetChannel(chan, val);
                }

                Color24 col = a * elt.color;
                foreach(var fixture in elt.colorFixtures)
                {
                    fixture.SetColor(col);
                }
            }
        }

        [Serializable]
        public class SequenceStepData
        {
            public string name;
            public SequenceStepDataElements[] elements;
            
            [Range(0.1f, 10f)]
            public float durationFactor = 1f;
        }

        [Serializable]
        public class SequenceStepDataElements
        {
            public DmxChannelType channel;

            [Range(0, 0xff)]
            public int value;

            public DmxTrackElement[] fixtures;

            public Color24 color;

            public DmxTrackElement[] colorFixtures;
        }

        public enum SequencerMode
        {
            Sequential,
            Random,
        }
    }
}