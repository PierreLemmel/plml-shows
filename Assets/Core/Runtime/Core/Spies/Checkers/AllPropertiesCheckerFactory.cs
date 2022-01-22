using Plml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Plml.Checkers
{
    internal static class AllPropertiesCheckerFactory
    {
        private static readonly IDictionary<Type, Func<object>> factoryCache = new Dictionary<Type, Func<object>>();

        public static IChecker<TModel> For<TModel>()
        {
            if (!factoryCache.ContainsKey(typeof(TModel)))
                factoryCache[typeof(TModel)] = BuildSpyForModel<TModel>();

            Func<object> factory = factoryCache[typeof(TModel)];
            IChecker<TModel> spy = (IChecker<TModel>)factory.Invoke();
            return spy;
        }

        private static Func<object> BuildSpyForModel<TModel>()
        {
            PropertyInfo[] properties = typeof(TModel)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.HasGetter() && prop.HasSetter())
                .ToArray();
            Type[] propTypes = properties.Select(property => property.PropertyType);
            Type genericFunc2Type = typeof(Func<,>);

            ParameterExpression model = Expression.Parameter(typeof(TModel), "model");
            Delegate[] functions = properties.Select(property =>
            {
                MemberExpression body = Expression.Property(model, property);

                Type funcType = genericFunc2Type.MakeGenericType(typeof(TModel), property.PropertyType);
                LambdaExpression lambda = Expression.Lambda(funcType, body, model);

                return lambda.Compile();
            });
            Type[] funcTypes = functions.Select(func => func.GetType());
            ConstructorInfo constructor = GetConstructorForSpy<TModel>(propTypes, funcTypes);

            ConstantExpression[] parameters = functions.Select(func => Expression.Constant(func));
            NewExpression newSpy = Expression.New(constructor, parameters);

            Expression<Func<object>> factoryLambda = Expression.Lambda<Func<object>>(newSpy);
            Func<object> factory = factoryLambda.Compile();

            return factory;
        }

        private static ConstructorInfo GetConstructorForSpy<TModel>(Type[] propTypes, Type[] funcTypes)
        {
            Type genericSpyType;
            switch(propTypes.Length)
            {
                case 1:
                    genericSpyType = typeof(PropertyChecker<,>);
                    break;
                case 2:
                    genericSpyType = typeof(PropertyChecker<,,>);
                    break;
                case 3:
                    genericSpyType = typeof(PropertyChecker<,,,>);
                    break;
                case 4:
                    genericSpyType = typeof(PropertyChecker<,,,,>);
                    break;
                case 5:
                    genericSpyType = typeof(PropertyChecker<,,,,,>);
                    break;
                case 6:
                    genericSpyType = typeof(PropertyChecker<,,,,,,>);
                    break;
                case 7:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,>);
                    break;
                case 8:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,>);
                    break;
                case 9:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,>);
                    break;
                case 10:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,>);
                    break;
                case 11:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,,>);
                    break;
                case 12:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,,,>);
                    break;
                case 13:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,,,,>);
                    break;
                case 14:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,,,,,>);
                    break;
                case 15:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,,,,,,>);
                    break;
                case 16:
                    genericSpyType = typeof(PropertyChecker<,,,,,,,,,,,,,,,,>);
                    break;
                default:
                    throw new NotSupportedException("Only classes with 1 to 16 properties are supported");
            }

            Type[] typeGenericArgs = new Type[propTypes.Length + 1];
            typeGenericArgs[0] = typeof(TModel);
            Array.Copy(propTypes, 0, typeGenericArgs, 1, propTypes.Length);

            Type spyType = genericSpyType.MakeGenericType(typeGenericArgs);
            ConstructorInfo constructor = spyType.GetConstructor(funcTypes);

            return constructor;
        }
    }
}