using Plml.Dmx;
using Plml.EnChiens.Animations.Improvibar;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.EnChiens
{
    public class EnChiensImprovibarAdapter : EnChiensAdapter
    {
        [HideInPlayMode]
        public DmxTrackElement parLedCourJardin;
        [HideInPlayMode]
        public DmxTrackElement parLedJardinCour;

        [HideInPlayMode]
        public DmxTrackElement parLedContre1;
        [HideInPlayMode]
        public DmxTrackElement parLedContre2;
        [HideInPlayMode]
        public DmxTrackElement parLedContre3;
        [HideInPlayMode]
        public DmxTrackElement parLedContre4;


        [HideInPlayMode]
        public DmxTrackElement parServoCour;

        private DmxTrackElement[] parsAll;
        private DmxTrackElement[] parsContre;


        private ContresPulsation contresPulsation;
        private FaceButtonsActivation faceButtons;


        [HideInPlayMode]
        public float bpm = 60;
        [HideInPlayMode]
        public float pulsationSmoothTime = 0.3f;

        [HideInPlayMode]
        public KeyCode jardinCourKey = KeyCode.Alpha1;

        [HideInPlayMode]
        public KeyCode courJardinKey = KeyCode.Alpha2;

        [Range(0.05f, 0.5f)]
        public float faceButtonsSmoothTime = 0.1f;

        [Range(1, 3)]
        public int chasingMinSpots = 1;
        [Range(1, 4)]
        public int chasingMaxSpots = 3;

        [Range(2, 12)]
        public int flickeringDurationInFrames = 2;

        [Range(2, 12)]
        public int chasingStepDurationInFrames = 9;

        [Range(2, 12)]
        public int pianoDurationInFrames = 6;

        private void Awake()
        {
            parsAll = new[]
            {
                parLedCourJardin,
                parLedJardinCour,
                parLedContre1,
                parLedContre2,
                parLedContre3,
                parLedContre4,
            };

            parsContre = new[]
            {
                parLedContre1,
                parLedContre2,
                parLedContre3,
                parLedContre4,
            };

            contresPulsation = new GameObject("Contres Pulsation")
                .AddComponent<ContresPulsation>();

            contresPulsation.transform.parent = transform;
            contresPulsation.parsContre = parsContre;
            contresPulsation.smoothTime = pulsationSmoothTime;
            contresPulsation.bpm = bpm;


            faceButtons = new GameObject("Face Buttons")
                .AddComponent<FaceButtonsActivation>();

            faceButtons.transform.parent = transform;
            faceButtons.jardinCourKey = jardinCourKey;
            faceButtons.courJardinKey = courJardinKey;

            faceButtons.smoothTime = faceButtonsSmoothTime;

            sequenceEnumerator = ChasingSequence.GetEnumerator();
        }

        public override void ResetLights()
        {
            foreach (var par in parsAll)
            {
                par.stroboscope = 0x00;
            }
        }

        public override void SetupServo(int dimmer, int pan, int tilt)
        {
            parServoCour.cold = dimmer;
            parServoCour.pan = pan;
            parServoCour.tilt = tilt;
        }

        public override void SetupJardinCour(Color color, int jardinCour, int courJardin)
        {
            parLedCourJardin.color = color;
            parLedJardinCour.color = color;

            parLedJardinCour.dimmer = Mathf.Max(jardinCour, faceButtons.jardinCour);
            parLedCourJardin.dimmer = Mathf.Max(courJardin, faceButtons.courJardin);
        }

        public override void SetupContres(Color globalColor, Color contresColor, int contres, int contre1, int contre2, int contre3, int contre4)
        {
            Color maxContreColor = Colors.Max(globalColor, contresColor);
            foreach (var contre in parsContre)
            {
                contre.color = maxContreColor;
            }

            parLedContre1.dimmer = Mathf.Max(contre1, contres);
            parLedContre2.dimmer = Mathf.Max(contre2, contres);
            parLedContre3.dimmer = Mathf.Max(contre3, contres);
            parLedContre4.dimmer = Mathf.Max(contre4, contres);
        }

        public override void UpdatePulsations(Color color, float pulsationMinValue, float pulsationMaxValue)
        {
            contresPulsation.enabled = true;
            contresPulsation.color = color;
            contresPulsation.minValue = pulsationMinValue;
            contresPulsation.maxValue = pulsationMaxValue;
        }

        public override void StopPulsations() => contresPulsation.enabled = false;

        public override void Chase(int strobe)
        {
            foreach (var par in GetChasingSpots())
            {
                par.dimmer = 0xff;
                par.stroboscope = strobe;
            }
        }

        public override void Flicker(int amplitude, int strobe)
        {
            if (IsFlickering())
            {
                foreach (var par in parsContre)
                {
                    par.dimmer = amplitude;
                    par.stroboscope = strobe;
                }
            }
        }

        public override void PlayPiano(int strobe)
        {
            int pianoIndex = GetPianoIndex();
            var contre = parsContre[pianoIndex];

            contre.dimmer = 0xff;
            contre.stroboscope = strobe;
        }

        private IEnumerable<DmxTrackElement> GetChasingSpots()
        {
            if (++chaseFrame >= chasingStepDurationInFrames)
            {
                sequenceEnumerator.MoveNext();
                chaseFrame = 0;
            }

            return sequenceEnumerator.Current;
        }

        private int chaseFrame = 1_000_000;
        private IEnumerator<IEnumerable<DmxTrackElement>> sequenceEnumerator;
        private IEnumerable<IEnumerable<DmxTrackElement>> ChasingSequence
        {
            get
            {
                IEnumerable<int> lastResult = Enumerable.Empty<int>();
                while (true)
                {
                    int count = URandom.Range(chasingMinSpots, chasingMaxSpots);

                    IEnumerable<int> indices = GetRandomSequence(parsAll.Length, count, lastResult);
                    lastResult = indices;

                    IEnumerable<DmxTrackElement> result = indices.Select(idx => parsAll[idx]);
                    yield return result;
                }
            }
        }

        private static IEnumerable<int> GetRandomSequence(int max, int count, IEnumerable<int> exclude)
        {
            IList<int> available = Enumerable.Range(0, max).Except(exclude).ToList();

            if (count >= available.Count)
                return available;
            else
            {
                ICollection<int> result = new List<int>(count);
                for (int i = 0; i < count; i++)
                {
                    int avIndex = URandom.Range(0, available.Count);
                    int parIndex = available[avIndex];
                    available.RemoveAt(avIndex);
                    result.Add(parIndex);
                }
                return result;
            }
        }

        private bool flickerResult = true;
        private int flickerFrameCount = 0;
        private bool IsFlickering()
        {
            if (++flickerFrameCount >= flickeringDurationInFrames)
            {
                flickerResult = !flickerResult;
                flickerFrameCount = 0;
            }

            return flickerResult;
        }


        private bool pianoAscending = true;
        private int pianoFrameCount = 0;
        private int pianoIndex;
        private int GetPianoIndex()
        {
            if (++pianoFrameCount >= pianoDurationInFrames)
            {
                if (pianoAscending)
                {
                    pianoIndex++;

                    if (pianoIndex >= parsContre.Length - 1)
                        pianoAscending = false;
                }
                else
                {
                    pianoIndex--;

                    if (pianoIndex <= 0)
                        pianoAscending = true;
                }
                pianoFrameCount = 0;
            }

            return pianoIndex;
        }

        
    }
}