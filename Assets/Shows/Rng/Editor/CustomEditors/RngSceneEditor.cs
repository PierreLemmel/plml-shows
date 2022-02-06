using UnityEngine;
using UnityEditor;

using UEditor = UnityEditor.Editor;

namespace Plml.Rng.Editor
{
    [CustomEditor(typeof(RngScene))]
    public class RngSceneEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            RngScene scene = (RngScene)target;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }
    }
}