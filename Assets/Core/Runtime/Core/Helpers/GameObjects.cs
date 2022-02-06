using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace Plml
{
    public static class GameObjects
    {
        public static void ClearChildren(this GameObject go)
        {
            Transform transform = go.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyWithChildren(transform.GetChild(i).gameObject);
        }

        public static void DestroyWithChildren(this GameObject go)
        {
            Transform transform = go.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyWithChildren(transform.GetChild(i).gameObject);

            UObject.DestroyImmediate(go);
        }

        public static GameObject AttachTo(this GameObject go, Component other)
        {
            go.transform.SetParent(other.transform);
            return go;
        }

        public static GameObject AttachTo(this GameObject go, GameObject other)
        {
            go.transform.SetParent(other.transform);
            return go;
        }

        public static GameObject WithComponent<TComponent>(this GameObject go) where TComponent : Component => go.WithComponent<TComponent>(out _);

        public static GameObject WithComponent<TComponent>(this GameObject go, out TComponent component) where TComponent : Component
        {
            component = go.AddComponent<TComponent>();
            return go;
        }

        public static bool HasComponent<TComponent>(this GameObject go) where TComponent : Component
        {
            TComponent component = go.GetComponent<TComponent>();
            return component != null;
        }

        public static bool HasComponentInChildren<TComponent>(this GameObject go) where TComponent : Component
        {
            TComponent component = go.GetComponentInChildren<TComponent>();
            return component != null;
        }

        public static IEnumerable<GameObject> GetChildren(this GameObject go)
        {
            Transform transform = go.transform;
            return Enumerable.Range(0, transform.childCount)
                .Select(i => transform.GetChild(i).gameObject);
        }

        public static bool HasChildren(this GameObject go) => go.transform.childCount > 0;

        public static GameObject GetChild(this GameObject go) => go.GetChildren().Single();
    }
}