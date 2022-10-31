namespace Plml.Dmx.Scripting.Compilation
{
    internal enum CompilationErrorType
    {
        // Tokenization
        InvalidCharacter,
        InvalidNumberFormat,
        InvalidBrackets,

        // AST
        UnknownVariable,
        UnsupportedToken,
        InvalidContext,
        MissingProperty,
        UnknownOperator,
        NoLeftHandSideForOperator,
        TypeError,

        InternalSyntaxTreeError,

        // Compilation
        UnsupportedSyntaxNode,
        UnsupportedVariableType,

        // Type System
        UnsupportedType,
    }
}