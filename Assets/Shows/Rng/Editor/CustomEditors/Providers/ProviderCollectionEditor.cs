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
            foreach (TProvider provider in collection.GetProviders())
            {
                provider.weight = EditorGUILayout.Slider(provider.name, provider.weight, RngProviderBase.MinWeight, RngProviderBase.MaxWeight);
            }
        }
    }
}
