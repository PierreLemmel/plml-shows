using System;

namespace Plml.Checkers
{
    internal static class ValueCheckerFactory
    {
        public static IChecker<TEquatable> For<TEquatable>() => new ValueChecker<TEquatable>();
    }
}
