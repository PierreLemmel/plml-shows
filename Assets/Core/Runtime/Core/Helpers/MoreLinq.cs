using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml
{
    public static class MoreLinq
    {
        public static bool IsEmpty<T>(this IEnumerable<T> sequence) => !sequence.Any();

        public static bool IsSingle<T>(this IEnumerable<T> sequence)
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            return enumerator.MoveNext() && !enumerator.MoveNext();
        }

        public static bool AreAllDistinct<T>(this IEnumerable<T> sequence)
        {
            HashSet<T> set = new HashSet<T>();

            foreach (T elt in sequence)
            {
                if (!set.Add(elt))
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, params T[] elts) => sequence.Except(elts.AsEnumerable());
    }
}