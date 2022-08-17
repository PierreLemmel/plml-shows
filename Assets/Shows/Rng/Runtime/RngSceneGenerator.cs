using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Plml.Rng
{
    public class RngSceneGenerator : MonoBehaviour
    {
        public RngScene GenerateIntroScene() => GenerateIntroOutroScene(true);
        public RngScene GenerateOutroScene() => GenerateIntroOutroScene(false);

        private RngScene GenerateIntroOutroScene(bool intro)
        {
            throw new NotImplementedException();
        }
    }
}
