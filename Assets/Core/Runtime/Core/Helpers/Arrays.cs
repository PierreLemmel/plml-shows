using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml
{
    public static class Arrays
    {
        public static TTo[] Select<TFrom, TTo>(this TFrom[] input, Func<TFrom, TTo> selector)
        {
            TTo[] result = new TTo[input.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = selector(input[i]);
            }

            return result;
        }

        public static TTo[] Select<TFrom, TTo>(this TFrom[] input, Func<TFrom, int, TTo> selector)
        {
            TTo[] result = new TTo[input.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = selector(input[i], i);
            }

            return result;
        }

        public static void MapTo<TFrom, TTo>(this TFrom[] input, TTo[] output, Func<TFrom, TTo> selector)
        {
            if (output.Length != input.Length)
                throw new InvalidOperationException($"Array length mismatch. Input: {input.Length}. Ouput: {output.Length}");

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = selector(input[i]);
            }
        }

        public static int IndexOf<T>(this T[] array, Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i]))
                    return i;

            throw new InvalidOperationException($"No item found matching predicate");
        }

        public static int FirstIndexOf<T>(this T[] array, T item) => array.IndexOf(t => Equals(t, item));

        public static T[] Merge<T>(IEnumerable<T[]> arrays)
        {
            int capacity = arrays.Sum(arr => arr.Length);
            T[] result = new T[capacity];

            int destIdx = 0;
            foreach (T[] array in arrays)
            {
                int length = array.Length;
                Array.Copy(array, 0, result, destIdx, length);
                destIdx += length;
            }

            return result;
        }

        public static T[] Merge<T>(params T[][] arrays)
        {
            IEnumerable<T[]> asEnumerable = arrays;
            return Merge(asEnumerable);
        }

        public static void EnsureSize<T>(ref T[] array, int size)
        {
            if (array == null || array.Length != size)
                array = new T[size];
        }

        public static T[] ShallowCopy<T>(this T[] array)
        {
            T[] result = new T[array.Length];
            array.CopyTo(result, 0);

            return result;
        }

        public static T[] Reverse<T>(this T[] array)
        {
            int length = array.Length;

            T[] result = new T[length];

            for (int i = 0; i < length; i++)
                result[length - 1 - i] = array[i];

            return result;
        }

        public static void CopyTo<T>(this T[] array, T[] other)
        {
            if (other.Length != array.Length)
                throw new InvalidOperationException("Both arrays must be of the same length");

            array.CopyTo(other, 0);
        }

        public static void Set<T>(this T[] array, T value) => array.Set(value, 0, array.Length);
        public static void Set<T>(this T[] array, T value, int startIndex) => array.Set(value, startIndex, array.Length - startIndex);
        public static void Set<T>(this T[] array, T value, int startIndex, int length)
        {
            int lastIndex = length - startIndex;
            for (int i = startIndex; i < lastIndex; i++)
                array[i] = value;
        }

        public static void Set<T>(this T[] array, Func<T> func) => array.Set(func, 0, array.Length);
        public static void Set<T>(this T[] array, Func<T> func, int startIndex) => array.Set(func, startIndex, array.Length - startIndex);
        public static void Set<T>(this T[] array, Func<T> func, int startIndex, int length)
        {
            int lastIndex = length - startIndex;
            for (int i = startIndex; i < lastIndex; i++)
                array[i] = func();
        }

        public static void Set<T>(this T[] array, Func<int, T> func) => array.Set(func, 0, array.Length);
        public static void Set<T>(this T[] array, Func<int, T> func, int startIndex) => array.Set(func, startIndex, array.Length - startIndex);
        public static void Set<T>(this T[] array, Func<int, T> func, int startIndex, int length)
        {
            int lastIndex = length - startIndex;
            for (int i = startIndex; i < lastIndex; i++)
                array[i] = func(i);
        }

        public static void Set<T>(this T[] array, Func<T, T> func) => array.Set(func, 0, array.Length);
        public static void Set<T>(this T[] array, Func<T, T> func, int startIndex) => array.Set(func, startIndex, array.Length - startIndex);
        public static void Set<T>(this T[] array, Func<T, T> func, int startIndex, int length)
        {
            int lastIndex = length - startIndex;
            for (int i = startIndex; i < lastIndex; i++)
                array[i] = func(array[i]);
        }

        public static void ShiftLeft<T>(this T[] array)
        {
            if (array.Length <= 1) return;

            T first = array[0];

            for (int i = 0; i < array.Length - 1; i++)
                array[i] = array[i + 1];

            array[^1] = first;
        }

        public static void ShiftRight<T>(this T[] array)
        {
            if (array.Length <= 1) return;

            T last = array[^1];

            for (int i = array.Length - 1; i >= 1; i--)
                array[i] = array[i - 1];

            array[0] = last;
        }

        public static T[] Repeated<T>(this T[] array, int count)
        {
            T[] result = new T[array.Length * count];

            for (int i = 0; i < count; i++)
                array.CopyTo(result, i * array.Length);

            return result;
        }

        public static T RandomElement<T>(this T[] array) => array[URandom.Range(0, array.Length)];

        public static T[] Create<T>(int length, Func<int, T> selector)
        {
            T[] result = new T[length];

            for (int i = 0; i < length; i++)
                result[i] = selector(i);

            return result;
        }

        public static T[,] Create<T>(int rows, int cols, Func<int, int, T> selector)
        {
            T[,] result = new T[cols, rows];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    result[col, row] = selector(col, row);

            return result;
        }

        public static T[,] Create<T>(int rows, int cols, Func<T> selector) => Create(rows, cols, (x, y) => selector());

        public static T[] Create<T>(int length, Func<T> selector) => Create(length, i => selector());
        public static T[] Create<T>(int length, T value) => Create(length, i => value);

        public static void ForEach<T>(this T[] array, Action<T, int> action)
        {
            for (int i = 0; i < array.Length; i++)
                action(array[i], i);
        }

        public static T[][] Split<T>(this T[] array, Func<T, bool> separatorFunc)
        {
            int i0 = 0;

            ICollection<T[]> chunks = new LinkedList<T[]>();

            for (int i = 0; i < array.Length; i++)
            {
                if (separatorFunc(array[i]))
                {
                    chunks.Add(array[i0..i]);
                    i0 = i + 1;
                }
            }

            chunks.Add(array[i0..^0]);

            return chunks
                .Where(chunk => chunk.Any())
                .ToArray();
        }

        public static T[][] Split<T>(this T[] array, T separator) => array
            .Split(t => Equals(t, separator));

        public static (ArraySegment<T> lhs, ArraySegment<T> rhs) Separate<T>(this T[] array, Func<T, bool> separatorFunc)
        {
            int index = array.IndexOf(separatorFunc);

            return (array[0..index], array[(index + 1)..^0]);
        }
    }
}