using Plml.Dmx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Plml.Rng
{
    public class RngSerializer : MonoBehaviour
    {
        public RngSerializerSettings settings;

        public void SerializeShow(RngShow show)
        {
            RngShowDataModel showDataModel = new()
            {
                settings = new()
                {
                    minScenes = show.showSettings.minScenes,
                    maxScenes = show.showSettings.maxScenes,
                    durationSpread = show.showSettings.durationSpread,
                    showDuration = show.showSettings.showDuration,
                },
                scenes = show
                    .content.scenes
                    .Select(scene => new RngSceneDataModel
                    {
                        timeWindow = scene.sceneWindow,
                        trackName = scene.track?.name,
                        audio = new AudioDataModel
                        {
                            hasAudio = scene.audioData?.audioClip != null,
                            volume = scene.audioData?.audioVolume ?? 0,
                            clipName = scene.audioData?.audioClip != null ? scene.audioData?.audioClip.name : null,
                            musicWindow = scene.audioData?.musicWindow ?? TimeWindow.Empty,
                        }
                    })
            };

            string json = JsonUtility.ToJson(showDataModel, true);
            
            string filename = Path.Combine(settings.path, $"{settings.prefix}-{DateTime.Now:yyMMdd-HHmmss}.json");
            Debug.Log($"Saving '{filename}'");
            File.WriteAllText(filename, json);
            Debug.Log($"'{filename}' saved");
        }

        [Serializable]
        private class RngShowDataModel
        {
            public RngShowSettingsDataModel settings;
            public RngSceneDataModel[] scenes;
        }

        [Serializable]
        private class RngShowSettingsDataModel
        {
            public int minScenes;
            public int maxScenes;
            public float durationSpread;
            public float showDuration;
        }

        [Serializable]
        private class RngSceneDataModel
        {
            public TimeWindow timeWindow;

            public string trackName;
            public AudioDataModel audio;
        }

        [Serializable]
        private class AudioDataModel
        {
            public bool hasAudio;
            public string clipName;
            public float volume;

            public TimeWindow musicWindow;
        }
    }
}
