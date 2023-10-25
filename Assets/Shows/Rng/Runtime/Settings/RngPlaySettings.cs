using System;

namespace Plml.Rng
{
    [Serializable]
    public class RngPlaySettings
    {
        public bool autoGenerateScenes = true;
        public bool autoSaveShow = true;
        public bool autoSendPlaylist = true;
        public string playlistUrl = "";
    }
}