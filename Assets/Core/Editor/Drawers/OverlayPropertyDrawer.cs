using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    public abstract class OverlayPropertyDrawer<TAttribute> : PropertyDrawer
        where TAttribute : Attribute
    {
        private const string GetDrawerTypeMethodName = "GetDrawerTypeForType";
        private const string ScriptAttributeUtilityName = "UnityEditor.ScriptAttributeUtility";

        private const string AttributeFieldName = "m_Attribute";
        private const string FieldInfoName = "m_FieldInfo";

        private static readonly Func<Type, Type> GetDrawerTypeForType;
        private static readonly Action<PropertyDrawer, PropertyAttribute> InitializeDrawerAttribute;
        private static readonly Action<PropertyDrawer, FieldInfo> InitializeDrawerFieldInfo;

        static OverlayPropertyDrawer()
        {
            Type scriptAttributeUtility = typeof(EditorGUI).Assembly.GetType(ScriptAttributeUtilityName);
            MethodInfo getDrawerMethod = scriptAttributeUtility.GetMethod(GetDrawerTypeMethodName, BindingFlags.Static | BindingFlags.NonPublic);

            ParameterExpression attributeType = Expression.Parameter(typeof(Type), nameof(attributeType));

            Expression<Func<Type, Type>> getDrawerExpr = Expression.Lambda<Func<Type, Type>>(Expression.Call(null, getDrawerMethod, attributeType), attributeType);
            GetDrawerTypeForType = getDrawerExpr.Compile();


            Type propertyDrawerType = typeof(PropertyDrawer);
            FieldInfo attributeField = propertyDrawerType.GetField(AttributeFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo fieldInfoField = propertyDrawerType.GetField(FieldInfoName, BindingFlags.Instance | BindingFlags.NonPublic);

            ParameterExpression drawer = Expression.Parameter(propertyDrawerType, nameof(drawer));
            ParameterExpression attributeParam = Expression.Parameter(typeof(PropertyAttribute), "attribute");
            ParameterExpression fieldInfoParam = Expression.Parameter(typeof(FieldInfo), "fieldInfo");

            Expression<Action<PropertyDrawer, PropertyAttribute>> initAttributeExpr = Expression
                .Lambda<Action<PropertyDrawer, PropertyAttribute>>(
                    Expression.Assign(
                        Expression.Field(drawer, attributeField), attributeParam), drawer, attributeParam);
            InitializeDrawerAttribute = initAttributeExpr.Compile();

            Expression<Action<PropertyDrawer, FieldInfo>> initFieldExpr = Expression
                .Lambda<Action<PropertyDrawer, FieldInfo>>(
                    Expression.Assign(
                        Expression.Field(drawer, fieldInfoField), fieldInfoParam), drawer, fieldInfoParam);
            InitializeDrawerFieldInfo = initFieldExpr.Compile();
        }

        protected OverlayPropertyDrawer() : base()
        {
            innerDrawer = new Lazy<PropertyDrawer>(GetInnerDrawerIfItExists);
        }

        private Lazy<PropertyDrawer> innerDrawer;
        protected PropertyDrawer InnerDrawer => innerDrawer.Value;

        private PropertyDrawer GetInnerDrawerIfItExists()
        {
            PropertyAttribute innerAttribute = fieldInfo
                .GetCustomAttributes<PropertyAttribute>()
                .Where(pa => !(pa is TAttribute))
                .SingleOrDefault();

            PropertyDrawer innerDrawer = null;
            if (innerAttribute != null)
            {
                Type attrType = innerAttribute.GetType();
                Type drawerType = GetDrawerTypeForType(attrType);

                innerDrawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
                InitializeDrawerAttribute(innerDrawer, innerAttribute);
                InitializeDrawerFieldInfo(innerDrawer, fieldInfo);
            }

            return innerDrawer;
        }
    }
}
