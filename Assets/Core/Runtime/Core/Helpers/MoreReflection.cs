using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plml
{
    public static class MoreReflection
    {
        public static bool HasCustomAttribute<TAttribute>(this ICustomAttributeProvider customAttributeProvider)
            where TAttribute : Attribute
            => customAttributeProvider.GetCustomAttributes(typeof(TAttribute), true).Any();

        public static bool TryGetCustomAttribute<TAttribute>(this ICustomAttributeProvider customAttributeProvider, out TAttribute attribute)
            where TAttribute : Attribute
        {
            object attributeObj = customAttributeProvider.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();

            bool exists = attributeObj != null;
            attribute = exists ? (TAttribute)attributeObj : null;

            return exists;
        }

        public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this ICustomAttributeProvider customAttributeProvider)
            where TAttribute : Attribute
            => customAttributeProvider.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>();

        public static bool HasGetter(this PropertyInfo property) => property.GetGetMethod() != null;
        public static bool HasSetter(this PropertyInfo property) => property.GetSetMethod() != null;

        public static bool IsAssignableFrom<TOther>(this Type type) => type.IsAssignableFrom(typeof(TOther));

        public static bool InheritsFrom<TClass>(this Type type) => type.IsSubclassOf(typeof(TClass));
        public static bool Implements<TInterface>(this Type type) => type.Implements(typeof(TInterface));
        public static bool Implements(this Type type, Type interfaceType) => interfaceType.IsAssignableFrom(type);

        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
            => (TDelegate)method.CreateDelegate(typeof(TDelegate));
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method, object instance) where TDelegate : Delegate
            => (TDelegate)method.CreateDelegate(typeof(TDelegate), instance);

        public static bool IsCaptureClass(this Type type) => type.Name.Contains("<>c");

        public static MethodInfo GetGenericMethodDefinition(this Type type, string name, BindingFlags bindingAttr) => type
            .GetMethods(bindingAttr)
            .Where(method => method.Name == name)
            .First(method => method.IsGenericMethodDefinition);
    }
}