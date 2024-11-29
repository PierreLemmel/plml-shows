using UnityEditor;
using UnityEngine;

namespace Plml.Dmx.Editor
{
    public static class DmxMenu
    {
        [MenuItem("Plml/Dmx/Create fixture definition", priority = 10)]
        public static void CreateFixtureDefinition()
        {
            FixtureDefinition fd = ScriptableObject.CreateInstance<FixtureDefinition>();

            AssetDatabase.CreateAsset(fd, "Assets/Core/Prefabs/Dmx/Definitions/NewFixtureDefinition.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = fd;
        }

        [MenuItem("Plml/Dmx/Dmx Window", priority = 0)]
        public static void ShowWindow() => EditorWindow.GetWindow<DmxWindow>(false, "Dmx");
    }
}