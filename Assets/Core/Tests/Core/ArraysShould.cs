using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Tests
{
    public class ArraysShould
    {
        [Test]
        [TestCaseSource(nameof(SplitTestCases))]
        public void Split_Returns_Expected(int[] input, int separator, int[][] expected)
        {
            int[][] result = input.Split(separator);

            Assert.AreEqual(expected.Length, result.Length);

            for (int i = 0; i < result.Length; i++)
                CollectionAssert.AreEquivalent(expected[i], result[i]);
        }


        public static IEnumerable<object[]> SplitTestCases => new object[][]
        {
            new object[]
            {
                new int[]
                {
                    1, 2, 3,
                    666,
                    4, 5
                },
                666,
                new int[][]
                {
                    new[] { 1, 2, 3 },
                    new[] { 4, 5 },
                }
            },

            new object[]
            {
                new int[]
                {
                    666,
                    1, 2, 3,
                    666,
                    4, 5,
                    666,
                    666,
                    666,
                    7, 8,
                    666,
                    9, 10,
                    666
                },
                666,
                new int[][]
                {
                    new[] { 1, 2, 3 },
                    new[] { 4, 5 },
                    new[] { 7, 8 },
                    new[] { 9, 10 },
                }
            },

            new object[]
            {
                new int[]
                {
                    1, 2, 3, 4, 5
                },
                666,
                new int[][]
                {
                    new[] { 1, 2, 3, 4, 5 },
                }
            },

            new object[]
            {
                Array.Empty<int>(),
                666,
                Array.Empty<int[]>(),
            }
        };
    }
}