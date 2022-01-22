using System;

namespace Plml.Checkers
{
    internal class PropertyChecker<TModel, T1, T2, T3, T4, T5, T6, T7, T8> : IChecker<TModel>
    {
        private readonly Func<TModel, T1> f1;
        private readonly Func<TModel, T2> f2;
        private readonly Func<TModel, T3> f3;
        private readonly Func<TModel, T4> f4;
        private readonly Func<TModel, T5> f5;
        private readonly Func<TModel, T6> f6;
        private readonly Func<TModel, T7> f7;
        private readonly Func<TModel, T8> f8;

        private bool initialized = false;
        private (T1, T2, T3, T4, T5, T6, T7, T8) lastResult;


        public PropertyChecker(Func<TModel, T1> func1,
            Func<TModel, T2> func2,
            Func<TModel, T3> func3,
            Func<TModel, T4> func4,
            Func<TModel, T5> func5,
            Func<TModel, T6> func6,
            Func<TModel, T7> func7,
            Func<TModel, T8> func8)
        {
            f1 = func1 ?? throw new ArgumentNullException(nameof(func1));
            f2 = func2 ?? throw new ArgumentNullException(nameof(func2));
            f3 = func3 ?? throw new ArgumentNullException(nameof(func3));
            f4 = func4 ?? throw new ArgumentNullException(nameof(func4));
            f5 = func5 ?? throw new ArgumentNullException(nameof(func5));
            f6 = func6 ?? throw new ArgumentNullException(nameof(func6));
            f7 = func7 ?? throw new ArgumentNullException(nameof(func7));
            f8 = func8 ?? throw new ArgumentNullException(nameof(func8));
        }

        public bool CheckForChanges(TModel model)
        {
            (T1, T2, T3, T4, T5, T6, T7, T8) newResult = (f1(model), f2(model), f3(model), f4(model), f5(model), f6(model), f7(model), f8(model));
            if (initialized)
            {
                bool hasChanged = !newResult.Equals(lastResult);
                lastResult = newResult;
                return hasChanged;
            }
            else
            {
                lastResult = newResult;
                initialized = true;
                return true;
            }
        }
    }
}