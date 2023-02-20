using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plml.Tests
{
    internal class BytesShould
    {
        [Test]
        [TestCaseSource(nameof(LSBTestCaseSource))]
        public void LSB_Returns_Expected(int input, byte expected) => Assert.That(Bytes.Lsb(input), Is.EqualTo(expected));

        public static IEnumerable<object[]> LSBTestCaseSource => new object[][]
        {
            new object[] { 0x000000ff, (byte)0xff },
            new object[] { 0x0000ffff, (byte)0xff },
            new object[] { 0x0fffffff, (byte)0xff },
            new object[] { 0x0fffffab, (byte)0xab },
            new object[] { 0x000000ab, (byte)0xab },
        };

        [Test]
        [TestCaseSource(nameof(MSBTestCaseSource))]
        public void MSB_Returns_Expected(int input, byte expected) => Assert.That(Bytes.Msb(input), Is.EqualTo(expected));

        public static IEnumerable<object[]> MSBTestCaseSource => new object[][]
        {
            new object[] { 0x000000ff, (byte)0x00 },
            new object[] { 0x0000ffff, (byte)0xff },
            new object[] { 0x0fffffff, (byte)0xff },
            new object[] { 0x0fffffab, (byte)0xff },
            new object[] { 0x0000abab, (byte)0xab },
            new object[] { 0x0000ab00, (byte)0xab },
        };
    }
}
