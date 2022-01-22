using System;

namespace Plml.Checkers
{
    internal class ValueChecker<TModel> : IChecker<TModel>
    {
        private bool initialized = false;
        private TModel lastValue;

        public ValueChecker()
        {
            if (!typeof(TModel).Implements<IEquatable<TModel>>())
                throw new InvalidOperationException($"Don't use ValueCheckerFactory.For for types not implenting IEquatable, use CheckerFactory.ForAllProperties instead");
        }

        public bool CheckForChanges(TModel model)
        {
            TModel newValue = model;

            if (initialized)
            {
                bool hasChanged = !Equals(newValue, lastValue);
                lastValue = newValue;
                return hasChanged;
            }
            else
            {
                lastValue = newValue;
                initialized = true;
                return true;
            }
        }
    }
}
