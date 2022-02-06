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
    }
}