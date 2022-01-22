using System;

namespace Plml.Checkers
{
    internal static class CheckerFactory<TModel>
    {
        public static IChecker<TModel> ForAllProperties()
        {
            if (typeof(TModel).Implements<IEquatable<TModel>>())
                throw new InvalidOperationException($"Don't use CheckerFactory.ForAllProperty for types implenting IEquatable, use ValueCheckerFactory.For instead");

            return AllPropertiesCheckerFactory.For<TModel>();
        }

        public static IChecker<TModel> ForProperty<T>(Func<TModel, T> func1)
            => new PropertyChecker<TModel, T>(func1);

        public static IChecker<TModel> ForProperties<T1, T2>(Func<TModel, T1> func1, Func<TModel, T2> func2)
            => new PropertyChecker<TModel, T1, T2>(func1, func2);

        public static IChecker<TModel> ForProperties<T1, T2, T3>(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3)
            => new PropertyChecker<TModel, T1, T2, T3>(func1, func2, func3);

        public static IChecker<TModel> ForProperties<T1, T2, T3, T4>(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3, Func<TModel, T4> func4)
            => new PropertyChecker<TModel, T1, T2, T3, T4>(func1, func2, func3, func4);

        public static IChecker<TModel> ForProperties<T1, T2, T3, T4, T5>(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3, Func<TModel, T4> func4, Func<TModel, T5> func5)
            => new PropertyChecker<TModel, T1, T2, T3, T4, T5>(func1, func2, func3, func4, func5);

        public static IChecker<TModel> ForProperties<T1, T2, T3, T4, T5, T6>(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3, Func<TModel, T4> func4,
            Func<TModel, T5> func5, Func<TModel, T6> func6)
            => new PropertyChecker<TModel, T1, T2, T3, T4, T5, T6>(func1, func2, func3, func4, func5, func6);

        public static IChecker<TModel> ForProperties<T1, T2, T3, T4, T5, T6, T7>(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3, Func<TModel, T4> func4,
            Func<TModel, T5> func5, Func<TModel, T6> func6, Func<TModel, T7> func7)
            => new PropertyChecker<TModel, T1, T2, T3, T4, T5, T6, T7>(func1, func2, func3, func4, func5, func6, func7);

        public static IChecker<TModel> ForProperties<T1, T2, T3, T4, T5, T6, T7, T8>(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3, Func<TModel, T4> func4,
            Func<TModel, T5> func5, Func<TModel, T6> func6, Func<TModel, T7> func7, Func<TModel, T8> func8)
            => new PropertyChecker<TModel, T1, T2, T3, T4, T5, T6, T7, T8>(func1, func2, func3, func4, func5, func6, func7, func8);
    }

    internal static class CheckerFactory
    {
        public static IChecker<TStruct[]> ForValueArray<TStruct>() where TStruct : struct => new ValueArrayChecker<TStruct>();
    }
}
