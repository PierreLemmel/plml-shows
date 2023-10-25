using System;

namespace Plml.Rng
{
    [Serializable]
    public class RngSceneContent
    {
        public RngScene[] scenes;
        
        public RngScene blackout;

        public RngScene intro;
        public RngScene outro;

        public RngScene preShow;
        public RngScene postShow;
    }
}