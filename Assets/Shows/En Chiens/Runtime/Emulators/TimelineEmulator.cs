using System;
using UnityEngine;
using UnityEngine.Playables;
using UText = UnityEngine.UI.Text;

namespace Plml.EnChiens.Emulators
{
    public class TimelineEmulator : MonoBehaviour
    {
        private UText text;

        [HideInPlayMode]
        public PlayableDirector director;

        private void Awake()
        {
            text = GetComponentInChildren<UText>();
        }

        private void Update()
        {
            const string timeFormat = "0.00";

            if (director.playableAsset != null)
            {
                text.text = string.Join(Environment.NewLine,
                    director.playableAsset.name,
                    $"{director.time.ToString(timeFormat)} / {director.duration.ToString(timeFormat)}"
                );
            }
            else
            {
                text.text = "No timeline selected";
            }
        }
    }
}
