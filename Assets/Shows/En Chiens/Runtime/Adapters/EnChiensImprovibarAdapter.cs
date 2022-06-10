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
        public int bpm = 60;
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
        public int chasingStepDurationInFrames = 9;

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

            sequenceEnumerator = ChasingSequence.GetEnumerator();
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

    }
}