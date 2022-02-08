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

            TProvider[] providers = collection.GetProviders();
            float totalWeight = providers.Any() ? providers.Sum(p => p.weight) : 0.0f;
            foreach (TProvider provider in providers)
            {
                provider.weight = EditorGUILayout.Slider(
                    $"{provider.name} ({provider.weight / totalWeight:P})",
                    provider.weight,
                    RngProviderBase.MinWeight,
                    RngProviderBase.MaxWeight
                );
            }
        }
    }
}
