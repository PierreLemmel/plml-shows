using Plml.Dmx;
using UnityEngine;

namespace Plml.Playground
{
    public class TestLedBars : MonoBehaviour
    {
        public DmxTrackElement ledBar1;
        public DmxTrackElement ledBar2;

        [CubicRange(0.1f, 100f)]
        public float speed = 10f;

        [CubicRange(0.1f, 5f)]
        public float slope = 1f;

        [Range(0f, 5f)]
        public float width = 2f;

        private void Update()
        {
            float x = 0;

            Color24[] ledBar1Colors = new Color24[32];
            Color24[] ledBar2Colors = new Color24[32];

            for (int i = 0; i < 32; i++)
            {

                float a = Random.Range(0f, 1f);
                Color32 col = a * Color.white;

                ledBar1Colors[i] = col;
                ledBar2Colors[i] = col;
            }

            ledBar1.SetColorArray32(ledBar1Colors);
            ledBar2.SetColorArray32(ledBar2Colors);

            x += speed * Time.deltaTime;
        }
    }
}
