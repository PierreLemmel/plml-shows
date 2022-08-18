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

        private RngShow show;

        private void Awake()
        {
            show = FindObjectOfType<RngShow>();

            introPlayBtn.onClick.AddListener(PlayIntro);
            introStopBtn.onClick.AddListener(StopIntro);

            outroPlayBtn.onClick.AddListener(PlayOutro);
            outroStopBtn.onClick.AddListener(StopOutro);

            SetIntroPlayable(true);
            SetOutroPlayable(true);
        }

        private void Update()
        {
            if (!show.isPlaying || show.done)
            {
                SetIntroPlayable(true);
                SetOutroPlayable(true);
            }
        }

        private void SetIntroPlayable(bool playable)
        {
            introPlayBtn.interactable = playable;
            introStopBtn.interactable = !playable;
        }

        private void SetOutroPlayable(bool playable)
        {
            outroPlayBtn.interactable = playable;
            outroStopBtn.interactable = !playable;
        }

        public void PlayIntro()
        {
            show.PlayIntro();
            SetIntroPlayable(false);
            outroPlayBtn.interactable = false;
        }

        public void StopIntro() => show.StopShow();

        public void PlayOutro()
        {
            show.PlayOutro();
            SetOutroPlayable(false);
            introPlayBtn.interactable = false;
        }

        public void StopOutro() => show.StopShow();
    }
}