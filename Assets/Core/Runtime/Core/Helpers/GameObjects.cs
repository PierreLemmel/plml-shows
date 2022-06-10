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

        public static GameObject WithComponent<TComponent>(this GameObject go, Action<TComponent> setup) where TComponent : Component
            => WithComponent(go, setup, out _);

        public static GameObject WithComponent<TComponent>(this GameObject go, out TComponent component) where TComponent : Component
        {
            component = go.AddComponent<TComponent>();
            return go;
        }

        public static GameObject WithComponent<TComponent>(this GameObject go, Action<TComponent> setup, out TComponent component) where TComponent : Component
        {
            GameObject gameObject = go.WithComponent(out component);
            setup(component);

            return gameObject;
        }

        public static GameObject WithChild(this GameObject go, string name) => go.WithChild(name, out _);

        public static GameObject WithChild(this GameObject go, string name, out GameObject child)
        {
            child = new GameObject(name)
                .AttachTo(go);

            return go;
        }

        public static GameObject WithChild(this GameObject go, string name, Action<GameObject> setup) => go.WithChild(name, setup, out _);

        public static GameObject WithChild(this GameObject go, string name, Action<GameObject> setup, out GameObject child)
        {
            child = new GameObject(name)
                .AttachTo(go);

            setup(child);

            return go;
        }

        public static GameObject AddChild(this GameObject go, string name) => new GameObject(name).AttachTo(go);

        public static GameObject AddChild(this GameObject go, string name, Action<GameObject> setup)
        {
            GameObject child = new(name);

            child.AttachTo(go);
            setup(child);

            return child;
        }

        public static TComponent AddComponent<TComponent>(this GameObject go, Action<TComponent> setup)
            where TComponent : Component
        {
            TComponent result = go.AddComponent<TComponent>();
            setup(result);
            return result;
        }

        public static void AddComponent<TComponent>(this GameObject go, Action<TComponent> setup, out TComponent component)
            where TComponent : Component
        {
            component = go.AddComponent<TComponent>();
            setup(component);
        }

        public static void AddComponent<TComponent>(this GameObject go, out TComponent component)
            where TComponent : Component => component = go.AddComponent<TComponent>();

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

        public static GameObject Clone(this GameObject go)
        {
            GameObject clone = UObject.Instantiate(go);
            clone.name = go.name;
            return clone;
        }

        public static void AddChildren(this GameObject go, int count, Func<int, string> nameFunc, Action<GameObject, int> setup)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject child = new(nameFunc(i));

                child.AttachTo(go);
                setup(child, i);
            }
        }

        public static void AddChildren(this GameObject go, int count, Func<int, string> nameFunc, Action<GameObject> setup)
            => go.AddChildren(count, nameFunc, (go, i) => setup(go));
    }
}