using Plml.Dmx;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Rng.UI
{
    public class RngIntroOutroPanel : MonoBehaviour
    {
        public Button introPlayBtn;
        public Button introStopBtn;

        public Button outroPlayBtn;
        public Button outroStopBtn;

        private DmxTrackControler dmxControler;
        private AudioSource audioSource;
        private RngShow show;

        private Coroutine introCoroutine;
        private Coroutine outroCoroutine;

        private void Awake()
        {
            dmxControler = FindObjectOfType<DmxTrackControler>();
            audioSource = FindObjectOfType<AudioSource>();
            show = FindObjectOfType<RngShow>();

            introPlayBtn.onClick.AddListener(PlayIntro);
            introPlayBtn.onClick.AddListener(StopIntro);

            outroPlayBtn.onClick.AddListener(PlayOutro);
            outroStopBtn.onClick.AddListener(StopOutro);

            introStopBtn.interactable = false;
            outroStopBtn.interactable = false;
        }

        public void PlayIntro()
        {

        }

        private IEnumerator PlayIntroCoroutine()
        {
            Debug.Log("Play Intro");
            yield return new WaitForSeconds(2f);
            Debug.Log("Intro Done");
        }

        public void StopIntro()
        {

        }

        public void PlayOutro()
        {

        }

        private IEnumerator PlayOutroCoroutine()
        {
            yield return new WaitForSeconds(2f);
        }

        public void StopOutro()
        {

        }
    }
}