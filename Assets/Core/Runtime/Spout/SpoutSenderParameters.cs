using System;
using UnityEngine;

namespace Plml.Spout
{
    [Serializable]
    public class SpoutSenderParameters
    {
        public bool DebugInConsole { get; set; }
        public string SenderName { get; set; } = "SpoutSender";
        public Vector2Int TargetDimensions { get; set; } = new Vector2Int(1924, 1080);
        public TextureFormat TextureFormat { get; set; } = TextureFormat.DXGI_FORMAT_R8G8B8A8_UNORM;
    }
}