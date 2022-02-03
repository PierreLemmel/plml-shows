using Plml.Dmx;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor.Dmx
{
    public static class DmxMenu
    {
        [MenuItem("Plml/Dmx/Create fixture definition")]
        public static void CreateFixtureDefinition()
        {
            FixtureDefinition fd = ScriptableObject.CreateInstance<FixtureDefinition>();

            AssetDatabase.CreateAsset(fd, "Assets/Core/Prefabs/Dmx/Definitions/NewFixtureDefinition.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = fd;
        }
    }
}