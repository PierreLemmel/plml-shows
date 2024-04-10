using Plml.Dmx;
using Plml.EnChiens.Animations;
using Plml.EnChiens.Animations.Improviste;
using Plml.Midi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.EnChiens.Improviste
{
    public class ImprovisteAdapter : EnChiensAdapter
    {
        [HideInPlayMode]
        public float bpm = 60;


        #region Fixtures
        [HideInPlayMode]
        public DmxTrackElement doucheRgbJar;

        [HideInPlayMode]
        public DmxTrackElement doucheRgbCentre;

        [HideInPlayMode]
        public DmxTrackElement doucheRgbCour;


        [HideInPlayMode]
        public DmxTrackElement rideauRgbJar;

        [HideInPlayMode]
        public DmxTrackElement rideauRgbCentre;

        [HideInPlayMode]
        public DmxTrackElement rideauRgbCour;



        [HideInPlayMode]
        public DmxTrackElement faceLedRgbJar;

        [HideInPlayMode]
        public DmxTrackElement faceLedRgbCentre;

        [HideInPlayMode]
        public DmxTrackElement faceLedRgbCour;


        [HideInPlayMode]
        public DmxTrackElement stroboscope;


        [HideInPlayMode]
        public DmxTrackElement faceBeam;

        [HideInPlayMode]
        public DmxTrackElement[] otherTradFixtures;
        #endregion

        [Range(0x00, 0xff)]
        public int douchesBeam = 140;

        [Range(0x00, 0xff)]
        public int strobeLvlOnFlicker = 150;

        [Range(0x00, 0xff)]
        public int tradsLevel = 180;

        [Range(0x00, 0xff)]
        public int faceBeamLevel = 200;


        #region Sides
        [HideInPlayMode]
        public KeyCode side1Key = KeyCode.Alpha1;

        [HideInPlayMode]
        public KeyCode side2Key = KeyCode.Alpha2;

        [HideInPlayMode]
        public DmxTrack side1Track;

        [HideInPlayMode]
        public DmxTrack side2Track;

        private SidesButtonsActivation sideButtons;
        public float faceButtonSmoothTime = 0.1f;
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


        private ImprovisteContresPulsation contresPulsation;


        [Range(0f, 2f)]
        public float pulsationSmoothTime = 0.3f;
        #endregion


        #region Groups
        private DmxTrackElement[] douches;
        private DmxTrackElement[] rideaux;
        private DmxTrackElement[] facesRgb;

        private DmxTrackElement[] allLedSpotsWithStrobe;
        private DmxTrackElement[] pianoSpots;
        #endregion


        private void Awake()
        {
            #region Groups
            rideaux = new[] { rideauRgbJar, rideauRgbCentre, rideauRgbCour };
            douches = new[] { doucheRgbJar, doucheRgbCentre, doucheRgbCour };
            facesRgb = new[] { faceLedRgbJar, faceLedRgbCentre, faceLedRgbCour };

            allLedSpotsWithStrobe = Arrays.Merge(
                rideaux,
                douches,
                facesRgb
            );

            pianoSpots = rideaux;
            #endregion


            #region Pulsation setup
            contresPulsation = new GameObject("Contres Pulsation")
                    .AddComponent<ImprovisteContresPulsation>();

            contresPulsation.transform.parent = transform;
            contresPulsation.contresRideau = rideaux;
            contresPulsation.smoothTime = pulsationSmoothTime;
            contresPulsation.bpm = bpm;
            #endregion


            #region Side buttons
            sideButtons = new GameObject("Side Buttons")
                        .AddComponent<SidesButtonsActivation>()
                        .AttachTo(this);

            sideButtons.SetupSideKeys(side1Key, side2Key);
            sideButtons.smoothTime = faceButtonSmoothTime;
            #endregion


            sequenceEnumerator = ChasingSequence.GetEnumerator();
        }

        #region Overrides
        public override void ResetLights()
        {
            foreach (var rideau in rideaux)
            {
                rideau.stroboscope = 0x00;
                rideau.dimmer = 0x00;
            }

            foreach (var douche in douches)
            {
                douche.stroboscope = 0x00;
                douche.dimmer = 0x00;
            }


            foreach (var trad in otherTradFixtures)
            {
                trad.value = 0x00;
            }

            faceBeam.dimmer = 0x00;
            faceBeam.beam = faceBeamLevel;

            stroboscope.stroboscope = 0x00;
            stroboscope.dimmer = 0x00;
        }


        #region Setups
        public override void SetupEnd(int value)
        {
            int corrected = (150 * value) / 255;

            faceBeam.dimmer = Mathf.Max(
                faceBeam.dimmer,
                corrected
            );
        }

        public override void SetupServo(int dimmer, int pan, int tilt)
        {
            int corrected = (150 * dimmer) / 255;

            faceBeam.dimmer = Mathf.Max(
                faceBeam.dimmer,
                corrected
            );
        }

        public override void SetupFond(int value)
        {
            foreach (var rideau in rideaux)
            {
                rideau.dimmer = Mathf.Max(
                    rideau.dimmer,
                    value
                );
            }
        }

        public override void SetupContres(Color globalColor, Color contresColor, int contres, int contre1, int contre2, int contre3, int contre4)
        {
            //doucheRgbJar.dimmer = Mathf.Max(
            //    doucheRgbJar.dimmer,
            //    contres,
            //    contre1
            //);
            //doucheRgbCentre.dimmer = Mathf.Max(
            //    doucheRgbCentre.dimmer,
            //    contres,
            //    contre2
            //);
            //doucheRgbCour.dimmer = Mathf.Max(
            //    doucheRgbCour.dimmer,
            //    contres,
            //    contre3
            //);
        }

        public override void SetupJardinCour(Color color, int jardinCour, int courJardin)
        {
            int val = Mathf.Max(jardinCour, courJardin);
            foreach (var face in facesRgb)
            {
                face.dimmer = Mathf.Max(
                    face.dimmer,
                    val
                );
            }
        }

        public override void SetupOthers(int others)
        {
            Color24 white24 = Color.white;

            int tradVal = (others * tradsLevel) / 255;
            foreach (var trad in otherTradFixtures)
            {
                //trad.value = Mathf.Max(
                //    trad.value,
                //    tradVal
                //);
            }

            foreach (var douche in douches)
            {
                douche.dimmer = Mathf.Max(
                    douche.dimmer,
                    others
                );
                douche.color = white24;
                douche.white = 0xff;
            }

            foreach (var rideau in rideaux)
            {
                rideau.dimmer = Mathf.Max(
                    rideau.dimmer,
                    others
                );
                rideau.color = white24;
            }

            side1Track.master = sideButtons.side1;
            side2Track.master = sideButtons.side2;
        }
        #endregion


        #region Effects
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
                foreach (var rideau in rideaux)
                {
                    rideau.dimmer = Mathf.Max(
                        rideau.dimmer,
                        amplitude
                    );
                    rideau.stroboscope = strobe;
                }

                stroboscope.stroboscope = strobeLvlOnFlicker;
                stroboscope.dimmer = Mathf.Max(
                    stroboscope.dimmer,
                    amplitude
                );
            }
        }

        public override void PlayPiano(int strobe)
        {
            int pianoIndex = GetPianoIndex();
            var contre = rideaux[pianoIndex];

            contre.dimmer = 0xff;
            contre.stroboscope = strobe;
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

                    IEnumerable<int> indices = GetRandomSequence(allLedSpotsWithStrobe.Length, count, lastResult);
                    lastResult = indices;

                    IEnumerable<DmxTrackElement> result = indices.Select(idx => allLedSpotsWithStrobe[idx]);
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
        #endregion
    }
}
