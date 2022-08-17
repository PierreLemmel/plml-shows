using System.Linq;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Plml.Rng.Editor
{
    public abstract class ProviderCollectionEditor<TProviderCollection, TProvider> : UEditor
        where TProvider : RngProviderBase
        where TProviderCollection : RngProviderCollectionBase<TProvider>
    {
        public override void OnInspectorGUI()
        {
            var collection = (TProviderCollection)target;

            base.OnInspectorGUI();

            if (!typeof(TProvider).IsAbstract && GUILayout.Button("Add Provider"))
            {
                GameObject providerObj = new($"New {typeof(TProvider).Name}");
                providerObj.AddComponent<TProvider>();

                providerObj.AttachTo(collection);
            }

            EditorGUILayout.LabelField("Weights");

            TProvider[] providers = collection.GetAllProviders();
            TProvider[] activeProviders = collection.GetActiveProviders();

            float totalWeight = activeProviders.Any() ? activeProviders
                .Sum(p => p.weight) : 0.0f;

            PlmlUI.Indented(() =>
            {
                foreach (TProvider provider in providers)
                {

                    EditorGUILayout.BeginHorizontal(
                        GUILayout.ExpandWidth(true),
                        GUILayout.MinWidth(Screen.width - 125.0f)
                    );

                    bool active = provider.active;
                    string extraLabel = active ? (provider.weight / totalWeight).ToString("P") : "Inactive";

                    PlmlUI.Disabled(() =>
                    {
                        provider.weight = EditorGUILayout.Slider(
                            $"{provider.name} ({extraLabel})",
                            provider.weight,
                            RngProviderBase.MinWeight,
                            RngProviderBase.MaxWeight,
                            GUILayout.ExpandWidth(true)
                        );
                    }, !active);

                    provider.active = EditorGUILayout.Toggle(active, GUILayout.ExpandWidth(false));

                    EditorGUILayout.EndHorizontal();
                }
            });
        }
    }
}
