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
        InvalidArgumentType,
        InternalSyntaxTreeError,

        // Compilation
        UnsupportedSyntaxNode,
        UnsupportedVariableType,

        // Optimization
        OptimizationError,

        // Type System
        UnsupportedType,
    }
}