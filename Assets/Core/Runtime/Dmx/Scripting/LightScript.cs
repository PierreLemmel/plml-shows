using Plml.Dmx.Scripting.Compilation;
using Plml.Dmx.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Dmx.Scripting
{
    public class LightScript : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxTrack track;

        [EditTimeOnly]
        public VariableDefinitionData variableDefinitions;

        private LightScriptCompilator compilator;
        private void Awake()
        {
            compilator = FindObjectOfType<LightScriptCompilator>() ?? throw new MissingComponentException($"Can't find {nameof(LightScriptCompilator)} in scene");

            variableDefinitions.fixtures.ForEach(Context.AddToContext);
            variableDefinitions.integers.ForEach(Context.AddToContext);
            variableDefinitions.floats.ForEach(Context.AddToContext);
            variableDefinitions.colors.ForEach(Context.AddToContext);

            Context.AddToContext(new FloatVariableInfo("t", defaultValue: 0f, isReadonly: true));
            Context.AddToContext(new FloatVariableInfo("dt", defaultValue: 0f, isReadonly: true));
            Context.AddToContext(new IntegerVariableInfo("frame", defaultValue: 0, isReadonly: true));
        }

        private void Start()
        {
            Recompile(initialize);
            Recompile(update);

            foreach (var ne in namedElements)
                Recompile(ne.element);

            Execute(initialize);
        }

        public void ExecuteAction(string name)
        {
            var elt = namedElements.SingleOrDefault(ne => ne.name == name)?.element
                ?? throw new InvalidOperationException($"Can't find a LightScript action named {name}");

            Execute(elt);
        }

        public LightScriptElement initialize;
        public LightScriptElement update;

        public NamedElement[] namedElements;

        public RuntimeContext Context { get; } = new();
        private void Update()
        {
            Context.Floats["t"].Value = Time.time;
            Context.Floats["dt"].Value = Time.deltaTime;
            Context.Integers["frame"].Value = Time.frameCount;

            if (initialize.shouldRecompile)
            {
                Recompile(initialize);
                Execute(initialize);
            }

            if (update.shouldRecompile)
                Recompile(update);

            Execute(update);

            foreach(var ne in namedElements)
            {
                var elt = ne.element;
                if (elt.shouldRecompile)
                    Recompile(elt);
            }
        }

        private void Recompile(LightScriptElement elt)
        {
            LightScriptCompilationData data = new()
            {
                fixtures = variableDefinitions.fixtures,
                integers = variableDefinitions.integers,
                floats = variableDefinitions.floats,
                colors = variableDefinitions.colors,

                text = elt.input
            };

            LightScriptCompilationResult result = compilator.Compile(data);
            if (result.isOk)
            {
                elt.action = result.action;
                elt.errorMessage = "";
            }
            else
            {
                elt.action = null;
                elt.errorMessage = result.message;
            }

            elt.couldRecompile = false;
            elt.shouldRecompile = false;
        }

        private void Execute(LightScriptElement elt) => elt.action?.Invoke(Context);

        private static void ResetElement(LightScriptElement elt)
        {
            elt.action = null;
            elt.errorMessage = "";
        }

        [Serializable]
        public class VariableDefinitionData
        {
            public FixtureVariableInfo[] fixtures;
            public IntegerVariableInfo[] integers;
            public FloatVariableInfo[] floats;
            public ColorVariableInfo[] colors;
        }

        [Serializable]
        public class NamedElement
        {
            public string name;
            public LightScriptElement element;
        }
    }
}