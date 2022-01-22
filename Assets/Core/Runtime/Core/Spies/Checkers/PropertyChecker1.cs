using System;

namespace Plml.Checkers
{
    internal class PropertyChecker<TModel, T> : IChecker<TModel>
    {
        private readonly Func<TModel, T> f;

        private bool initialized = false;
        private T lastResult;


        public PropertyChecker(Func<TModel, T> func)
        {
            f = func ?? throw new ArgumentNullException(nameof(func));
        }

        public bool CheckForChanges(TModel model)
        {
            T newResult = f(model);

            if (initialized)
            {
                bool hasChanged = !Equals(newResult, lastResult);
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