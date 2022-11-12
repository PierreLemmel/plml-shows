using Plml.Dmx.Scripting.Compilation;
using Plml.Dmx.Scripting.Runtime;
using System;
using UnityEngine;

namespace Plml.Dmx.Scripting
{
    public class LightScript : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxTrack track;

        [EditTimeOnly]
        public VariableDefinitionData variableDefinitions;

        [Multiline]
        public string input;

        private LightScriptCompilator compilator;
        private void Awake()
        {
            compilator = FindObjectOfType<LightScriptCompilator>() ?? throw new MissingComponentException($"Can't find {nameof(LightScriptCompilator)} in scene");

            variableDefinitions.fixtures.ForEach(f => Context.AddToContext(f));
            variableDefinitions.integers.ForEach(i => Context.AddToContext(i));
            variableDefinitions.floats.ForEach(f => Context.AddToContext(f));
            variableDefinitions.colors.ForEach(c => Context.AddToContext(c));

            Context.AddToContext(new FloatVariableInfo("t", defaultValue: 0f, isReadonly: true));
            Context.AddToContext(new FloatVariableInfo("dt", defaultValue: 0f, isReadonly: true));
            Context.AddToContext(new IntegerVariableInfo("frame", defaultValue: 0, isReadonly: true));

            if (!string.IsNullOrEmpty(input))
                Compile();
        }

        private LightScriptAction lsAction;
        public RuntimeContext Context { get; } = new();
        private void Update()
        {
            Context.Floats["t"].Value = Time.time;
            Context.Floats["dt"].Value = Time.deltaTime;
            Context.Integers["frame"].Value = Time.frameCount;

            if (lsAction != null)
            {
                

                lsAction(Context);
            }
        }

        public void Compile()
        {
            LightScriptCompilationData data = new()
            {
                fixtures = variableDefinitions.fixtures,
                integers = variableDefinitions.integers,
                floats = variableDefinitions.floats,
                colors = variableDefinitions.colors,

                text = input
            };

            if (compilator.TryCompile(data, out var action))
            {
                lsAction = action;
            }
        }

        [Serializable]
        public class VariableDefinitionData
        {
            public FixtureVariableInfo[] fixtures;
            public IntegerVariableInfo[] integers;
            public FloatVariableInfo[] floats;
            public ColorVariableInfo[] colors;
        }


        public class VariableRuntimeData
        {

        }
    }
}