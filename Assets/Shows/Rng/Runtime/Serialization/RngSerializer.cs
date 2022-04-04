using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Plml.Rng
{
    public class RngSerializer : MonoBehaviour
    {
        public RngSerializerSettings settings;

        public void SerializeShow(RngShow show)
        {

        }


        private class RngShowDataModel
        {
            public RngShowSettingsDataModel Settings { get; set; }
            public IEnumerable<RngSceneDataModel> Scenes { get; set; }
        }

        private class RngShowSettingsDataModel
        {
        }


        private class RngSceneDataModel
        {
            public TimeWindow TimeWindow { get; set; }

            public AudioDataModel Audio { get; set; }
        }

        private class AudioDataModel
        {
            public string ClipName { get; set; }
            public float Volume { get; set; }

            public TimeWindow TimeWindow { get; set; }
        }
    }
}
