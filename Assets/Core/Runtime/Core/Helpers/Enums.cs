using System;
using System.Collections.Generic;

namespace Plml
{
    public static class Enums
    {
        public static bool IsOneOf<TEnum>(this TEnum @enum, params TEnum[] values) where TEnum : Enum
        {
            var eqc = EqualityComparer<TEnum>.Default;
            foreach (var value in values)
                if (eqc.Equals(@enum, value))
                    return true;
            return false;
        }
    }
}