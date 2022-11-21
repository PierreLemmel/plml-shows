using System;
using UnityEngine;

namespace Plml.Dmx.Scripting
{
    [Serializable]
    public class LightScriptElement
    {
        [Multiline]
        public string input;

        public bool couldRecompile;
        public bool shouldRecompile;

        public bool isCompiled;

        public string errorMessage;

        [NonSerialized]
        public LightScriptAction action;
    }
}