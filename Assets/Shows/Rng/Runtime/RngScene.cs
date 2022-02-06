using Plml.Dmx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.Rng
{
    public class RngScene : MonoBehaviour
    {
        private DmxTrackControler controler;

        [EditTimeOnly]
        public float totalDuration = 3600.0f;

        [PlayTimeOnly]
        public float remainingDuration = 3600.0f;

        private void Awake()
        {
            controler = FindObjectOfType<DmxTrackControler>();
        }
    }
}