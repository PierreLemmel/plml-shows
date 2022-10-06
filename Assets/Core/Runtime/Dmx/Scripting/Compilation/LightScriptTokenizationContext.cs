using System;

namespace Plml.Dmx.Scripting.Compilation
{
    internal enum LightScriptTokenizationContext
    {
        NewToken,
        Number,
        Operator,
        Identifier,
        TokenEnd,
        StatementEnding,
        SingleChar,

        UNINITIALIZED
    }

    internal enum LightScriptTokenizationNumberTypeContext
    {
        Integer,
        Float,
        Hexadecimal
    }
}