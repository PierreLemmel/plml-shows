using Plml.Dmx;
using Plml.EnChiens.Animations;
using Plml.EnChiens.Animations.Gouvernail;
using Plml.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.EnChiens.Gouvernail
{
    public class EnChiensGouvernailAdapter : EnChiensAdapter
    {
        [HideInPlayMode]
        public float bpm = 60;

        [HideInPlayMode]
        public DmxTrackElement[] faces;

        [HideInPlayMode]
        public DmxTrackElement faceMain;

        [HideInPlayMode]
        public DmxTrackElement decoupeJardin;

        [HideInPlayMode]
        public DmxTrackElement decoupeCour;

        [HideInPlayMode]
        public DmxTrackElement diagonalJardin;

        [HideInPlayMode]
        public DmxTrackElement diagonalCour;

        [HideInPlayMode]
        public DmxTrackElement lateralJardin;

        [HideInPlayMode]
        public DmxTrackElement lateralCour;

        [HideInPlayMode]
        public DmxTrackElement doucheAvant;

        [HideInPlayMode]
        public DmxTrackElement doucheRencontre;

        [HideInPlayMode]
        public DmxTrackElement doucheJuliette;

        [HideInPlayMode]
        public DmxTrackElement fondsChauds;

        [HideInPlayMode]
        public DmxTrackElement fondsFroids;

        [HideInPlayMode]
        public MidiNote side1Note = MidiNote.C2;

        [HideInPlayMode]
        public MidiNote side2Note = MidiNote.D2;

        [HideInPlayMode]
        public DmxTrack side1Track;

        [HideInPlayMode]
        public DmxTrack side2Track;

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

        [Range(0x00, 0xff)]
        public int tradMaster = 160;

        private GouvernailContresPulsation contresPulsation;
        private SidesButtonsActivation sideButtons;

        [Range(0f, 2f)]
        public float pulsationSmoothTime = 0.3f;

        public float faceButtonSmoothTime = 0.1f;
        
        private DmxTrackElement[] allSpots;
        private DmxTrackElement[] fonds;
        private DmxTrackElement[] pianoSpots;
        private DmxTrackElement[] conclusionSpots;

        private void Awake()
        {
            fonds = new[] { fondsChauds, fondsFroids };

            allSpots = Arrays.Merge(
                new DmxTrackElement[]
                {
                    faceMain,
                    decoupeJardin,
                    decoupeCour,
                    lateralJardin,
                    lateralCour,
                    diagonalJardin,
                    diagonalCour,
                    doucheAvant,
                    doucheRencontre,
                    doucheJuliette,
                },
                faces,
                fonds
            );

            conclusionSpots = new[] { fondsChauds };

            contresPulsation = new GameObject("Contres Pulsation")
                .AddComponent<GouvernailContresPulsation>();

            contresPulsation.transform.parent = transform;
            contresPulsation.parsContre = fonds;
            contresPulsation.smoothTime = pulsationSmoothTime;
            contresPulsation.bpm = bpm;


            sideButtons = new GameObject("Side Buttons")
                .AddComponent<SidesButtonsActivation>();

            sideButtons.transform.parent = transform;
            
            sideButtons.side1Note = side1Note;
            sideButtons.side2Note = side2Note;

            sideButtons.smoothTime = faceButtonSmoothTime;

            pianoSpots = faces;

            sequenceEnumerator = ChasingSequence.GetEnumerator();
        }

        public override void ResetLights()
        {
            foreach (var par in allSpots)
            {
                par.value = 0x00;
            }
        }


        public override void SetupContres(Color globalColor, Color contresColor, int contres, int contre1, int contre2, int contre3, int contre4)
        {
            decoupeJardin.value = (contre1 * tradMaster) / 0xff;
            lateralJardin.value = (contre2 * tradMaster) / 0xff;
            lateralCour.value = (contre3 * tradMaster) / 0xff;
            decoupeCour.value = (contre4 * tradMaster) / 0xff;
        }

        public override void SetupServo(int dimmer, int pan, int tilt)
        {
            doucheRencontre.value = (dimmer * tradMaster) / 0xff;
        }

        public override void UpdatePulsations(Color color, float pulsationMinValue, float pulsationMaxValue)
        {
            contresPulsation.enabled = true;
            contresPulsation.minValue = (pulsationMinValue * tradMaster) / 0xff;
            contresPulsation.maxValue = (pulsationMaxValue * tradMaster) / 0xff;
        }

        public override void StopPulsations() => contresPulsation.enabled = false;

        public override void Chase(int strobe)
        {
            foreach (var spot in GetChasingSpots())
            {
                spot.value = tradMaster;
            }
        }

        public override void Flicker(int amplitude, int strobe)
        {
            if (IsFlickering())
            {
                foreach (var spot in fonds)
                {
                    spot.value = (amplitude * tradMaster) / 0xff;
                }
            }
        }

        public override void PlayPiano(int strobe)
        {
            int pianoIndex = GetPianoIndex();
            var spot = pianoSpots[pianoIndex];

            spot.value = tradMaster;
        }

        public override void SetupOthers(int others)
        {
            int othersValue = (others * tradMaster) / 0xff;

            foreach (var spot in allSpots)
            {
                spot.value = Mathf.Max(spot.value, othersValue);
            }

            side1Track.master = sideButtons.side1;
            side2Track.master = sideButtons.side2;
        }

        public override void SetupEnd(int value)
        {
            int endValue = (value * tradMaster) / 0xff;

            foreach (var spot in conclusionSpots)
            {
                spot.value = Mathf.Max(spot.value, endValue);
            }
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

                    IEnumerable<int> indices = GetRandomSequence(allSpots.Length, count, lastResult);
                    lastResult = indices;

                    IEnumerable<DmxTrackElement> result = indices.Select(idx => allSpots[idx]);
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
    }
}