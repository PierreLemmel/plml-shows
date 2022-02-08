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

        public static void MapTo<TFrom, TTo>(this TFrom[] input, TTo[] output, Func<TFrom, TTo> selector)
        {
            if (output.Length != input.Length)
                throw new InvalidOperationException($"Array length mismatch. Input: {input.Length}. Ouput: {output.Length}");

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = selector(input[i]);
            }
        }

        public static int IndexOf<T>(this T[] array, T item) => Array.IndexOf(array, item);

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
    }
}