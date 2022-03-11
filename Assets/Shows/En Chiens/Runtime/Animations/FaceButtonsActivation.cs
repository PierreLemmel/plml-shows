using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.EnChiens.Animation
{
    public class FaceButtonsActivation : MonoBehaviour
    {
        public KeyCode jardinCourKey;
        public KeyCode courJardinKey;

        [Range(0x00, 0xff)]
        public int jardinCour;
        [Range(0x00, 0xff)]
        public int courJardin;

        public float smoothTime;

        private float jcTarget = 0x00;
        private float jcValue = 0.0f;
        private float jcVel = 0.0f;

        private float cjTarget = 0x00;
        private float cjValue = 0.0f;
        private float cjVel = 0.0f;

        private void Update()
        {
            if (Input.GetKeyDown(jardinCourKey))
                jcTarget = jcTarget == 0x00 ? 0xff : 0x00;

            if (Input.GetKeyDown(courJardinKey))
                cjTarget = cjTarget == 0x00 ? 0xff : 0x00;

            jcValue = Mathf.SmoothDamp(jcValue, jcTarget, ref jcVel, smoothTime);
            jardinCour = (int)jcValue;

            cjValue = Mathf.SmoothDamp(cjValue, cjTarget, ref cjVel, smoothTime);
            courJardin = (int)cjValue;
        }
    }
}
