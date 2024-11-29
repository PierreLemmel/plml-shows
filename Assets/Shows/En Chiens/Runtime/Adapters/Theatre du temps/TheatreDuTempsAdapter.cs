using Plml.Dmx;
using Plml.EnChiens.Animations;
using Plml.EnChiens.Animations.Improviste;
using Plml.EnChiens.Animations.TheatreDuTemps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.EnChiens.TheatreDuTemps
{
    public class TheatreDuTempsAdapter : EnChiensAdapter
    {
        [HideInPlayMode]
        public float bpm = 60;

        #region Fixtures
        [HideInPlayMode]
        public DmxTrackElement doucheCentrale;

        [HideInPlayMode]
        public DmxTrackElement rattrapageDoucheCentrale;

        [HideInPlayMode]
        public DmxTrackElement latTradJardin;

        [HideInPlayMode]
        public DmxTrackElement latTradCour;

        [HideInPlayMode]
        public DmxTrackElement[] facesFroides;

        [HideInPlayMode]
        public DmxTrackElement latLedJar1;

        [HideInPlayMode]
        public DmxTrackElement latLedJar2;

        [HideInPlayMode]
        public DmxTrackElement latLedCour1;

        [HideInPlayMode]
        public DmxTrackElement latLedCour2;

        [HideInPlayMode]
        public DmxTrackElement contreBeamcolor1;

        [HideInPlayMode]
        public DmxTrackElement contreBeamcolor2;

        [HideInPlayMode]
        public DmxTrackElement contreBeamcolor3;

        [DmxRange]
        public IntRange adjStrobeRange = new(0, 255);

        [DmxRange]
        public IntRange beamColorStrobeRange = new(0, 255);
        #endregion

        #region Effects
        [Range(0, 3)]
        public int chasingMinSpots = 1;
        [Range(1, 4)]
        public int chasingMaxSpots = 3;

        [Range(2, 12)]
        public int flickeringDurationInFrames = 2;

        [Range(2, 12)]
        public int chasingStepDurationInFrames = 9;

        [Range(2, 12)]
        public int pianoDurationInFrames = 6;

        private TheatreDuTempsContresPulsation contresPulsation;

        [Range(0f, 2f)]
        public float pulsationSmoothTime = 0.3f;
        #endregion

        #region Levels
        [Range(0, 0xff)]
        public int doucheCentraleLvl = 0xff;

        [Range(0, 0xff)]
        public int rattrapageDoucheCentraleLvl = 80;

        [Range(0, 0xff)]
        public int latTradLevels = 200;

        [Range(0, 0xff)]
        public int facesFroidesLvl = 170;
        #endregion

        #region Groups
        private LedElement[] allLedSpots;
        private DmxTrackElement[] adjLedSpots;
        private DmxTrackElement[] beamColors;
        private LedElement[] pianoSpots;

        private DmxTrackElement[] allTradSpots;
        #endregion

        private void Awake()
        {
            #region Groups
            adjLedSpots = new[] { latLedJar1, latLedJar2, latLedCour1, latLedCour2 };
            beamColors = new[] { contreBeamcolor1, contreBeamcolor2, contreBeamcolor3 };

            LedElement[] adjLedElts = adjLedSpots
                .Select(elt => new LedElement(elt, adjStrobeRange));
            LedElement[] beamColorLedElts = beamColors
                .Select(elt => new LedElement(elt, beamColorStrobeRange));

            pianoSpots = new[]
            {
                adjLedElts[0], adjLedElts[1],
                beamColorLedElts[0], beamColorLedElts[1], beamColorLedElts[2],
                adjLedElts[3], adjLedElts[2],
            };

            

            allLedSpots = Arrays.Merge(
                adjLedSpots.Select(elt => new LedElement(elt, adjStrobeRange)),
                beamColors.Select(elt => new LedElement(elt, beamColorStrobeRange))
            );

            allTradSpots = Arrays.Merge(
                facesFroides,
                new[]
                {
                    doucheCentrale,
                    rattrapageDoucheCentrale,
                    latTradJardin,
                    latTradCour
                }
            );
            #endregion

            #region Effects
            contresPulsation = new GameObject("Contres Pulsation")
                    .AddComponent<TheatreDuTempsContresPulsation>();

            contresPulsation.transform.parent = transform;
            contresPulsation.beamColors = beamColors;
            contresPulsation.smoothTime = pulsationSmoothTime;
            contresPulsation.bpm = bpm;
            #endregion

            sequenceEnumerator = ChasingSequence.GetEnumerator();
        }


        #region Overrides
        public override void ResetLights()
        {
            foreach (var ledElt in allLedSpots)
            {
                var ledSpot = ledElt.element;

                ledSpot.dimmer = 0x00;
                ledSpot.stroboscope = 0x00;
            }

            foreach (var tradSpot in allTradSpots)
            {
                tradSpot.value = 0x00;
            }
        }

        #region Setups
        public override void SetupOthers(int others)
        {
            int corrected = CorrectDmxValue(others, facesFroidesLvl);
            foreach (var face in facesFroides)
            {
                face.SetValueIfGreater(corrected);
            }

            foreach (var ledElt in allLedSpots)
            {
                var led = ledElt.element;

                led.white = 0xff;
                led.SetDimmerIfGreater(others);
            }
        }

        public override void SetupServo(int dimmer, int pan, int tilt)
        {
            int centraleCorrected = (doucheCentraleLvl * dimmer) / 255;
            int rattrapageCorrected = (rattrapageDoucheCentraleLvl * dimmer) / 255;

            doucheCentrale.SetValueIfGreater(centraleCorrected);
            rattrapageDoucheCentrale.SetValueIfGreater(rattrapageCorrected);
        }


        public override void SetupJardinCour(Color color, int jardinCour, int courJardin)
        {
            if (currentAnimation != EnChiensSpectacle.Animation.Introduction)
                return;

            int correctedJC = CorrectDmxValue(jardinCour, latTradLevels);
            int correctedCJ = CorrectDmxValue(courJardin, latTradLevels);

            latTradJardin.SetValueIfGreater(correctedJC);
            latTradCour.SetValueIfGreater(correctedCJ);
        }

        public override void SetupFond(int value)
        {
            foreach (var fond in beamColors)
            {
                fond.SetDimmerIfGreater(value);
            }
        }

        public override void SetupEnd(int value)
        {
            int correctedVal = CorrectDmxValue(value, latTradLevels);

            latTradJardin.SetValueIfGreater(correctedVal);
            latTradCour.SetValueIfGreater(correctedVal);
        }
        #endregion

        #region Effects
        public override void Chase(int strobe)
        {
            foreach (var ledElement in GetChasingSpots())
            {
                (DmxTrackElement proj, IntRange strobeRange) = ledElement;

                int correctedStrobe = CorrectStrobeValue(strobe, strobeRange);

                proj.dimmer = 0xff;
                proj.stroboscope = correctedStrobe;
            }
        }

        public override void Flicker(int amplitude, int strobe)
        {
            if (IsFlickering())
            {
                int flickerStrobe = CorrectStrobeValue(strobe, beamColorStrobeRange);

                foreach (var contre in beamColors)
                {
                    contre.SetDimmerIfGreater(amplitude);
                    contre.stroboscope = flickerStrobe;
                }
            }
        }

        private bool strobeOnPiano = false;
        public override void PlayPiano(int strobe)
        {
            int pianoIndex = GetPianoIndex();
            
            (DmxTrackElement currentSpot, IntRange strobeRange) = pianoSpots[pianoIndex];
            

            currentSpot.dimmer = 0xff;
            currentSpot.white = 0xff;

            if (strobeOnPiano)
            {
                int pianoStrobe = CorrectStrobeValue(strobe, strobeRange);
                currentSpot.stroboscope = pianoStrobe;
            }
        }

        public override void UpdatePulsations(Color color, float pulsationMinValue, float pulsationMaxValue)
        {
            contresPulsation.enabled = true;
            contresPulsation.minValue = pulsationMinValue;
            contresPulsation.maxValue = pulsationMaxValue;
        }

        public override void StopPulsations() => contresPulsation.enabled = false;
        #endregion
        #endregion


        #region Private utils
        private int CorrectStrobeValue(int strobe, IntRange strobeRange) => strobeRange.min + strobeRange.range * strobe / 255;

        private IEnumerable<LedElement> GetChasingSpots()
        {
            if (++chaseFrame >= chasingStepDurationInFrames)
            {
                sequenceEnumerator.MoveNext();
                chaseFrame = 0;
            }

            return sequenceEnumerator.Current;
        }

        private int chaseFrame = 1_000_000;
        private IEnumerator<IEnumerable<LedElement>> sequenceEnumerator;
        private IEnumerable<IEnumerable<LedElement>> ChasingSequence
        {
            get
            {
                IEnumerable<int> lastResult = Enumerable.Empty<int>();
                while (true)
                {
                    int count = URandom.Range(chasingMinSpots, chasingMaxSpots);

                    IEnumerable<int> indices = GetRandomSequence(allLedSpots.Length, count, lastResult);
                    lastResult = indices;

                    var result = indices.Select(idx => allLedSpots[idx]);
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

                    if (pianoIndex >= pianoSpots.Length - 1)
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

        private int CorrectDmxValue(int value, int max) => (value * max) / 255;
        #endregion

        private struct LedElement
        {
            public DmxTrackElement element;
            public IntRange strobeRange;

            public LedElement(DmxTrackElement element, IntRange strobeRange)
            {
                this.element = element;
                this.strobeRange = strobeRange;
            }

            public void Deconstruct(out DmxTrackElement element, out IntRange strobeRange)
            {
                element = this.element;
                strobeRange = this.strobeRange;
            }
        }
    }
}
