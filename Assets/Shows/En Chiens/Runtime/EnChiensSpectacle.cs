using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using Plml.Dmx;

namespace Plml.EnChiens
{
    [RequireComponent(typeof(PlayableDirector))]
    public class EnChiensSpectacle : MonoBehaviour
    {
        [HideInPlayMode]
        public bool autoPlay = false;

        [Range(0.0f, 300.0f)]
        public float introStartTime = 0.0f;

        [Range(0.0f, 300.0f)]
        public float outroStartTime = 0.0f;

        private PlayableDirector director;

        [HideInPlayMode]
        public PlayableAsset introTimeline;

        [HideInPlayMode]
        public PlayableAsset outroTimeline;


        [Range(0x00, 0xff)]
        public int jardinCour;

        [Range(0x00, 0xff)]
        public int courJardin;


        [Range(0x00, 0xff)]
        public int contres;
        public Color color = Color.black;
        public Color contresColor = Color.black;

        [Range(0x00, 0xff)]
        public int contre1;
        [Range(0x00, 0xff)]
        public int contre2;
        [Range(0x00, 0xff)]
        public int contre3;
        [Range(0x00, 0xff)]
        public int contre4;

        [Range(0x00, 0xff)]
        public int others;

        [Range(0x00, 0xff)]
        public int panServo;

        [Range(0x00, 0xff)]
        public int tiltServo;

        [Range(0x00, 0xff)]
        public int servo;

        public bool isChasing = false;
        


        [Range(0, 0xff)]
        public int stroboscope;

        public bool contreFlickering = false;
        [Range(2, 12)]
        public int flickeringDurationInFrames = 3;
        [Range(0, 0xff)]
        public int flickerAmplitude = 0xff;
        [Range(0, 0xff)]
        public int flickerStrobe = 100;

        public bool pianoPlaying = false;
        [Range(2, 12)]
        public int pianoDurationInFrames = 3;

        public bool contrePulsating = false;
        public Color pulsationColor = Color.black;

        [Range(0, 0xff)]
        public float pulsationMinValue = 0.0f;

        [Range(0, 0xff)]
        public float pulsationMaxValue = 0.0f;


        public bool stop = false;

        private EnChiensAdapter adapter;

        public void PlayIntro()
        {
            director.initialTime = introStartTime;
            director.playableAsset = introTimeline;
            director.Play();
        }

        public void PlayOutro()
        {
            director.initialTime = outroStartTime;
            director.playableAsset = outroTimeline;
            director.Play();
        }

        private void Awake()
        {
            director = GetComponent<PlayableDirector>();
            director.playOnAwake = false;

            
        }

        private void Start()
        {
            if (autoPlay)
                PlayIntro();
        }

        private void Update()
        {
            foreach (var par in parsAll)
            {
                par.stroboscope = 0x00;
            }

            parLedCourJardin.color = color;
            parLedJardinCour.color = color;

            Color maxContreColor = Colors.Max(color, contresColor);
            foreach (var contre in parsContre)
            {
                contre.color = maxContreColor;
            }

            parLedJardinCour.dimmer = Mathf.Max(jardinCour, faceButtons.jardinCour);
            parLedCourJardin.dimmer = Mathf.Max(courJardin, faceButtons.courJardin);

            parLedContre1.dimmer = Mathf.Max(contre1, contres);
            parLedContre2.dimmer = Mathf.Max(contre2, contres);
            parLedContre3.dimmer = Mathf.Max(contre3, contres);
            parLedContre4.dimmer = Mathf.Max(contre4, contres);

            int servoDim = Mathf.Max(others, servo);
            parServoCour.cold = servoDim;
            parServoCour.pan = panServo;
            parServoCour.tilt = tiltServo;

            if (isChasing)
            {
                foreach (var par in GetChasingSpots())
                {
                    par.dimmer = 0xff;
                    par.stroboscope = stroboscope;
                }
            }

            if (contreFlickering)
            {
                if (IsFlickering())
                {
                    foreach (var par in parsContre)
                    {
                        par.dimmer = flickerAmplitude;
                        par.stroboscope = flickerStrobe;
                    }
                }
            }

            if (pianoPlaying)
            {
                int pianoIndex = GetPianoIndex();
                var contre = parsContre[pianoIndex];

                contre.dimmer = 0xff;
                contre.stroboscope = stroboscope;
            }

            if (contrePulsating)
            {
                contresPulsation.enabled = true;
                contresPulsation.color = pulsationColor;
                contresPulsation.minValue = pulsationMinValue;
                contresPulsation.maxValue = pulsationMaxValue;
            }
            else
            {
                contresPulsation.enabled = false;
            }

            if (stop)
            {
#if UNITY_EDITOR
                Debug.Log("Stop");
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            faceButtons.smoothTime = faceButtonsSmoothTime;
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