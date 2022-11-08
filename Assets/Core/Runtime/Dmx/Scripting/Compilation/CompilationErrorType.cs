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
        InvalidInternalContext,

        // Compilation
        UnsupportedSyntaxNode,
        UnsupportedVariableType,
        MissingImplicitConversion,
        MissingExplicitConversion,

        // Optimization
        OptimizationError,

        // Type System
        UnsupportedType,
    }
}