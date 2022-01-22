using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml
{
    public static class MoreDelegates
    {
        public static TDelegate Combine<TDelegate>(this IEnumerable<TDelegate> delegates) where TDelegate : Delegate => (TDelegate)Delegate.Combine(delegates.ToArray());
    }
}