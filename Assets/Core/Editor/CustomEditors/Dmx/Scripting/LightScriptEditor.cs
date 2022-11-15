using Plml.Dmx.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Plml.Dmx.Scripting.LightScript;
using UEditor = UnityEditor.Editor;


namespace Plml.Dmx.Scripting.Editor
{
    [CustomEditor(typeof(LightScript))]
    public class LightScriptEditor : UEditor
    {
        private static Dictionary<string, bool> expandedCache = new();
        private static Dictionary<string, int> indexCache = new();

        public override void OnInspectorGUI()
        {
            LightScript lightScript = (LightScript)target;

            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                DisplayContext(lightScript.Context, lightScript.variableDefinitions);


                if (lightScript.namedElements.Any())
                {
                    string[] options = lightScript
                        .namedElements
                        .Select(ne => ne.name)
                        .ToArray();

                    string indexKey = targetKey;
                    if (!indexCache.TryGetValue(indexKey, out int index))
                    {
                        index = 0;
                        indexCache.Add(indexKey, index);
                    }

                    index = EditorGUILayout.Popup(index, options);

                    indexCache[indexKey] = index;
                }
            }
        }

        private string targetKey => $"{target.name}-{target.GetInstanceID()}";

        private void DisplayContext(ILightScriptContext context, VariableDefinitionData definitions)
        {
            string ctxExpandedKey = targetKey;

            bool ctxExpanded = CheckForKey(ctxExpandedKey);
            ctxExpanded = EditorGUILayout.Foldout(ctxExpanded, "Variables", EditorStyles.foldoutHeader);

            if (ctxExpanded)
            {
                EditorGUI.indentLevel++;

                if (definitions.integers.Any())
                {
                    string intExpandedKey = ctxExpandedKey + ":int";

                    bool intExpanded = CheckForKey(intExpandedKey);
                    intExpanded = EditorGUILayout.Foldout(intExpanded, "Integers", EditorStyles.foldoutHeader);

                    if (intExpanded)
                    {
                        EditorGUI.indentLevel++;
                        
                        foreach (var varInfo in definitions.integers)
                        {
                            string name = varInfo.name;

                            IntVariable variable = context.Integers[varInfo.name];

                            if (variable.Info.hasBounds)
                                variable.Value = EditorGUILayout.IntSlider(name, variable.Value, variable.Info.minValue, variable.Info.maxValue);
                            else
                                variable.Value = EditorGUILayout.IntField(name, variable.Value);
                        }

                        EditorGUI.indentLevel--;
                    }

                    expandedCache[intExpandedKey] = intExpanded;
                }
                else
                    EditorGUILayout.LabelField("No integers to display");


                if (context.Floats.Any())
                {
                    string fltExpandedKey = ctxExpandedKey + ":flt";

                    bool fltExpanded = CheckForKey(fltExpandedKey);
                    fltExpanded = EditorGUILayout.Foldout(fltExpanded, "Floats", EditorStyles.foldoutHeader);

                    if (fltExpanded)
                    {
                        EditorGUI.indentLevel++;

                        foreach (var varInfo in definitions.floats)
                        {
                            string name = varInfo.name;

                            FloatVariable variable = context.Floats[varInfo.name];

                            if (variable.Info.hasBounds)
                                variable.Value = EditorGUILayout.Slider(name, variable.Value, variable.Info.minValue, variable.Info.maxValue);
                            else
                                variable.Value = EditorGUILayout.FloatField(name, variable.Value);
                        }

                        EditorGUI.indentLevel--;
                    }

                    expandedCache[fltExpandedKey] = fltExpanded;
                }
                else
                    EditorGUILayout.LabelField("No floats to display");


                if (context.Colors.Any())
                {
                    string colExpandedKey = ctxExpandedKey + ":col";

                    bool colExpanded = CheckForKey(colExpandedKey);
                    colExpanded = EditorGUILayout.Foldout(colExpanded, "Colors", EditorStyles.foldoutHeader);

                    if (colExpanded)
                    {
                        EditorGUI.indentLevel++;

                        foreach (var varInfo in definitions.colors)
                        {
                            string name = varInfo.name;

                            ColorVariable variable = context.Colors[varInfo.name];

                            variable.Value = EditorGUILayout.ColorField(name, variable.Value);
                        }

                        EditorGUI.indentLevel--;
                    }

                    expandedCache[colExpandedKey] = colExpanded;
                }
                else
                    EditorGUILayout.LabelField("No colors to display");



                EditorGUI.indentLevel--;
            }

            expandedCache[ctxExpandedKey] = ctxExpanded;

            bool CheckForKey(string key)
            {
                if (!expandedCache.TryGetValue(key, out bool result))
                    expandedCache.Add(key, result);

                return result;
            }
        }
    }
}