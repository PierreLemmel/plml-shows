using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml
{
    public static class Components
    {
        public static TComponent AddComponent<TComponent>(this Component comp)
            where TComponent : Component
            => comp.gameObject.AddComponent<TComponent>();

        public static TComponent AddComponent<TComponent>(this Component comp, Action<TComponent> setup)
            where TComponent : Component
        {
            TComponent result = comp.gameObject.AddComponent<TComponent>();
            setup(result);
            return result;
        }

        public static void AddComponent<TComponent>(this Component comp, Action<TComponent> setup, out TComponent component)
            where TComponent : Component
        {
            component = comp.gameObject.AddComponent<TComponent>();
            setup(component);
        }

        public static void AddComponent<TComponent>(this Component comp, out TComponent component)
            where TComponent : Component => component = comp.gameObject.AddComponent<TComponent>();

        public static bool HasComponent<TComponent>(this Component comp) where TComponent : Component
        {
            TComponent component = comp.GetComponent<TComponent>();
            return component != null;
        }

        public static bool HasComponentInChildren<TComponent>(this Component comp) where TComponent : Component
        {
            TComponent component = comp.GetComponentInChildren<TComponent>();
            return component != null;
        }

        public static bool HasComponentInParents<TComponent>(this Component comp) where TComponent : Component
        {
            TComponent component = comp.GetComponentInParent<TComponent>();
            return component != null;
        }

        public static bool HasComponentInParents(this Component comp, Type componentType)
        {
            Component component = comp.GetComponentInParent(componentType);
            return component != null;
        }

        public static TComponent AttachTo<TComponent>(this TComponent component, Component other) where TComponent : Component
        {
            component.transform.SetParent(other.transform);
            return component;
        }

        public static TComponent AttachTo<TComponent>(this TComponent component, GameObject other) where TComponent : Component
        {
            component.transform.SetParent(other.transform);
            return component;
        }

        public static void ClearChildren(this Component go)
        {
            Transform transform = go.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
                transform.GetChild(i).gameObject.DestroyWithChildren();
        }

        public static GameObject AddChild(this Component comp, string name) => comp.gameObject.AddChild(name);
        public static GameObject AddChild(this Component comp, string name, Action<GameObject> setup) => comp.gameObject.AddChild(name, setup);

        public static TComponent WithChild<TComponent>(this TComponent comp, string name) where TComponent : Component => comp.WithChild(name, out _);
        public static TComponent WithChild<TComponent>(this TComponent comp, string name, out GameObject child) where TComponent : Component
        {
            child = new GameObject(name)
                .AttachTo(comp);

            return comp;
        }

        public static TComponent WithChild<TComponent>(this TComponent comp, string name, Action<GameObject> setup) where TComponent : Component => comp.WithChild(name, setup);
        public static TComponent WithChild<TComponent>(this TComponent comp, string name, Action<GameObject> setup, out GameObject child) where TComponent : Component
        {
            child = new GameObject(name)
                .AttachTo(comp);

            setup(child);

            return comp;
        }

        public static void AddChildren(this Component comp, int count, Func<int, string> nameFunc, Action<GameObject, int> setup)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject child = new(nameFunc(i));

                child.AttachTo(comp);
                setup(child, i);
            }
        }

        public static void AddChildren(this Component comp, int count, Func<int, string> nameFunc, Action<GameObject> setup)
            => comp.AddChildren(count, nameFunc, (go, i) => setup(go));

        public static void SetScale(this Component comp, float scale) => comp.transform.localScale = new(scale, scale, scale);

        public static IEnumerable<Transform> EnumerateChildren(this Component comp) => comp.gameObject.EnumerateChildren();
        public static IEnumerable<TComponent> EnumerateComponentsInDirectChildren<TComponent>(this Component comp) where TComponent : Component
            => comp.gameObject.EnumerateComponentsInDirectChildren<TComponent>();

        public static TComponent GetComponentInDirectChildren<TComponent>(this Component comp) where TComponent : Component
            => comp.gameObject.GetComponentInDirectChildren<TComponent>();

        public static TComponent[] GetComponentsInDirectChildren<TComponent>(this Component comp) where TComponent : Component
            => comp.gameObject.GetComponentsInDirectChildren<TComponent>();
    }
}