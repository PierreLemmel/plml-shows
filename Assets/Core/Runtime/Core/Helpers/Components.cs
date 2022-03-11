using System;
using UnityEngine;

namespace Plml
{
    public static class Components
    {
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
    }
}