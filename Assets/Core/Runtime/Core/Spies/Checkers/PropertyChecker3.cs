using System;

namespace Plml.Checkers
{
    internal class PropertyChecker<TModel, T1, T2, T3> : IChecker<TModel>
    {
        private readonly Func<TModel, T1> f1;
        private readonly Func<TModel, T2> f2;
        private readonly Func<TModel, T3> f3;

        private bool initialized = false;
        private (T1, T2, T3) lastResult;


        public PropertyChecker(Func<TModel, T1> func1, Func<TModel, T2> func2, Func<TModel, T3> func3)
        {
            f1 = func1 ?? throw new ArgumentNullException(nameof(func1));
            f2 = func2 ?? throw new ArgumentNullException(nameof(func2));
            f3 = func3 ?? throw new ArgumentNullException(nameof(func3));
        }

        public bool CheckForChanges(TModel model)
        {
            (T1, T2, T3) newResult = (f1(model), f2(model), f3(model));

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