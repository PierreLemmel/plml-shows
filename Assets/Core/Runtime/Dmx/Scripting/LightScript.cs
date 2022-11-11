using System;
using UnityEngine;

namespace Plml.Dmx.Scripting
{
    public class LightScript : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxTrack track;

        public VariableData variables;

        [Multiline]
        public string input;

        private LightScriptCompilator compilator;
        private void Awake()
        {
            compilator = FindObjectOfType<LightScriptCompilator>() ?? throw new MissingComponentException($"Can't find {nameof(LightScriptCompilator)} in scene");

            context = new LightScriptContext();

            variables.fixtures.ForEach(f => context.AddToContext(f.name, f.fixture));
            variables.integers.ForEach(i => context.AddToContext(i.name, i.defaultValue));
            variables.floats.ForEach(f => context.AddToContext(f.name, f.defaultValue));
            variables.colors.ForEach(c => context.AddToContext(c.name, c.defaultValue));

            context.AddToContext("t", 0f);
            context.AddToContext("dt", 0f);
            context.AddToContext("frame", 0);

            Compile();
        }

        private LightScriptAction lsAction;
        private ILightScriptContext context;
        private void Update()
        {
            if (lsAction != null)
            {
                context.Floats["t"] = Time.time;
                context.Floats["dt"] = Time.deltaTime;
                context.Integers["frame"] = Time.frameCount;

                lsAction(context);
            }
        }

        public void Compile()
        {
            LightScriptData data = new()
            {
                fixtures = variables.fixtures,
                integers = variables.integers,
                floats = variables.floats,
                colors = variables.colors,

                text = input
            };

            if (compilator.TryCompile(data, out var action))
            {
                lsAction = action;
            }
        }

        [Serializable]
        public class VariableData
        {
            public LightScriptFixtureData[] fixtures;
            public LightScriptIntegerData[] integers;
            public LightScriptFloatData[] floats;
            public LightScriptColorData[] colors;
        }
    }
}