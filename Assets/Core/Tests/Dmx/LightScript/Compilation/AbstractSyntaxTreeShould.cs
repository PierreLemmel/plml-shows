using NUnit.Framework;
using Plml.Dmx.Scripting;
using Plml.Dmx.Scripting.Compilation;
using Plml.Dmx.Scripting.Compilation.Nodes;
using Plml.Dmx.Scripting.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plml.Tests.Dmx.Scripting.Compilation
{
    internal class AbstractSyntaxTreeShould
    {
        [Test]
        [TestCaseSource(nameof(EquivalentNodesTestCaseSource))]
        public void Node_Equality_Should_Return_True_When_Nodes_Are_Equivalent(SyntaxNode lhs, SyntaxNode rhs) => Assert.AreEqual(lhs, rhs);

        public static IEnumerable<object[]> EquivalentNodesTestCaseSource => new object[][]
        {
            new object[]
            {
                new ConstantNode(new Color24(0xff, 0x00, 0x00)),
                new ConstantNode(new Color24(0xff, 0x00, 0x00)),
            },
            new object[]
            {
                new ConstantNode(0xff),
                new ConstantNode(0xff),
            },
            new object[]
            {
                new ConstantNode(0.5f),
                new ConstantNode(0.5f),
            },
            new object[]
            {
                new MultiplicationNode(
                    new ConstantNode(0.5f),
                    new ConstantNode(0xff)
                ),
                new MultiplicationNode(
                    new ConstantNode(0.5f),
                    new ConstantNode(0xff)
                )
            },
            new object[]
            {
                new UnaryMinusNode(
                    new MemberAccessNode(
                        new VariableNode(
                            LightScriptType.Fixture,
                            "parLed"
                        ),
                        "dimmer"
                    )
                ),
                new UnaryMinusNode(
                    new MemberAccessNode(
                        new VariableNode(
                            LightScriptType.Fixture,
                            "parLed"
                        ),
                        "dimmer"
                    )
                )
            }
        };
    }
}
