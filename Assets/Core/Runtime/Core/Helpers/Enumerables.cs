using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml
{
    public static class Enumerables
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

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T elt in sequence)
                action(elt);
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> sequences) => sequences.SelectMany(seq => seq);
        public static IEnumerable<T> Merge<T>(params IEnumerable<T>[] sequences) => sequences.Merge();

        public static IEnumerable<int> Sequence(int count) => Enumerable
            .Repeat(0, count)
            .Select((_, i) => i);
    }
}