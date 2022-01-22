using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Plml.EnChiens.Editor
{
    [CustomEditor(typeof(EnChiensSpectacle))]
    public class EnChiensSpectacleEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Intro"))
            {
                GetShow().PlayIntro();
            }

            if (GUILayout.Button("Outro"))
            {
                GetShow().PlayOutro();
            }

            DrawDefaultInspector();
        }

        private EnChiensSpectacle GetShow() => (EnChiensSpectacle)target;
    }
}