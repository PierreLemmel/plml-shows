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
        
        [Range(0, 0xff)]
        public int flickerAmplitude = 0xff;
        [Range(0, 0xff)]
        public int flickerStrobe = 100;

        public bool pianoPlaying = false;

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

            adapter = FindObjectOfType<EnChiensAdapter>();
        }

        private void Start()
        {
            if (autoPlay)
                PlayIntro();
        }

        private void Update()
        {
            adapter.ResetLights();

            adapter.SetupContres(color, contresColor, contres, contre1, contre2, contre3, contre4);
            adapter.SetupJardinCour(color, jardinCour, courJardin);

            int servoDim = Mathf.Max(others, servo);
            adapter.SetupServo(servoDim, panServo, tiltServo);

            adapter.SetupOthers(others);

            if (isChasing)
                adapter.Chase(stroboscope);


            if (contreFlickering)
                adapter.Flicker(flickerAmplitude, flickerStrobe);


            if (pianoPlaying)
                adapter.PlayPiano(stroboscope);


            if (contrePulsating)
                adapter.UpdatePulsations(pulsationColor, pulsationMinValue, pulsationMaxValue);
            else
                adapter.StopPulsations();

            if (stop)
            {
#if UNITY_EDITOR
                Debug.Log("Stop");
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            adapter.CommitValues();
        }
    }
}