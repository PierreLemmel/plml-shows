using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml
{
    public static class Utils
    {
        public static void AddRange<T>(this ICollection<T> collection, params T[] elts)
        {
            foreach (T elt in elts)
                collection.Add(elt);
        }
    }
}