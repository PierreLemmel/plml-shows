using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Plml.Dmx.Scripting.Editor
{
    [CustomPropertyDrawer(typeof(LightScriptElement))]
    public class LightScriptElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);

            bool expanded = EditorGUI.Foldout(
                new(position.x, position.y, position.width, lineHeight),
                property.isExpanded, label);

            property.isExpanded = expanded;
            if (expanded)
            {
                EditorGUI.indentLevel++;
                int lines = 1;
                
                var inputProperty = property.FindPropertyRelative(nameof(LightScriptElement.input));
                string oldInput = inputProperty.stringValue;

                int inputLineCount = GetLineCount(property);
                Rect inputPosition = new(position.x, position.y + lines++ * lineHeight, position.width, inputLineCount * lineHeight);
                EditorGUI.PropertyField(inputPosition, inputProperty);


                lines += inputLineCount;

                if (Application.isPlaying)
                {
                    string newInput = inputProperty.stringValue;

                    if (newInput != oldInput)
                        property.FindPropertyRelative(nameof(LightScriptElement.couldRecompile)).boolValue = true;


                    SerializedProperty errorMsgProperty = property.FindPropertyRelative(nameof(LightScriptElement.errorMessage));
                    string errorMsg = errorMsgProperty.stringValue;
                    if (errorMsg.IsNotEmpty())
                    {
                        int errorLineCount = GetLineCount(errorMsgProperty);
                        Rect errorMsgPosition = new(position.x, position.y + lines * lineHeight, position.width, errorLineCount * lineHeight);
                        lines += errorLineCount;

                        EditorGUI.HelpBox(errorMsgPosition, errorMsg, MessageType.Error);
                    }


                    EditorGUI.BeginDisabledGroup(!property.FindPropertyRelative(nameof(LightScriptElement.couldRecompile)).boolValue);

                    Rect buttonPos = new(position.x, position.y + lines++ * lineHeight, position.width, lineHeight);
                    if (GUI.Button(buttonPos, "Recompile"))
                        property.FindPropertyRelative(nameof(LightScriptElement.shouldRecompile)).boolValue = true;

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1;

            if (property.isExpanded)
            {
                totalLines += GetLineCount(property);

                if (Application.isPlaying)
                {
                    SerializedProperty errorMsgProperty = property.FindPropertyRelative(nameof(LightScriptElement.errorMessage));
                    if (errorMsgProperty.stringValue.IsNotEmpty())
                    {
                        totalLines += GetLineCount(errorMsgProperty);
                    }

                    totalLines += 2;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }

        private static int GetLineCount(SerializedProperty property)
        {
            int lines = property.FindPropertyRelative(nameof(LightScriptElement.input))?.stringValue?.Split("\n").Length ?? 0;
            return Mathf.Max(2, lines);
        }
    }
}