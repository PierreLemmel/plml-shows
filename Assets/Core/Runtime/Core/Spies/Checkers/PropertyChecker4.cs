using System;

namespace Plml.Checkers
{
    internal class PropertyChecker<TModel, T1, T2, T3, T4> : IChecker<TModel>
    {
        private readonly Func<TModel, T1> f1;
        private readonly Func<TModel, T2> f2;
        private readonly Func<TModel, T3> f3;
        private readonly Func<TModel, T4> f4;

        private bool initialized = false;
        private (T1, T2, T3, T4) lastResult;


        public PropertyChecker(Func<TModel, T1> func1,
            Func<TModel, T2> func2,
            Func<TModel, T3> func3,
            Func<TModel, T4> func4)
        {
            f1 = func1 ?? throw new ArgumentNullException(nameof(func1));
            f2 = func2 ?? throw new ArgumentNullException(nameof(func2));
            f3 = func3 ?? throw new ArgumentNullException(nameof(func3));
            f4 = func4 ?? throw new ArgumentNullException(nameof(func4));
        }

        public bool CheckForChanges(TModel model)
        {
            (T1, T2, T3, T4) newResult = (f1(model), f2(model), f3(model), f4(model));

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